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
    public class Addresses
    {
        public string? Number { get; set; }
        public string? Street { get; set; }
        public string? Area { get; set; }
        public string? Town { get; set; }
        public string? County { get; set; }
        public string? Country { get; set; }
        public string? PostCode { get; set; }
    }

    public static class Address
    {
        private static string GetRandomHouseNumber(int lowNumber = 1, int highNumber = 99)
        {
            DebugOutput.Log($"GetRandomHouseNumber {lowNumber} {highNumber}");
            var number = Numbers.GetRandomNumberBetween(lowNumber, highNumber, true);
            return number.ToString();
        }

        private static string GetRandomTowns()
        {
            var listOfTowns = new List<string>();
            listOfTowns.Add("Aberdeen");
            listOfTowns.Add("Clydebank");
            listOfTowns.Add("Dundee");
            listOfTowns.Add("Edinburgh");
            listOfTowns.Add("Glasgow");
            listOfTowns.Add("Invercarse");
            listOfTowns.Add("Inverness");
            listOfTowns.Add("Kinross");
            listOfTowns.Add("Mayfair");
            listOfTowns.Add("Perth");
            listOfTowns.Add("Ross");
            listOfTowns.Add("London");
            var randomNumber = Numbers.GetRandomNumberBetween(0, listOfTowns.Count - 1, true);
            DebugOutput.Log($"Random Number is {randomNumber}");
            return listOfTowns[randomNumber];
        }

        private static string GetRandomStreetName()
        {
            DebugOutput.Log($"GetRandomStreetName ");
            var street = "";

            var listOfFirstPartStreets = new List<string>();
            listOfFirstPartStreets.Add("Beach");
            listOfFirstPartStreets.Add("Ravenhill");
            listOfFirstPartStreets.Add("Stable");
            listOfFirstPartStreets.Add("William");
            var randomNumber = Numbers.GetRandomNumberBetween(0, listOfFirstPartStreets.Count - 1, true);
            DebugOutput.Log($"Random Number is {randomNumber}");
            street = listOfFirstPartStreets[randomNumber];

            var listOfSecondPartStreets = new List<string>();
            listOfSecondPartStreets.Add("Avenue");
            listOfSecondPartStreets.Add("Road");
            listOfSecondPartStreets.Add("Street");
            listOfSecondPartStreets.Add("Square");
            randomNumber = Numbers.GetRandomNumberBetween(0, listOfSecondPartStreets.Count - 1, true);
            DebugOutput.Log($"Random Number is {randomNumber}");
            street = street + " " + listOfSecondPartStreets[randomNumber];

            return street;
        }

        private static string GetRandomArea()
        {
            DebugOutput.Log($"GetRandomTowns ");
            var listOfAreas = new List<string>();
            listOfAreas.Add("EastEdge");
            listOfAreas.Add("Northgate");
            listOfAreas.Add("Southhgate");
            listOfAreas.Add("Wstern Gables");
            var randomNumber = Numbers.GetRandomNumberBetween(0, listOfAreas.Count - 1, true);
            DebugOutput.Log($"Random Number is {randomNumber}");
            return listOfAreas[randomNumber];
        }

        private static string GetCountry(string county)
        {
            DebugOutput.Log($"GetCountry {county} ");
            switch(county.ToLower())
            {
                default:
                {
                    return "Scotland";
                }
                case "london":
                {
                    return "England";
                }
            }

        }

        private static string GetPostCode(string county)
        {
            DebugOutput.Log($"GetPostCode {county} ");
            var postCode = "";
            var first2Chars = county.Substring(0,2).ToUpper();
            var randomNumber2 = Numbers.GetRandomNumberBetween(10, 99, true);
            postCode = first2Chars + randomNumber2.ToString();
            postCode = postCode + " ";
            var randomNumber1 = Numbers.GetRandomNumberBetween(1, 9, true);
            postCode = postCode = randomNumber1.ToString();
            var last2Chars = Letters.GetRandomLetter(2);
            postCode = postCode + last2Chars;

            DebugOutput.Log($"Post code = {postCode}");
            return postCode;
        }

        private static string GetCounty(string townName)
        {
            DebugOutput.Log($"GetCounty {townName} ");
            switch(townName.ToLower())
            {
                default:
                {
                    return "Shire";
                }
                case "aberdeen":
                case "stonehaven":
                {
                    return "Aberdeenshire";
                }
                case "dundee":
                case "invercarse":
                {
                    return "Dundee City";
                }
                case "edinburgh":
                case "queensferry":
                {
                    return "Edinburgh City";
                }
                case "glasgow":
                case "clydebank":
                {
                    return "Glasgow City";
                }
                case "inverness":
                case "ross":
                {
                    return "Highlands";
                }
                case "perth":
                case "kinross":
                {
                    return "Perth & Kinross";
                }
                case "london":
                case "mayfair":
                {
                    return "London";
                }
            }
        }

        public static Addresses GetRandomAddress(bool flat = false, bool area = true, bool county = true, bool postCode = true)
        {
            DebugOutput.Log($"GetRandomAddress {flat} {area} {county} {postCode} ");
            var newAddress = new Addresses();
            if (flat) newAddress.Number = "Flat " + GetRandomHouseNumber();
            else newAddress.Number = GetRandomHouseNumber();

            newAddress.Street = GetRandomStreetName();

            if (area) newAddress.Area = GetRandomArea();

            newAddress.Town = GetRandomTowns();

            newAddress.County = GetCounty(newAddress.Town);

            newAddress.Country = GetCountry(newAddress.County);

            if (postCode) newAddress.PostCode = GetPostCode(newAddress.County);

            return newAddress;
        }
    }
}