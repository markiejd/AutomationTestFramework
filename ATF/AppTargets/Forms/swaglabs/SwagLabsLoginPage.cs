using Core; 
using OpenQA.Selenium; 
  
   namespace AppTargets.Forms 
   { 
       public class SwagLabsLoginPage : FormBase 
       { 
           public SwagLabsLoginPage() : base(By.Id("SwagLabsLogin"), "SwagLabsLogin page") 
           { 
               /// Add Elements - Element Idenifiers are LOWER case (apart from ID) 
               /// 
               Elements.Add("ID", By.Id("login-button")); 
  
           } 
       } 
   } 
