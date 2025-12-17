module RabbitPlg

open System
open System.Threading.Tasks
open Proto

type Hello = { Who: string }

type GreetingActor() =
    interface IActor with
        member this.ReceiveAsync (context: IContext): Task = 
            match context.Message with
            | :? Hello as hello ->
                printfn "Hello, %s!" hello.Who
            | _ ->
                ()
            Task.CompletedTask
