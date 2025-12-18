using System.Drawing;
using Reqnroll;
using Core.Logging;
using Core;
using Generic.Steps.Helpers.Interfaces;
using Core.FileIO;
using Core.Configuration;

namespace Generic.Steps.Helpers.Classes
{
	/// <summary>
	/// Helper class for image-related step operations in automated testing.
	/// Handles image comparison, element detection, and screenshot capture functionality.
	/// </summary>
	public class ImageStepHelper : StepHelper, IImageStepHelper
	{
		private readonly ITargetForms targetForms;

		/// <summary>
		/// Initializes a new instance of the ImageStepHelper class.
		/// </summary>
		/// <param name="featureContext">The Reqnroll feature context for test execution</param>
		/// <param name="targetForms">Interface for interacting with target forms</param>
		public ImageStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
		{
			this.targetForms = targetForms;
		}

		/// <summary>
		/// Used as a counter to see how many pixels matched in a failure
		/// </summary>
		// int maxPixels = 0;

		/// <summary>
		/// Determines whether a specified image element is displayed on the current page.
		/// </summary>
		/// <param name="imageElementName">The name of the image element to check</param>
		/// <returns>True if the image element is displayed; otherwise, false</returns>
		public bool ImageElementDisplayed(string imageElementName)
        {
            return ElementInteraction.IsElementDisplayed(CurrentPage, imageElementName, "Image");
		}

		/// <summary>
		/// Checks whether a sub-image exists within a larger image.
		/// </summary>
		/// <param name="subImage">The name of the sub-image to search for</param>
		/// <param name="image">The name of the parent image to search in</param>
		/// <returns>True if the sub-image is found within the parent image; otherwise, false</returns>
		public bool ImageExistsInImage(string subImage, string image)
		{
			DebugOutput.Log($"Proc - ImageExistsInElement {subImage} {image}");
			var imageLocation = ImageLocationInImage(subImage, image);

			// Check if the sub-image location was found
			if (imageLocation == null)
			{
				DebugOutput.Log($"Did not find {subImage} in {image}");
				return false;
			}

			DebugOutput.Log($"Found image within image @ {imageLocation}");
			return true;
		}

		/// <summary>
		/// Clicks on the middle point of a specified image element.
		/// </summary>
		/// <param name="image">The name of the image element to click on</param>
		/// <returns>True if the click operation was successful; otherwise, false</returns>
		public bool ClickOnMiddleOfImage(string image)
        {
			return false;
        }

		/// <summary>
		/// Moves the cursor to a specified image element on the current page.
		/// </summary>
		/// <param name="image">The name of the image element to move to</param>
		/// <returns>True if the image is displayed and cursor movement was successful; otherwise, false</returns>
		public bool MoveToImage(string image)
		{
			DebugOutput.Log($"Proc - MoveToImage {image}");
			return ElementInteraction.IsElementDisplayed(CurrentPage, image, "Image");
		}

		/// <summary>
		/// Captures a screenshot of a specific image element on the current page.
		/// </summary>
		/// <param name="image">The name of the image element to capture</param>
		/// <returns>True if the screenshot was successfully captured; otherwise, false</returns>
		public bool GetImageOfElement(string image)
        {
			DebugOutput.Log($"Proc - GetImageOfElement {image} ");
			return ElementInteraction.GetScreenShotOfElement(CurrentPage, image, "image");
        }

		/// <summary>
		/// Captures a screenshot of the entire current page.
		/// </summary>
		/// <returns>True if the page screenshot was successfully captured; otherwise, false</returns>
		public bool GetImageOfPage()
		{
			DebugOutput.Log($"Proc - GetImageOfPage {CurrentPage.Name}");
			return ElementInteraction.GetScreenShotOfPage(CurrentPage);
        }

