module TeenTitansApi.Main

open Suave
open Suave.Successful

[<EntryPoint>]
let main argv =
    startWebServer defaultConfig (OK "Hello World!")
    0
