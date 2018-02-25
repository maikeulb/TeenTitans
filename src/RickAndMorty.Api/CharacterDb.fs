namespace RickAndMortyApi.Db

open System
open System.Collections.Generic

module CharacterDb =
    open FSharp.Data

    type RickAndMortyCharacter = {
        Id : int
        Name : string
        Status : string
        Gender : string
        Species: string
        Origin : string
    }

    let characterStore = new Dictionary<int, RickAndMortyCharacter>()

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
            characterStore.Add(newCharacter.Id, newCharacter))
    
    let getCharacters () =
        characterStore.Values :> seq<RickAndMortyCharacter>
    let getCharacterById id =
        if characterStore.ContainsKey(id) then
            Some characterStore.[id]
        else
            None
    let createCharacter character =
        let id = characterStore.Values.Count + 1
        let newCharacter = {character with Id = id}
        characterStore.Add(id, newCharacter)
        newCharacter

    let updateCharacterById characterId characterToBeUpdated =
        if characterStore.ContainsKey(characterId) then
            let updatedCharacter = {characterToBeUpdated with Id = characterId}
            characterStore.[characterId] <- updatedCharacter
            Some updatedCharacter
        else
            None

    let updateCharacter characterToBeUpdated =
        updateCharacterById characterToBeUpdated.Id characterToBeUpdated

    let deleteCharacter characterId =
        characterStore.Remove(characterId) |> ignore

    let characterExists  = characterStore.ContainsKey
