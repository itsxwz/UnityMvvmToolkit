using System.Globalization;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Internal.Helpers;

namespace UnityMvvmToolkit.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 100,000
        /// </summary>
        private static readonly CultureInfo CommaCulture = new("en")
        {
            NumberFormat = { NumberDecimalSeparator = "," }
        };

        /// <summary>
        /// 100.000
        /// </summary>
        private static readonly CultureInfo PointCulture = new("en")
        {
            NumberFormat = { NumberDecimalSeparator = "." }
        };

        /// <summary>
        /// 尝试解析为float
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="result">浮点数</param>
        /// <returns>是否解析成功</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParse(this string str, out float result)
        {
            return float.TryParse(str, NumberStyles.Any, CommaCulture, out result) ||
                   float.TryParse(str, NumberStyles.Any, PointCulture, out result);
        }

        public static CommandBindingData ToCommandBindingData(this string bindingString, int elementId)
        {
            return BindingStringHelper.GetCommandBindingData(elementId, bindingString);
        }

        public static PropertyBindingData ToPropertyBindingData(this string bindingString)
        {
            return BindingStringHelper.GetPropertyBindingData(bindingString);
        }
    }
}