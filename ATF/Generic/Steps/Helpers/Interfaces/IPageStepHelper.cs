

namespace Generic.Steps.Helpers.Interfaces
{
    public interface IPageStepHelper : IStepHelper
    {
        public bool ComparePageImage(string pageName);
        // public List<IWebElement> GetAllPageElements(string pageName, int timeOut = 0);
        public List<string> GetAllPageKeys(string pageName);
        // IWebElement? GetPageElement(string pageName, string elementName, int timeout = 0);
        // IWebElement? GetCurrentPageElement(string elementName, int timeOut = 0);
        // List<IWebElement>? GetCurrentPageElements(string elementName, int timeout = 0);
        // By? GetCurrentPageLocators(string elementName);
        string GetCurrentPageName();
        bool GetImagesOfAllElementsInPageFile(string pageName, bool update = false);
        bool IsDisplayed(string pageName, int timeOut = 0);
        bool IsExists(string pageName);
        bool IsMessageDisplayed(string message);
        bool IsNotDisplayed(string pageName, int timeOut = 0);
        void SetCurrentPage(string pageName);
        bool SetCurrentPageSize(int width = 800, int height = 600);
    }
}
