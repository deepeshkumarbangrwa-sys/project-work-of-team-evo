using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class HelloWorld : Controller
    {
        public string Index()
        {
            return "This is my first trial";
        }
        /*
        public IActionResult Index()
        {
             
           return View();
        } 
        */
    }
}
