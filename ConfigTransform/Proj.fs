module ConfigTransform.Proj

open Microsoft.Build.Evaluation
open System.IO
open System.Collections.Generic

let pathCombine a b = Path.Combine(a, b)

let getConfigFiles (file:string) (excludes:ISet<string>) =
    ProjectCollection.GlobalProjectCollection.UnloadAllProjects()
    let dir = Path.GetDirectoryName file
    let p = Project(file)
    p.Items
    |> Seq.filter (fun i -> i.ItemType = "Content")
    |> Seq.map (fun i -> i.EvaluatedInclude)
    |> Seq.filter (fun path -> Path.GetExtension path = ".config")
    |> Seq.filter (fun path -> false = (Path.GetFileNameWithoutExtension path).Contains ".")
    |> Seq.filter (fun path -> false = excludes.Contains path)
    |> Seq.map (fun path -> pathCombine dir path |> Path.GetFullPath)
    |> Seq.toArray
