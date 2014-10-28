namespace prismic
open FSharp.Data
open FSharp.Data.JsonExtensions

module Experiments =

    type Variation = {
        id: string;
        ref: string;
        label: string }
    with
        static member fromJson : JsonValue -> Variation
    end

    and Experiment = {
        id: string;
        googleId: string;
        name: string;
        variations: Variation seq }
    with
        static member fromJson : JsonValue -> Experiment
    end

    and Experiments = { draft : Experiment seq; running: Experiment seq }
    with
        static member empty : Experiments
        static member fromJson : JsonValue -> Experiments
    end

