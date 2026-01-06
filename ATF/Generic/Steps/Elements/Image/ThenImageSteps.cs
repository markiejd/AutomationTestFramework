using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Generic.Elements.Steps.Image
{
    [Binding]
    public class ThenImageSteps : StepsBase
    {
        public ThenImageSteps(IStepHelpers helpers) : base(helpers)
        {
        }

        [Then(@"Page Image Is Captured")]
        public void ThenPageImageIsCaptured()
        {
            string proc = $"Then Page Image Is Captured";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Image.GetImageOfPage())
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }


        [Then(@"Image Of Element ""([^""]*)"" Is Captured")]
        public bool ThenImageOfElementIsCaptured(string imageName)
        {
            string proc = $"Then Image {imageName} Is Captured";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Image.GetImageOfElement(imageName))
                {
                    return true;
                }
                CombinedSteps.Failure(proc);
                return false;
            }
            return false;
        }

        [Then(@"Image Of Element ""([^""]*)"" Is Equal To Image ""([^""]*)"" In TestCompare Directory")]
        public bool ThenImageOfElementIsEqualToInTestCompareDirectory(string imageOfElement, string oldImageOfElement)
        {
            string proc = $"Then Image {imageOfElement} On Screen Is Equal To {oldImageOfElement} In TestOutput Directory";
            if (CombinedSteps.OutputProc(proc))
            {
                if(Helpers.Image.GetImageOfElement(imageOfElement))
                {
                    if (Helpers.Image.Compared(imageOfElement, oldImageOfElement, 75))
                    {
                        return true;
                    }
                }
                CombinedSteps.Failure(proc);
            }
            return false;
        }

        [Then(@"Image Of Element ""([^""]*)"" Is Not Equal To Image ""([^""]*)"" In TestCompare Directory")]
        public void ThenImageOfElementIsNotEqualToInTestOutputDirectory(string imageOfElement, string oldImageOfElement)
        {
            string proc = $"Then Image {imageOfElement} On Screen Is Not Equal To {oldImageOfElement} In TestOutput Directory";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Image.GetImageOfElement(imageOfElement))
                {
                    if (!Helpers.Image.Compared(imageOfElement, imageOfElement, 100))
                    {
                        return;
                    }
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Image Of Element ""([^""]*)"" Contains Image ""([^""]*)"" Found In TestCompare Directory")]
        public void ThenImageOnScreenContainsImageFoundInTestCompareDirectory(string imageOfElement, string subImage)
        {
            string proc = $"Then Image {imageOfElement} On Screen Contains Image {subImage} Found In TestCompare Directory";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Image.GetImageOfElement(imageOfElement))
                {
                    if (Helpers.Image.ImageExistsInImage(subImage, imageOfElement))
                    {
                        return;
                    }
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Image ""([^""]*)"" Is Found On Current Page")]
        public void ThenImageIsFoundOnCurrentPage(string image)
        {
            string proc = $"Then Image {image} Is Found On Current page";
            if (CombinedSteps.OutputProc(proc))
            {
                //This creates the currentPage.png in TestOutput
                if (Helpers.Image.ScreenShotPage())
                {
                    if (Helpers.Image.FindSubImageFromFile(image, "currentPage"))
                    {
                        return ;
                    }
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }

        [Then(@"Image Element ""([^""]*)"" Is Displayed")]
        public void ThenImageElementIsDisplayed(string elementName)
        {
            string proc = $"Then Image Element {elementName} Is Displayed";
            if (CombinedSteps.OutputProc(proc))
            {
                if (Helpers.Image.ImageElementDisplayed(elementName)) 
                {
                    return;
                }
                CombinedSteps.Failure(proc);
                return;
            }
        }




    }
}
