# Star Wars Api

This project queries the [Star Wars API](https://swapi.dev/) and retrieves a few pieces of information.

## Basic Usage

Either run the StarWarsApi project from Visual Studio or navigate to the StarWarsApi project folder and run `dotnet run`.

## Endpoints

###### /starwars/GetFirstEpisodeSpecies
Returns the classification of all species in the 1st episode

###### /starwars/GetSkywalkerStarships
Returns a list of the Starships related to Luke Skywalker

###### /starwars/GetTotalGalaxyPopulation
Returns the total population of all planets in the Galaxy

For more info, visit the documentation of SWAPI: [SWAPI/Documentation](https://swapi.dev/documentation)

## Tests

The test project mocks api responses based on locally saved JSON data.
In Visual Studio, make sure that "Copy to Output Directory - Copy always" is selected for all of the files in the TestData directory.

## Notes

This project uses entities defined in the [SharpTrooper](https://github.com/olcay/SharpTrooper/tree/master) C# library for 
interacting with the Star Wars API.