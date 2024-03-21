using Microsoft.AspNetCore.Mvc;

namespace YourNamespace
{
    public class SOSController : Controller
    {
        public IActionResult SOS()
        {
            ViewBag.ApiKey = "AIzaSyASIaT2MwHe6aZBLVhPC_fOVprHdamlstc"; 
            return View();
        }
    }
}