		/// <summary>
		/// Takes a screenshot of the entire current page and saves it for comparison or logging.
		/// </summary>
		/// <returns>True if the screenshot was successfully captured; otherwise, false</returns>
		public bool ScreenShotPage()
        {
			return ElementInteraction.GetScreenShotOfPage(CurrentPage);
        }

		/// <summary>
		/// Clicks on a specific location within a sub-image that exists inside a parent image.
		/// </summary>
		/// <param name="subImage">The name of the sub-image to locate and click</param>
		/// <param name="image">The name of the parent image containing the sub-image</param>
		/// <returns>True if the click operation was successful; otherwise, false</returns>
		public bool ClickOnImageInImage(string subImage, string image)
		{
			return false;
		}

		/// <summary>
		/// Finds the pixel coordinates (x, y) of the first pixel of a sub-image within a parent image.
		/// </summary>
		/// <param name="subImage">The name of the sub-image to locate</param>
		/// <param name="image">The name of the parent image to search within</param>
		/// <returns>A string representing the location, or null if the sub-image is not found</returns>
		private string? ImageLocationInImage(string subImage, string image)
		{
			DebugOutput.Log($"Proc - ImageExistsInElement {subImage} {image}");
			return null;
		}

		/// <summary>
		/// Searches for a sub-image loaded from file within a parent image.
		/// </summary>
		/// <param name="imageToBeFound">The name of the sub-image file to locate</param>
		/// <param name="parentImage">The name of the parent image to search within</param>
		/// <returns>True if the sub-image is found in the parent image; otherwise, false</returns>
		public bool FindSubImageFromFile(string imageToBeFound, string parentImage)
		{
			DebugOutput.Log($"Proc - FindSubImageFromFile {imageToBeFound} {parentImage}");
			// Check if the image location is found and return the result
			if (ImageLocationInImage(imageToBeFound, parentImage) != null) return true;
			return false;
		}

		/// <summary>
		/// Compares two images loaded from their full file paths to determine similarity.
		/// </summary>
		/// <param name="firstImageFullPath">The full file path to the first image</param>
		/// <param name="secondImageFullPath">The full file path to the second image</param>
		/// <param name="percentAccept">The acceptable percentage of pixel similarity (default: 99.99%)</param>
		/// <returns>True if the images match within the acceptable percentage; otherwise, false</returns>
		public bool CompareFullPath(string firstImageFullPath, string secondImageFullPath, double percentAccept = 99.99)
		{
			DebugOutput.Log($"Proc - CompareFullPath {firstImageFullPath} {secondImageFullPath} {percentAccept}");
			
			// Only proceed if running on Windows OS
			if (OperatingSystem.IsWindows())
			{
				// Load both images from the provided file paths
				var img1 = new Bitmap($"{firstImageFullPath}");
				var img2 = new Bitmap($"{secondImageFullPath}");
				DebugOutput.Log($"Comparing");

				// Validate that both images were successfully loaded
				if ((img1 == null) || (img2 == null))
				{
					DebugOutput.Log($" No files to comapre!");
					return false;
				}

				DebugOutput.Log($"We have 2 images to compare");

				// Check if images have the same dimensions
				if (img1.Size != img2.Size)
				{
					DebugOutput.Log($"Images are of different sizes");
					return false;
				}

				DebugOutput.Log($"They ARE the same size - taken at same resolution");
				
				// Calculate pixel-level differences between the two images
				var percentageDiff = GetPixelDifference(img1, img2, true);

				// Evaluate if the difference is within acceptable range
				if (percentageDiff != 0)
				{
					DebugOutput.Log($"Image is {percentageDiff} different - acceptable level is {percentAccept}");
					float percentSame = 100 - percentageDiff;

					// Compare calculated similarity against the acceptable threshold
					if (percentSame < percentAccept)
					{
						DebugOutput.Log($"Would accept {percentAccept}% but we have {percentSame}%");
						return false;
					}

					DebugOutput.Log($"We are within acceptable level of Pixels {percentSame}% being the same");
					return true;
				}
				else
				{
					// Images are pixel-perfect matches
					DebugOutput.Log($"Exactly the same image!");
					return true;
				}	
			}
			return false;
		}

