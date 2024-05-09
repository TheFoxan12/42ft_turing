module FSTest.Utils
open System
open FSharp.Data

let jsonDocumentToString (element : Runtime.BaseTypes.IJsonDocument) =
    element.ToString()[1..element.ToString().Length - 2]
// function that helps to deal with IJsonDocument interface
// used to cast to string

let jsonArrayToList liste =
    Array.toList liste

let jsonListToString liste =
    List.map jsonDocumentToString liste

let jsonArrayToString liste =
    liste |> jsonArrayToList |> jsonListToString

let stringToChar (element: string) =
    element[0]

let stringListToChar liste =
    List.map stringToChar liste

let charListToString (chars: char list) =
    chars
    |> Array.ofList
    |> String
