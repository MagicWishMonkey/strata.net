using System;
using System.Security;
using System.Security.Cryptography;
namespace Strata.Crypto {
    public class Hash {
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
                    return System.Text.Encoding.Default.GetString(data);
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
                    return System.Text.Encoding.Default.GetString(data);
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
                    return System.Text.Encoding.Default.GetString(data);
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
                    return System.Text.Encoding.Default.GetString(data);
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
                    return System.Text.Encoding.Default.GetString(data);
                }
            } catch (Exception ex) {
                throw new ArgumentException("SHA512.Hash Error -> " + ex.Message, ex);
            }
        }
        #endregion
    }
}
