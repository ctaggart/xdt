namespace ConfigTransform

open System
open System.IO
open Microsoft.Build.Framework
open System.Collections.Generic

type ConfigTransform() =
    inherit Task()

    [<Required>]
    member val ProjectFile = String.Empty with set, get
    [<Required>]
    member val Exclude = String.Empty with set, get
    [<Required>]
    member val Configuration = String.Empty with set, get
    [<Required>]
    member val EnvVar = String.Empty with set, get
    
    member internal x.GetProjectFile() =
        x.ProjectFile |> Path.GetFullPath

    member internal x.GetConfigFiles() =
        let excludes = HashSet<string>(StringComparer.OrdinalIgnoreCase)
        if false = String.IsNullOrEmpty x.Exclude then
            for exclude in x.Exclude.Split [|';'|] do
                excludes.Add exclude |> ignore
        Proj.getConfigFiles (x.GetProjectFile()) excludes

    member internal x.GetEnvVars() =
        x.EnvVar.Split [|';'|]
        |> Seq.map (fun var ->
            var, Environment.GetEnvironmentVariable var
        )

    override x.Execute() =
        try
            for c in x.GetConfigFiles() do
                x.MessageHigh "config file: %s" c
            x.MessageHigh "configuration: %s" x.Configuration
            x.MessageHigh "env vars: %A" (x.GetEnvVars())
            () // TODO
        with
        | _ as ex -> x.Error "%s" ex.Message

        not x.HasErrors
    