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
    public static class Numbers
    {
        private static readonly Random getrandom = new Random();

        public static int GetRandomNumberBetween(int min, int max, bool inclusive = true)
        {
			DebugOutput.Log($"Proc - GetRandomNumberBetween {min} {max} inculsive {inclusive}");
            if (min >= max)
            {
                DebugOutput.Log($"Min {min} must be smaller than {max} - here have a zero!");
                return 0;
            }
            lock(getrandom)
            {
                int randomNumber = 0;
                if(inclusive)
                {
                    randomNumber = getrandom.Next(min, max);
                }
                else
                {
                    int newMin = min + 1;
                    int newMax = max - 1;
                    if (newMin >= newMax)
                    {
                        DebugOutput.Log($"NEW Min {newMin} must be smaller than New Max {newMax} - here have a zero!");
                        return 0;
                    }
                    randomNumber = getrandom.Next(newMin, newMax);
                }
				DebugOutput.Log($"Random roll of {randomNumber} between {min} and {max}");
				return randomNumber;
            }
        }

    }
}
