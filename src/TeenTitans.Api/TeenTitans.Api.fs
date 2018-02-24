namespace TeenTitansApi

module App =   
    open Suave.Web
    open SuaveRestApi.Rest
    open SuaveRestApi.Db
    open SuaveRestApi.MusicStoreDb
    open Suave

    [<EntryPoint>]
    let main argv =    

        let titansWebPart = rest "titan" {
            GetAllTitans = Db.getTitan
            GetByTitanId = Db.getTitan
            CreateTitan = Db.createTitan
            UpdateTitan = Db.updateTitan
            UpdateTitanById = Db.updateTitanById
            DeleteTitan = Db.deleteTitan
            TitanExists = Db.isTitanExists
        }

	let app = choose[titanWebPart]

        startWebServer defaultConfig app
            
        0
