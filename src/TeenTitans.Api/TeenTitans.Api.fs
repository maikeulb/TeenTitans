namespace TeenTitansApi

module App =
    open Suave.Web
    open TeenTitansApi.Rest
    open TeenTitansApi.Db
    open Suave

    [<EntryPoint>]
    let main argv =

        let titansWebPart = rest "api/titans" {
            GetTitans = Db.getTitan
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
