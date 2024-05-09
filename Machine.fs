module FSTest.Machine
open System.IO
open FSharp.Data
open FSTest.Records
open FSTest.Utils
open FSTest.TransitionsHandling

// model.json is the model to load turing machines
// testing if it exists before loading it in a provider
if not <| File.Exists("model.json") then
        printfn "The model file is missing"
        exit(1)
type Machine = JsonProvider<"model.json">
//

// function to convert machine tape to string representation with angle brackets
// at the head position
let machineTapeToString (machine: turingMachine) =
    machine.tape
    |> List.insertAt(machine.headPos + 1) '>'
    |> List.insertAt(machine.headPos) '<'
    |> charListToString
//

// function to modify an element in a list at a given index
let updateElement index newVal targetList =
    targetList |> List.indexed |> List.map (fun (i, elmt) -> if i = index then newVal else elmt)

// functions to strip the blanks around the tape
let stripBegin blank tape =
    let index = tape |> List.tryFindIndex (fun elt -> elt <> blank)
    match index with
    | Some(a) -> tape[a..]
    | None -> tape

let stripEnd blank tape =
    let index = tape |> List.tryFindIndexBack (fun elt -> elt <> blank)
    match index with
    | Some(a) -> tape[..a]
    | None -> tape

let stripTape blank tape =
    tape |> stripBegin blank |> stripEnd blank
//

// function to display the machine state
let logMachineState machine transition =
    printfn "[%A] %A" (machineTapeToString machine) (transitionToString transition)
