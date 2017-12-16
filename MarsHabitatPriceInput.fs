namespace MacCode6

open System
open Foundation
open AppKit
open CoreML
type MarsHabitatPricerInput ()  = 
    inherit NSObject()

    let mutable solarPanels = 1. 
    let mutable greenhouses = 2.
    let mutable plotsize = 1000.

    let inputsChanged = new Event<IMLFeatureProvider>()

    [<CLIEvent>]
    member this.InputsChanged = inputsChanged.Publish

    member this.SolarPanelCount f  = solarPanels <- f ; inputsChanged.Trigger(this) 
    member this.GreenhousesCount f = greenhouses <- f; inputsChanged.Trigger(this) 
    member this.Plotsize f = plotsize <- f; inputsChanged.Trigger(this) 

    interface IMLFeatureProvider with 
        member this.FeatureNames =  
             ["solarPanels"; "greenhouses"; "size"]
             |> List.map (fun s -> new NSString())
             |> Array.ofList
             |> fun ss -> new NSSet<NSString>(ss)

        member this.GetFeatureValue featureName = 
            match featureName with 
            | "solarPanels" -> MLFeatureValue.Create(solarPanels)
            | "greenhouses" -> MLFeatureValue.Create(greenhouses)
            | "size" -> MLFeatureValue.Create(plotsize)
            | _ -> raise <| new ArgumentOutOfRangeException("featureName")
