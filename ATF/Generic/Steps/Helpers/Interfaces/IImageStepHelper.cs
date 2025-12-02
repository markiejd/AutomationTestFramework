namespace Generic.Steps.Helpers.Interfaces
{
	public interface IImageStepHelper : IStepHelper
	{
		bool Compared(string firstImage, string secondImage, double percentAccept = 99.99);
		bool ClickOnMiddleOfImage(string image);
		bool ClickOnImageInImage(string subImage, string image);
		bool ImageExistsInImage(string subImage, string image);
		bool ImageElementDisplayed(string imageElementName);
		bool FindSubImageFromFile(string imageToBeFound, string parentImage);
		bool GetImageOfPage();
		bool GetImageOfElement(string image);
		bool MoveToImage(string image);
		bool ScreenShotPage();
	}
}
