using System;
using System.Security;
using System.Security.Cryptography;


namespace Strata.Crypto {
    public class Hash {
        private static string[] WORD_LIST = "Ack|Alabama|Alanine|Alaska|Alpha|Angel|Apart|April|Arizona|Arkansas|Artist|Asparagus|Aspen|August|Autumn|Avocado|Bacon|Bakerloo|Batman|Beer|Berlin|Beryllium|Black|Blossom|Blue|Bluebird|Bravo|Bulldog|Burger|Butter|California|Carbon|Cardinal|Carolina|Carpet|Cat|Ceiling|Charlie|Chicken|Coffee|Cola|Cold|Colorado|Comet|Connecticut|Crazy|Cup|Dakota|December|Delaware|Delta|Diet|Don|Double|Early|Earth|East|Echo|Edward|Eight|Eighteen|Eleven|Emma|Enemy|Equal|Failed|Fanta|Fifteen|Fillet|Finch|Fish|Five|Fix|Floor|Florida|Football|Four|Fourteen|Foxtrot|Freddie|Friend|Fruit|Gee|Georgia|Glucose|Golf|Green|Grey|Hamper|Happy|Harry|Hawaii|Helium|High|Hot|Hotel|Hydrogen|Idaho|Illinois|India|Indigo|Ink|Iowa|Island|Item|Jersey|Jig|Johnny|Juliet|July|Jupiter|Kansas|Kentucky|Kilo|King|Kitten|Lactose|Lake|Lamp|Lemon|Leopard|Lima|Lion|Lithium|London|Louisiana|Low|Magazine|Magnesium|Maine|Mango|March|Mars|Maryland|Massachusetts|May|Mexico|Michigan|Mike|Minnesota|Mirror|Mississippi|Missouri|Mobile|Mockingbird|Monkey|Montana|Moon|Mountain|Muppet|Music|Nebraska|Neptune|Network|Nevada|Nine|Nineteen|Nitrogen|North|November|Nuts|October|Ohio|Oklahoma|One|Orange|Oranges|Oregon|Oscar|Oven|Oxygen|Papa|Paris|Pasta|Pennsylvania|Pip|Pizza|Pluto|Potato|Princess|Purple|Quebec|Queen|Quiet|Red|River|Robert|Robin|Romeo|Rugby|Sad|Salami|Saturn|September|Seven|Seventeen|Shade|Sierra|Single|Sink|Six|Sixteen|Skylark|Snake|Social|Sodium|Solar|South|Spaghetti|Speaker|Spring|Stairway|Steak|Stream|Summer|Sweet|Table|Tango|Ten|Tennessee|Tennis|Texas|Thirteen|Three|Timing|Triple|Twelve|Twenty|Two|Uncle|Undress|Uniform|Uranus|Utah|Vegan|Venus|Vermont|Victor|Video|Violet|Virginia|Washington|West|Whiskey|White|William|Winner|Winter|Wisconsin|Wolfram|Wyoming|Xray|Yankee|Yellow|Zebra|Zulu".Split('|');


        #region -------- PRIVATE - MD5 --------
        private static string ToHex(byte[] bytes, bool upperCase) {
            var result = new System.Text.StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }
        #endregion


        #region -------- PUBLIC - MD5 --------
        public static string MD5(string data) {
            System.Diagnostics.Debug.Assert((data != null && data.Length > 0), "The data parameter is null/empty!");
            var bytes = System.Text.Encoding.Default.GetBytes(data);
            return MD5(bytes);
        }

        public static string MD5(byte[] bytes) {
            System.Diagnostics.Debug.Assert((bytes != null && bytes.Length > 0), "The bytes parameter is null/empty!");
            
            try {
                using (MD5CryptoServiceProvider csp = new MD5CryptoServiceProvider()) {
                    byte[] data = csp.ComputeHash(bytes);
                    return ToHex(data, false);
                }
            } catch (Exception ex) {
                throw new ArgumentException("MD5.Hash Error -> " + ex.Message, ex);
            }
        }
        #endregion


        #region -------- PUBLIC - SHA1 --------
        public static string SHA1(string data) {
            System.Diagnostics.Debug.Assert((data != null && data.Length > 0), "The data parameter is null/empty!");
            var bytes = System.Text.Encoding.Default.GetBytes(data);
            return SHA1(bytes);
        }

