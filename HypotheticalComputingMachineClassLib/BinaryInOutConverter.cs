using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineClassLib
{
    public static class BinaryInOutConverter
    {
        public static string ByteToBinaryStr(byte b)
        {
            string binaryStr = "";
            byte i;

            for (i = 0; b > 0; i++)
            {
                binaryStr = $"{b & 0b1}" + binaryStr;
                b >>= 1;
            }
            for (; i < 8; i++)
                binaryStr = '0' + binaryStr;

            return binaryStr;
        }

        public static byte BinaryStrToByte(string binaryStr)
        {
            byte binaryCoef = 0b1, translatedByte = 0;

            for (int i = 7; i >= 0; i--)
            {
                if (binaryStr[i] == '1') translatedByte += binaryCoef;
                binaryCoef <<= 1;
            }

            return translatedByte;
        }

        public static string BytesToLongBinaryStr(byte[] bytes, char separator)
        {
            string lbs = "";
            int bytesLen = bytes.Length, truncatedBytesLen = bytesLen - 1;

            for (int i = 0; i < bytesLen; i++)
            {
                lbs += ByteToBinaryStr(bytes[i]);
                if (i < truncatedBytesLen)
                    lbs += separator;
            }

            return lbs;
        }

        public static byte[] LongBinaryStrToBytes(string lbs, int bytesLen)
        {
            byte[] bytes = new byte[bytesLen];
            bytesLen -= 1;

            for (int i = 0; i < bytesLen; i++)
            {
                bytes[i] = BinaryStrToByte(lbs[0..8]);
                lbs = lbs[9..];
            }
            bytes[bytesLen] = BinaryStrToByte(lbs);

            return bytes;
        }

        public static int LongBinaryStrToInt32(string lbs, int bytesLen = 4)
        {
            int integer = 0;
            bytesLen = bytesLen - 1;

            for (int i = 0; i < bytesLen; i++)
            {
                integer += BinaryStrToByte(lbs[0..8]) << (8 * (bytesLen - i));
                lbs = lbs[9..];
            }
            integer += BinaryStrToByte(lbs);

            return integer;
        }

        public static int BytesToInt32(byte[] bytes)
        {
            long res = 0;
            int len = bytes.Length - 1;

            for (int i = len; i >= 0; i--)
            {
                res += bytes[i] << ((len - i) * 8);
            }
            res = bytes.Length switch
            {
                1 => (sbyte)res,
                2 => (short)res,
                4 => (int)res,
                8 => res,
            };

            return (int)res;
        }

        public static byte[] Int32ToBytes(int data, int byteCount = 4)
        {
            byte[] bytes = new byte[byteCount];
            byteCount -= 1;

            for (int i = byteCount; i >= 0; i--)
            {
                bytes[i] = (byte)(data >> ((byteCount - i) * 8));
            }

            return bytes;
        }

        public static string Int32ToLongBinaryStr(int data, int byteCount = 4)
        {
            byte curByte;
            string binaryStr = "";
            byteCount -= 1;

            for (int i = byteCount; i >= 0; i--)
            {
                curByte = (byte)(data >> ((byteCount - i) * 8));
                binaryStr += ByteToBinaryStr(curByte) + " "; 
            }
            binaryStr = binaryStr.Remove(binaryStr.Length - 1);

            return binaryStr;
        }
    }
}
