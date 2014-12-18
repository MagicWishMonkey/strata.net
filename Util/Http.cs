using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
namespace Strata.Util {
    public class HttpRequest {
        private string _method = "GET";
        public HttpRequest(string url) {
            Uri uri;
            if(!Uri.TryCreate(url, UriKind.Absolute, out uri))
                throw new Exception("The url is not a valid uri!");
            this.Uri = uri;
        }
        public HttpRequest(Uri uri) {
            this.Uri = uri;
        }



        #region -------- PUBLIC - Execute --------
        public byte[] Execute(string method = null) {
            if (!String.IsNullOrEmpty(method))
                this.Method = method;
            if (String.IsNullOrEmpty(this.Method))
                this.Method = "GET";

            Console.WriteLine("[" + this.Method + "] " + this.Uri.AbsoluteUri);

            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.Uri);
                request.Method = this.Method;
                request.Timeout = (1000 * 60);
                request.AllowAutoRedirect = true;
                request.Accept = "*.*";
                //request.Credentials = CredentialCache.DefaultCredentials;


                //Accept:text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
                //Accept-Charset:ISO-8859-1,utf-8;q=0.7,*;q=0.3
                //Accept-Encoding:gzip,deflate,sdch
                //Accept-Language:en-US,en;q=0.8

                //if (this.Body != null && this.Body.Length > 0) {
                //    var stream = request.GetRequestStream();
                //    stream.WriteData(this.Body);
                //}

