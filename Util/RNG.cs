using System;
using System.Collections.Generic;
namespace Strata.Util {
    public sealed class RNG {
        #region -------- VARIABLES & CONSTRUCTOR --------
        private static int _counter = 0;
        private static System.Random _rand;
        static RNG() {
            //int seed = DateTime.Now.Millisecond + System.Threading.Interlocked.Increment(ref _counter);
            _rand = new Random(Guid.NewGuid().GetHashCode());
        }
        #endregion

        #region -------- PUBLIC - Random/RandomDouble --------
        public static int Random(int max) {
            lock (_rand) {
                return _rand.Next(max);
            }
        }

        public static int Random(int min, int max) {
            if (min >= max) throw new ArgumentException("The min parameter must be less than the max!");
            lock (_rand) {
                return _rand.Next(min, max);
            }
        }


        public static double RandomDouble(int max) {
            return RandomDouble(0, max);
        }

        public static double RandomDouble(int min, int max) {
            if (min >= max) throw new ArgumentException("The min parameter must be less than the max!");
            double num = NextDouble;
            return (num * ((double)max - (double)min)) + min;
        }
        #endregion

        #region -------- PUBLIC - RandomInts/RandomDoubles/Fill --------
        public int[] RandomInts(int length) {
            int[] array = new int[length];
            Fill(array);
            return array;
        }
        public int[] RandomInts(int length, int maxValue) {
            int[] array = new int[length];
            Fill(array, maxValue);
            return array;
        }
        public int[] RandomInts(int length, int minValue, int maxValue) {
            int[] array = new int[length];
            Fill(array, minValue, maxValue);
            return array;
        }

        public double[] RandomDoubles(int length) {
            double[] array = new double[length];
            Fill(array);
            return array;
        }
        public double[] RandomDoubles(int length, int maxValue) {
            double[] array = new double[length];
            Fill(array, maxValue);
            return array;
        }
        public double[] RandomDoubles(int length, int minValue, int maxValue) {
            double[] array = new double[length];
            Fill(array, minValue, maxValue);
            return array;
        }


        public static void Fill(List<int> list) {
            if (list == null) throw new ArgumentNullException("The list parameter is null!");
            int count = list.Count;
            lock (_rand) {
                for (int i = 0; i < count; i++) {
                    list[i] = _rand.Next();
                }
            }
        }

        public static void Fill(int[] array) {
            if (array == null) throw new ArgumentNullException("The array parameter is null!");
            int count = array.Length;
            lock (_rand) {
                for (int i = 0; i < count; i++) {
                    array[i] = _rand.Next();
                }
            }
        }

        public static void Fill(List<double> list) {
            if (list == null) throw new ArgumentNullException("The list parameter is null!");
            int count = list.Count;
            lock (_rand) {
                for (int i = 0; i < count; i++) {
                    list[i] = _rand.NextDouble();
                }
            }
        }

        public static void Fill(double[] array) {
            if (array == null) throw new ArgumentNullException("The array parameter is null!");
            int count = array.Length;
            lock (_rand) {
                for (int i = 0; i < count; i++) {
                    array[i] = _rand.NextDouble();
                }
            }
        }


        public static void Fill(List<int> list, int max) {
            Fill(list, 0, max);
        }
        public static void Fill(int[] array, int max) {
            Fill(array, 0, max);
        }
        public static void Fill(List<int> list, int min, int max) {
            if (list == null) throw new ArgumentNullException("The list parameter is null!");
            if (min >= max) throw new ArgumentException("The min parameter must be less than the max!");
            int count = list.Count;
            lock (_rand) {
                for (int i = 0; i < count; i++) {
                    list[i] = _rand.Next(min, max);
                }
            }
        }
        public static void Fill(int[] array, int min, int max) {
            if (array == null) throw new ArgumentNullException("The array parameter is null!");
            if (min >= max) throw new ArgumentException("The min parameter must be less than the max!");
            int count = array.Length;
            lock (_rand) {
                for (int i = 0; i < count; i++) {
                    array[i] = _rand.Next(min, max);
                }
            }
        }



        public static void Fill(List<double> list, int max) {
            Fill(list, 0, max);
        }
        public static void Fill(double[] array, int max) {
            Fill(array, 0, max);
        }
        public static void Fill(List<double> list, int min, int max) {
            if (list == null) throw new ArgumentNullException("The list parameter is null!");
            if (min >= max) throw new ArgumentException("The min parameter must be less than the max!");
            int count = list.Count;
            lock (_rand) {
                double factor = ((double)max - (double)min);
                for (int i = 0; i < count; i++) {
                    double num = _rand.NextDouble();
                    num = (num * factor) + min;
                    list[i] = num;
                }
            }
        }
        public static void Fill(double[] array, int min, int max) {
            if (array == null) throw new ArgumentNullException("The array parameter is null!");
            if (min >= max) throw new ArgumentException("The min parameter must be less than the max!");
            int count = array.Length;
            lock (_rand) {
                double factor = ((double)max - (double)min);
                for (int i = 0; i < count; i++) {
                    double num = _rand.NextDouble();
                    num = (num * factor) + min;
                    array[i] = num;
                }
            }
        }
        #endregion

        #region -------- PUBLIC - RandomString --------
        public static string RandomString() {
            int length = Random(5, 20);
            return RandomString(length);
        }

        public static string RandomString(int minLength, int maxLength) {
            if (minLength >= maxLength) throw new ArgumentNullException("The minLength parameter must be greater than or equal to the maxLength!");
            int length = Random(minLength, maxLength);
            return RandomString(length);
        }

        public static string RandomString(int length) {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();

            int remaining = length;
            while (remaining != 0) {
                string block = Convert.ToString(_rand.Next());
                int blockSize = block.Length;
                if ((buffer.Length + blockSize) > length) {
                    int overflow = (buffer.Length + blockSize) - length;
                    block = block.Substring(0, (blockSize - overflow));
                }

                buffer.Append(block);
                remaining -= block.Length;
            }

            return buffer.ToString();
        }
        #endregion

        #region -------- PROPERTIES --------
        public static int Next {
            get {
                lock (_rand) {
                    return _rand.Next();
                }
            }
        }


        public static double NextDouble {
            get {
                lock (_rand) {
                    return _rand.NextDouble();
                }
            }
        }

        public static string NextString {
            get {
                return Convert.ToString(Next);
            }
        }

        public static bool CoinToss {
            get {
                return (Random(0, 2) == 1) ? true : false;
            }
        }

        public static int NextCount {
            get {
                System.Threading.Interlocked.Increment(ref _counter);
                if (_counter >= Int32.MaxValue - 1)
                    _counter = Random(9999);
                return _counter;
            }
        }
        #endregion
    }
}
