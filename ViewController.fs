namespace MacCode6
open System
open Foundation
open AppKit
open CoreGraphics
open CoreML

[<Register("ViewController")>]
type ViewController (handle : IntPtr)  = 
   inherit NSViewController (handle)

   let LoadCoreMLModel modelName = 
      let assetPath = NSBundle.MainBundle.GetUrlForResource(modelName, "mlmodelc")
      let (model, err) = MLModel.Create(assetPath)
      match err <> null with 
      | true -> raise <| new Exception(err.ToString())
      | false -> model

   member val Model : MLModel = LoadCoreMLModel("MarsHabitatPricer")

   member val SolarPanelView = new InputView("Solar Panels", [ 1 .. 5])
   member val GreenhousesView = new InputView("Greenhouses", [1.. 5])
   member val PlotsizeView = new InputView("Acres", [ 1000 .. 1000 .. 10000])
   member val PricePredictionView = new OutputView()

   member val PriceInput = new MarsHabitatPricerInput()

   member this.UpdatePrediction (price : double) = this.PricePredictionView.Price <- price

   override this.ViewDidLoad () = 
        let buildUI () =
            let halfHeight = float(this.View.Frame.Height) / 2.
            let thirdWidth = float(this.View.Frame.Width) / 3.

            this.View.AddSubview(this.SolarPanelView)
            this.SolarPanelView.TranslatesAutoresizingMaskIntoConstraints <- false

            this.GreenhousesView.TranslatesAutoresizingMaskIntoConstraints <- false
            //this.GreenhousesView.BackgroundColor <- NSColor.Blue.CGColor
            this.View.AddSubview(this.GreenhousesView)

            this.PlotsizeView.TranslatesAutoresizingMaskIntoConstraints <- false
            //this.PlotsizeView.BackgroundColor <- NSColor.Green.CGColor
            this.View.AddSubview(this.PlotsizeView)

            this.PricePredictionView.TranslatesAutoresizingMaskIntoConstraints <- false
            //this.PricePredictionView.BackgroundColor <- NSColor.Yellow.CGColor
            this.View.AddSubview(this.PricePredictionView)

            [ 
               this.SolarPanelView.LeadingAnchor.ConstraintEqualToAnchor this.View.LeadingAnchor;
               this.SolarPanelView.WidthAnchor.ConstraintEqualToAnchor this.GreenhousesView.WidthAnchor;
               this.SolarPanelView.TopAnchor.ConstraintEqualToAnchor this.View.TopAnchor;

               this.GreenhousesView.LeadingAnchor.ConstraintEqualToAnchor this.SolarPanelView.TrailingAnchor;
               this.GreenhousesView.TopAnchor.ConstraintEqualToAnchor this.SolarPanelView.TopAnchor;
               this.GreenhousesView.BottomAnchor.ConstraintEqualToAnchor this.SolarPanelView.BottomAnchor;
               this.GreenhousesView.WidthAnchor.ConstraintEqualToAnchor this.PlotsizeView.WidthAnchor;

               this.PlotsizeView.LeadingAnchor.ConstraintEqualToAnchor this.GreenhousesView.TrailingAnchor;
               this.PlotsizeView.TopAnchor.ConstraintEqualToAnchor this.SolarPanelView.TopAnchor;
               this.PlotsizeView.BottomAnchor.ConstraintEqualToAnchor this.SolarPanelView.BottomAnchor;
               this.PlotsizeView.TrailingAnchor.ConstraintEqualToAnchor this.View.TrailingAnchor;

               this.PricePredictionView.LeadingAnchor.ConstraintEqualToAnchor this.View.LeadingAnchor;
               this.PricePredictionView.TrailingAnchor.ConstraintEqualToAnchor this.View.TrailingAnchor;
               this.PricePredictionView.TopAnchor.ConstraintEqualToAnchor this.SolarPanelView.BottomAnchor;
               this.PricePredictionView.HeightAnchor.ConstraintEqualToAnchor this.SolarPanelView.HeightAnchor;
               this.PricePredictionView.BottomAnchor.ConstraintEqualToAnchor this.View.BottomAnchor;

            ]
            |> List.iter (fun c -> c.Active <- true)

        buildUI()

        // Wire up UI widgets to class defining machine-learning model inputs 
        this.SolarPanelView.Updated.Add this.PriceInput.SolarPanelCount
        this.GreenhousesView.Updated.Add this.PriceInput.GreenhousesCount
        this.PlotsizeView.Updated.Add this.PriceInput.Plotsize

        // When the input object has changed, have the model generate a prediction
        this.PriceInput.InputsChanged.Add (fun featureProvider -> 
            let (prediction, err) = this.Model.GetPrediction(featureProvider)
            match err <> null with 
            | true -> raise <| new Exception (err.ToString())
            | false -> this.UpdatePrediction(prediction.GetFeatureValue("price").DoubleValue)
        )