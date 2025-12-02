
using System.Drawing;
using System.Drawing.Imaging;
using Core.Configuration;
using Core.FileIO;
using Core.Logging;

namespace Core.Transformations
{
    public static class ImageValues
    {

        public static bool SaveBitMapAs(Bitmap bit1, string fullFileName)
        {
            if(!OperatingSystem.IsWindows()) return false;
            DebugOutput.Log($"Proc - SaveBitMapAs {bit1.Size}  {fullFileName}");
            var extension = fullFileName.Substring(fullFileName.Length - 3, 3);
            DebugOutput.Log($"extension is {extension}");
            switch(extension.ToLower())
            {
                case "png": return SaveBitMapAsPNG(bit1, fullFileName);
                default: break;
            }
            DebugOutput.Log($"Failed to save! Don't know type {extension}");
            return false;
        }

        private static bool SaveBitMapAsPNG(Bitmap bit1, string fullFileName)
        {
            if(!OperatingSystem.IsWindows()) return false;
            try
            {
                bit1.Save(fullFileName, ImageFormat.Png);
                return true;
            }
            catch (Exception e)
            {
                DebugOutput.Log($"Failed to create png from bitmap {fullFileName}  {e}");
                return false;
            }
        }

        



        private static List<int>? FindSubImage(Color[,] imageMatrix, Color[,] subImageMatrix, int x, int y)
        {
            DebugOutput.Log($"FindSubImage STARTING AT {x}x{y}");
            List<int> xAndY = new List<int>();
            var totalSubImagePixels = subImageMatrix.GetLength(0) - 1 * subImageMatrix.GetLength(1) - 1;
            int startX = x;
            int startY = y;
            int maxX = startX + subImageMatrix.GetLength(0) - 1;
            int maxY = startY + subImageMatrix.GetLength(1) - 1;
            int pixelsFound = 0;
            DebugOutput.Log($"Our sub image SIZE is '{subImageMatrix.GetLength(0)} x {subImageMatrix.GetLength(1)}' pixels, thats {totalSubImagePixels} to be found in the image '{imageMatrix.GetLength(0)} x {imageMatrix.GetLength(1)}' matching pixels required START X {startX} START Y {startY} MAXX {maxX} MAXY{maxY}");

            //  x is the x coodinates on the full image MaxX = startX + subImageMatrix.GetLength(0)
            //  y is the y coodinates on the full image MaxY = startY + subImageMatrix.GetLength(1)
            // loopSubImage X and Y are on the sub image (0 -> subImageMatrix.GetLength(0) - 1) x (0 -> subImageMatrix.GetLength(1) - 1) - we start at 0, so have to subtract 1 from length
            for (int loopSubImageX = 0; loopSubImageX < subImageMatrix.GetLength(0) - 1; loopSubImageX++)
            {
                for (int loopSubImageY = 0; loopSubImageY < subImageMatrix.GetLength(1) - 1; loopSubImageY++)
                {
                    if (subImageMatrix[loopSubImageX, loopSubImageY] != imageMatrix[x, y])
                    {
                        return null;
                    }
                    else
                    {
                        pixelsFound ++;
                    }
                    y++;
                    if (y >= maxY) y = startY;
                }
                x++;
                if (x >= maxX) x = startX;
            }
            var middleX = startX + (subImageMatrix.GetLength(0) / 2);
            var middleY = startY + (subImageMatrix.GetLength(1) / 2);
            DebugOutput.Log($"Returning middle is {middleX},{middleY}");
            xAndY.Add(middleX);
            xAndY.Add(middleY);
            return xAndY;
        }


        private static Color[,] GetBitMapColourMatrix(Bitmap bmp)
        {
			Color[,] matrix;
			matrix = new Color[1, 1];
            if (!OperatingSystem.IsWindows()) return matrix;
            int height = bmp.Height;
            int width = bmp.Width;
            matrix = new Color[width, height];
            if (OperatingSystem.IsWindows())
            {
                for (int x = 0; x <= bmp.Width - 1; x++)
                {
                    for (int y = 0; y < bmp.Height - 1; y++)
                    {
                        matrix[x, y] = bmp.GetPixel(x, y);
                    }
                }
            }
            return matrix;
        }


