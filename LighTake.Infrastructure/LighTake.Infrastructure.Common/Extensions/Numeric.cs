using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// Summary for the Numbers class
    /// </summary>
    public static class Numeric
    {
        /// <summary>
        /// Determines whether a number is a natural number (positive, non-decimal)
        /// </summary>
        /// <param name="sItem">The s item.</param>
        /// <returns>
        /// 	<c>true</c> if [is natural number] [the specified s item]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNaturalNumber(this string sItem)
        {
            Regex notNaturalPattern = new Regex("[^0-9]");
            Regex naturalPattern = new Regex("0*[1-9][0-9]*");

            return !notNaturalPattern.IsMatch(sItem) && naturalPattern.IsMatch(sItem);
        }

        /// <summary>
        /// Determines whether [is whole number] [the specified s item].
        /// </summary>
        /// <param name="sItem">The s item.</param>
        /// <returns>
        /// 	<c>true</c> if [is whole number] [the specified s item]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWholeNumber(this string sItem)
        {
            Regex notWholePattern = new Regex("[^0-9]");
            return !notWholePattern.IsMatch(sItem);
        }

        /// <summary>
        /// Determines whether the specified s item is integer.
        /// </summary>
        /// <param name="sItem">The s item.</param>
        /// <returns>
        /// 	<c>true</c> if the specified s item is integer; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInteger(this string sItem)
        {
            Regex notIntPattern = new Regex("[^0-9-]");
            Regex intPattern = new Regex("^-[0-9]+$|^[0-9]+$");

            return !notIntPattern.IsMatch(sItem) && intPattern.IsMatch(sItem);
        }

        /// <summary>
        /// Determines whether the specified s item is number.
        /// </summary>
        /// <param name="sItem">The s item.</param>
        /// <returns>
        /// 	<c>true</c> if the specified s item is number; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumber(this string sItem)
        {
            double result;
            return (double.TryParse(sItem, NumberStyles.Float, NumberFormatInfo.CurrentInfo, out result));
        }

        /// <summary>
        /// Determines whether the specified value is an even number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is even; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEven(this int value)
        {
            return ((value & 1) == 0);
        }

        /// <summary>
        /// Determines whether the specified value is an odd number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is odd; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOdd(this int value)
        {
            return ((value & 1) == 1);
        }

        /// <summary>
        /// Generates a random number with an upper bound
        /// </summary>
        /// <param name="high">The high.</param>
        /// <returns></returns>
        public static int Random(int high)
        {
            byte[] random = new Byte[4];
            new RNGCryptoServiceProvider().GetBytes(random);
            int randomNumber = BitConverter.ToInt32(random, 0);

            return Math.Abs(randomNumber % high);
        }

        /// <summary>
        /// Generates a random number between the specified bounds
        /// </summary>
        /// <param name="low">The low.</param>
        /// <param name="high">The high.</param>
        /// <returns></returns>
        public static int Random(int low, int high)
        {
            return new Random().Next(low, high);
        }

        /// <summary>
        /// Generates a random double
        /// </summary>
        /// <returns></returns>
        public static double Random()
        {
            return new Random().NextDouble();
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Lenght"></param>
        /// <returns></returns>
        public static decimal NewRound(this decimal source, int Lenght)
        {
            return Math.Round(source, Lenght);
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Lenght"></param>
        /// <param name="mr"></param>
        /// <returns></returns>
        public static decimal NewRound(this decimal source, int Lenght, MidpointRounding mr)
        {
            return Math.Round(source, Lenght, mr);
        }

        public static string ToPercentage(this decimal source)
        {
            return (source * 100) + "%";
        }

        public static string ToDollarFormat(this decimal source)
        {
            return "$" + source;
        }

        public static string ToRMBFormat(this decimal source)
        {
            return "￥" + source;
        }
    }
}