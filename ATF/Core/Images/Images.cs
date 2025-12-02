using Core;
using Core.Logging;
using Core.Transformations;

namespace Core.Images
{
    public class ImageDetails
    {
        public string? AltText { get; set; }
        public string? SRC { get; set; }
    }

    public class RGBADetails
    {
        public int Red { get; set; } 
        public int Green { get; set; }
        public int Blue { get; set; }
        public int Alpha { get; set; }
    }

    public class FailedContrastRatio
    {
        public RGBADetails ElementRGBA { get; set; } = new RGBADetails();
        public RGBADetails BackgroundRGBA { get; set; } = new RGBADetails();
        public double ElementLuminance { get; set; }
        public double BackgroundLuminance { get; set; }
        public double ContrastRatio { get; set; }
        public string? PageName { get; set; }
        public string? ElementId { get; set; }
        public string? ElementNumber { get; set; }
    }
    

    public class ImageWorkings
    {

        public double GetContrastRatio(double objectLuminance, double backgroundLuminance)
        {
            DebugOutput.Log($"GetContrastRatio {objectLuminance} {backgroundLuminance}");
            double l1;
            double l2;
            if (objectLuminance >= backgroundLuminance)
            {
                l2 = objectLuminance;
                l1 = backgroundLuminance;
            }
            else
            {
                l1 = objectLuminance;
                l2 = backgroundLuminance;
            }
            var left = l1 + 0.05;
            var right = l2 + 0.05;
            DebugOutput.Log($"GetContrastRatio Div {right} / {left}");
            var contrastRatio = l2 / l1;
            return contrastRatio;
        }

        public bool GetGoodContrastRatio(double contrastRatio)
        {
            if (contrastRatio >= 7) return true;
            return false;
        }

        public double GetRelativeLuminance(RGBADetails imageRGBA)
        {
            DebugOutput.OutputMethod($"GetRelativeLuminance {imageRGBA.Red} {imageRGBA.Green} {imageRGBA.Blue} {imageRGBA.Alpha}");
            var red = imageRGBA.Red; // 19
            double rsRGB = (float)red / 255; // 0.074
            DebugOutput.Log($"rsRGB {red}/255 = {rsRGB}");
            if (rsRGB <= 0.03928)
            {
                rsRGB = rsRGB / 12.92;
            } 
            else
            {
                var leftside = (rsRGB + 0.055)/1.055;  // (1 + 0.055) / 1.055 = 1
                rsRGB = Math.Pow(leftside, 2.4); // 1
            }
            var rsRGBRed = rsRGB;
            DebugOutput.Log($"rsRGBRed {rsRGBRed}");
            
            var green = imageRGBA.Green;
            rsRGB = (float)green / 255;
            DebugOutput.Log($"rsRGB {rsRGB}");
            if (rsRGB <= 0.03928)
            {
                rsRGB = rsRGB / 12.92;
            } 
            else
            {
                var leftside = (rsRGB + 0.055)/1.055;
                rsRGB = Math.Pow(leftside, 2.4);
            }
            var rsRGBGreen= rsRGB;
            DebugOutput.Log($"rsRGBGreen {rsRGBGreen}");
            
            var blue = imageRGBA.Blue;
            rsRGB = (float)blue / 255;
            DebugOutput.Log($"rsRGB {rsRGB}");
            if (rsRGB <= 0.03928)
            {
                rsRGB = rsRGB / 12.92;
            } 
            else
            {
                var leftside = (rsRGB + 0.055)/1.055;
                rsRGB = Math.Pow(leftside, 2.4);
            }
            var rsRGBBlue = rsRGB;
            DebugOutput.Log($"rsRGBBlue {rsRGBBlue}");

            var lRed = 0.2126 * rsRGBRed;
            var lGreen = 0.7152 * rsRGBGreen;
            var lBlue = 0.0722 * rsRGBBlue;
            DebugOutput.Log($"TOTAL {lRed} {lGreen} {lBlue}");
            var l = lRed + lGreen + lBlue;
            DebugOutput.Log($"LUM {l}");            
            return l;
        }


        /// <summary>
        /// Supply a string like rgba(29, 29, 29, 1)  and I'll give you the RGBA Model
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        public static RGBADetails GetRGBADetailsFromString(string colour)
        {
            DebugOutput.OutputMethod($"GetRGBAColoursFromElement {colour}");
            var rgba = new RGBADetails();
            if (colour != null)
            {
                // rgba(29, 29, 29, 1)
                var rgbColour = colour.Replace("rgba", "");
                rgbColour = rgbColour.Replace("(", "");
                rgbColour = rgbColour.Replace(")", "");
                rgbColour = rgbColour.Replace(" ", "");
                DebugOutput.Log($"We should know {rgbColour} left");
                var indyColour = StringValues.BreakUpByDelimitedToList(rgbColour, ",");
                rgba.Red = int.Parse(indyColour[0]);
                rgba.Green = int.Parse(indyColour[1]);
                rgba.Blue = int.Parse(indyColour[2]);
                rgba.Alpha = int.Parse(indyColour[3]);
            }           
            return rgba;
        }




    }

}