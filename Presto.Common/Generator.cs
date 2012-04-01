using System;

namespace Presto.Common {
    /// <summary>
    /// Generates different sequences and objects needed throughout the application.
    /// </summary>
    public static class Generator {

        /// <summary>
        /// The random number generator.
        /// </summary>
        private static readonly Random RandomNumberGenerator = new Random();

        private const string charsAlphaNumeric = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZabcdefghiklmnopqrstuvwxyz";

        /// <summary>
        /// Generates a random alpha-numeric string of the specified length.
        /// </summary>
        /// <param name="size">The length of the alpha numeric string to be generated.</param>
        /// <returns>A random alpha numeric string.</returns>
        public static string RandomAlphaNumeric(int size) {
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++) {
                buffer[i] = charsAlphaNumeric[RandomNumberGenerator.Next(charsAlphaNumeric.Length)];
            }
            return new string(buffer);
        }

        /// <summary>
        /// Generates a random 32 bit integer.
        /// </summary>
        /// <returns>A random 32 bit integer.</returns>
        public static int RandomInteger() {
            return RandomNumberGenerator.Next(int.MaxValue);
        }

    }
}
