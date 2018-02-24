namespace RickAndMortyApi.Rest

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave
open Suave.Operators
open Suave.Http
open Suave.Successful

[<AutoOpen>]
module Titans =
    open Suave.RequestErrors
    open Suave.Filters

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
        GetTitans : unit -> 'a seq
        GetTitanById : int -> 'a option
        TitanExists : int -> bool
        CreateTitan : 'a -> 'a
        UpdateTitan : 'a -> 'a option
        UpdateTitanById : int -> 'a -> 'a option
        DeleteTitan : int -> unit
    }

    let rest resourceName resource =

        let resourcePath = "/" + resourceName
        let resourceIdPath = new PrintfFormat<(int -> string),unit,string,string,int>(resourcePath + "/%d")
        let badRequest = BAD_REQUEST "Resource not found"
        let handleResource requestError = function
            | Some r -> r |> JSON
            | _ -> requestError

        let getAllResources= warbler (fun _ -> resource.GetTitans () |> JSON)
        let getResourceById =
            resource.GetTitanById >> handleResource (NOT_FOUND "Titan not found")
        let updateResourceById id =
            request (getResourceFromReq >> (resource.UpdateTitanById id) >> handleResource badRequest)
        let deleteResourceById id =
            resource.DeleteTitan id
            NO_CONTENT
        let ResourceExists id =
            if resource.TitanExists id then OK "" else NOT_FOUND ""

        choose [
            path resourcePath >=> choose [
                GET >=> getAllResources
                POST >=> request (getResourceFromReq >> resource.CreateTitan >> JSON)
                PUT >=> request (getResourceFromReq >> resource.UpdateTitan >> handleResource badRequest)
            ]
            DELETE >=> pathScan resourceIdPath deleteResourceById
            GET >=> pathScan resourceIdPath getResourceById
            PUT >=> pathScan resourceIdPath updateResourceById
            HEAD >=> pathScan resourceIdPath ResourceExists
        ]
