module FSTest.Parsing

let error_message = "usage: ft_turing [-h] jsonfile input\n
positional arguments:
  jsonfile\t\tjson description of the machine
  input\t\t\tinput of the machine

optional arguments:
  -h, --help\t\tshow this help message and exit"
// error and help message to be displayed when -h or --help argument is used
// or when the program is called with wrong arguments

let isArgsValid (args : string array) : bool =
    args.Length = 3
    && (args |> Array.contains("-h") |> not)
    && (args |> Array.contains("--help") |> not)
// function to verify if args passed to the program are valid i.e.
// contains 2 elements and not -h or --help

let elementSizeTooLong (element: string)=
    element.Length > 1

let listContainsTooLong liste =
    List.exists elementSizeTooLong liste

let isAlphabetValid alphabet =
    listContainsTooLong alphabet |> not
