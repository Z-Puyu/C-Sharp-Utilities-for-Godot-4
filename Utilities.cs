using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GodotUtilities {
    /// <summary>
    /// A static class containing implementations of potentially useful utility methods.
    /// </summary>
    public static class Utilities {
        private readonly static RandomNumberGenerator rng = new RandomNumberGenerator();

        /// <summary>
        /// Map a non-negative integer pair to a non-negative integer using Cantor Pairing.
        /// </summary>
        /// <param name="x">The first non-negative integer.</param>
        /// <param name="y">The second non-negative integer.</param>
        /// <returns>A unique non-negative integer image for the integer pair.</returns>
        public static ulong CantorPairing(ulong x, ulong y) {
			return (x + y) * (x + y + 1) / 2 + y;
		}

        /// <summary>
        /// Map a non-negative integer <c>Tuple</c> to a non-negative integer using Cantor Pairing.
        /// </summary>
        /// <param name="tuple">The non-negative integer tuple.</param>
        /// <returns>A unique non-negative integer image for the integer pair.</returns>
        public static ulong CantorPairing((ulong, ulong) tuple) {
			return (tuple.Item1 + tuple.Item2) * (tuple.Item1 + tuple.Item2 + 1) / 2 + tuple.Item2;
		}

        /// <summary>
        /// Map a 2D integer-valued vector to a non-negative integer using a bijection.
        /// </summary>
        /// <param name="pair">A pair of integers.</param>
        /// <returns>A unique integer image of <paramref name="pair"/>.</returns>
		public static ulong UniqueId(this Vector2I pair) {
			static ulong q(long z) => (ulong)(z >= 0 ? 2 * z : -2 * z - 1); // Z -> N
			return Utilities.CantorPairing(q(pair.X), q(pair.Y));
		}

        /// <summary>
        /// Map an integer pair to a non-negative integer using a bijection.
        /// </summary>
        /// <param name="pair">A pair of integers.</param>
        /// <returns>A unique integer image of <paramref name="pair"/>.</returns>
		public static ulong UniqueId(this (long, long) pair) {
			static ulong q(long z) => (ulong)(z >= 0 ? 2 * z : -2 * z - 1); // Z -> N
			return Utilities.CantorPairing(q(pair.Item1), q(pair.Item2));
		}

        /// <summary>
        /// Map two integers to a non-negative integer using a bijection.
        /// </summary>
        /// <param name="x">The first integer.</param>
        /// <param name="y">The second integer.</param>
        /// <returns>A unique integer image of (<paramref name="x"/>, <paramref name="y"/>).</returns>
		public static ulong UniqueId(long x, long y) {
			static ulong q(long z) => (ulong)(z >= 0 ? 2 * z : -2 * z - 1); // Z -> N
			return Utilities.CantorPairing(q(x), q(y));
		}

        /// <summary>
        /// Produce a random 32-bit unsigned integer.
        /// </summary>
        /// <returns>A random non-negative integer between <c>0</c> and <c>4294967295</c> inclusive.</returns>
        public static uint Randi() {
            return Utilities.rng.Randi();
        }

        /// <summary>
        /// Produce a random 32-bit integer between <paramref name="inf"/> and <paramref name="sup"/> inclusive.
        /// </summary>
        /// <param name="inf">The smallest possible integer to be generated.</param>
        /// <param name="sup">The greatest possible integer to be generated.</param>
        /// <returns>A random integer between <paramref name="inf"/> and <paramref name="sup"/> inclusive.</returns>
        public static int Randi(int inf, int sup) {
            return Utilities.rng.RandiRange(inf, sup);
        }

        /// <summary>
        /// Produce a random real number between <c>0</c> and <c>1</c> inclusive.
        /// </summary>
        /// <returns>A random real number between <c>0</c> and <c>1</c> inclusive.</returns>
        public static double Rand() {
            return Utilities.rng.Randf();
        }

        /// <summary>
        /// Produce a random real number between <paramref name="inf"/> and <paramref name="sup"/> inclusive.
        /// </summary>
        /// <param name="inf">The smallest possible real number to be generated.</param>
        /// <param name="sup">The greatest possible real number to be generated.</param>
        /// <returns>A random real number between <paramref name="inf"/> and <paramref name="sup"/> inclusive.</returns>
        public static double Rand(double inf, double sup) {
            return Utilities.rng.RandfRange((float)inf, (float)sup);
        }

        /// <summary>
        /// Produce a random boolean value based on a fair coin.
        /// </summary>
        /// <returns>A random boolean value.</returns>
        public static bool FairCoin() {
            return Utilities.rng.RandiRange(0, 1) == 1;
        }

        /// <summary>
        /// Generate a random permutation of a list of objects.
        /// </summary>
        /// <param name="list">The list to be shuffled randomly.</param>
        /// <typeparam name="T">The type of objects in <paramref name="list"/>.</typeparam>
        /// <returns>A random permutation of <paramref name="list"/>.</returns>
        public static IList<T> Shuffle<T>(this IList<T> list) {
            IList<T> @out = [.. list];
            for (int i = list.Count - 1; i > 0; i -= 1) {
                int k = Utilities.Randi(0, i);
                (@out[i], @out[k]) = (@out[k], @out[i]);
            }
            return @out;
        }

        /// <summary>
        /// Select a random item from a list.
        /// </summary>
        /// <param name="list">The list where the item is to be selected.</param>
        /// <typeparam name="T">The type of objects in <paramref name="list"/>.</typeparam>
        /// <returns>A random item from <paramref name="list"/>.</returns>
        public static T RandomSelect<T>(this IList<T> list) {
            return list[Utilities.Randi(0, list.Count - 1)];
        }

        /// <summary>
        /// Select <paramref name="k"/> items from a list at random.
        /// </summary>
        /// <typeparam name="T">The type of items in <paramref name="list"/>.</typeparam>
        /// <param name="list">The list where items are selected.</param>
        /// <param name="k">The number of items to be selected.</param>
        /// <returns>An <c>IEnumerable</c> containing the <paramref name="k"/> items selected at random from <paramref name="list"/>. If <paramref name="k"/> is greater than or equal to the size of <paramref name="list"/>, the whole list is returned.</returns>
        public static IEnumerable<T> RandomSelect<T>(this IList<T> list, int k) {
            if (k >= list.Count) {
                return list;
            }
            return list.Shuffle().Take(k);
        }
    }
}