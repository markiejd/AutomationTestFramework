using Core; 
using OpenQA.Selenium;

   namespace AppTargets.Forms 
   {
       public class ATFAPITesterStarterPage : FormBase  
       {
           public ATFAPITesterStarterPage() : base(By.Id("ATFAPITesterStarterPage"), "ATFAPITesterStarterPage page") 
           {
               /// Add Elements - Element Idenifiers are LOWER case (apart from ID) 
               ////
               Elements.Add("ID", By.Id("ENTERIDHERE"));
           }
       }
   }