                IAsyncResult rez = request.BeginGetResponse(null, null);
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(rez);
                string ct = response.ContentType;
                byte[] data = ReadData(response.GetResponseStream());
                Console.WriteLine(data.Length + " bytes received");
                return data;
            } catch (Exception ex) {
                throw new Exception("Http Request Error-> " + ex.Message + "\rUrl:" + this.Uri.AbsoluteUri, ex);
            }
        }
        #endregion


        #region -------- PRIVATE HELPERS --------
        //private static void Redirect(Stream source, Stream destination) {
        //    byte[] block = new byte[2048];
        //    int offset = 0;
        //    int count = source.Read(block, 0, block.Length);
        //    while (count > 0) {
        //        destination.Write(block, 0, count);
        //        offset += count;
        //        block = new byte[1024];
        //        count = source.Read(block, 0, block.Length);
        //        if (count < block.Length)
        //            Array.Resize(ref block, count);
        //    }
        //    destination.Flush();
        //}

        private static byte[] ReadData(System.IO.Stream stream) {
            byte[] data = new byte[10240];

            System.IO.MemoryStream buffer = new MemoryStream();
            int size = stream.Read(data, 0, data.Length);
            while (size > 0) {
                int count = size;

                buffer.Write(data, 0, count);
                size = stream.Read(data, 0, data.Length);
            }
            data = buffer.ToArray();
            buffer.Dispose();
            return data;
        }
        #endregion




        #region -------- PROPERTIES --------
        public Uri Uri {
            get;
            set;
        }
        public string Method {
            get { return this._method; }
            set {
                if (value == null)
                    throw new Exception("The method value cannot be null.");

                var method = value.Trim().ToUpper();
                if (method == "GET")
                    this._method = method;
                else if (method == "POST")
                    this._method = "POST";
                else if (method == "HEAD")
                    this._method = "HEAD";
                else
                    throw new Exception("The specified method type is not supported: " + value);
            }
        }
        #endregion
    }

    public class Http {
        #region -------- VARIABLES & CONSTRUCTOR(S) --------
        private static char[] _entityEndingChars;
        static Http() {
            _entityEndingChars = new char[] { ';', '&' };
        }
        #endregion

        #region -------- PUBLIC - HtmlDecode --------
        public static string HtmlDecode(string s) {
            if (s == null) {
                return null;
            }
            if (s.IndexOf('&') < 0) {
                return s;
            }
            StringBuilder sb = new StringBuilder();
            StringWriter output = new StringWriter(sb);
            HtmlDecode(s, output);
            return sb.ToString();
        }

        public static void HtmlDecode(string s, TextWriter output) {
            if (s != null) {
                if (s.IndexOf('&') < 0) {
                    output.Write(s);
                } else {
                    int length = s.Length;
                    for (int i = 0; i < length; i++) {
                        char ch = s[i];
                        if (ch == '&') {
                            int num3 = s.IndexOfAny(_entityEndingChars, i + 1);
                            if ((num3 > 0) && (s[num3] == ';')) {
                                string entity = s.Substring(i + 1, (num3 - i) - 1);
                                if ((entity.Length > 1) && (entity[0] == '#')) {
                                    try {
                                        if ((entity[1] == 'x') || (entity[1] == 'X')) {
                                            ch = (char)int.Parse(entity.Substring(2), NumberStyles.AllowHexSpecifier);
                                        } else {
                                            ch = (char)int.Parse(entity.Substring(1));
                                        }
                                        i = num3;
                                    } catch (FormatException) {
                                        i++;
                                    } catch (ArgumentException) {
                                        i++;
                                    }
                                } else {
                                    i = num3;
                                    char ch2 = HtmlEntities.Lookup(entity);
                                    if (ch2 != '\0') {
                                        ch = ch2;
                                    } else {
                                        output.Write('&');
                                        output.Write(entity);
                                        output.Write(';');
                                        goto Label_0103;
                                    }
                                }
                            }
                        }
                        output.Write(ch);
                    Label_0103: ;
                    }
                }
            }
        }
        #endregion

        #region -------- PUBLIC - HtmlEncode --------
        public static string HtmlEncode(string s) {
            throw new NotImplementedException();
        }
        #endregion

        #region -------- PUBLIC - UrlDecode --------
        public static string UrlDecode(string s) {
            return UrlDecode(s, Encoding.UTF8);
        }

        public static string UrlDecode(string s, Encoding e) {
            if (String.IsNullOrEmpty(s))
                return s;
            return UrlDecodeStringFromStringInternal(s, e);
        }

        private static string UrlDecodeStringFromStringInternal(string s, Encoding e) {
            int length = s.Length;
            UrlDecoder decoder = new UrlDecoder(length, e);
            for (int i = 0; i < length; i++) {
                char ch = s[i];
                if (ch == '+') {
                    ch = ' ';
                } else if ((ch == '%') && (i < (length - 2))) {
                    if ((s[i + 1] == 'u') && (i < (length - 5))) {
                        int num3 = HexToInt(s[i + 2]);
                        int num4 = HexToInt(s[i + 3]);
                        int num5 = HexToInt(s[i + 4]);
                        int num6 = HexToInt(s[i + 5]);
                        if (((num3 < 0) || (num4 < 0)) || ((num5 < 0) || (num6 < 0))) {
                            goto Label_0106;
                        }
                        ch = (char)((((num3 << 12) | (num4 << 8)) | (num5 << 4)) | num6);
                        i += 5;
                        decoder.AddChar(ch);
                        continue;
                    }
                    int num7 = HexToInt(s[i + 1]);
                    int num8 = HexToInt(s[i + 2]);
                    if ((num7 >= 0) && (num8 >= 0)) {
                        byte b = (byte)((num7 << 4) | num8);
                        i += 2;
                        decoder.AddByte(b);
                        continue;
                    }
                }
            Label_0106:
                if ((ch & 0xff80) == 0) {
                    decoder.AddByte((byte)ch);
                } else {
                    decoder.AddChar(ch);
                }
            }
            return decoder.GetString();
        }

        private static int HexToInt(char h) {
            if ((h >= '0') && (h <= '9')) {
                return (h - '0');
            }
            if ((h >= 'a') && (h <= 'f')) {
                return ((h - 'a') + 10);
            }
            if ((h >= 'A') && (h <= 'F')) {
                return ((h - 'A') + 10);
            }
            return -1;
        }
        #endregion

        #region -------- PUBLIC - UrlEncode --------
        public static string UrlEncode(string s) {
            return UrlEncode(s, Encoding.UTF8);
        }
        public static string UrlEncode(string s, Encoding e) {
            if (s == null)
                return s;
            return Encoding.ASCII.GetString(UrlEncodeToBytes(s, e));
        }
        public static byte[] UrlEncodeToBytes(string str, Encoding e) {
            if (str == null) {
                return null;
            }
            byte[] bytes = e.GetBytes(str);
            return UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false);
        }
        private static byte[] UrlEncodeBytesToBytesInternal(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue) {
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < count; i++) {
                char ch = (char)bytes[offset + i];
                if (ch == ' ') {
                    num++;
                } else if (!IsSafe(ch)) {
                    num2++;
                }
            }
            if ((!alwaysCreateReturnValue && (num == 0)) && (num2 == 0)) {
                return bytes;
            }
            byte[] buffer = new byte[count + (num2 * 2)];
            int num4 = 0;
            for (int j = 0; j < count; j++) {
                byte num6 = bytes[offset + j];
                char ch2 = (char)num6;
                if (IsSafe(ch2)) {
                    buffer[num4++] = num6;
                } else if (ch2 == ' ') {
                    buffer[num4++] = 0x2b;
                } else {
                    buffer[num4++] = 0x25;
                    buffer[num4++] = (byte)IntToHex((num6 >> 4) & 15);
                    buffer[num4++] = (byte)IntToHex(num6 & 15);
                }
            }
            return buffer;
        }

        internal static char IntToHex(int n) {
            if (n <= 9) {
                return (char)(n + 0x30);
            }
            return (char)((n - 10) + 0x61);
        }

        internal static bool IsSafe(char ch) {
            if ((((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z'))) || ((ch >= '0') && (ch <= '9'))) {
                return true;
            }
            switch (ch) {
                case '\'':
                case '(':
                case ')':
                case '*':
                case '-':
                case '.':
                case '_':
                case '!':
                    return true;
            }
            return false;
        }
        #endregion
    }

    internal sealed class UrlDecoder {
        #region -------- VARIABLES & CONSTRUCTOR(S) --------
        private int _bufferSize;
        private byte[] _byteBuffer;
        private char[] _charBuffer;
        private Encoding _encoding;
        private int _numBytes;
        private int _numChars;
        internal UrlDecoder(int bufferSize, Encoding encoding) {
            this._bufferSize = bufferSize;
            this._encoding = encoding;
            this._charBuffer = new char[bufferSize];
        }
        #endregion

        #region -------- INTERNAL - AddByte --------
        internal void AddByte(byte b) {
            if (this._byteBuffer == null) {
                this._byteBuffer = new byte[this._bufferSize];
            }
            this._byteBuffer[this._numBytes++] = b;
        }
        #endregion

        #region -------- INTERNAL - AddChar --------
        internal void AddChar(char ch) {
            if (this._numBytes > 0) {
                this.FlushBytes();
            }
            this._charBuffer[this._numChars++] = ch;
        }
        #endregion

        #region -------- INTERNAL - GetString --------
        internal string GetString() {
            if (this._numBytes > 0) {
                this.FlushBytes();
            }
            if (this._numChars > 0) {
                return new string(this._charBuffer, 0, this._numChars);
            }
            return string.Empty;
        }
        #endregion

        #region -------- PRIVATE - FlushBytes --------
        private void FlushBytes() {
            if (this._numBytes > 0) {
                this._numChars += this._encoding.GetChars(this._byteBuffer, 0, this._numBytes, this._charBuffer, this._numChars);
                this._numBytes = 0;
            }
        }
        #endregion
    }

    internal sealed class HtmlEntities {
        #region -------- VARIABLES & CONSTRUCTOR(S) --------
        private static Dictionary<string, char> _entitiesLookupTable;
        private static string[] _entitiesList = new string[] { 
            "\"-quot", "&-amp", "<-lt", ">-gt", "\x00a0-nbsp", "\x00a1-iexcl", "\x00a2-cent", "\x00a3-pound", "\x00a4-curren", "\x00a5-yen", "\x00a6-brvbar", "\x00a7-sect", "\x00a8-uml", "\x00a9-copy", "\x00aa-ordf", "\x00ab-laquo", 
            "\x00ac-not", "\x00ad-shy", "\x00ae-reg", "\x00af-macr", "\x00b0-deg", "\x00b1-plusmn", "\x00b2-sup2", "\x00b3-sup3", "\x00b4-acute", "\x00b5-micro", "\x00b6-para", "\x00b7-middot", "\x00b8-cedil", "\x00b9-sup1", "\x00ba-ordm", "\x00bb-raquo", 
            "\x00bc-frac14", "\x00bd-frac12", "\x00be-frac34", "\x00bf-iquest", "\x00c0-Agrave", "\x00c1-Aacute", "\x00c2-Acirc", "\x00c3-Atilde", "\x00c4-Auml", "\x00c5-Aring", "\x00c6-AElig", "\x00c7-Ccedil", "\x00c8-Egrave", "\x00c9-Eacute", "\x00ca-Ecirc", "\x00cb-Euml", 
            "\x00cc-Igrave", "\x00cd-Iacute", "\x00ce-Icirc", "\x00cf-Iuml", "\x00d0-ETH", "\x00d1-Ntilde", "\x00d2-Ograve", "\x00d3-Oacute", "\x00d4-Ocirc", "\x00d5-Otilde", "\x00d6-Ouml", "\x00d7-times", "\x00d8-Oslash", "\x00d9-Ugrave", "\x00da-Uacute", "\x00db-Ucirc", 
            "\x00dc-Uuml", "\x00dd-Yacute", "\x00de-THORN", "\x00df-szlig", "\x00e0-agrave", "\x00e1-aacute", "\x00e2-acirc", "\x00e3-atilde", "\x00e4-auml", "\x00e5-aring", "\x00e6-aelig", "\x00e7-ccedil", "\x00e8-egrave", "\x00e9-eacute", "\x00ea-ecirc", "\x00eb-euml", 
            "\x00ec-igrave", "\x00ed-iacute", "\x00ee-icirc", "\x00ef-iuml", "\x00f0-eth", "\x00f1-ntilde", "\x00f2-ograve", "\x00f3-oacute", "\x00f4-ocirc", "\x00f5-otilde", "\x00f6-ouml", "\x00f7-divide", "\x00f8-oslash", "\x00f9-ugrave", "\x00fa-uacute", "\x00fb-ucirc", 
            "\x00fc-uuml", "\x00fd-yacute", "\x00fe-thorn", "\x00ff-yuml", "Œ-OElig", "œ-oelig", "Š-Scaron", "š-scaron", "Ÿ-Yuml", "ƒ-fnof", "ˆ-circ", "˜-tilde", "Α-Alpha", "Β-Beta", "Γ-Gamma", "Δ-Delta", 
            "Ε-Epsilon", "Ζ-Zeta", "Η-Eta", "Θ-Theta", "Ι-Iota", "Κ-Kappa", "Λ-Lambda", "Μ-Mu", "Ν-Nu", "Ξ-Xi", "Ο-Omicron", "Π-Pi", "Ρ-Rho", "Σ-Sigma", "Τ-Tau", "Υ-Upsilon", 
            "Φ-Phi", "Χ-Chi", "Ψ-Psi", "Ω-Omega", "α-alpha", "β-beta", "γ-gamma", "δ-delta", "ε-epsilon", "ζ-zeta", "η-eta", "θ-theta", "ι-iota", "κ-kappa", "λ-lambda", "μ-mu", 
            "ν-nu", "ξ-xi", "ο-omicron", "π-pi", "ρ-rho", "ς-sigmaf", "σ-sigma", "τ-tau", "υ-upsilon", "φ-phi", "χ-chi", "ψ-psi", "ω-omega", "ϑ-thetasym", "ϒ-upsih", "ϖ-piv", 
            " -ensp", " -emsp", " -thinsp", "‌-zwnj", "‍-zwj", "‎-lrm", "‏-rlm", "–-ndash", "—-mdash", "‘-lsquo", "’-rsquo", "‚-sbquo", "“-ldquo", "”-rdquo", "„-bdquo", "†-dagger", 
            "‡-Dagger", "•-bull", "…-hellip", "‰-permil", "′-prime", "″-Prime", "‹-lsaquo", "›-rsaquo", "‾-oline", "⁄-frasl", "€-euro", "ℑ-image", "℘-weierp", "ℜ-real", "™-trade", "ℵ-alefsym", 
            "←-larr", "↑-uarr", "→-rarr", "↓-darr", "↔-harr", "↵-crarr", "⇐-lArr", "⇑-uArr", "⇒-rArr", "⇓-dArr", "⇔-hArr", "∀-forall", "∂-part", "∃-exist", "∅-empty", "∇-nabla", 
            "∈-isin", "∉-notin", "∋-ni", "∏-prod", "∑-sum", "−-minus", "∗-lowast", "√-radic", "∝-prop", "∞-infin", "∠-ang", "∧-and", "∨-or", "∩-cap", "∪-cup", "∫-int", 
            "∴-there4", "∼-sim", "≅-cong", "≈-asymp", "≠-ne", "≡-equiv", "≤-le", "≥-ge", "⊂-sub", "⊃-sup", "⊄-nsub", "⊆-sube", "⊇-supe", "⊕-oplus", "⊗-otimes", "⊥-perp", 
            "⋅-sdot", "⌈-lceil", "⌉-rceil", "⌊-lfloor", "⌋-rfloor", "〈-lang", "〉-rang", "◊-loz", "♠-spades", "♣-clubs", "♥-hearts", "♦-diams"
        };
        static HtmlEntities() {
            lock (_entitiesList) {
                _entitiesLookupTable = new Dictionary<string, char>(_entitiesList.Length);
                foreach (string str in _entitiesList) {
                    _entitiesLookupTable.Add(str.Substring(2), str[0]);
                    //hashtable[str.Substring(2)] = str[0];
                }
                //_entitiesLookupTable = hashtable;
            }
        }
        #endregion

        #region -------- PUBLIC - Lookup --------
        internal static char Lookup(string entity) {
            if (_entitiesLookupTable.ContainsKey(entity))
                return (char)_entitiesLookupTable[entity];

            //object obj2 = _entitiesLookupTable[entity];
            //if (obj2 != null) {
            //    return (char)obj2;
            //}
            return '\0';
        }
        #endregion
    }
}
