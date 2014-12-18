using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Strata.Util.Codecs.JSON {
    public class JSONParser {
        enum Token {
            None = -1,
            Curly_Open,
            Curly_Close,
            Squared_Open,
            Squared_Close,
            Colon,
            Comma,
            String,
            Number,
            True,
            False,
            Null
        }


        private char[] _json;
        private StringBuilder _buffer;
        private int _ix;
        private int _max;
        private Token _token = Token.None;
        private JSONParser(string json) {
            this._json = json.ToCharArray();
            this._buffer = new StringBuilder();
            this._ix = 0;
            this._max = this._json.Length;
        }

        public static object Parse(string json) {
            var parser = new JSONParser(json);
            var value = parser.ParseValue();
            return value;
        }

        private JSONParser Scan() {
            var ix = this._ix;
            var max = this._max;
            var json = this._json;
            while (ix < this._max) {
                var c = json[ix];
                if (c == ' ' || c == '\n' || c == '\r' || c == '\t') {
                    ix++;
                    continue;
                }
            }
            this._ix = ix;
            return this;
        }

        private Token Next(bool force = false) {
            if (this._token != Token.None && !force) {
                return this._token;
            }
            this._token = this.SeekToken();
            return this._token;
        }

        private Token SeekToken() {
            var ix = this._ix;
            var max = this._max;
            var json = this._json;

            char c;// = ' ';

            while (ix < max) {
                c = json[ix++];
                if (c == ' ' || c == '\n' || c == '\r' || c == '\t') {
                    continue;
                }

                switch (c) {
                    case '{':
                        this._ix = ix;
                        return Token.Curly_Open;
                    case '}':
                        this._ix = ix;
                        return Token.Curly_Close;
                    case '[':
                        this._ix = ix;
                        return Token.Squared_Open;
                    case ']':
                        this._ix = ix;
                        return Token.Squared_Close;
                    case ',':
                        this._ix = ix;
                        return Token.Comma;
                    case '"':
                        this._ix = ix;
                        return Token.String;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '-':
                    case '+':
                    case '.':
                        ix--;
                        this._ix = ix;
                        return Token.Number;
                    case ':':
                        this._ix = ix;
                        return Token.Colon;
                    case 't':
                        if (ix < (max - 4)) {
                            if (json[ix] == 'r'
                                && json[ix + 1] == 'u'
                                && json[ix + 2] == 'e') {
                                this._ix++;
                                this._ix++;
                                this._ix++;
                                return Token.True;
                            }
                        }
                        break;
                    case 'f':
                        if (ix < (max - 4)) {
                            if (json[ix] == 'a'
                                && json[ix + 1] == 'l'
                                && json[ix + 2] == 's'
                                && json[ix + 3] == 'e') {
                                this._ix++;
                                this._ix++;
                                this._ix++;
                                this._ix++;
                                return Token.False;
                            }
                        }
                        break;
                    default:
                        continue;
                }
            }

            throw new Exception("Could not find token at index " + --ix);
        }


        private void ClearToken() {
            this._token = Token.None;
        }

        private object ParseValue(bool forceNext = true) {
            switch (this.Next(forceNext)) {
                case Token.Number:
                    this.ClearToken();
                    var numVal = this.ParseNumber();
                    return numVal;
                case Token.String:
                    this.ClearToken();
                    var txtVal = this.ParseString();
                    return txtVal;
                case Token.Curly_Open:
                    this.ClearToken();
                    var objVal = this.ParseObject();
                    return objVal;
                case Token.Squared_Open:
                    this.ClearToken();
                    var lstVal = this.ParseArray();
                    return lstVal;
                case Token.True:
                    this.ClearToken();
                    return true;
                case Token.False:
                    this.ClearToken();
                    return false;
                case Token.Null:
                    this.ClearToken();
                    return null;
            }

            throw new Exception("Unrecognized token at index" + this._ix);
        }

        private Dictionary<string, dynamic> ParseObject() {
            Dictionary<string, dynamic> table = new Dictionary<string, dynamic>();


            var ix = this._ix;
            var max = this._max;
            var json = this._json;
            var buffer = new StringBuilder();//this._buffer;


            while (true) {
                switch (this.Next()) {
                    case Token.Comma:
                        this.ClearToken();
                        break;

                    case Token.Curly_Close:
                        this.ClearToken();
                        return table;

                    default:
                        var name = this.ParseString();
                        if (this.Next(true) != Token.Colon) {
                            throw new Exception("Expected colon at index " + this._ix);
                        }
                        if (name == "count")
                            Log.Trace("wait");
                        var value = this.ParseValue();
                        table[name] = value;
                        break;
                }
            }
            return table;
        }

        private ArrayList ParseArray() {
            var list = new ArrayList();
            while (true) {
                switch (this.Next(true)) {
                    case Token.Comma:
                        this.ClearToken();
                        break;
                    case Token.Squared_Close:
                        this.ClearToken();
                        return list;
                    default:
                        var value = this.ParseValue(false);
                        list.Add(value);
                        break;
                }
            }
            return list;
        }

        private string ParseString() {
            var ix = this._ix;
            var max = this._max;
            var json = this._json;
            var buffer = new StringBuilder();//this._buffer;

            char c;
            while (ix < max) {
                c = json[ix++];
                if (c == '"')
                    break;
                buffer.Append(c);
            }
            this._ix = ix;
            return buffer.ToString();
        }

        private object ParseNumber() {
            var ix = this._ix;
            var max = this._max;
            var json = this._json;
            var buffer = new StringBuilder();

            char c;
            bool abort = false;
            while (ix < max && !abort) {
                c = json[ix++];
                switch (c) {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '-':
                    case '+':
                    case '.':
                    case 'e':
                    case 'E':
                        buffer.Append(c);
                        break;
                    default:
                        abort = true;
                        break;
                }
            }
            this._ix = ix;
            var num = buffer.ToString();

            int i;
            if (Int32.TryParse(num, out i)) {
                return i;
            }

            double d;
            if (Double.TryParse(num, out d)) {
                return d;
            }

            decimal dec;
            if (Decimal.TryParse(num, out dec)) {
                return dec;
            }

            long l;
            if (long.TryParse(num, out l)) {
                return l;
            }
            return num;
        }
    }
}
