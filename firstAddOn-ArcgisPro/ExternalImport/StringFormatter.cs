using System;
using System.Collections.Generic;
using System.Text;

namespace CrosscutUtility
{
    public class StringFormatter
    {
        /// <summary>
        /// Format Decimal into number of decimal place in string
        /// </summary>
        /// <param name="input">input decimal object</param>
        /// <param name="decimalPlace">Number of decimal place, 2  shall be 0.00</param>
        /// <returns>Result decimal places string</returns>
        public static string FormatDecimal(object input, int decimalPlace)
        {            
            string PreFormat = "{0:0." + (new String('0', decimalPlace)) + "}";
            return string.Format(PreFormat, input);
        }
    }
}
