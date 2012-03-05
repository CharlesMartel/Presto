using System;

namespace Presto.Common {
    public static class Generator {
        private static readonly Random RandomNumberGenerator = new Random();

        private const string charsAlphaNumeric = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZabcdefghiklmnopqrstuvwxyz";

        public static string RandomAlphaNumeric(int size) {
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++) {
                buffer[i] = charsAlphaNumeric[RandomNumberGenerator.Next(charsAlphaNumeric.Length)];
            }
            return new string(buffer);
        }

        public static int RandomInteger() {
            return RandomNumberGenerator.Next(int.MaxValue);
        }

    }
}
