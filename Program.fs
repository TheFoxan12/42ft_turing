open System
open System.IO
open FSTest
open FSharp.Data
open Parsing
open Display
open Machine
open TransitionsHandling
open Records
open Utils

let main =
    let args = Environment.GetCommandLineArgs()
    if not <| isArgsValid(args) then
        printfn $"{error_message}"
        exit(1)

    let file = args[1]
    if not <| File.Exists(file) then
        printfn $"The file {file} in argument does not exist"
        exit(1)

    let rawMachine =
        try
            Machine.Load(Path.GetFullPath(file))
        with
        | _ -> printfn "Error while parsing the json file. \
        The Json file must be a valid json file and follow the model."; exit(1)

    // parse the name
    let machineName = jsonDocumentToString rawMachine.Name
    //

    // parse and check the alphabet
    let alphabetString = jsonArrayToString rawMachine.Alphabet

    if not <| isAlphabetValid(alphabetString) then
        printfn "Alphabet is invalid, it contains elements with multiple characters"
        exit(1)
    let alphabet = stringListToChar alphabetString
    //

    // parse and check the blank character
    let blankString = jsonDocumentToString rawMachine.Blank
    if elementSizeTooLong blankString then
        printfn $"The blank character must be only one character"
        exit(1)
    let blank = stringToChar blankString
    if alphabet |> List.contains blank |> not then
        printfn $"The blank character must be part of the alphabet"
        exit(1)
    //

    // parse and check states
    let states = jsonArrayToString rawMachine.States
    if states.Length = 0 then
        printf "There must be at least one state in the states list"
        exit(1)
    //

    // parse and check initial state
    let initial = jsonDocumentToString rawMachine.Initial
    if states |> List.contains initial |> not then
        printfn "The initial state must be part of the possible states list"
        exit(1)
    //

    // parse and check finals states
    let finalStates = jsonArrayToString rawMachine.Finals
    if finalStates |> List.exists (fun element -> List.contains element states |> not) then
        printfn "Final states must be part of the possible states list"
        exit(1)
    //

    // parse and check transitions
    let transitionsRaw = rawMachine.Transitions
    let transitions = transitionsRaw.JsonValue.Properties()

    let recorded =
        transitions
        |> Array.collect recordTransition

    let isInAlphabet (element: char) =
        alphabet |> List.contains element

    let isInStates (element: string) =
        states |> List.contains element

    let isValidAction (element: action) =
        element = RIGHT || element = LEFT

    let isTransitionValid (element: transition) =
        isInAlphabet element.initialChar
        && isInAlphabet element.targetChar
        && isInStates element.initialState
        && isInStates element.targetState
        && isValidAction element.targetAction

    let isTransitionInvalid (element: transition) =
        if isTransitionValid element |> not then
            printfn $"invalid element : {element}"
            true
        else
            false

    let areTransitionsInvalid (transitions: transition array): bool =
        transitions
        |> Array.exists isTransitionInvalid

    if areTransitionsInvalid recorded then
        printfn "Transitions states must be part of states, \
        read and write characters must be part of the alphabet, action must be RIGHT or LEFT"
        exit(1)

    if recorded |> Array.exists (fun (elt: transition) -> (finalStates |> List.contains elt.targetState)) |> not then
        printfn "No transition can lead to a final state"
        exit(1)
    //

    printfn "**********************************************************************"
    printfn $"*                          {machineName}                          *"
    printfn "**********************************************************************"
    printListCom "Alphabet : " alphabetString
    printListCom "States : " states
    printfn $"Initial : {initial}"
    printListCom "Finals : " finalStates
    recorded
    |> Array.iter displayState
    printfn "**********************************************************************"

    let input = args[2]
    let tape = input |> Seq.toList
    if tape.IsEmpty then
        printfn "Input cannot be empty"
        exit(1)
    if tape |> List.exists (fun character -> (List.contains character alphabet |> not)) then
        printfn "All elements from input must be part of the alphabet"
        exit(1)

    let turingMachine = {
        states=recorded
        state=initial
        finales=finalStates
        tape=tape
        headPos=0
        history=[]
        }

    let rec execute (machine: turingMachine) =
        if machine.finales |> List.contains machine.state then
            machine
        else
            let currentChar = machine.tape[machine.headPos]
            let currentTransition =
                try
                machine.states |> Array.find (fun element ->
                    element.initialState = machine.state
                    && element.initialChar = currentChar
                    )
                with
                    | :? System.Collections.Generic.KeyNotFoundException ->
                        printfn $"no case found for state {machine.state} and char {currentChar} at \
                        [{machineTapeToString machine}]"; exit(1)

            let mutable newHeadPos =
                match currentTransition.targetAction with
                | RIGHT -> machine.headPos + 1
                | LEFT -> machine.headPos - 1
                | NONE -> machine.headPos

            let updatedCharTape =
                machine.tape |> updateElement machine.headPos currentTransition.targetChar

            logMachineState machine currentTransition

            // checking if the machine is going forever in one direction
            let currentStateTransitions =
                let targets =
                    recorded
                    |> Array.choose (
                        fun elt -> if elt.initialState = machine.state then Some elt.targetState else None)
                recorded |> Array.choose (fun elt -> if (targets |> Array.contains elt.initialState) then Some elt else None)

            let willChangeTo direction =
                currentStateTransitions |> Array.exists (fun elt ->
                    elt.initialChar = blank && (elt.targetAction = direction))

            let manageRight tape =
                if willChangeTo LEFT then
                    List.append tape ['.']
                else
                    printfn "The machine is going to the right forever"
                    exit(1)

            let manageLeft tape =
                if willChangeTo RIGHT then
                    newHeadPos <- 0; List.append ['.'] tape
                else
                    printfn "The machine is going to the left forever"
                    exit(1)
            //

            machine.tape <-
                if newHeadPos = updatedCharTape.Length || newHeadPos < 0
                then
                    match currentTransition.targetAction with
                    | RIGHT -> manageRight updatedCharTape
                    | LEFT -> manageLeft updatedCharTape
                    | NONE -> exit(3)
                else
                    updatedCharTape

            let history_tape =
                machine.tape |> stripTape blank
            let newMachineState =
                {state=currentTransition; tape=history_tape; headPos=machine.headPos}

            if machine.history |> List.contains newMachineState then
                printfn "There is an infinite loop in the program"; exit(1)
            machine.history <-
                machine.history |> List.append [newMachineState]

            machine.state <- currentTransition.targetState
            machine.headPos <- newHeadPos
            execute machine

    let output = execute turingMachine
    printfn "result : [%A]" (charListToString output.tape)
    0

main |> ignore