        public static string SHA1(byte[] bytes) {
            System.Diagnostics.Debug.Assert((bytes != null && bytes.Length > 0), "The bytes parameter is null/empty!");

            try {
                using (SHA1CryptoServiceProvider csp = new SHA1CryptoServiceProvider()) {
                    byte[] data = csp.ComputeHash(bytes);
                    return ToHex(data, false);
                }
            } catch (Exception ex) {
                throw new ArgumentException("SHA1.Hash Error -> " + ex.Message, ex);
            }
        }
        #endregion


        #region -------- PUBLIC - SHA256 --------
        public static string SHA256(string data) {
            System.Diagnostics.Debug.Assert((data != null && data.Length > 0), "The data parameter is null/empty!");
            var bytes = System.Text.Encoding.Default.GetBytes(data);
            return SHA256(bytes);
        }

        public static string SHA256(byte[] bytes) {
            System.Diagnostics.Debug.Assert((bytes != null && bytes.Length > 0), "The bytes parameter is null/empty!");

            try {
                using (SHA256CryptoServiceProvider csp = new SHA256CryptoServiceProvider()) {
                    byte[] data = csp.ComputeHash(bytes);
                    return ToHex(data, false);
                }
            } catch (Exception ex) {
                throw new ArgumentException("SHA256.Hash Error -> " + ex.Message, ex);
            }
        }
        #endregion


        #region -------- PUBLIC - SHA384 --------
        public static string SHA384(string data) {
            System.Diagnostics.Debug.Assert((data != null && data.Length > 0), "The data parameter is null/empty!");
            var bytes = System.Text.Encoding.Default.GetBytes(data);
            return SHA384(bytes);
        }

        public static string SHA384(byte[] bytes) {
            System.Diagnostics.Debug.Assert((bytes != null && bytes.Length > 0), "The bytes parameter is null/empty!");

            try {
                using (SHA384CryptoServiceProvider csp = new SHA384CryptoServiceProvider()) {
                    byte[] data = csp.ComputeHash(bytes);
                    return ToHex(data, false);
                }
            } catch (Exception ex) {
                throw new ArgumentException("SHA384.Hash Error -> " + ex.Message, ex);
            }
        }
        #endregion


        #region -------- PUBLIC - SHA512 --------
        public static string SHA512(string data) {
            System.Diagnostics.Debug.Assert((data != null && data.Length > 0), "The data parameter is null/empty!");
            var bytes = System.Text.Encoding.Default.GetBytes(data);
            return SHA512(bytes);
        }

        public static string SHA512(byte[] bytes) {
            System.Diagnostics.Debug.Assert((bytes != null && bytes.Length > 0), "The bytes parameter is null/empty!");

            try {
                using (SHA512CryptoServiceProvider csp = new SHA512CryptoServiceProvider()) {
                    byte[] data = csp.ComputeHash(bytes);
                    return ToHex(data, false);
                }
            } catch (Exception ex) {
                throw new ArgumentException("SHA512.Hash Error -> " + ex.Message, ex);
            }
        }
        #endregion
        

        #region -------- PUBLIC - Human --------
        public static string Human(string data) {
            return Human(data, WORD_LIST);
        }

        public static string Human(string data, string separator) {
            return Human(data, WORD_LIST, separator);
        }

        public static string Human(string data, string[] words) {
            return Human(data, words, "-");
        }

        public static string Human(string data, string[] words, string separator) {
            System.Diagnostics.Debug.Assert((data != null && data.Length > 0), "The data parameter is null/empty!");

            var target = 3;
            if (data.Length > 15)
                target += 1;
            if (data.Length > 250)
                target += 1;
            if (data.Length > 1000)
                target += 1;

            var bytes = System.Text.Encoding.Default.GetBytes(data);
            var length = bytes.Length;

            var chunk_size = length / target;
            var ix = 0;
            var segments = new System.Collections.Generic.List<byte>();
            var inner = 0;
            var sum = 0;
            while (ix < length) {
                sum += bytes[ix];
                inner++;
                if (inner == chunk_size) {
                    var offset = sum % 256;
                    segments.Add((byte)offset);
                    sum = 0;
                    inner = 0;
                }
                ix++;
            }

            if (segments.Count > target)
                segments.RemoveAt(segments.Count - 1);


            var buffer = new System.Text.StringBuilder();
            for (var i = 0; i < segments.Count; i++) {
                ix = (int)segments[i];
                var word = words[ix];
                if (buffer.Length == 0)
                    buffer.Append(word);
                else
                    buffer.Append(separator + word);
            }
            var result = buffer.ToString();
            return result;
        }
        #endregion
    }
}
