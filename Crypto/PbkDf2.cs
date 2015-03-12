using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;


namespace Strata.Crypto {
    
    public class PBKDF2 {
        private uint BlockIndex = 1;
        private int BufferStartIndex = 0;
        private int BufferEndIndex = 0;

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="algorithm">HMAC algorithm to use.</param>
        /// <param name="input">The input used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <param name="iterations">The number of iterations for the operation.</param>
        /// <exception cref="System.ArgumentNullException">Algorithm cannot be null - Password cannot be null. -or- Salt cannot be null.</exception>
        public PBKDF2(HMAC algorithm, Byte[] input, Byte[] salt, int iterations) {
            if (algorithm == null) { throw new ArgumentNullException("algorithm", "Algorithm cannot be null."); }
            if (salt == null) { throw new ArgumentNullException("salt", "Salt cannot be null."); }
            if (input == null) { throw new ArgumentNullException("input", "input cannot be null."); }
            this.Algorithm = algorithm;
            this.Algorithm.Key = input;
            this.Salt = salt;
            this.Iterations = iterations;
            this.BlockSize = this.Algorithm.HashSize / 8;
            this.BufferBytes = new byte[this.BlockSize];
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="algorithm">HMAC algorithm to use.</param>
        /// <param name="input">The input used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <param name="iterations">The number of iterations for the operation.</param>
        /// <exception cref="System.ArgumentNullException">Algorithm cannot be null - Password cannot be null. -or- Salt cannot be null.</exception>
        public PBKDF2(HMAC algorithm, string input, string salt, int iterations)
            : this(algorithm, Encoding.Default.GetBytes(input), Encoding.Default.GetBytes(salt), iterations) {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="input">The input used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <param name="iterations">The number of iterations for the operation.</param>
        /// <exception cref="System.ArgumentNullException">Algorithm cannot be null - Password cannot be null. -or- Salt cannot be null.</exception>
        public PBKDF2(string input, string salt, int iterations)
            : this(new HMACSHA256(), Encoding.Default.GetBytes(input), Encoding.Default.GetBytes(salt), iterations) {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="input">The input used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <exception cref="System.ArgumentNullException">Algorithm cannot be null - Password cannot be null. -or- Salt cannot be null.</exception>
        public PBKDF2(string input, string salt)
            : this(new HMACSHA512(), Encoding.Default.GetBytes(input), Encoding.Default.GetBytes(salt), 1000) {
        }

        
        /// <summary>
        /// Return a Base64 encoded PBKDF2 password string.
        /// </summary>
        /// <param name="username">The username string.</param>
        /// <param name="password">The plaintext password string.</param>
        /// <param name="salt">The salt string.</param>
        /// <returns></returns>
        public static string GeneratePassword(string username, string password, string salt) {
            var input = username.Trim().ToLower();
            input += "#" + password.Trim();

            var pbk = new PBKDF2(input, salt);
            var raw = pbk.GetBytes(256);
            var txt = System.Text.Encoding.Default.GetString(raw);
            var pwd = Convert.ToBase64String(raw);
            return pwd;
        }

        /// <summary>
        /// Return a Base64 encoded PBKDF2 password string.
        /// </summary>
        /// <param name="password">The plaintext password string.</param>
        /// <param name="salt">The salt string.</param>
        /// <returns></returns>
        public static string GeneratePassword(string password, string salt) {
            var input = password.Trim();
            var pbk = new PBKDF2(input, salt);
            var raw = pbk.GetBytes(256);
            var txt = System.Text.Encoding.Default.GetString(raw);
            var pwd = Convert.ToBase64String(raw);
            return pwd;
        }


        /// <summary>
        /// Returns a pseudo-random key from a password, salt and iteration count.
        /// </summary>
        /// <param name="count">Number of bytes to return.</param>
        /// <returns>Byte array.</returns>
        public Byte[] GetBytes(int count) {
            byte[] result = new byte[count];
            int resultOffset = 0;
            int bufferCount = this.BufferEndIndex - this.BufferStartIndex;

            if (bufferCount > 0) { //if there is some data in buffer
                if (count < bufferCount) { //if there is enough data in buffer
                    Buffer.BlockCopy(this.BufferBytes, this.BufferStartIndex, result, 0, count);
                    this.BufferStartIndex += count;
                    return result;
                }
                Buffer.BlockCopy(this.BufferBytes, this.BufferStartIndex, result, 0, bufferCount);
                this.BufferStartIndex = this.BufferEndIndex = 0;
                resultOffset += bufferCount;
            }

            while (resultOffset < count) {
                int needCount = count - resultOffset;
                this.BufferBytes = this.Func();
                if (needCount > this.BlockSize) { //we one (or more) additional passes
                    Buffer.BlockCopy(this.BufferBytes, 0, result, resultOffset, this.BlockSize);
                    resultOffset += this.BlockSize;
                } else {
                    Buffer.BlockCopy(this.BufferBytes, 0, result, resultOffset, needCount);
                    this.BufferStartIndex = needCount;
                    this.BufferEndIndex = this.BlockSize;
                    return result;
                }
            }
            return result;
        }

        private byte[] Func() {
            var hash1Input = new byte[this.Salt.Length + 4];
            Buffer.BlockCopy(this.Salt, 0, hash1Input, 0, this.Salt.Length);
            Buffer.BlockCopy(GetBytesFromInt(this.BlockIndex), 0, hash1Input, this.Salt.Length, 4);
            var hash1 = this.Algorithm.ComputeHash(hash1Input);

            byte[] finalHash = hash1;
            for (int i = 2; i <= this.Iterations; i++) {
                hash1 = this.Algorithm.ComputeHash(hash1, 0, hash1.Length);
                for (int j = 0; j < this.BlockSize; j++) {
                    finalHash[j] = (byte)(finalHash[j] ^ hash1[j]);
                }
            }
            if (this.BlockIndex == uint.MaxValue) { throw new InvalidOperationException("Derived key too long."); }
            this.BlockIndex += 1;
            return finalHash;
        }

        private static byte[] GetBytesFromInt(uint i) {
            var bytes = BitConverter.GetBytes(i);
            if (BitConverter.IsLittleEndian) {
                return new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
            } else {
                return bytes;
            }
        }



        private HMAC Algorithm {
            get;
            set;
        }


        private int Iterations {
            get;
            set;
        }

        private int BlockSize {
            get;
            set;
        }


        private byte[] Input {
            get;
            set;
        }


        private byte[] Salt {
            get;
            set;
        }

        private byte[] BufferBytes {
            get;
            set;
        }

    }
}


//import hmac
//import hashlib
//from operator import xor
//from itertools import izip, starmap
//from struct import Struct


//class PasswordGenerator:
//    """
//    A PBKDF2 password hashing algorithm borrowed from
//    https://github.com/mitsuhiko/python-pbkdf2.

//    """
//    __KEYLEN__ = 24
//    __ITERATIONS__ = 1000
//    __HASH_FUNC__ = hashlib.sha512
//    __PACK_INT__ = Struct('>I').pack

//    @staticmethod
//    def generate(input, salt, iterations=None, keylen=None, hash_func=None):
//        assert isinstance(input, basestring), "The input parameter is not a valid string!"
//        assert isinstance(salt, basestring), "The input parameter is not a valid string!"

//        if iterations is None:
//            iterations = PasswordGenerator.__ITERATIONS__
//        if keylen is None:
//            keylen = PasswordGenerator.__KEYLEN__
//        if hash_func is None:
//            hash_func = PasswordGenerator.__HASH_FUNC__

//        mac = hmac.new(input, None, hash_func)
//        def _pseudorandom(x, mac=mac):
//            h = mac.copy()
//            h.update(x)
//            return map(ord, h.digest())
//        buf = []
//        for block in xrange(1, -(-keylen // mac.digest_size) + 1):
//            rv = u = _pseudorandom(salt + PasswordGenerator.__PACK_INT__(block))
//            for i in xrange(iterations - 1):
//                u = _pseudorandom(''.join(map(chr, u)))
//                rv = starmap(xor, izip(rv, u))
//            buf.extend(rv)
//        txt = ''.join(map(chr, buf))[:keylen]
//        return txt.encode('hex')


//    @staticmethod
//    def check(data, salt, expected, iterations=None, keylen=None, hash_func=None):
//        actual = PasswordGenerator.generate(data, salt, iterations=iterations, keylen=keylen, hash_func=hash_func)
//        if actual != expected:
//            return False
//        return True


