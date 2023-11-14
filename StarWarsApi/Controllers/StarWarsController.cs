using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharpTrooper.Entities;
using System.Collections;
using System.Numerics;

namespace StarWarsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StarWarsController : ControllerBase
    {
        private readonly ILogger<StarWarsController> _logger;
        private readonly HttpClient _httpClient;

        public StarWarsController(ILogger<StarWarsController> logger, IHttpClientFactory factory)
        {
            _logger = logger;

            // I'm using the HttpClient instead of SharpTrooperCore's built in WebRequest functionality so I can utilize the async methods
            _httpClient = factory.CreateClient("StarWars");
        }

        [HttpGet("GetSkywalkerStarships")]
        public async Task<ActionResult<List<Starship>>> GetSkywalkerStarshipsAsync()
        {
            var luke = await _httpClient.GetFromJsonAsync<People>("people/1/");
            var starShipUrls = luke.starships;
            var starShips = new List<Starship>();
            foreach(var shipUrl in starShipUrls)
            {
                var ship = await _httpClient.GetFromJsonAsync<Starship>(shipUrl);
                starShips.Add(ship);
            }
            return Ok(starShips);
        }

        [HttpGet("GetFirstEpisodeSpecies")]
        public async Task<ActionResult<string>> GetFirstEpisodeSpeciesAsync()
        {
            var allSpecies = await GetMultipleSharpEntitiesAsync<Specie>("species/");
            var firstEpisodeSpecies = allSpecies.Where(s => s.films.Contains("https://swapi.dev/api/films/1/")).Select(s => s.classification).ToList();
            return Ok(firstEpisodeSpecies);
        }

        [HttpGet("GetTotalGalaxyPopulation")]
        public async Task<ActionResult<string>> GetTotalGalaxyPopulationAsync()
        {
            var allPlanets = await GetMultipleSharpEntitiesAsync<Planet>("planets/");

            long totalGalaxyPopulation = 0;
            foreach (var planet in allPlanets)
            {
                if(long.TryParse(planet.population, out var population))
                {
                    totalGalaxyPopulation += population;
                }
            }

            return Ok(totalGalaxyPopulation.ToString());
        }

        private async Task<IEnumerable<T>> GetMultipleSharpEntitiesAsync<T>(string endpoint) where T : SharpEntity
        {
            var allEntities = new List<T>();
            var entitiesResult = await _httpClient.GetFromJsonAsync<SharpEntityResults<T>>(endpoint);
            allEntities.AddRange(entitiesResult.results);

            while (entitiesResult.next != null)
            {
                entitiesResult = await _httpClient.GetFromJsonAsync<SharpEntityResults<T>>(entitiesResult.next);
                if (entitiesResult != null)
                {
                    allEntities.AddRange(entitiesResult.results);
                }
            }

            return allEntities;
        }
    }
}
