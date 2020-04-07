using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MvcTemplate.Components.Mvc;
using MvcTemplate.Services;
using NSubstitute;
using System;
using Xunit;

namespace MvcTemplate.Controllers.Tests
{
    public class ServicedControllerTests : ControllerTests
    {
        private ServicedController<IService> controller;
        private IService service;

        public ServicedControllerTests()
        {
            service = Substitute.For<IService>();
            controller = Substitute.ForPartsOf<ServicedController<IService>>(service);

            controller.ControllerContext.RouteData = new RouteData();
            controller.ControllerContext.HttpContext = Substitute.For<HttpContext>();
            controller.HttpContext.RequestServices.GetService(typeof(ILanguages)).Returns(Substitute.For<ILanguages>());
        }
        public override void Dispose()
        {
            controller.Dispose();
            service.Dispose();
        }

        [Fact]
        public void ServicedController_SetsService()
        {
            Object actual = controller.Service;
            Object expected = service;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void OnActionExecuting_SetsServiceCurrentAccountId()
        {
            controller.CurrentAccountId.Returns(1);

            controller.OnActionExecuting(null);

            Int64 expected = controller.CurrentAccountId;
            Int64 actual = service.CurrentAccountId;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Dispose_Service()
        {
            controller.Dispose();

            service.Received().Dispose();
        }

        [Fact]
        public void Dispose_MultipleTimes()
        {
            controller.Dispose();
            controller.Dispose();
        }
    }
}
