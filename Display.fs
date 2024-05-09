module FSTest.Display

// function to print a list
let printList liste =
    printf "[ "
    liste |> List.iter (printf "%s, ")
    printfn "]"

// function to print a list with a comment in the beginning
let printListCom comment liste =
    printf $"{comment}"
    printList liste
