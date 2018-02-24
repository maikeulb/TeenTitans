namespace RickAndMortyApi.Db

open System
open System.Collections.Generic

type Titan = {
    Id : int
    Name : string
}

module Db =
    open FSharp.Data

    let titanStorage = new Dictionary<int, Titan>()

    let [<Literal>] URL = "https://rickandmortyapi.com/api/"
    let [<Literal>] CharacterUrl = URL + "character/1"
    let [<Literal>] PageCharacterUrl = URL + "character/"

    type SWAPI = JsonProvider<URL>
    type PageCharacter = JsonProvider<PageCharacterUrl>
    type Character = JsonProvider<CharacterUrl>

    let root = SWAPI.GetSample()
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

    let starWarsCharacter = loadCharacter root.Characters

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

    starWarsCharacter
    |> Seq.iter (fun p -> 
            let newPerson = {
                Id          = p.Url.Substring(PageCharacterUrl.Length) |> extractNumber
                Name        = p.Name
            }
            titanStorage.Add(newPerson.Id, newPerson))
    
    let getTitans () =
        titanStorage.Values :> seq<Titan>
    let getTitanById id =
        if titanStorage.ContainsKey(id) then
            Some titanStorage.[id]
        else
            None
    let createTitan person =
        let id = titanStorage.Values.Count + 1
        let newTitan = {person with Id = id}
        titanStorage.Add(id, newTitan)
        newTitan

    let updateTitanById personId personToBeUpdated =
        if titanStorage.ContainsKey(personId) then
            let updatedTitan = {personToBeUpdated with Id = personId}
            titanStorage.[personId] <- updatedTitan
            Some updatedTitan
        else
            None

    let updateTitan personToBeUpdated =
        updateTitanById personToBeUpdated.Id personToBeUpdated

    let deleteTitan personId =
        titanStorage.Remove(personId) |> ignore

    let titanExists  = titanStorage.ContainsKey
