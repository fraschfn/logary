﻿module Logary.Tests.Lifecycles

open NodaTime
open Fuchu
open Logary.Configuration
open Logary
open Logary.Targets
open Logary.Metrics
open Hopac
open Hopac.Infixes
open TestDSL

[<Tests>]
let tests =
  testList "lifecycles" [

    testCase "logary" <| fun _ ->
      Config.confLogary "tests"
      |> Config.validate
      |> Config.runLogary
      |> run
      |> Config.shutdownSimple
      |> run
      |> ignore

    testCase "target" <| fun _ ->
      let target =
        Target.confTarget (pn "tw") (TextWriter.create (TextWriter.TextWriterConf.Create(Fac.textWriter(), Fac.textWriter())))
        |> Target.validate
        |> Target.init Fac.emptyRuntime
        |> run

      Message.eventDebug "Hello"
      |> Target.log target
      |> run
      |> ignore

      Target.shutdown target <|> (timeOutMillis 200 ^->. Promise.Now.never ())
      |> run
      |> ignore
    ]
