module FSTest.TransitionsHandling
open System
open FSharp.Data
open FSTest.Records
open FSTest.Utils

// function to record transition in a record, with elements in string, char and action
let elementToTransition elementInitialState (element: (string * JsonValue) array) =
    if element.Length <> 4 then
        invalidArg (nameof element) $"Transitions states need to be 4 length list ({elementInitialState})"
    try
    let recorded = {
                   initialState = elementInitialState;
                   initialChar = JsonExtensions.AsString (snd element[0]) |> stringToChar;
                   targetState = JsonExtensions.AsString (snd element[1]);
                   targetChar = JsonExtensions.AsString (snd element[2]) |> stringToChar;
                   targetAction =
                       match JsonExtensions.AsString (snd element[3]) with
                       | "RIGHT" -> RIGHT
                       | "LEFT" -> LEFT
                       | _ -> NONE;
                   }
    recorded
    with
    | :? IndexOutOfRangeException -> invalidArg (nameof element)
                                         $"Read and write chars can't be none ({elementInitialState})"

// element to transition with the try with
let elementToTransitionSafe elementInitialState (element: (string * JsonValue) array) =
    try
        elementToTransition elementInitialState element
    with
        | :? ArgumentException as e -> printfn $"{e.Message}" ; exit(1)

// function to record all transitions in an array
let recordTransition (element: string * JsonValue) =
    let elementInitialState = fst element
    let statesArray = element |> snd
    let recorded =
        statesArray
        |> JsonExtensions.AsArray
        |> Array.map JsonExtensions.Properties
        |> Array.map (elementToTransitionSafe elementInitialState)
    recorded

// function to represent a transition in a string
let transitionToString (state: transition) =
    $"({state.initialState}, {state.initialChar}) \
    -> ({state.targetState}, {state.targetChar}, {state.targetAction})"

// function to display a transition state
let displayState (state: transition) =
    printfn $"{(transitionToString state)}"
