using Microsoft.Extensions.Logging;
using Moq;
using StarWarsApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StarWarsTests
{
    /// <summary>
    /// Main class file will contain only tests for clarity.
    /// </summary>
    public partial class StarWarsControllerTest
    {
        private static HttpResponseMessage CreateFakeResponseMessage(string expectedJson)
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            StringContent httpContent = new StringContent(expectedJson, System.Text.Encoding.UTF8, "application/json");
            responseMessage.Content = httpContent;
            return responseMessage;
        }

        private static string ReadTestFile(string filePath)
        {
            using (StreamReader file = new StreamReader(filePath))
            {
                return file.ReadToEnd();
            }
        }

        /// <summary>
        /// Creates a test <see cref="StarWarsController"/> with the specified <see cref="MockResponseHandler"/>
        /// </summary>
        /// <param name="mockResponseHandler"></param>
        /// <returns></returns>
        private static StarWarsController CreateTestController(MockResponseHandler mockResponseHandler)
        {
            var mockLogger = new Mock<ILogger<StarWarsController>>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var client = new HttpClient(mockResponseHandler);
            client.BaseAddress = new Uri("https://swapi.dev/api/");
            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            return new StarWarsController(mockLogger.Object, mockHttpClientFactory.Object);
        }
    }
}
