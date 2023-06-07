using HypotheticalComputingMachineClassLib;

namespace HypotheticalComputingMachineTests
{
    [TestClass]
    public class CommandTests : HCMTests
    {

        [TestInitialize]
        public void Init()
        {
            RestartHCM();

            return;
        }


        [TestMethod]
        public void Exit()
        {
            _hcm.WriteData(new byte[4], _defaultCsStartAddress); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
        }

        [TestMethod]
        public void AddTestSimple()
        {
            byte[] op1 = new byte[4] { 0b0, 0b0, 0b0, 0b10 },
                   op2 = new byte[4] { 0b0, 0b0, 0b0, 0b11 };

            _hcm.WriteData(op1, _dsAddress.Next()); // операнд 1: 2
            _hcm.WriteData(op2, _dsAddress.Next()); // операнд 2: 3
            _hcm.WriteData(new byte[4] { 0b00010000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда add
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.AddNaive(op1, op2));
        }

        // Проверка на переносы.
        [TestMethod]
        public void AddTestWithCarrying()
        {
            byte[] op1 = new byte[4] { 0b10000000, 0b10000000, 0b10000000, 0b10000001 },
                   op2 = new byte[4] { 0b10000000, 0b10000000, 0b10000000, 0b10000000 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00010000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда add
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.AddNaive(op1, op2));
        }

        // Проверка флагов.
        [TestMethod]
        public void AddTestWithRPChecking()
        {
            byte[] op1 = new byte[4] { 0b0, 0b0, 0b0, 0b10 },
                   op2 = new byte[4] { 0b0, 0b0, 0b0, 0b11 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00010000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда add
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.AddNaive(op1, op2));
            CompareRP(new bool[4] { false, false, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b01000000, 0b0, 0b0, 0b1 };
            op2 = new byte[4] { 0b01000000, 0b0, 0b0, 0b0 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00010000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда add
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.AddNaive(op1, op2));
            CompareRP(new bool[4] { true, false, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b00000000, 0b0, 0b0, 0b0 };
            op2 = new byte[4] { 0b00000000, 0b0, 0b0, 0b0 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00010000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда add
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.AddNaive(op1, op2));
            CompareRP(new bool[4] { false, true, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b11000000, 0b0, 0b0, 0b1 };
            op2 = new byte[4] { 0b11000000, 0b0, 0b0, 0b0 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00010000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда add
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.AddNaive(op1, op2));
            CompareRP(new bool[4] { true, false, true, false });

            RestartHCM();

            op1 = new byte[4] { 0b0, 0b0, 0b0, 0b1 };
            op2 = new byte[4] { 0b11111111, 0b11111111, 0b11111111, 0b11111111 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00010000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда add
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.AddNaive(op1, op2));
            CompareRP(new bool[4] { false, true, true, false });

            RestartHCM();

            op1 = new byte[4] { 0b10000000, 0b10000000, 0b10000000, 0b10000001 };
            op2 = new byte[4] { 0b10000000, 0b10000000, 0b10000000, 0b10000000 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00010000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда add
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.AddNaive(op1, op2));
            CompareRP(new bool[4] { false, false, true, false });
        }

        [TestMethod]
        public void SubTestSimple()
        {
            byte[] op1 = new byte[4] { 0b0, 0b0, 0b0, 0b101 },
                   op2 = new byte[4] { 0b0, 0b0, 0b0, 0b11 };

            _hcm.WriteData(op1, _dsAddress.Next()); // операнд 1: 5
            _hcm.WriteData(op2, _dsAddress.Next()); // операнд 2: 3
            _hcm.WriteData(new byte[4] { 0b00100000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда sub
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.SubNaive(op1, op2));
        }

        // Проверка на переносы.
        [TestMethod]
        public void SubTestWithCarrying()
        {
            byte[] op1 = new byte[4] { 0b01000000, 0b0, 0b0, 0b0 },
                   op2 = new byte[4] { 0b00000001, 0b00000001, 0b00000001, 0b00000001 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00100000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next());
            _hcm.WriteData(new byte[4], _csAddress.Next());
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.SubNaive(op1, op2));
        }

        // Проверка флагов.
        [TestMethod]
        public void SubTestWithRPChecking()
        {
            byte[] op1 = new byte[4] { 0b0, 0b0, 0b0, 0b101 },
                   op2 = new byte[4] { 0b0, 0b0, 0b0, 0b11 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00100000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда sub
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.SubNaive(op1, op2));
            CompareRP(new bool[4] { false, false, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b10001110, 0b0, 0b0, 0b1 };
            op2 = new byte[4] { 0b00000110, 0b0, 0b0, 0b1 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00100000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда sub
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.SubNaive(op1, op2));
            CompareRP(new bool[4] { true, false, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b0, 0b0, 0b0, 0b1 };
            op2 = new byte[4] { 0b0, 0b0, 0b0, 0b1 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00100000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда sub
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.SubNaive(op1, op2));
            CompareRP(new bool[4] { false, true, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b0, 0b0, 0b0, 0b11 };
            op2 = new byte[4] { 0b10000000, 0b0, 0b0, 0b101 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00100000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда sub
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.SubNaive(op1, op2));
            CompareRP(new bool[4] { false, false, true, false });

            RestartHCM();

            op1 = new byte[4] { 0b11111110, 0b0, 0b0, 0b1 };
            op2 = new byte[4] { 0b11111111, 0b0, 0b0, 0b1 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00100000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда sub
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.SubNaive(op1, op2));
            CompareRP(new bool[4] { true, false, true, false });
        }

        [TestMethod]
        public void ImulTestSimple()
        {
            byte[] op1 = new byte[4] { 0b0, 0b0, 0b0, 0b10 },
                   op2 = new byte[4] { 0b0, 0b0, 0b0, 0b11 };

            _hcm.WriteData(op1, _dsAddress.Next()); // операнд 1: 2
            _hcm.WriteData(op2, _dsAddress.Next()); // операнд 2: 3
            _hcm.WriteData(new byte[4] { 0b00110000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда imul
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.ImulNaive(op1, op2));
        }

        // Проверка на переносы.
        [TestMethod]
        public void ImulTestWithCarrying()
        {
            byte[] op1 = new byte[4] { 0b0, 0b11000000, 0b11000000, 0b11000000 },
                   op2 = new byte[4] { 0b0, 0b0, 0b0, 0b10 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00110000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда imul
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.ImulNaive(op1, op2));

            RestartHCM();

            op1 = new byte[4] { 0b11111111, 0b11111111, 0b11111111, 0b11111110 };
            op2 = new byte[4] { 0b11111111, 0b0, 0b0, 0b1 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00110000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда imul
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.ImulNaive(op1, op2));
        }

        // Проверка флагов.
        [TestMethod]
        public void ImulTestWithRPChecking()
        {
            byte[] op1 = new byte[4] { 0b0, 0b0, 0b0, 0b10 },
                   op2 = new byte[4] { 0b0, 0b0, 0b0, 0b11 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00110000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда imul
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.ImulNaive(op1, op2));
            CompareRP(new bool[4] { false, false, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b00000000, 0b0, 0b0, 0b1 };
            op2 = new byte[4] { 0b10000000, 0b0, 0b0, 0b111 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00110000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда imul
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.ImulNaive(op1, op2));
            CompareRP(new bool[4] { true, false, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b0, 0b0, 0b0, 0b0 };
            op2 = new byte[4] { 0b0, 0b0, 0b0, 0b1 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00110000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда imul
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.ImulNaive(op1, op2));
            CompareRP(new bool[4] { false, true, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b10000000, 0b0, 0b1, 0b11111110 };
            op2 = new byte[4] { 0b11111111, 0b0, 0b0, 0b0 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00110000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда imul
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.ImulNaive(op1, op2));
            CompareRP(new bool[4] { false, false, true, false });

            RestartHCM();

            op1 = new byte[4] { 0b11111111, 0b11111111, 0b11111111, 0b11111111 };
            op2 = new byte[4] { 0b01111111, 0b0, 0b0, 0b0 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b00110000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда imul
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.ImulNaive(op1, op2));
            CompareRP(new bool[4] { true, false, true, false });
        }

        [TestMethod]
        public void IdivTestSimple()
        {
            byte[] op1 = new byte[4] { 0b0, 0b0, 0b0, 0b110 },
                   op2 = new byte[4] { 0b0, 0b0, 0b0, 0b11 };
            
            _hcm.WriteData(op1, _dsAddress.Next()); // операнд 1: 6
            _hcm.WriteData(op2, _dsAddress.Next()); // операнд 2: 3
            _hcm.WriteData(new byte[4] { 0b01000000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда idiv
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.IdivNaive(op1, op2));

            RestartHCM();

            op1 = new byte[4] { 0b11111111, 0b11111111, 0b11111111, 0b11100101 };
            op2 = new byte[4] { 0b00, 0b0, 0b0, 0b101 };

            _hcm.WriteData(op1, _dsAddress.Next()); // операнд 1: -27
            _hcm.WriteData(op2, _dsAddress.Next()); // операнд 2: 5
            _hcm.WriteData(new byte[4] { 0b01000000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда idiv
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.IdivNaive(op1, op2));

            /*
            op1 = new byte[2] { 0b11111111, 0b11100101 };
            op2 = new byte[2] { 0b0, 0b101 };

            (byte[] res, _) = BinaryOperators.Idiv(op1, op2);
            byte[] res2 = BinaryOperators.IdivNaive(op1, op2);
            Assert.IsTrue(Enumerable.SequenceEqual(res, res2));
            */
        }

        // Проверка на переносы.
        [TestMethod]
        public void IdivTestWithCarrying()
        {
            byte[] op1 = new byte[4] { 0b11, 0b11, 0b11, 0b10 },
                   op2 = new byte[4] { 0b0, 0b0, 0b0, 0b10 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01000000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда idiv
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.IdivNaive(op1, op2));

            RestartHCM();

            op1 = new byte[4] { 0b0, 0b0, 0b0, 0b00011011 };
            op2 = new byte[4] { 0b11111111, 0b11111111, 0b11111111, 0b11111011 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01000000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда idiv
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.IdivNaive(op1, op2));

            RestartHCM();

            op1 = new byte[4] { 0b11111111, 0b11111111, 0b11111111, 0b11100101 };
            op2 = new byte[4] { 0b00, 0b0, 0b0, 0b101 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01000000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда idiv
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.IdivNaive(op1, op2));

            RestartHCM();

            op1 = new byte[4] { 0b11111111, 0b11111111, 0b11111111, 0b11100101 };
            op2 = new byte[4] { 0b11111111, 0b11111111, 0b11111111, 0b11111011 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01000000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда idiv
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.IdivNaive(op1, op2));
        }

        // Проверка флагов.
        [TestMethod]
        public void IdivTestWithRPChecking()
        {
            byte[] op1 = new byte[4] { 0b0, 0b0, 0b0, 0b110 },
                   op2 = new byte[4] { 0b0, 0b0, 0b0, 0b11 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01000000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда idiv
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.IdivNaive(op1, op2));
            CompareRP(new bool[4] { false, false, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b11111111, 0b11111111, 0b11111111, 0b11110111 };
            op2 = new byte[4] { 0b0, 0b0, 0b0, 0b101 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01000000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда idiv
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.IdivNaive(op1, op2));
            CompareRP(new bool[4] { true, false, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b0, 0b0, 0b0, 0b0 };
            op2 = new byte[4] { 0b0, 0b1, 0b0, 0b1 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01000000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда idiv
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.IdivNaive(op1, op2));
            CompareRP(new bool[4] { false, true, false, false });

            // Проверка на аварийный останов.

            RestartHCM();

            op1 = new byte[4] { 0b0, 0b0, 0b0, 0b1 };
            op2 = new byte[4] { 0b0, 0b0, 0b0, 0b0 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01000000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда idiv
            _program.RunTilEnd();
            CompareRP(new bool[4] { false, false, true, false });
        }

        [TestMethod]
        public void AndTestSimple()
        {
            byte[] op1 = new byte[4] { 0b01100110, 0b00110011, 0b11001100, 0b10011001 },
                   op2 = new byte[4] { 0b11001100, 0b11001100, 0b01100110, 0b00110011 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01010000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда and
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.AndNaive(op1, op2));
        }

        // Проверка флагов.
        [TestMethod]
        public void AndTestWithRPChecking()
        {
            byte[] op1 = new byte[4] { 0b01100110, 0b00110011, 0b11001100, 0b10011001 },
                   op2 = new byte[4] { 0b11001100, 0b11001100, 0b01100110, 0b00110011 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01010000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда and
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.AndNaive(op1, op2));
            CompareRP(new bool[4] { true, false, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b0, 0b0, 0b0, 0b0 };
            op2 = new byte[4] { 0b1, 0b1, 0b1, 0b1 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01010000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда and
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.AndNaive(op1, op2));
            CompareRP(new bool[4] { false, true, false, false });
        }

        [TestMethod]
        public void OrTestSimple()
        {
            byte[] op1 = new byte[4] { 0b01100110, 0b00110011, 0b11001100, 0b10011001 },
                   op2 = new byte[4] { 0b11001100, 0b11001100, 0b01100110, 0b00110011 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01100000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда or
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.OrNaive(op1, op2));
        }

        // Проверка флагов.
        [TestMethod]
        public void OrTestWithRPChecking()
        {
            byte[] op1 = new byte[4] { 0b01100110, 0b00110011, 0b11001100, 0b10011001 },
                   op2 = new byte[4] { 0b01001100, 0b11001100, 0b01100110, 0b00110011 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01100000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда or
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.OrNaive(op1, op2));
            CompareRP(new bool[4] { true, false, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b0, 0b0, 0b0, 0b0 };
            op2 = new byte[4] { 0b0, 0b0, 0b0, 0b0 };

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(op2, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01100000, 0b00000000, 0b00000001, 0b00000010 }, _csAddress.Next()); // команда or
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.OrNaive(op1, op2));
            CompareRP(new bool[4] { false, true, false, false });
        }

        [TestMethod]
        public void SalTestSimple()
        {
            byte[] op1 = new byte[4] { 0b0, 0b0, 0b0, 0b10 };
            byte op2 = 0b11;

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01110000, 0b00000000, op2, 0b00000010 }, _csAddress.Next()); // команда sal
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.SalNaive(op1, new byte[] { op2 }));
        }

        // Проверка на переносы.
        [TestMethod]
        public void SalTestWithCarrying()
        {
            byte[] op1 = new byte[4] { 0b0, 0b01100000, 0b01100000, 0b01100000 };
            byte op2 = 0b10;

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01110000, 0b00000000, op2, 0b00000010 }, _csAddress.Next()); // команда sal
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.SalNaive(op1, new byte[] { op2 }));

            RestartHCM();

            op1 = new byte[4] { 0b11111111, 0b11101111, 0b11101111, 0b11101110 };
            op2 = 0b1100;

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01110000, 0b00000000, op2, 0b00000010 }, _csAddress.Next()); // команда sal
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.SalNaive(op1, new byte[] { op2 }));
        }

        // Проверка флагов.
        [TestMethod]
        public void SalTestWithRPChecking()
        {
            byte[] op1 = new byte[4] { 0b0, 0b0, 0b0, 0b10 };
            byte op2 = 0b11;

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01110000, 0b00000000, op2, 0b00000010 }, _csAddress.Next()); // команда sal
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.SalNaive(op1, new byte[] { op2 }));
            CompareRP(new bool[4] { true, false, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b11111111, 0b11111111, 0b11111111, 0b11111110 };
            op2 = 0b101;

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01110000, 0b00000000, op2, 0b00000010 }, _csAddress.Next()); // команда sal
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.SalNaive(op1, new byte[] { op2 }));
            CompareRP(new bool[4] { true, false, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b0, 0b0, 0b0, 0b1 };
            op2 = 0b00100000;

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01110000, 0b00000000, op2, 0b00000010 }, _csAddress.Next()); // команда sal
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.SalNaive(op1, new byte[] { op2 }));
            CompareRP(new bool[4] { false, true, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b11111111, 0b0, 0b0, 0b0 };
            op2 = 0b00001001;

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b01110000, 0b00000000, op2, 0b00000010 }, _csAddress.Next()); // команда sal
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.SalNaive(op1, new byte[] { op2 }));
            CompareRP(new bool[4] { false, true, false, false });
        }

        [TestMethod]
        public void SarTestSimple()
        {
            byte[] op1 = new byte[4] { 0b0, 0b0, 0b0, 0b110 };
            byte op2 = 0b1;

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b10000000, 0b00000000, op2, 0b00000010 }, _csAddress.Next()); // команда sar
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.SarNaive(op1, new byte[] { op2 }));
        }

        // Проверка на переносы.
        [TestMethod]
        public void SarTestWithCarrying()
        {
            byte[] op1 = new byte[4] { 0b110, 0b110, 0b110, 0b0 };
            byte op2 = 0b10;

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b10000000, 0b00000000, op2, 0b00000010 }, _csAddress.Next()); // команда sar
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.SarNaive(op1, new byte[] { op2 }));

            RestartHCM();

            op1 = new byte[4] { 0b110, 0b110, 0b110, 0b0 };
            op2 = 0b1100;

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b10000000, 0b00000000, op2, 0b00000010 }, _csAddress.Next()); // команда sar
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, BinaryOperators.SarNaive(op1, new byte[] { op2 }));
        }

        // Проверка флагов.
        [TestMethod]
        public void SarTestWithRPChecking()
        {
            byte[] op1 = new byte[4] { 0b0, 0b0, 0b0, 0b110 };
            byte op2 = 0b1;

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b10000000, 0b00000000, op2, 0b00000010 }, _csAddress.Next()); // команда sar
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.SarNaive(op1, new byte[] { op2 }));
            CompareRP(new bool[4] { true, false, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b11111111, 0b0, 0b0, 0b0 };
            op2 = 0b00010111;

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b10000000, 0b00000000, op2, 0b00000010 }, _csAddress.Next()); // команда sar
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.SarNaive(op1, new byte[] { op2 }));
            CompareRP(new bool[4] { true, false, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b1, 0b0, 0b0, 0b0 };
            op2 = 0b00100000;

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b10000000, 0b00000000, op2, 0b00000010 }, _csAddress.Next()); // команда sar
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.SarNaive(op1, new byte[] { op2 }));
            CompareRP(new bool[4] { false, true, false, false });

            RestartHCM();

            op1 = new byte[4] { 0b0, 0b0, 0b0, 0b11111111 };
            op2 = 0b00001001;

            _hcm.WriteData(op1, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b10000000, 0b00000000, op2, 0b00000010 }, _csAddress.Next()); // команда sar
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            CompareDataInDS(2, BinaryOperators.SarNaive(op1, new byte[] { op2 }));
            CompareRP(new bool[4] { false, true, false, false });
        }

        [TestMethod]
        public void CallRetTestSimple()
        {

            byte callCmdAddr = (byte)_csAddress.Next(),
                 salCmdAddr = (byte)_csAddress.Next(),
                 haltCmdAddr = (byte)_csAddress.Next(),
                 addCmdAddr = (byte)_csAddress.Next();

            _hcm.WriteData(new byte[4] { 0b0, 0b0, 0b0, 0b10 }, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b0, 0b0, 0b0, 0b11 }, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b10100000, addCmdAddr, 0b0, 0b1010 }, callCmdAddr); // команда call
            _hcm.WriteData(new byte[4] { 0b01110000, 0b00000010, 0b00000001, 0b00000010 }, salCmdAddr); // команда sal
            _hcm.WriteData(new byte[4], haltCmdAddr); // команда halt (останов)
            _hcm.WriteData(new byte[4] { 0b00010000, 0b00000000, 0b00000001, 0b00000010 }, addCmdAddr); // команда add
            _hcm.WriteData(new byte[4] { 0b10010000, 0b0, 0b0, 0b1010 }, _csAddress.Next()); // команда ret
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, new byte[4] { 0b0, 0b0, 0b0, 0b1010 });
        }

        [TestMethod]
        public void CallRetTestWithRecursion()
        {
            /*
             * Проверка на то, что 
             * 1) call действительно куда-то переходит
             * 2) переходит на нужную команду
             * 3) ret возвращает передачу управления на нужную команду
             */

            byte callCmd1Addr = (byte)_csAddress.Next(),
                 salCmdAddr = (byte)_csAddress.Next(),
                 haltCmdAddr = (byte)_csAddress.Next(),
                 callCmd2Addr = (byte)_csAddress.Next(),
                 orCmdAddr = (byte)_csAddress.Next(),
                 retCmd1Addr = (byte)_csAddress.Next(),
                 addCmdAddr = (byte)_csAddress.Next();

            _hcm.WriteData(new byte[4] { 0b0, 0b0, 0b0, 0b10 }, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b0, 0b0, 0b0, 0b11 }, _dsAddress.Next());
            _hcm.WriteData(new byte[4], _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b0, 0b1, 0b0, 0b0 }, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b10100000, callCmd2Addr, 0b0, 0b1010 }, callCmd1Addr); // команда call (1)
            _hcm.WriteData(new byte[4] { 0b01110000, 0b00000010, 0b00000001, 0b00000010 }, salCmdAddr); // команда sal
            _hcm.WriteData(new byte[4], haltCmdAddr); // команда halt (останов)
            _hcm.WriteData(new byte[4] { 0b10100000, addCmdAddr, 0b0, 0b1011 }, callCmd2Addr); // команда call (2)
            _hcm.WriteData(new byte[4] { 0b01100000, 0b00000010, 0b00000011, 0b00000010 }, orCmdAddr); // команда or
            _hcm.WriteData(new byte[4] { 0b10010000, 0b0, 0b0, 0b1010 }, retCmd1Addr); // команда ret (1)
            _hcm.WriteData(new byte[4] { 0b00010000, 0b00000000, 0b00000001, 0b00000010 }, addCmdAddr); // команда add
            _hcm.WriteData(new byte[4] { 0b10010000, 0b0, 0b0, 0b1011 }, _csAddress.Next()); // команда ret (2)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, new byte[4] { 0b0, 0b10, 0b0, 0b1010 });
        }

        [TestMethod]
        public void BranchJmpTestSimple()
        {
            byte addCmdAddr = (byte)_csAddress.Next(),
                 branchCmd1Addr = (byte)_csAddress.Next(),
                 salCmdAddr = (byte)_csAddress.Next(),
                 jmpCmdAddr = (byte)_csAddress.Next(),
                 sarCmdAddr = (byte)_csAddress.Next(),
                 haltCmdAddr = (byte)_csAddress.Next();

            _hcm.WriteData(new byte[4] { 0b0, 0b0, 0b0, 0b10 }, _dsAddress.Next()); // операнд 1: 2
            _hcm.WriteData(new byte[4] { 0b0, 0b0, 0b0, 0b11 }, _dsAddress.Next()); // операнд 2: 3
            _hcm.WriteData(new byte[4] { 0b00010000, 0b00000000, 0b00000001, 0b00000010 }, addCmdAddr); // команда add
            _hcm.WriteData(new byte[4] { 0b10110000, sarCmdAddr, haltCmdAddr, salCmdAddr }, branchCmd1Addr); // команда branch
            _hcm.WriteData(new byte[4] { 0b01110000, 0b00000010, 0b00000001, 0b00000010 }, salCmdAddr); // команда add
            _hcm.WriteData(new byte[4] { 0b11000000, haltCmdAddr, 0b0, 0b0 }, jmpCmdAddr); // команда jmp
            _hcm.WriteData(new byte[4] { 0b10000000, 0b00000010, 0b00000001, 0b00000010 }, sarCmdAddr); // команда sar
            _hcm.WriteData(new byte[4], haltCmdAddr); // команда halt (останов)

            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, new byte[4] { 0b0, 0b0, 0b0, 0b1010 });

            RestartAddressSpaces();

            _hcm.WriteData(new byte[4] { 0b11111111, 0b11111111, 0b11111111, 0b11111110 }, _dsAddress.Next()); // операнд 1: -2
            _hcm.WriteData(new byte[4] { 0b11111111, 0b11111111, 0b11111111, 0b11111101 }, _dsAddress.Next()); // операнд 2: -3

            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, new byte[4] { 0b11111111, 0b11111111, 0b11111111, 0b11111101 });

            RestartAddressSpaces();

            _hcm.WriteData(new byte[4] { 0b0, 0b0, 0b0, 0b10 }, _dsAddress.Next()); // операнд 1: 2
            _hcm.WriteData(new byte[4] { 0b11111111, 0b11111111, 0b11111111, 0b11111110 }, _dsAddress.Next()); // операнд 2: -2

            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInDS(2, new byte[4]);
        }

        [TestMethod]
        public void EndWhileTest()
        {
            _dsAddress.Next();
            _dsAddress.NextOverNTimes(9);
            for (byte i = 1; i <= 10; i++)
                _hcm.WriteData(new byte[4] { 0b0, 0b0, 0b0, i }, _dsAddress.Next());
            _dsAddress.NextOverNTimes(10);
            _hcm.WriteData(new byte[4] { 0b00001001, 0b00000001, 0b0, 0b00000001 }, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b0, 0b0, 0b0, 0b00001111 }, _dsAddress.Next());

            _hcm.WriteData(new byte[4] { 0b11110001, 0b00011110, 0b0, 0b0 }, _csAddress.Next()); // команда load to cache
            _hcm.WriteData(new byte[4] { 0b01110001, 0b00001001, 0b00000100, 0b00010011 }, _csAddress.Next()); // команда sal
            _hcm.WriteData(new byte[4] { 0b00010001, 0b00010011, 0b00011111, 0b00010011 }, _csAddress.Next()); // команда add
            _hcm.WriteData(new byte[4] { 0b11010001, 0b00011110, 0b00101001, 0b00101100 }, _csAddress.Next()); // команда end while
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            for (int i = 1; i <= 10; i++)
                CompareDataInDS(i + 19, new byte[4] { 0b0, 0b0, 0b0, (byte)((i << 4) + 15) });
        }

        [TestMethod]
        public void CacheLoadingAndSavingTestSimple()
        {
            _hcm.WriteData(new byte[4] { 0b0, 0b0, 0b0, 0b1111 }, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b11110001, 0b0, 0b0, 0b0 }, _csAddress.Next()); // команда load to cache
            _hcm.WriteData(new byte[4] { 0b11100001, 0b1, 0b0, 0b0 }, _csAddress.Next()); // команда save cache
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInCache(1, new byte[4] { 0b0, 0b0, 0b0, 0b1111 });
            CompareDataInDS(1, new byte[4] { 0b0, 0b0, 0b0, 0b1111 });
        }

        [TestMethod]
        public void CacheLoadingAndSavingTestWithChecking0CellUnavalaible()
        {
            _hcm.WriteData(new byte[4] { 0b0, 0b0, 0b0, 0b1111 }, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b11110000, 0b0, 0b0, 0b0 }, _csAddress.Next()); // команда load to cache
            _hcm.WriteData(new byte[4] { 0b11100000, 0b1, 0b0, 0b0 }, _csAddress.Next()); // команда save cache
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();
            Assert.IsTrue(_program.EOF);
            CompareDataInCache(0, new byte[4]);
            CompareDataInDS(1, new byte[4]);
        }
    }
}