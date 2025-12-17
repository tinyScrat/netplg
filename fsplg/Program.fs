open System
open System.Threading
open Suave
open Proto
open RabbitPlg
open PostgresPlg

[<EntryPoint>]
let main _ =
    let actorSystem = ActorSystem()
    let props = Props.FromProducer(fun () -> GreetingActor() :> IActor)
    let pid = actorSystem.Root.Spawn props
    actorSystem.Root.Send(pid, { Who = "Jesse" })

    let connStr = "Host=localhost;Username=jesse;Database=jesse"
    testDb connStr

    let cts = new CancellationTokenSource()
    let conf = { defaultConfig with
                    cancellationToken = cts.Token
                    bindings = [ HttpBinding.createSimple HTTP "0.0.0.0" 8000] }
    let listening, server =
        startWebServerAsync conf (Successful.OK "Hello, world")

    Async.Start(server, cts.Token)

    Console.ReadKey true |> ignore

    cts.Cancel()

    0
