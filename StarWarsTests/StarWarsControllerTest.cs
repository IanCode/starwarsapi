using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpTrooper.Entities;
using StarWarsApi.Controllers;
using System.Net;
using System.Net.Http.Json;
using System.Web.Http;
using Xunit.Abstractions;

namespace StarWarsTests
{
    /// <summary>
    /// These tests don't test the star wars api directly due to potential future changes, 
    /// but they test the business logic of the <see cref="StarWarsController"/> given expected api responses.
    /// </summary>
    public partial class StarWarsControllerTest
    {
        // This allows Console.WriteLine to write to the output pane of the test explorer.
        private readonly ITestOutputHelper output;

        //Make sure that "Copy to Output Directory - Copy always" is selected for all of the files in the TestData directory.
        private string TESTDATAPATH = Environment.CurrentDirectory + @"\TestData\";
        public StarWarsControllerTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task GetSkywalkerStarshipsTest()
        {
            var expectedStarShip12Json = ReadTestFile($"{TESTDATAPATH}starship12.json");
            var expectedStarShip22Json = ReadTestFile($"{TESTDATAPATH}starship22.json");
            var expectedLukeJson = ReadTestFile($"{TESTDATAPATH}luke.json");

            var mockResponseHandler = new MockResponseHandler();

            // creating fake responses for the endpoints associated with luke and luke's starships
            // the star wars api returns all Uris with trailing slashes so we're going to stick with that
            mockResponseHandler.AddFakeResponse(new Uri("https://swapi.dev/api/starships/12/"), CreateFakeResponseMessage(expectedStarShip12Json));
            mockResponseHandler.AddFakeResponse(new Uri("https://swapi.dev/api/starships/22/"), CreateFakeResponseMessage(expectedStarShip22Json));
            mockResponseHandler.AddFakeResponse(new Uri("https://swapi.dev/api/people/1/"), CreateFakeResponseMessage(expectedLukeJson));

            var controller = CreateTestController(mockResponseHandler);

            var response = await controller.GetSkywalkerStarshipsAsync();
            var result = (OkObjectResult)response.Result;
            var value = result.Value as List<Starship>;

            // some extra serialization to ensure string equality.
            var starShip12 = JsonConvert.DeserializeObject<Starship>(expectedStarShip12Json);
            var starShip22 = JsonConvert.DeserializeObject<Starship>(expectedStarShip22Json);
            Assert.Equal(JsonConvert.SerializeObject(starShip12), JsonConvert.SerializeObject(value[0]));
            Assert.Equal(JsonConvert.SerializeObject(starShip22), JsonConvert.SerializeObject(value[1]));
        }

        [Fact]
        public async Task GetFirstEpisodeSpeciesTest()
        {
            var mockResponseHandler = new MockResponseHandler();

            var expectedSpeciesJson = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                expectedSpeciesJson.Add(ReadTestFile($"{TESTDATAPATH}speciespage" + i + ".json"));
                // creating fake responses for all of the pages from the species endpoint
                var absoluteUri = i == 0 ? "https://swapi.dev/api/species/" : $"https://swapi.dev/api/species/?page={i + 1}";
                mockResponseHandler.AddFakeResponse(new Uri(absoluteUri), CreateFakeResponseMessage(expectedSpeciesJson[i]));
            }

            var firstEpisodeSpeciesJson = ReadTestFile($"{TESTDATAPATH}firstepisodespecies.json");

            var controller = CreateTestController(mockResponseHandler);

            var response = await controller.GetFirstEpisodeSpeciesAsync();
            var result = (OkObjectResult)response.Result;
            var value = result.Value as List<string>;

            // some extra serialization to ensure string equality.
            var species = JsonConvert.DeserializeObject<List<string>>(firstEpisodeSpeciesJson);
            Assert.Equal(JsonConvert.SerializeObject(species), JsonConvert.SerializeObject(value));
        }

        [Fact]
        public async Task GetTotalGalaxyPopulationTest()
        {
            var totalGalaxyPopulation = "1711401432500";

            var mockResponseHandler = new MockResponseHandler();

            var expectedPlanetsJson = new List<string>();
            for (int i = 0; i < 6; i++)
            {
                expectedPlanetsJson.Add(ReadTestFile($"{TESTDATAPATH}planetspage" + i + ".json"));
                // creating fake responses for all of the pages from the planets endpoint
                var absoluteUri = i == 0 ? "https://swapi.dev/api/planets/" : $"https://swapi.dev/api/planets/?page={i + 1}";
                mockResponseHandler.AddFakeResponse(new Uri(absoluteUri), CreateFakeResponseMessage(expectedPlanetsJson[i]));
            }

            var controller = CreateTestController(mockResponseHandler);

            var response = await controller.GetTotalGalaxyPopulationAsync();
            var result = (OkObjectResult)response.Result;
            var value = result.Value as string;

            Assert.Equal(totalGalaxyPopulation, value);
        }
    }
}