        public static bool CompareBitMapsByPixel(Bitmap bit1, Bitmap bit2, double tolerancePercent = 0.01)
        {
            var matrix1 = GetBitMapColourMatrix(bit1);
            var matrix2 = GetBitMapColourMatrix(bit2);
            DebugOutput.Log($"Size of Image 1 {matrix1.GetLength(0)} x {matrix1.GetLength(1)}");
            DebugOutput.Log($"Size of Image 2 {matrix2.GetLength(0)} x {matrix2.GetLength(1)}");
            if (matrix1.GetLength(0) != matrix2.GetLength(0)) return false;
            if (matrix1.GetLength(1) != matrix2.GetLength(1)) return false;
            var firstPixelToFind = matrix1[0, 0];
            int pixelsWrong = 0;
            for (int x = 0; x < matrix1.GetLength(0); x++)
            {
                for (int y = 0; y < matrix1.GetLength(1); y ++)
                {
					// DebugOutput.Log($"Checking THIS x={x}  y={y}  color={matrix1[x, y]} verses color={matrix2[x, y]}");
                    if (matrix1[x, y] != matrix2[x, y])
                    {
                        pixelsWrong ++;
                    }
                }
            }
            var numberOfPixels = matrix1.GetLength(0) * matrix1.GetLength(1);
            DebugOutput.Log($"{numberOfPixels} PIXELS we have gone through and compared! Found {pixelsWrong} pixels wrong");
            float actualTolerancePercent = (float)pixelsWrong / (float)numberOfPixels;
            actualTolerancePercent = actualTolerancePercent * 100;
            DebugOutput.Log($"OUT {actualTolerancePercent}");
            if (actualTolerancePercent > tolerancePercent)
            {
                DebugOutput.Log($" actual wrong pixels % = {actualTolerancePercent}% This is less than tolerance {tolerancePercent}");
                return false;
            }
            DebugOutput.Log($" actual wrong pixels % = {actualTolerancePercent}% This is within tolerance {tolerancePercent}");
            return true;
        }

        public static bool CompareBitMaps(Bitmap bit1, Bitmap bit2)
        {
            if(!OperatingSystem.IsWindows()) return false;
            DebugOutput.Log($"Proc - CompareBitMaps {bit1.Size}  {bit2.Size}");
            if (bit1.Size == bit2.Size) return CompareBitMapsByPixel(bit1, bit2);
            DebugOutput.Log($"Wrong size!");
            FileUtils.SetCurrentDirectoryToTop();
            var directory = Directory.GetCurrentDirectory();
            var testOutputDir = directory + "\\" + "AppSpecFlow\\TestOutput\\";
            var fullFileName1 = testOutputDir + "image1.PNG";
            var fullFileName2 = testOutputDir + "image2.PNG";
            try
            {
                if (!SaveBitMapAs(bit1, fullFileName1)) return false;
                if (!SaveBitMapAs(bit2, fullFileName2)) return false;
                DebugOutput.Log($"both saved!");
                return false;
            }
            catch
            {
                DebugOutput.Log($"Failed to save bitmaps as PNG {fullFileName1} and {fullFileName2}");
            }
            return false;
        }


        public static Bitmap? GetBitMapFromFile(string fullFileLocation)
        {
            if(!OperatingSystem.IsWindows()) return null;
            DebugOutput.Log($"Proc - GetBitMapFromFile {fullFileLocation}");
            try
            {
                if (FileUtils.OSFileCheck(fullFileLocation))
                {
                    var bitmap = new Bitmap(@fullFileLocation);
                    DebugOutput.Log($"returning bitmap!");
                    return bitmap;
                }
            }
            catch
            {
                DebugOutput.Log($"Failed to get bitmap!");
            }              
            DebugOutput.Log($"Returning null!");                
            return null;
        }


    }
}