using LiveDiabetes.Controllers;
using LiveDiabetes.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace LiveDiabetesTeste
{
    public class InsulinControllerTests{
        [Fact]
        public void CalculateDose_ReturnViewResult()
        {
            //Avarage
            var controller = new InsulinController();

            //Act
            var result = controller.CalculateDose(5.0, 4.0, 1.0, 3.0, 2.0);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Home/Tabela.cshtml", viewResult.ViewName);
            Assert.NotNull(viewResult.ViewData["Dose"]);
        }
    }
}
