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
        GetCharacters : unit -> 'a seq
        GetCharacterById : int -> 'a option
        CharacterExists : int -> bool
        CreateCharacter : 'a -> 'a
        UpdateCharacter : 'a -> 'a option
        UpdateCharacterById : int -> 'a -> 'a option
        DeleteCharacter : int -> unit
    }

    let rest resourceName resource =

        let resourcePath = "/" + resourceName
        let resourceIdPath = new PrintfFormat<(int -> string),unit,string,string,int>(resourcePath + "/%d")
        let badRequest = BAD_REQUEST "Resource not found"
        let handleResource requestError = function
            | Some r -> r |> JSON
            | _ -> requestError

        let getAllResources= warbler (fun _ -> resource.GetCharacters () |> JSON)
        let getResourceById =
            resource.GetCharacterById >> handleResource (NOT_FOUND "Character not found")
        let updateResourceById id =
            request (getResourceFromReq >> (resource.UpdateCharacterById id) >> handleResource badRequest)
        let deleteResourceById id =
            resource.DeleteCharacter id
            NO_CONTENT
        let ResourceExists id =
            if resource.CharacterExists id then OK "" else NOT_FOUND ""

        choose [
            path resourcePath >=> choose [
                GET >=> getAllResources
                POST >=> request (getResourceFromReq >> resource.CreateCharacter >> JSON)
                PUT >=> request (getResourceFromReq >> resource.UpdateCharacter >> handleResource badRequest)
            ]
            DELETE >=> pathScan resourceIdPath deleteResourceById
            GET >=> pathScan resourceIdPath getResourceById
            PUT >=> pathScan resourceIdPath updateResourceById
            HEAD >=> pathScan resourceIdPath ResourceExists
        ]
