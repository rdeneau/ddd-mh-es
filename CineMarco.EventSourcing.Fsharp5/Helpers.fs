module Helpers

module ListHelper =
  let intersect list1 list2 =
    Set.intersect (Set.ofList list1) (Set.ofList list2)
    |> Set.toList

module MapHelper =
  let changeMany keys mapper table =
    let mapEntry key =
      let item =
        table
        |> Map.find key
        |> mapper
      (key, item)

    keys
    |> List.map mapEntry
    |> Map.ofList
