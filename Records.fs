module FSTest.Records

// record to represent the action target in the machine description
type action =
    | NONE
    | RIGHT
    | LEFT

// record to represent and implement transitions
type transition = {
    initialState: string
    initialChar: char
    targetState: string
    targetChar: char
    targetAction: action
}

// record to implement one machine state
type machineState = {
    state: transition
    tape: char list
    headPos: int
}

// record to implement the full machine, main record used in the program
type turingMachine = {
    states: transition array
    mutable state: string
    finales: string list
    mutable tape: char list
    mutable headPos: int
    mutable history: machineState list
}
