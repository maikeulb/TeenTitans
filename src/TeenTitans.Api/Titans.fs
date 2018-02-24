namespace TeenTitansApi.Rest

[<AutoOpen>]
module Titans =  

    let JSON v =     
        let jsonSerializerSettings = new JsonSerializerSettings()
        jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
    
        JsonConvert.SerializeObject(v, jsonSerializerSettings)
	|> OK 
	>=> Writers.setMimeType "application/json; charset=utf-8"

    let fromJson<'a> json =
        JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a    

    let getResourceFromReq<'a> (req : HttpRequest) = 
        let getString rawForm = System.Text.Encoding.UTF8.GetString(rawForm)
        req.rawForm |> getString |> fromJson<'a>


    type TitansResource<'a> = {
        GetAllTitans : unit -> 'a seq        
        GetTitanById : int -> 'a option        
	TitanExists : int -> 'a bool        
    }

    let rest resourceName resource =

        let resourcePath = "/" + resourceName
        let resourceIdPath = new PrintfFormat<(int -> string),unit,string,string,int>(resourcePath + "/%d")
        let badRequest = BAD_REQUEST "Resource not found"
        let handleResource requestError = function
            | Some r -> r |> JSON
            | _ -> requestError

        let getAllResources= warbler (fun _ -> resource.GetAll () |> JSON)
        let getResourceById = 
            resource.GetTitanById >> handleResource (NOT_FOUND "Titan not found")
        let ResourceExists id =
            if resource.TitanExists id then OK "" else NOT_FOUND ""

        choose [
            path resourcePath >=> choose [
                GET >=> getAllResources
            ]
            GET >=> pathScan resourceIdPath getResourceById
            HEAD >=> pathScan resourceIdPath ResourceExists
        ]
