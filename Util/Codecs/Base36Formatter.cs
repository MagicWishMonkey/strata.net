using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strata.Util.Codecs {
    public static class Base36Formatter {
        internal static readonly string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static string Encode(int num) {
            var chars = Base36Formatter.characters;
            var buffer = "";
            while (num > 0) {
                int i;
                num = Math.DivRem(num, 36, out i);
                buffer = chars[i] + buffer;
            }
            return buffer;
        }

        public static int Decode(string data) {
            var chars = Base36Formatter.characters;
            int result = 0;
            var reversed = data.ToUpper().Reverse();
            int pos = 0;
            foreach (char c in reversed) {
                result += chars.IndexOf(c) * (int)Math.Pow(36, pos);
                pos++;
            }
            return result;
        }
    }
}
