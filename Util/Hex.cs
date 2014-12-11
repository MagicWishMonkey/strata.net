using System;
using System.Text;
namespace Strata.Util {
    public sealed class Hex {
        #region -------- VARIABLES AND CONSTRUCTOR(S) --------
        private static byte[] highDigits;
        private static byte[] lowDigits;
        static Hex() {
            byte[] digits = { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8',
                                (byte)'9', (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F' };
            int i;
            byte[] high = new byte[256];
            byte[] low = new byte[256];

            for (i = 0; i < 256; i++) {
                high[i] = digits[i >> 4];
                low[i] = digits[i & 0x0F];
            }

            highDigits = high;
            lowDigits = low;
        }
        #endregion

        #region -------- PUBLIC - ToString --------
        public static string ToString(byte[] data) {
            if (data == null || data.Length == 0) return "";
            int size = data.Length;
            char[] chars = new char[size * 2];
            int ix = 0;
            for (int i = 0; i < size; i++) {
                int val = data[i] & 0xFF;
                chars[ix++] = (char)highDigits[val];
                chars[ix++] = (char)lowDigits[val];
            }
            string txt = new string(chars);
            return txt;
        }
        #endregion

        #region -------- PUBLIC - GenerateHexDump --------
        public static string GenerateHexDump(byte[] data) {
            if (data == null || data.Length == 0)
                return "";
            int size = data.Length;
            //ByteBuffer buffer = new ByteBuffer(data);
            System.IO.MemoryStream buffer = new System.IO.MemoryStream(data);
            //long remaining = (buffer.Length - buffer.Position);
            string ascii = "";
            //StringBuilder sb = new StringBuilder((buffer.Remaining * 3) - 1);
            StringBuilder sb = new StringBuilder(((int)(buffer.Length - buffer.Position) * 3) - 1);
            System.IO.StringWriter writer = new System.IO.StringWriter(sb);
            int lineCount = 0;
            for (int i = 0; i < size; i++) {
                int val = buffer.ReadByte() & 0xFF;
                writer.Write((char)highDigits[val]);
                writer.Write((char)lowDigits[val]);
                writer.Write(" ");
                ascii += GetAsciiEquivalent(val) + " ";
                lineCount++;
                if (i == 0)
                    continue;
                if ((i + 1) % 8 == 0)
                    writer.Write("  ");
                if ((i + 1) % 16 == 0) {
                    writer.Write(" ");
                    writer.Write(ascii);
                    writer.WriteLine();
                    ascii = "";
                    lineCount = 0;
                } else if (i == size - 1) {///HALF-ASSED ATTEMPT TO GET THE LAST LINE OF ASCII TO LINE UP CORRECTLY
                    //while(lineCount < 84) {
                    //    writer.Write(" ");
                    //    lineCount++;
                    //}
                    for (int y = lineCount; y < 25; y++) {
                        writer.Write(" ");
                    }
                    writer.Write(ascii);
                    writer.WriteLine();
                }
            }
            return sb.ToString();
        }
        #endregion

        #region -------- PRIVATE - GetAsciiEquivalent --------
        private static string GetAsciiEquivalent(int val) {
            if (val > 30 && val < 130)
                return Convert.ToString((char)val);
            return ".";
        }
        #endregion
    }
}