namespace TeenTitansApi.Rest

[<AutoOpen>]
module Titans =  

  type TitansResource<'a> = {
    GetAllTitans : unit -> 'a seq        
  }
