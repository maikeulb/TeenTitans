namespace TeenTitansApi.Db

open System.Collections.Generic

type Titan = {
    Id : int
    Name : string
    Age : int
    Mentor : string
}

module Db =

    let titanStorage = new Dictionary<int, Titan>()
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
