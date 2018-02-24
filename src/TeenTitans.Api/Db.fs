namespace TeenTitansApi.Db

open System
open System.Collections.Generic

type Titan = {
    Id : int
    Name : string
    BirthYear : string
    Email : string    
}

module Db =
    open FSharp.Data

    let titanStorage = new Dictionary<int, Titan>()

    (* [<Literal>] *)
    (* let baseUrl = "https://swapi.co/api/" *)
    (* [<Literal>] *) 
    (* let peopleUrl = baseUrl + "people/" *)

(*     type StarWarsAllPeople = JsonProvider<"https://swapi.co/api/people/?page=3"> *)
(*     (1* type Paging = JsonProvider<"""{"next": "https://swapi.co/api/people/?page=2"}"""> *1) *)
(*     let peopleParser url = StarWarsAllPeople.Parse(url).Results *)
(*     (1* let rec getAll parser nextUrl acc = *1) *)
(*     (1*     match nextUrl with *1) *) 
(*     (1*     | "" -> List.rev acc |> Array.concat *1) *)
(*     (1*     | url -> *1) *)
(*     (1*         let text = Http.RequestString(url) *1) *)
(*     (1*         (2* let next = Paging.Parse(text).Next *2) *1) *)
(*     (1*         (2* let contents = parser text *2) *1) *)
(*     (1*         (2* getAll parser next (contents::acc) *2) *1) *)

    (* let getAllStarWarsPeople () = peopleParser peopleUrl [] *) 
    let [<Literal>] URL = "http://swapi.co/api/"
    let [<Literal>] PeopleUrl = URL + "people/1"
    let [<Literal>] PagePeopleUrl = URL + "people/"
    let [<Literal>] FilmUrl = URL + "films/1"

    type SWAPI = JsonProvider<URL>
    type PagePeople = JsonProvider<PagePeopleUrl>
    type People = JsonProvider<PeopleUrl>
    type Film = JsonProvider<FilmUrl>

    let root = SWAPI.GetSample()
    let buildUrl url id = sprintf "%s%s" url id

    let loadPeople (pageUrl) = 
       let rec iter (pageUrl : string) =
           seq{
               if (pageUrl <> "") then
                  let currentPage = PagePeople.Load(pageUrl)
                  yield! currentPage.Results |> Array.toSeq
                  yield! iter currentPage.Next
           }
       iter pageUrl

    let starWarsPeople = loadPeople root.People


    (* let starWarsPeople = peopleParser peopleUrl [] *)

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

    starWarsPeople
    |> Seq.iter (fun p -> 
            let newPerson = {
                Id          = p.Url.Substring(PagePeopleUrl.Length) |> extractNumber
                Name        = p.Name
                BirthYear   = p.BirthYear
                Email       = sprintf "%s@starwars.com" <| p.Name.Replace(" ", "_")
            }
            titanStorage.Add(newPerson.Id, newPerson))
    
    let getTitan () =
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
