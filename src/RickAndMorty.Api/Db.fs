namespace RickAndMortyApi.Db

open System
open System.Collections.Generic

type Titan = {
    Id : int
    Name : string
}

module Db =
    open FSharp.Data

    let resourceStore = new Dictionary<int, Titan>()

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
            let newPerson = {
                Id          = p.Url.Substring(PageCharacterUrl.Length) |> extractNumber
                Name        = p.Name
            }
            resourceStore.Add(newPerson.Id, newPerson))
    
    let getCharacters () =
        resourceStore.Values :> seq<Titan>
    let getCharacterById id =
        if resourceStore.ContainsKey(id) then
            Some resourceStore.[id]
        else
            None
    let createCharacter person =
        let id = resourceStore.Values.Count + 1
        let newCharacter = {person with Id = id}
        resourceStore.Add(id, newCharacter)
        newCharacter

    let updateCharacterById personId personToBeUpdated =
        if resourceStore.ContainsKey(personId) then
            let updatedCharacter = {personToBeUpdated with Id = personId}
            resourceStore.[personId] <- updatedCharacter
            Some updatedCharacter
        else
            None

    let updateCharacter personToBeUpdated =
        updateCharacterById personToBeUpdated.Id personToBeUpdated

    let deleteCharacter personId =
        resourceStore.Remove(personId) |> ignore

    let characterExists  = resourceStore.ContainsKey
