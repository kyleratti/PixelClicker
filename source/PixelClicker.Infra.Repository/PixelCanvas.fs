module PixelClicker.Infra.Repository.PixelCanvas

open System.Threading
open FSharp.Control
open FruityFoundation.DataAccess.Abstractions
open FruityFoundation.DataAccess.Abstractions.FSharp
open PixelClicker.Core.Contracts
open PixelClicker.Core.Models

type PixelCanvasRepository () =
    let getCanvasSnapshot (connection : ReadOnly IDatabaseConnection) (cancellationToken : CancellationToken) = taskSeq {
        let sql = """
            SELECT
                x_coordinate AS X
                ,y_coordinate AS Y
                ,color_hex AS ColorHex
                ,max_pixel_id AS MaxPixelId
            FROM latest_pixel_canvas
            """
        let parms = Array.empty

        yield! (sql, parms)
            ||> ReadOnlyDb.queryUnbuffered<CanvasPixelDto> connection cancellationToken
    }

    let setPixelColor (connection : ReadWrite IDatabaseConnection) (cancellationToken : CancellationToken) (coordinates : int * int) (color : string) = task {
        let sql = """
            INSERT INTO pixel_canvas (x_coordinate, y_coordinate, color_hex, created_at)
            VALUES (@x, @y, @color, CURRENT_TIMESTAMP)
            RETURNING pixel_id
            """
        let parms = [|
            ("@x", box (coordinates |> fst))
            ("@y", box (coordinates |> snd))
            ("@color", box color)
        |]

        return! (sql, parms) ||> ReadWriteDb.execute connection cancellationToken
    }

    interface IPixelCanvasRepository with
        member this.GetCanvasSnapshot (connection : ReadOnly IDatabaseConnection, cancellationToken : CancellationToken) = taskSeq {
            yield! getCanvasSnapshot connection cancellationToken
        }

        member this.SetPixelColor(connection : ReadWrite IDatabaseConnection, x : int, y : int, color : string, cancellationToken : CancellationToken) = task {
            return! ((x, y), color) ||> setPixelColor connection cancellationToken
        }
