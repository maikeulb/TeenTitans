namespace RickAndMortyApi.Db

open System
open System.Collections.Generic

type RickAndMortyCharacter = {
    Id : int
    Name : string
    Status : string
    Gender : string
    Species: string
    Origin : string
}

module Db =
    open FSharp.Data

    let resourceStore = new Dictionary<int, RickAndMortyCharacter>()

    let [<Literal>] URL = "https://rickandmortyapi.com/api/"
    let [<Literal>] CharacterUrl = URL + "character/1"
    let [<Literal>] PageCharacterUrl = URL + "character/"

    type RAMAPI = JsonProvider<URL>
    type PageCharacter = JsonProvider<PageCharacterUrl>
    type Character = JsonProvider<CharacterUrl>

    let root = RAMAPI.GetSample()
    let buildUrl url id = sprintf "%s%s" url id

    let loadCharacter (pageUrl) = 
       let rec iter (pageUrl : string) =
           seq{
               if (pageUrl <> "") then
                  let currentPage = PageCharacter.Load(pageUrl)
                  yield! currentPage.Results |> Array.toSeq
                  yield! iter currentPage.Info.Next
           }
       iter pageUrl

    let rickAndMortyCharacter = loadCharacter root.Characters

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

    rickAndMortyCharacter
    |> Seq.iter (fun p -> 
            let newCharacter = {
                Id          = p.Url.Substring(PageCharacterUrl.Length) |> extractNumber
                Name        = p.Name
                Status      = p.Status
                Gender      = p.Gender
                Species     = p.Species
                Origin      = p.Origin.Name
            }
            resourceStore.Add(newCharacter.Id, newCharacter))
    
    let getCharacters () =
        resourceStore.Values :> seq<RickAndMortyCharacter>
    let getCharacterById id =
        if resourceStore.ContainsKey(id) then
            Some resourceStore.[id]
        else
            None
    let createCharacter character =
        let id = resourceStore.Values.Count + 1
        let newCharacter = {character with Id = id}
        resourceStore.Add(id, newCharacter)
        newCharacter

    let updateCharacterById characterId characterToBeUpdated =
        if resourceStore.ContainsKey(characterId) then
            let updatedCharacter = {characterToBeUpdated with Id = characterId}
            resourceStore.[characterId] <- updatedCharacter
            Some updatedCharacter
        else
            None

    let updateCharacter characterToBeUpdated =
        updateCharacterById characterToBeUpdated.Id characterToBeUpdated

    let deleteCharacter characterId =
        resourceStore.Remove(characterId) |> ignore

    let characterExists  = resourceStore.ContainsKey
