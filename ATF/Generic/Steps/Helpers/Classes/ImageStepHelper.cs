
using System.Drawing;
using Reqnroll;
using Core.Logging;
using Core;
using Generic.Steps.Helpers.Interfaces;
using Core.FileIO;
using Core.Configuration;

namespace Generic.Steps.Helpers.Classes
{
	public class ImageStepHelper : StepHelper, IImageStepHelper
	{
		private readonly ITargetForms targetForms;
		public ImageStepHelper(FeatureContext featureContext, ITargetForms targetForms) : base(featureContext)
		{
			this.targetForms = targetForms;
		}

		/// <summary>
		/// Used as a counter to see how many pixels matched in a failure
		/// </summary>
		// int maxPixels = 0;

		public bool ImageElementDisplayed(string imageElementName)
		{
            return ElementInteraction.IsElementDisplayed(CurrentPage, imageElementName, "Image");
		}

		/// <summary>
		/// Does a subImage exist in an Image
		/// </summary>
		/// <param name="subImage"></param>
		/// <param name="image"></param>
		/// <returns></returns>
		public bool ImageExistsInImage(string subImage, string image)
		{
			DebugOutput.Log($"Proc - ImageExistsInElement {subImage} {image}");
			var imageLocation = ImageLocationInImage(subImage, image);

			if (imageLocation == null)
			{
				DebugOutput.Log($"Did not find {subImage} in {image}");
				return false;
			}

			DebugOutput.Log($"Found image within image @ {imageLocation}");
			return true;
		}

		public bool ClickOnMiddleOfImage(string image)
        {
			return false;
        }

		public bool MoveToImage(string image)
		{
			DebugOutput.Log($"Proc - MoveToImage {image}");
			return ElementInteraction.IsElementDisplayed(CurrentPage, image, "Image");
		}

		public bool GetImageOfElement(string image)
        {
			DebugOutput.Log($"Proc - GetImageOfElement {image} ");
			return ElementInteraction.GetScreenShotOfElement(CurrentPage, image, "image");
        }

		public bool GetImageOfPage()
		{
			DebugOutput.Log($"Proc - GetImageOfPage {CurrentPage.Name}");
			return ElementInteraction.GetScreenShotOfPage(CurrentPage);
        }

		public bool ScreenShotPage()
        {
			return ElementInteraction.GetScreenShotOfPage(CurrentPage);
        }

		public bool ClickOnImageInImage(string subImage, string image)
		{
			return false;
		}



		/// <summary>
		/// Finds the x,y of the first pixel of a subimage within an image
		/// </summary>
		/// <param name="subImage"></param>
		/// <param name="image"></param>
		/// <returns></returns>
		private string? ImageLocationInImage(string subImage, string image)
		{
			DebugOutput.Log($"Proc - ImageExistsInElement {subImage} {image}");
			return null;
		}

		public bool FindSubImageFromFile(string imageToBeFound, string parentImage)
		{
			DebugOutput.Log($"Proc - FindSubImageFromFile {imageToBeFound} {parentImage}");
			if (ImageLocationInImage(imageToBeFound, parentImage) != null) return true;
			return false;
		}


		public bool CompareFullPath(string firstImageFullPath, string secondImageFullPath, double percentAccept = 99.99)
		{
			DebugOutput.Log($"Proc - CompareFullPath {firstImageFullPath} {secondImageFullPath} {percentAccept}");
			if (OperatingSystem.IsWindows())
			{
				var img1 = new Bitmap($"{firstImageFullPath}");
				var img2 = new Bitmap($"{secondImageFullPath}");
				DebugOutput.Log($"Comparing");

				if ((img1 == null) || (img2 == null))
				{
					DebugOutput.Log($" No files to comapre!");
					return false;
				}

				DebugOutput.Log($"We have 2 images to compare");

				if (img1.Size != img2.Size)
				{
					DebugOutput.Log($"Images are of different sizes");
					return false;
				}

				DebugOutput.Log($"They ARE the same size - taken at same resolution");
				var percentageDiff = GetPixelDifference(img1, img2, true);

				if (percentageDiff != 0)
				{
					DebugOutput.Log($"Image is {percentageDiff} different - acceptable level is {percentAccept}");
					float percentSame = 100 - percentageDiff;

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
					DebugOutput.Log($"Exactly the same image!");
					return true;
				}	
			}
			return false;
		}


		/// <summary>
		/// Compare two exact sized images true if same within acceptable range
		/// </summary>
		/// <param name="firstImage"></param>
		/// <param name="secondImage"></param>
		/// <param name="percentAccept"></param>
		/// <returns></returns>
		public bool Compared(string firstImage, string secondImage, double percentAccept = 99.99)
		{
			DebugOutput.Log($"Proc - Compared {firstImage} {secondImage} {percentAccept}");
			
			var directory = Directory.GetCurrentDirectory().Replace("\\", "/");
			DebugOutput.Log($"My current directory is ... {directory}!");
			string fileDirectory = directory + @"/AppSpecFlow/TestCompare/" + TargetConfiguration.Configuration.AreaPath + "/";
			string fileDirectoryNew = directory + @"/AppSpecFlow/TestOutput/PageImages/";
			var image1 = $"{fileDirectory}{firstImage}.png";
			var image2 = $"{fileDirectoryNew}{secondImage}.png";

			if (!FileUtils.OSFileCheck(image1))
			{
				DebugOutput.Log($"File {image1} does not exist");
				return false;
			}
			if (!FileUtils.OSFileCheck(image2))
			{
				DebugOutput.Log($"File {image2} does not exist");
				return false;
			}

			return CompareFullPath(image1, image2, percentAccept);
		}

		/// <summary>
		/// Count pixels differences
		/// </summary>
		/// <param name="img1"></param>
		/// <param name="img2"></param>
		/// <returns></returns>
		private float GetPixelDifference(Bitmap img1, Bitmap img2, bool closeEnough = false)
		{
			DebugOutput.Log($"Proc - GetPixelDifference {img1} {img2}");
			float diff = 0;
			int numberOfPixels = 0;
			if (OperatingSystem.IsWindows())
			{
				for (int y = 0; y < img1.Height; y++)
				{
					for (int x = 0; x < img1.Width; x++)
					{
						numberOfPixels++;
						if (OperatingSystem.IsWindows())
						{
							var pixel1 = img1.GetPixel(x, y);
							var pixel2 = img2.GetPixel(x, y);


							if (pixel1 != pixel2)
							{
								if (closeEnough)
								{
									if (Math.Abs(pixel1.R - pixel2.R) > 2 || Math.Abs(pixel1.G - pixel2.G) > 2 || Math.Abs(pixel1.B - pixel2.B) > 2)
									{
										diff++;
									}
								}
								else
								{
									diff++;
								}
							}
						}
					}
				}
			}

			DebugOutput.Log($"Numbr of Pixels = {numberOfPixels}");
			DebugOutput.Log($"Number of Pixels different = {diff}");
			var percentDifferent = (diff / numberOfPixels) * 100;
			return percentDifferent;
		}
	}
}
