using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace HypotheticalComputingMachineClassLib.HCMModel_D1
{
    internal class HypotheticalComputingMachineD1ModelDeserializer
    {
        private static Regex _paDefinitionRX = new Regex(@"^\[\s?PA\s?=\s?(?<pa>\d{1,3})\s?\]\s*$", RegexOptions.Compiled);

        private static Regex _segEndDefinitionRX = new Regex(@"^\s?\}\s*$", RegexOptions.Compiled);

        private static Regex _byteStrDefinitionRX = new Regex(@"^\s*(?<i>\d{1,3})\s?\|(\s(?<byte>[01]{8})){4}\s*$", RegexOptions.Compiled);

        private static Regex _dsBufferBeginDefinitionRX = new Regex(@"^\{\s?DS\s?:\s*$", RegexOptions.Compiled);

        private static Regex _cacheBufferBeginDefinitionRX = new Regex(@"^\{\s?Cache\s?:\s*$", RegexOptions.Compiled);

        public void LoadFromFile(HypotheticalComputingMachineD1Model hcm, Stream configStream)
        {
            byte pa;
            byte[][] dsBuffer, cacheBuffer;

            StreamReader swConfig;
            using (swConfig = new StreamReader(configStream))
            {
                ReadPA();
                ReadDS();
                ReadCache();
            }

            IHCMDataSegment ds = hcm.DS, cache = hcm.Cache;
            hcm.CS.StartCSFrom(pa);
            for (int i = 0; i <= 255; i++)
                hcm.DS.WriteToMemCell(i, dsBuffer[i], false);
            for (int i = 1; i <= 15; i++)
                hcm.Cache.WriteToMemCell(i, cacheBuffer[i], false);

            return;

            void ReadPA()
            {
                string paStr = swConfig.ReadLine();
                if (paStr is null) CatchError("Отсутствует блок PA!");
                Match paMatch = _paDefinitionRX.Match(paStr);
                if (!paMatch.Success) CatchError("Ошибка: блок PA не считан!");
                pa = byte.Parse(paMatch.Groups["pa"].Captures[0].Value);

                return;
            }

            void ReadDS()
            {
                string dsBufferErrorStr = "Ошибка: блок DS не считан!", dsBufferBeginStr = swConfig.ReadLine();
                if (dsBufferBeginStr is null) CatchError("Отсутствует объявление блока DS!");
                Match dsBufferBeginMatch = _dsBufferBeginDefinitionRX.Match(dsBufferBeginStr);
                if (!dsBufferBeginMatch.Success) CatchError(dsBufferErrorStr);

                dsBuffer = new byte[256][];
                for (int i = 0; i <= 255; i++)
                {
                    ReadByteStr(i, "DS", dsBuffer);
                }

                string segEndErrorStr = "Отсутствует завершение блока DS!", segEndStr = swConfig.ReadLine();
                if (segEndStr is null) CatchError(segEndErrorStr);
                Match segEndMatch = _segEndDefinitionRX.Match(segEndStr);
                if (!segEndMatch.Success) CatchError(segEndErrorStr);

                return;
            }

            void ReadCache()
            {
                string cacheBufferErrorStr = "Ошибка: блок Cache не считан!", cacheBufferBeginStr = swConfig.ReadLine();
                if (cacheBufferBeginStr is null) CatchError("Отсутствует объявление блока Cache!");
                Match cacheBufferBeginMatch = _cacheBufferBeginDefinitionRX.Match(cacheBufferBeginStr);
                if (!cacheBufferBeginMatch.Success) CatchError(cacheBufferErrorStr);

                cacheBuffer = new byte[16][];
                for (int i = 1; i <= 15; i++)
                {
                    ReadByteStr(i, "Cache", cacheBuffer);
                }

                string segEndErrorStr = "Отсутствует завершение блока Cache!", segEndStr = swConfig.ReadLine();
                if (segEndStr is null) CatchError(segEndErrorStr);
                Match segEndMatch = _segEndDefinitionRX.Match(segEndStr);
                if (!segEndMatch.Success) CatchError(segEndErrorStr);

                return;
            }

            void ReadByteStr(int i, string blockName, byte[][] seg)
            {
                string bytesStr = swConfig.ReadLine();
                if (bytesStr is null) CatchError($"{blockName}: отсутствует {i}-я строка байтов!");
                Match bytesMatch = _byteStrDefinitionRX.Match(bytesStr);
                if (!bytesMatch.Success) CatchError($"{blockName}: {i}-я строка байтов не считана!");
                int statedI = int.Parse(bytesMatch.Groups["i"].Captures[0].Value);
                if (statedI != i) CatchError($"{blockName}: {statedI}-я строка байтов стоит на месте {i}-ой!");
                seg[i] = ParseBytes(bytesMatch);

                return;
            }

            byte[] ParseBytes(Match bytesMatch)
            {
                byte[] bytes = new byte[4];
                CaptureCollection bytesCaptures = bytesMatch.Groups["byte"].Captures;
                byte bCoef;

                for (int i = 0; i <= 3; i++)
                {
                    bytes[i] = BinaryInOutConverter.BinaryStrToByte(bytesCaptures[i].Value);
                }

                return bytes;
            }
        }

        private void CatchError(string message)
        {
            throw new InvalidDataException(message);
        }
    }
}
