namespace RickAndMortyApi

module App =
    open Suave.Web
    open RickAndMortyApi.Rest
    open RickAndMortyApi.Db
    open Suave

    [<EntryPoint>]
    let main argv =

        let charactersWebPart = rest "api/characters" {
            GetCharacters = Db.getCharacters
            GetCharacterById = Db.getCharacterById
            CreateCharacter = Db.createCharacter
            UpdateCharacter = Db.updateCharacter
            UpdateCharacterById = Db.updateCharacterById
            DeleteCharacter = Db.deleteCharacter
            CharacterExists = Db.characterExists
        }

        let app = charactersWebPart

        startWebServer defaultConfig app

        0
