namespace MacCode6
open System
open Foundation
open AppKit
open CoreGraphics
open ObjCRuntime
  
[<AllowNullLiteral>]
type InputView (identifier : string, items : int list) as this = 
    inherit NSView ()

    let updated = new Event<float>()

    let spinner = new NSPopUpButton()

    do
      let label = new NSText()
      label.Value <- identifier
      label.Editable <- false
      this.AddSubview label

      this.AddSubview spinner

      spinner.Target <- this
      spinner.Action <- new Selector("SpinnerValueSelected")

      items 
      |> List.map (fun i -> i.ToString())
      |> List.toArray
      |> spinner.AddItems

      label.Frame <- new CGRect(10., 10., 100., 50.)
      spinner.Frame <- new CGRect(10., 70., 100., 50.)

    [<CLIEvent>]
    member this.Updated = updated.Publish

    [<Outlet("SpinnerValueSelected")>]
    member this.SpinnerValueSelected = 
       spinner.TitleOfSelectedItem
       |> float
       |> updated.Trigger
