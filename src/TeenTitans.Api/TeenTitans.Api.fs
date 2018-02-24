namespace RickAndMortyApi

module App =
    open Suave.Web
    open RickAndMortyApi.Rest
    open RickAndMortyApi.Db
    open Suave

    [<EntryPoint>]
    let main argv =

        let titansWebPart = rest "api/titans" {
            GetTitans = Db.getTitans
            GetTitanById = Db.getTitanById
            CreateTitan = Db.createTitan
            UpdateTitan = Db.updateTitan
            UpdateTitanById = Db.updateTitanById
            DeleteTitan = Db.deleteTitan
            TitanExists = Db.titanExists
        }

        let app = titansWebPart

        startWebServer defaultConfig app

        0
