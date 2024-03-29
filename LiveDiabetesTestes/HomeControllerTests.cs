using LiveDiabetes.Controllers;
using LiveDiabetes.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace LiveDiabetesTestes
{
    public class HomeControllerTests{
        [Fact]
        public void Tabela_ReturnViewResult(){

            //Avarege
            var controller = new HomeController(null);

            //Act
            var result = controller.Tabela();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Gerir_ReturnViewResult()
        {

            //Avarege
            var controller = new HomeController(null);

            //Act
            var result = controller.Gerir();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Medidor_ReturnViewResult(){

            //Avarege
            var controller = new HomeController(null);

            //Act
            var result = controller.Medidor();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void SOS_ReturnViewResult()
        {

            //Avarege
            var controller = new HomeController(null);

            //Act
            var result = controller.SOS();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Index_ReturnViewResult()
        {

            //Avarege
            var controller = new HomeController(null);

            //Act
            var result = controller.Index();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_ReturnViewResult()
        {

            //Avarege
            var controller = new HomeController(null);

            //Act
            var result = controller.Privacy();

            //Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
