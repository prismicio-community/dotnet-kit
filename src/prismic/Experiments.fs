namespace prismic
open Microsoft.FSharp.Collections
open FSharp.Data
open FSharp.Data.JsonExtensions

module Experiments =

    type Variation = { id: string; ref: string; label: string }
                        static member fromJson (json:JsonValue) = {
                            id = json?id.AsString()
                            ref = json?ref.AsString()
                            label = json?label.AsString()
                        }

    and Experiment = { id: string; googleId: string; name: string; variations: Variation seq }
                        static member fromJson (json:JsonValue) = {
                            id = json?id.AsString()
                            googleId = json?googleId.AsString()
                            name = json?name.AsString()
                            variations = json?variations.AsArray() |> Array.map Variation.fromJson
                        }

    and Experiments = { draft: Experiment seq; running: Experiment seq }
                        static member empty = { draft = Array.empty ; running = Array.empty }
                        static member fromJson (json:JsonValue) = {
                            draft = json?draft.AsArray() |> Array.map Experiment.fromJson
                            running = json?running.AsArray() |> Array.map Experiment.fromJson
                        }

