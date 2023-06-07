using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineClassLib
{
    public class BinaryOperators
    {
        private static byte[] NaiveBinaryOperator(byte[] op1Bytes, byte[] op2Bytes, Func<int, int, int> binaryOperator)
        {
            int op1 = BinaryInOutConverter.BytesToInt32(op1Bytes),
                op2 = BinaryInOutConverter.BytesToInt32(op2Bytes),
                res = binaryOperator(op1, op2);

            return BinaryInOutConverter.Int32ToBytes(res);
        }

        public static byte[] AddNaive(byte[] op1Bytes, byte[] op2Bytes)
        {
            return NaiveBinaryOperator(op1Bytes, op2Bytes, (op1, op2) => op1 + op2);
        }

        public static byte[] SubNaive(byte[] op1Bytes, byte[] op2Bytes)
        {
            return NaiveBinaryOperator(op1Bytes, op2Bytes, (op1, op2) => op1 - op2);
        }

        public static byte[] ImulNaive(byte[] op1Bytes, byte[] op2Bytes)
        {
            return NaiveBinaryOperator(op1Bytes, op2Bytes, (op1, op2) => op1 * op2);
        }

        public static byte[] IdivNaive(byte[] op1Bytes, byte[] op2Bytes)
        {
            return NaiveBinaryOperator(op1Bytes, op2Bytes, (op1, op2) => op1 / op2);
        }

        public static byte[] AndNaive(byte[] op1Bytes, byte[] op2Bytes)
        {
            return NaiveBinaryOperator(op1Bytes, op2Bytes, (op1, op2) => op1 & op2);
        }

        public static byte[] OrNaive(byte[] op1Bytes, byte[] op2Bytes)
        {
            return NaiveBinaryOperator(op1Bytes, op2Bytes, (op1, op2) => op1 | op2);
        }

        public static byte[] SalNaive(byte[] op1Bytes, byte[] op2Bytes)
        {
            return NaiveBinaryOperator(op1Bytes, op2Bytes, (op1, op2) => op2 < 32 ? op1 << op2 : 0);
        }

        public static byte[] SarNaive(byte[] op1Bytes, byte[] op2Bytes)
        {
            return NaiveBinaryOperator(op1Bytes, op2Bytes, (op1, op2) => op2 < 32 ? op1 >> op2 : (op1 >= 0 ? 0 : ((2 << 31) - 1 + 2 << 31)));
        }

        public static byte[] Complement(byte[] op)
        {
            (byte[] res, _) = Sub(new byte[op.Length], op);

            return res;
        }

        public static byte[] And(byte[] op1, byte[] op2)
        {
            byte[] res = new byte[op1.Length];

            for (int i = 0; i < op1.Length; i++)
            {
                res[i] = (byte)(op1[i] & op2[i]);
            }

            return res;
        }

        public static byte[] Or(byte[] op1, byte[] op2)
        {
            byte[] res = new byte[op1.Length];

            for (int i = 0; i < op1.Length; i++)
            {
                res[i] = (byte)(op1[i] | op2[i]);
            }

            return res;
        }

        public static byte[] Sal(byte[] op1, int shiftTimes)
        {
            byte[] res = new byte[op1.Length];
            byte op1Byte, resByte, carry = 0;
            int j = op1.Length - 1;
            bool resSign = op1[0] >> 7 == 1;

            while (shiftTimes >= 8)
            {
                shiftTimes -= 8;
                j--;
            }

            for (int i = op1.Length - 1; j >= 0; i--, j--)
            {
                op1Byte = op1[i];
                resByte = (byte)(carry + (op1Byte << shiftTimes));
                carry = (byte)(op1Byte >> (8 - shiftTimes));
                res[j] += resByte;
            }

            return res;
        }

        public static byte[] Sar(byte[] op1, int shiftTimes)
        {
            byte[] res = new byte[op1.Length];
            byte op1Byte, resByte, carry = 0;
            int j = 0;
            bool resSign = op1[0] >> 7 == 1;

            while (shiftTimes >= 8)
            {
                res[j] = resSign ? (byte)255 : (byte)0;
                shiftTimes -= 8;
                j++;
            }
            if (resSign && shiftTimes > 0)
                res[j] = (byte)(((1 << shiftTimes) - 1) << (8 - shiftTimes));

            for (int i = 0; j < op1.Length; i++, j++)
            {
                op1Byte = op1[i];
                resByte = (byte)(carry + (op1Byte >> shiftTimes));
                carry = (byte)(op1Byte << (8 - shiftTimes));
                res[j] += resByte;
            }

            return res;
        }

        public static (byte[] Result, byte Carry) Add(byte[] op1, byte[] op2)
        {
            byte[] res = new byte[op1.Length];
            byte op1Byte, op2Byte, resByte, carry = 0;

            for (int i = op1.Length - 1; i >= 0; i--)
            {
                for (int j = 0; j < 8; j++)
                {
                    (op1Byte, op2Byte) = ((byte)((op1[i] >> j) & 1), (byte)((op2[i] >> j) & 1));
                    resByte = (byte)(op1Byte + op2Byte + carry);
                    carry = (byte)(resByte >> 1);
                    resByte &= 0b1;
                    res[i] += (byte)(resByte << j);
                }
            }

            return (res, carry);
        }

        public static (byte[] Result, byte Borrow) Sub(byte[] op1, byte[] op2)
        {
            byte[] res = new byte[op1.Length];
            byte op1Byte, op2Byte, resByte, borrow = 0;

            for (int i = op1.Length - 1; i >= 0; i--)
            {
                for (int j = 0; j < 8; j++)
                {
                    (op1Byte, op2Byte) = ((byte)((op1[i] >> j) & 1), (byte)((op2[i] >> j) & 1));
                    resByte = (byte)(op1Byte - op2Byte + borrow);
                    borrow = (byte)((sbyte)resByte >> 1);
                    resByte &= 0b1;
                    res[i] += (byte)(resByte << j);
                }
            }

            return (res, borrow == 255 ? (byte)1 : (byte)0);
        }

        public static (byte[] Result, byte Carry) Imul(byte[] op1, byte[] op2)
        {
            byte[] shiftedOp1 = new byte[op1.Length], res = new byte[op1.Length];
            byte op1Byte, op2Byte, resByte, carry = 0, auxCarry;
            int shiftTimes = 0;

            op1.CopyTo(shiftedOp1, 0);
            for (int i = op1.Length - 1; i >= 0; i--)
            {
                for (int j = 0; j < 8; j++)
                {
                    op2Byte = (byte)((op2[i] >> j) & 1);
                    if (op2Byte == 0b1)
                    {
                        shiftedOp1 = Sal(shiftedOp1, shiftTimes);
                        shiftTimes = 1;
                        (res, auxCarry) = Add(res, shiftedOp1);
                        // Флаг переполнения устанавливется, если хоть раз переполнение имело место.
                        if (carry != 0b1) carry = auxCarry;
                    }
                    else
                        shiftTimes++;
                }
            }
            // Исправление знака.
            res[0] = (byte)((res[0] & 0b01111111) | ((op1[0] ^ op2[0]) & 0b10000000));

            return (res, carry);
        }

        // Используется алгоритм деления А (операнд 1 расширяется в два раза и сдвигается в последние байты,
        // операнд 2 расширяется, но последние байты заполняются нулями, результат каждой операции сдвигается влево на 1 разряд).
        public static (byte[] Result, byte[] Mod) Idiv(byte[] op1, byte[] op2)
        {
            if (op2.All(b => b == 0)) throw new DivideByZeroException();

            byte[] extdOp1 = new byte[op1.Length << 1], extdOp2 = new byte[op2.Length << 1], res = new byte[op1.Length];
            byte op1Sign,
                 extdOp1Sign = op1[0],
                 op2Sign = op2[0],
                 resSign;

            op1Sign = extdOp1Sign;
            op1.CopyTo(extdOp1, 0);
            extdOp1 = Sar(extdOp1, op1.Length << 3);
            op2.CopyTo(extdOp2, 0);
            resSign = (byte)((extdOp1Sign ^ op2Sign) & 0b10000000);
            DecideAtomicOperation();
            for (int i = 0; i < res.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    extdOp1 = Sal(extdOp1, 1);
                    DecideAtomicOperation();
                    res[i] |= (byte)((~(op1Sign ^ extdOp1Sign) & 0b10000000) >> j);
                }
            }
            // Исправление знака.
            if (resSign != 0)
                res = Complement(res);

            return (res, extdOp1[0..op1.Length]);

            void DecideAtomicOperation()
            {
                if (((extdOp1Sign ^ op2Sign) & 0b10000000) != 0)
                    (extdOp1, _) = Add(extdOp1, extdOp2);
                else
                    (extdOp1, _) = Sub(extdOp1, extdOp2);
                extdOp1Sign = extdOp1[0];

                return;
            }
        }
    }
}
