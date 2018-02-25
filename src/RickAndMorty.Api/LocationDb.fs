namespace RickAndMortyApi.Db

open System
open System.Collections.Generic

module LocationDb =

    type RickAndMortyLocation = {
        Id : int
        Name : string
        Type : string
        Dimension : string
        Residents : string []
    }

    open FSharp.Data

    let locationStore = new Dictionary<int, RickAndMortyLocation>()

    let [<Literal>] URL = "https://rickandmortyapi.com/api/"
    let [<Literal>] LocationUrl = URL + "location/1"
    let [<Literal>] PageLocationUrl = URL + "location/"
    let [<Literal>] CharacterUrl = URL + "character/1"

    type RAMAPI = JsonProvider<URL>
    type PageLocation = JsonProvider<PageLocationUrl>
    type Location = JsonProvider<LocationUrl>
    type Character = JsonProvider<CharacterUrl>

    let root = RAMAPI.GetSample()
    let buildUrl url id = sprintf "%s%s" url id

    let loadLocation (pageUrl) = 
       let rec iter (pageUrl : string) =
           seq{
               if (pageUrl <> "") then
                  let currentPage = PageLocation.Load(pageUrl)
                  yield! currentPage.Results |> Array.toSeq
                  yield! iter currentPage.Info.Next
           }
       iter pageUrl

    let rickAndMortyLocations = loadLocation root.Locations

    let withCharacters = rickAndMortyLocations |> Seq.map (fun p -> p, p.Residents |> Seq.map Character.Load)

    let extractNumber (s : string) =
        let nums = 
            s 
            |> Seq.filter (fun c -> Char.IsDigit c)
            |> Seq.toArray
            |> String.Concat
        let result, num = Int32.TryParse nums
        match result with
        | true -> num
        | false -> failwith "No number present"

    rickAndMortyLocations
    |> Seq.iter (fun p -> 
            let newLocation = {
                Id          = p.Url.Substring(PageLocationUrl.Length) |> extractNumber
                Name        = p.Name
                Type        = p.Type
                Dimension   = p.Dimension
                Residents   = p.Residents
            }
            locationStore.Add(newLocation.Id, newLocation))
    
    let getLocations () =
        locationStore.Values :> seq<RickAndMortyLocation>
    let getLocationById id =
        if locationStore.ContainsKey(id) then
            Some locationStore.[id]
        else
            None
    let createLocation location =
        let id = locationStore.Values.Count + 1
        let newLocation = {location with Id = id}
        locationStore.Add(id, newLocation)
        newLocation

    let updateLocationById locationId locationToBeUpdated =
        if locationStore.ContainsKey(locationId) then
            let updatedLocation = {locationToBeUpdated with Id = locationId}
            locationStore.[locationId] <- updatedLocation
            Some updatedLocation
        else
            None

    let updateLocation locationToBeUpdated =
        updateLocationById locationToBeUpdated.Id locationToBeUpdated

    let deleteLocation locationId =
        locationStore.Remove(locationId) |> ignore

    let locationExists  = locationStore.ContainsKey
