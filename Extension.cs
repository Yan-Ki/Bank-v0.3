using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    /// <summary>
    /// Методы расширения
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// Метод проверки строки на содержание цифр
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumberContains(this string input)
        {
            foreach (char c in input)
                if (char.IsNumber(c))
                    return true;
            return false;
        }
    }
}
