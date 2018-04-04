#r @"../packages/FSharp.Data/lib/net45/FSharp.Data.dll"

open FSharp.Data
open System

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

let allCharacters = loadPeople root.People
let withFilms = allCharacters |> Seq.map (fun p -> p, p.Films |> Seq.map Film.Load)
let (_,filmsStarringDarthVader) = withFilms |> Seq.find (fun (p,films) -> p.Name.Contains("Darth Vader"))
filmsStarringDarthVader |> Seq.map (fun f -> f.Title) |> Seq.toList

printfn "%s" filmsStarringDarthVader
