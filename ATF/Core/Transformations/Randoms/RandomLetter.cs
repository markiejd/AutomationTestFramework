using Core.Configuration;
using Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Transformations
{
    public static class Letters
    {
        private static string GetCharacter(bool upper = false, bool safe = false, bool alphaOnly = false)
        {
            DebugOutput.Log($"GetCharacter ");
            var listOfChars = new List<string>();
            listOfChars.Add("a");
            listOfChars.Add("b");
            listOfChars.Add("c");
            listOfChars.Add("d");
            listOfChars.Add("e");
            listOfChars.Add("f");
            listOfChars.Add("g");
            listOfChars.Add("h");
            if (!safe) listOfChars.Add("i");
            listOfChars.Add("j");
            listOfChars.Add("k");
            listOfChars.Add("l");
            listOfChars.Add("m");
            listOfChars.Add("n");
            if (!safe) listOfChars.Add("o");
            listOfChars.Add("q");
            listOfChars.Add("r");
            listOfChars.Add("s");
            listOfChars.Add("t");
            listOfChars.Add("u");
            listOfChars.Add("v");
            listOfChars.Add("w");
            listOfChars.Add("x");
            listOfChars.Add("y");
            listOfChars.Add("z");
            if (!safe && !alphaOnly) listOfChars.Add("!");

            var randomNumber = Numbers.GetRandomNumberBetween(0, listOfChars.Count - 1);
            DebugOutput.Log($"Random Number is {randomNumber}");
            var returnChar = listOfChars[randomNumber];
            if (upper) returnChar = returnChar.ToUpper();
            DebugOutput.Log($"Returning {returnChar}");
            return returnChar;
        }

        public static string GetRandomLetter(int numberOfLetters)
        {
            DebugOutput.Log($"GetRandomLetter {numberOfLetters}");
            string returnString = "";
            for (int i = 1; i < numberOfLetters; i ++)
            {
                var newChar = GetCharacter(true, true);
                returnString = returnString + newChar;
            }
            return returnString;
        }
    }
}