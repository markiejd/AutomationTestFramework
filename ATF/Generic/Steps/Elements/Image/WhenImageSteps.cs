using System.Configuration;
using System.Reflection.Metadata.Ecma335;
using Core;
using Core.FileIO;
using Core.Logging;
using Generic.Steps;
using Generic.Steps.Helpers.Classes;
using Generic.Steps.Helpers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Generic.Elements.Steps.Image
{
    [Binding]
    public class WhenImageSteps : StepsBase
    {
        public WhenImageSteps(IStepHelpers helpers) : base(helpers)
        {
        }
        
        [When(@"I Move To Image ""(.*)""")]
        public void WhenIMoveToImage(string ImageName)
        {
            string proc = $"Whne I Move To Image {ImageName}";
            if (CombinedSteps.OuputProc(proc))
            {
                if (Helpers.Image.MoveToImage(ImageName))
                {
                    return;
                }
            }
        }



    }
}