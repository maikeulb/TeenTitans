namespace RickAndMortyApi.Rest

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave
open Suave.Operators
open Suave.Http
open Suave.Successful

[<AutoOpen>]
module Locations =
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

    type LocationsResource<'a> = {
        GetLocations : unit -> 'a seq
        GetLocationById : int -> 'a option
        LocationExists : int -> bool
        CreateLocation : 'a -> 'a
        UpdateLocation : 'a -> 'a option
        UpdateLocationById : int -> 'a -> 'a option
        DeleteLocation : int -> unit
    }

    let locationsRest resourceName resource =

        let resourcePath = "/" + resourceName
        let resourceIdPath = new PrintfFormat<(int -> string),unit,string,string,int>(resourcePath + "/%d")
        let badRequest = BAD_REQUEST "Resource not found"
        let handleResource requestError = function
            | Some r -> r |> JSON
            | _ -> requestError
        let getAllResources= warbler (fun _ -> resource.GetLocations () |> JSON)
        let getResourceById =
            resource.GetLocationById >> handleResource (NOT_FOUND "Location not found")
        let updateResourceById id =
            request (getResourceFromReq >> (resource.UpdateLocationById id) >> handleResource badRequest)
        let deleteResourceById id =
            resource.DeleteLocation id
            NO_CONTENT
        let ResourceExists id =
            if resource.LocationExists id then OK "" else NOT_FOUND ""

        choose [
            path resourcePath >=> choose [
                GET >=> getAllResources
                POST >=> request (getResourceFromReq >> resource.CreateLocation >> JSON)
                PUT >=> request (getResourceFromReq >> resource.UpdateLocation >> handleResource badRequest)
            ]
            DELETE >=> pathScan resourceIdPath deleteResourceById
            GET >=> pathScan resourceIdPath getResourceById
            PUT >=> pathScan resourceIdPath updateResourceById
            HEAD >=> pathScan resourceIdPath ResourceExists
        ]