		/// <summary>
		/// Compares two test images from predefined directories to verify they match within an acceptable tolerance.
		/// </summary>
		/// <param name="firstImage">The filename (without extension) of the baseline image</param>
		/// <param name="secondImage">The filename (without extension) of the test output image</param>
		/// <param name="percentAccept">The acceptable percentage of pixel similarity (default: 99.99%)</param>
		/// <returns>True if the images match within the acceptable percentage; otherwise, false</returns>
		public bool Compared(string firstImage, string secondImage, double percentAccept = 99.99)
		{
			DebugOutput.Log($"Proc - Compared {firstImage} {secondImage} {percentAccept}");
			
			// Get the current working directory and normalize path separators
			var directory = Directory.GetCurrentDirectory().Replace("\\", "/");
			DebugOutput.Log($"My current directory is ... {directory}!");
			
			// Construct paths to the baseline and test output image directories
			string fileDirectory = directory + @"/AppSpecFlow/TestCompare/" + TargetConfiguration.Configuration.AreaPath + "/";
			string fileDirectoryNew = directory + @"/AppSpecFlow/TestOutput/PageImages/";
			
			// Build full file paths with .png extension
			var image1 = $"{fileDirectory}{firstImage}.png";
			var image2 = $"{fileDirectoryNew}{secondImage}.png";

			// Validate that the baseline image exists
			if (!FileUtils.OSFileCheck(image1))
			{
				DebugOutput.Log($"File {image1} does not exist");
				return false;
			}
			
			// Validate that the test output image exists
			if (!FileUtils.OSFileCheck(image2))
			{
				DebugOutput.Log($"File {image2} does not exist");
				return false;
			}

			// Compare both images and return the result
			return CompareFullPath(image1, image2, percentAccept);
		}

		/// <summary>
		/// Compares two bitmap images pixel-by-pixel to calculate the percentage of different pixels.
		/// </summary>
		/// <param name="img1">The first image to compare</param>
		/// <param name="img2">The second image to compare</param>
		/// <param name="closeEnough">If true, allows small color variations (up to 2 units per RGB channel); otherwise requires exact matches</param>
		/// <returns>The percentage of pixels that differ between the two images</returns>
		private float GetPixelDifference(Bitmap img1, Bitmap img2, bool closeEnough = false)
		{
			DebugOutput.Log($"Proc - GetPixelDifference {img1} {img2}");
			float diff = 0;
			int numberOfPixels = 0;
			
			// Only proceed if running on Windows OS
			if (OperatingSystem.IsWindows())
			{
				// Iterate through each pixel in both images
				for (int y = 0; y < img1.Height; y++)
				{
					for (int x = 0; x < img1.Width; x++)
					{
						numberOfPixels++;
						
						// Perform pixel comparison on Windows
						if (OperatingSystem.IsWindows())
						{
							var pixel1 = img1.GetPixel(x, y);
							var pixel2 = img2.GetPixel(x, y);

							// Check if pixels are different
							if (pixel1 != pixel2)
							{
								if (closeEnough)
								{
									// Allow small color variations within RGB tolerance of 2 units
									if (Math.Abs(pixel1.R - pixel2.R) > 2 || Math.Abs(pixel1.G - pixel2.G) > 2 || Math.Abs(pixel1.B - pixel2.B) > 2)
									{
										diff++;
									}
								}
								else
								{
									// Count any pixel difference as a mismatch
									diff++;
								}
							}
						}
					}
				}
			}

			DebugOutput.Log($"Numbr of Pixels = {numberOfPixels}");
			DebugOutput.Log($"Number of Pixels different = {diff}");
			
			// Calculate and return the percentage of different pixels
			var percentDifferent = (diff / numberOfPixels) * 100;
			return percentDifferent;
		}
	}
}
