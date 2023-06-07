using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HypotheticalComputingMachineClassLib.CodeInterpreter;
using HypotheticalComputingMachineClassLib.HCMModel_D1;
using System.Collections;

namespace HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel
{
    public enum ProgramState : byte
    {
        NotRunning = 1,
        RunningByStep = 3,
        RunningTilEnd = 4,
        RunningEnded = 9
    }

    public interface IHCMProgram<out THCM> 
        where THCM : IHypotheticalComputingMachineModel
    {
        ProgramState State { get; set; }

        bool EOF { get; set; }

        event Action CommandHasBeenExecuted;

        event Action PreparingToNextCommandExecuting;

        void Reset();

        IEnumerable Run();

        void RunTilEnd();

        void RunByStep();
    }

    public class HCMProgram<THCM> : IHCMProgram<THCM>
        where THCM : IHypotheticalComputingMachineModel
    {
        private THCM _hcm;

        private IHCMDataSegment _ds;

        private IHCMCodeSegment _cs;

        private CodeInterpreter<THCM> _interpreter;

        private IEnumerator _program;

        private bool _eof = false;

        public ProgramState State { get; set; }

        public virtual bool EOF 
        { 
            get => _eof;
            set
            {
                if (value)
                {
                    _program = null;
                    State = ProgramState.RunningEnded;
                }
                _eof = value;
            }
        }

        public event Action CommandHasBeenExecuted;

        public event Action PreparingToNextCommandExecuting;

        public HCMProgram(
            THCM hcm,
            Func<THCM, CodeInterpreter<THCM>> interpreterFactory)
        {
            _hcm = hcm;
            _ds = hcm.DS;
            _cs = hcm.CS;
            _interpreter = interpreterFactory(hcm);
        }

        public void Reset()
        {
            EOF = false;
            State = ProgramState.NotRunning;
            _program = null;

            return;
        }

        public IEnumerable Run()
        {
            while (true)
            {
                int sak = BinaryInOutConverter.BytesToInt32(_cs.SAK);
                byte[] rk = _ds.ReadFromMemCell(sak);
                _cs.RK = rk;
                _interpreter.Interprete(_cs.RK).Execute();
                CommandHasBeenExecuted?.Invoke();
                PreparingToNextCommandExecuting?.Invoke();
                if (EOF) yield break; else yield return null;
            }
        }

        private void StartIfNotYet()
        {
            if (_program is null)
            {
                Reset();
                _hcm.SoftReset();
                _program = Run().GetEnumerator();
            }

            return;
        }

        public void RunTilEnd()
        {
            StartIfNotYet();
            State = ProgramState.RunningTilEnd;
            while (_program.MoveNext()) ;

            return; 
        }

        public void RunByStep()
        {
            StartIfNotYet();
            State = ProgramState.RunningByStep;
            _program.MoveNext();

            return;
        }
    }
}
