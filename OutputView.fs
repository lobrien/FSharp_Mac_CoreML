namespace MacCode6
open System
open Foundation
open AppKit
open CoreGraphics
  
[<AllowNullLiteral>]
type OutputView () as this = 
    inherit NSView ()

    let label = new NSText()

    do
        label.Value <- "Prediction"
        label.Editable <- false
        this.AddSubview label

        label.Frame <- new CGRect(10., 10., 300., 50.)
    
    member this.Price with set p = label.Value <- String.Format("Predicted price: {0:#,###} Quatloos", [| p |])
