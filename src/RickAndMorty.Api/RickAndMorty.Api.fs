namespace RickAndMortyApi

module Api =
    open Suave.Web
    open RickAndMortyApi.Rest
    open RickAndMortyApi.Db
    open Suave

    [<EntryPoint>]
    let main argv =

        let charactersWebPart = charactersRest "api/characters" {
            GetCharacters = CharacterDb.getCharacters
            GetCharacterById = CharacterDb.getCharacterById
            CreateCharacter = CharacterDb.createCharacter
            UpdateCharacter = CharacterDb.updateCharacter
            UpdateCharacterById = CharacterDb.updateCharacterById
            DeleteCharacter = CharacterDb.deleteCharacter
            CharacterExists = CharacterDb.characterExists
        }

        let locationWebPart = locationsRest "api/locations" {
            GetLocations = LocationDb.getLocations
            GetLocationById = LocationDb.getLocationById
            CreateLocation = LocationDb.createLocation
            UpdateLocation = LocationDb.updateLocation
            UpdateLocationById = LocationDb.updateLocationById
            DeleteLocation = LocationDb.deleteLocation
            LocationExists = LocationDb.locationExists
        }

        let app = choose[charactersWebPart; locationWebPart] 

        startWebServer defaultConfig app

        0
