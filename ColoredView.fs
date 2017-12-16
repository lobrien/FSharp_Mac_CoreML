namespace MacCode6

open System
open Foundation
open AppKit
open CoreGraphics

[<AllowNullLiteral>]
type ColoredView () = 
   inherit NSView () 

   member val BackgroundColor : CGColor = NSColor.Red.CGColor with get, set
   override this.WantsUpdateLayer = true

   override this.UpdateLayer () = 
      this.Layer.BackgroundColor <- this.BackgroundColor

   member this.SetBackgroundColor (c : NSColor) = this.BackgroundColor <- c.CGColor

   override this.IntrinsicContentSize = new CGSize(200., 400.)

   override this.ToString () = sprintf "ColoredView [%s] %s" (this.Frame.ToString()) (this.BackgroundColor.Components.ToString())
