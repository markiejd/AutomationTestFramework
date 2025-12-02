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
    public static class Names
    {
        public static string GetRandomSurname(string textCase = "camel")
        {
            DebugOutput.Log($"GetRandomSurname {textCase}");
            var listOfSurnames = new List<string>();
            listOfSurnames.Add("duffy");
            listOfSurnames.Add("smith");
            listOfSurnames.Add("jones");
            listOfSurnames.Add("gia");
            listOfSurnames.Add("Aguirre");
            listOfSurnames.Add("Alexander");
            listOfSurnames.Add("Ali");
            listOfSurnames.Add("Allen");
            listOfSurnames.Add("Allison");
            listOfSurnames.Add("Alvarado");
            listOfSurnames.Add("Alvarez");
            listOfSurnames.Add("Andersen");
            listOfSurnames.Add("Anderson");
            listOfSurnames.Add("Andrade");
            listOfSurnames.Add("Andrews");
            listOfSurnames.Add("Anthony");
            listOfSurnames.Add("Archer");
            listOfSurnames.Add("Arellano");
            listOfSurnames.Add("Arias");
            listOfSurnames.Add("Armstrong");
            listOfSurnames.Add("Arnold");
            listOfSurnames.Add("Arroyo");
            listOfSurnames.Add("Ashley");
            listOfSurnames.Add("Atkins");
            listOfSurnames.Add("Atkinson");
            listOfSurnames.Add("Austin");
            listOfSurnames.Add("Avery");
            listOfSurnames.Add("Avila");

            var randomNumber = Numbers.GetRandomNumberBetween(0, listOfSurnames.Count - 1);
            DebugOutput.Log($"Random Number is {randomNumber}");
            var gottenName = listOfSurnames[randomNumber];            
            var returnText = StringValues.GetTextInCase(gottenName, textCase);
            if (returnText == null) returnText = listOfSurnames[listOfSurnames.Count -1];
            return returnText;
        }


        public static string GetRandomFirstName(string textCase = "camel")
        {
            DebugOutput.Log($"GetRandomFirstName {textCase}");
            var listOfForenames = new List<string>();
            listOfForenames.Add("mark");
            listOfForenames.Add("steve");
            listOfForenames.Add("Josue");
            listOfForenames.Add("Millard");
            listOfForenames.Add("Monte");
            listOfForenames.Add("Julius");
            listOfForenames.Add("Dewayne");
            listOfForenames.Add("Leon");
            listOfForenames.Add("Twila");
            listOfForenames.Add("Bryan");
            listOfForenames.Add("Patrica");
            listOfForenames.Add("Lizzie");
            listOfForenames.Add("Sondra");
            listOfForenames.Add("Kari");

            var randomNumber = Numbers.GetRandomNumberBetween(0, listOfForenames.Count - 1);
            DebugOutput.Log($"Random Number is {randomNumber}");
            var gottenName = listOfForenames[randomNumber];            
            var returnText = StringValues.GetTextInCase(gottenName, textCase);
            if (returnText == null) returnText = listOfForenames[listOfForenames.Count -1];
            return returnText;
        }

    }



}