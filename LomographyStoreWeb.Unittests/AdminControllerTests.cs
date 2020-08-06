using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LomographyStoreWeb.Controllers;
using LomographyStoreWeb.Models;
using LomographyStoreWeb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace LomographyStoreWeb.Unittests
{
    [TestClass]
    public class AdminControllerTests
    {
        private Mock<ILogger<AdminController>> _loggerMock;
        private Mock<IHttCustomClient> _httpClientMock;
        private Mock<IFormFile> _formFileMock; 
        private PhotoItem _photoItem;
        private ControllerContext _controllerContext;

        [TestInitialize]
        public void Initialize()
        {
            _photoItem = new PhotoItem
            {
                Id = Guid.NewGuid().ToString(), Name = "Selfie", Description = "Selfie in the night", Camera = "Lomo", Film = "Kodak"
            };
        
            _loggerMock = new Mock<ILogger<AdminController>>();
        }

        private void InitHttpClient(HttpStatusCode httpStatusCode)
        {
            var serializedString = JsonConvert.SerializeObject(_photoItem);

            var responseResult = new HttpResponseMessage(httpStatusCode) 
            {
                Content =  new StringContent(serializedString, System.Text.Encoding.UTF8, "application/json" ) 
            };

            _httpClientMock = new Mock<IHttCustomClient>();
            _httpClientMock.Setup(x => x.AddNewProduct(It.IsAny<PhotoItem>())).ReturnsAsync(responseResult);
            _httpClientMock.Setup(x => x.AddNewImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MultipartFormDataContent>())).ReturnsAsync(responseResult);
        }

        private void InitFormFile()
        {
            _formFileMock = new Mock<IFormFile>();
            _formFileMock.Setup(x => x.Length).Returns(12);
            _formFileMock.Setup(x => x.Name).Returns("selfie photo");
            _formFileMock.Setup(x => x.FileName).Returns("selfie_photo.jpg");

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            var content = "Hello World from a Fake File";
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            _formFileMock.Setup(x => x.OpenReadStream()).Returns(ms);
        }

        private void InitControllerContext()
        {
            Mock<HttpRequest> requestMock = CreateMockRequest();
            Mock<HttpContext> httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Request).Returns(requestMock.Object);
            
            _controllerContext = new ControllerContext(){ HttpContext = httpContextMock.Object };
        }

        private static Mock<HttpRequest> CreateMockRequest()
        {            
            var dictForm = new Dictionary<string, StringValues>
            {
                { "imageId", "1234" },
                { "camera", "Diana" }
            };
            FormCollection form = new FormCollection(dictForm);

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Form).Returns(form);
        
            return mockRequest;
        }

        [TestMethod]
        public async Task AddPhoto_InitializedSuccessfully_ReturnsOkay()
        {
            // Arrange
            InitHttpClient(HttpStatusCode.OK);
            var adminController = new AdminController(_loggerMock.Object, _httpClientMock.Object);

            // Act
            var result = await adminController.AddPhoto(_photoItem);
            
            // Assert
            var viewResult = result as ViewResult; 

            Assert.AreEqual(4, viewResult.ViewData.Count);
            Assert.AreNotEqual(_photoItem, viewResult.ViewData["PhotoModel"]);
            Assert.AreEqual(_photoItem.Id, (viewResult.ViewData["PhotoModel"] as PhotoItem).Id);
            Assert.AreEqual(_photoItem.Camera, (viewResult.ViewData["PhotoModel"] as PhotoItem).Camera);
            Assert.AreEqual(_photoItem.Description, (viewResult.ViewData["PhotoModel"] as PhotoItem).Description);
            Assert.AreEqual(_photoItem.Id, viewResult.ViewData["NewPhotoId"]);
            Assert.AreEqual(_photoItem.Camera, viewResult.ViewData["Camera"]);
            Assert.AreEqual("New photo description added", viewResult.ViewData["ProgressMessage"]);
            Assert.AreEqual("Index", viewResult.ViewName);
        }

        [TestMethod]
        public async Task AddPhoto_InitializedNegatively_ReturnsError()
        {
            // Arrange
            InitHttpClient(HttpStatusCode.BadRequest);
            var adminController = new AdminController(_loggerMock.Object, _httpClientMock.Object);

            // Act
            var result = await adminController.AddPhoto(_photoItem);
            
            // Assert
            var viewResult = result as ViewResult; 

            Assert.AreEqual(1, viewResult.ViewData.Count);
            Assert.AreEqual("Error happened while adding photo description", viewResult.ViewData["ErrorMessage"]);

            Assert.AreEqual("Index", viewResult.ViewName);
        }

        [TestMethod]
        public async Task AddNewImage_InitializedSuccessfully_ReturnsOkay()
        {
            // Arrage     
            InitHttpClient(HttpStatusCode.OK);
            InitFormFile();
            InitControllerContext();

            var adminController = new AdminController(_loggerMock.Object, _httpClientMock.Object)
            {
                ControllerContext = _controllerContext
            };

            // Act
            var result = await adminController.AddNewImage(_formFileMock.Object);

            // Assert
            var viewResult = result as ViewResult; 

            Assert.AreEqual(1, viewResult.ViewData.Count);
            Assert.AreEqual("Photo added", viewResult.ViewData["ProgressMessage"]);

            Assert.AreEqual("Index", viewResult.ViewName);
        }

        [TestMethod]
        public async Task AddNewImage_WhenHttpResponseNegative_ReturnsError()
        {
            // Arrage           
            InitHttpClient(HttpStatusCode.BadGateway);
            InitFormFile();
            InitControllerContext();

            var adminController = new AdminController(_loggerMock.Object, _httpClientMock.Object)
            {
                ControllerContext = _controllerContext
            };

            // Act
            var result = await adminController.AddNewImage(_formFileMock.Object);

            // Assert
            var viewResult = result as ViewResult; 

            Assert.AreEqual(1, viewResult.ViewData.Count);
            Assert.AreEqual("Error happened while adding new image", viewResult.ViewData["ErrorMessage"]);

            Assert.AreEqual("Index", viewResult.ViewName);
        }

        [TestMethod]
        public async Task AddNewImage_InitializedNegatively_ReturnsError()
        {
            // Arrage 
            InitHttpClient(HttpStatusCode.BadGateway);
            InitControllerContext();

            var adminController = new AdminController(_loggerMock.Object, _httpClientMock.Object)
            {
                ControllerContext = _controllerContext
            };

            var iformFile = new Mock<IFormFile>();

            // Act
            var result = await adminController.AddNewImage(iformFile.Object);

            // Assert
            var viewResult = result as ViewResult; 

            Assert.AreEqual(1, viewResult.ViewData.Count);
            Assert.AreEqual("image can not be null", viewResult.ViewData["ErrorMessage"]);

            Assert.AreEqual("Index", viewResult.ViewName);
        }
    }
}
