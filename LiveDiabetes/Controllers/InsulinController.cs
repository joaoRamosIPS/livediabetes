using Microsoft.AspNetCore.Mvc;
using LiveDiabetes.Models;
using System.Diagnostics;

namespace LiveDiabetes.Controllers
{
    public class InsulinController : Controller
    {
        [HttpPost]
        public ActionResult CalculateDose(double currentValue, double idealValue, double fsi, double carbs, double ratio)
        {
            try
            {
                InsulinCalculation calculator = new InsulinCalculation();
                double dose = calculator.CalculateInsulinDose(currentValue, idealValue, fsi, carbs, ratio);

                ViewBag.Dose = dose;
                return View("~/Views/Home/Tabela.cshtml");
            }
            catch (Exception ex)
            {
                // Log the error
                Debug.WriteLine(ex.Message);
                // Inform the user
                ViewBag.Error = "Houve um erro no cálculo. Por favor, verifique os valores inseridos.";
                return View("~/Views/Home/Tabela.cshtml");
            }
        }

    }
}