// fsharplint:disable FL0065
module Aurum.Interop.ProcessWrapper

open System.Diagnostics
open System.Threading

type V2rayProcess
    (
        v2rayProcessInfo: ProcessStartInfo,
        loggerTaskBuilder: System.IO.StreamReader -> CancellationToken -> Tasks.Task<unit>
    ) =
    let disposed = false
    let v2rayProcessInfo = v2rayProcessInfo
    let cancelLogging = new CancellationTokenSource()

    interface System.IDisposable with
        member this.Dispose() =
            if (not disposed) then
                cancelLogging.Cancel()
                cancelLogging.Dispose()
                this.Process.Dispose()
                disposed = false |> ignore

    member this.Process = new Process()

    member this.Start() =
        (this.Process = Process.Start(v2rayProcessInfo))
        |> ignore

        let loggerTask =
            loggerTaskBuilder this.Process.StandardOutput cancelLogging.Token

        loggerTask.Start()
        ()

    member this.Stop() =
        cancelLogging.Cancel()
        this.Process.Kill(true)

let startV2rayProcess executablePath (configPath: string) =
    let v2rayProcessInfo =
        ProcessStartInfo(executablePath, $"-config={configPath} -format=json")

    v2rayProcessInfo.UseShellExecute = false |> ignore
    v2rayProcessInfo.CreateNoWindow = false |> ignore

    v2rayProcessInfo.RedirectStandardError = true
    |> ignore

    v2rayProcessInfo.RedirectStandardOutput = true
    |> ignore

    let createV2rayLoggerTask (stdout: System.IO.StreamReader) (cancellationToken: CancellationToken) =
        backgroundTask {
            while (not cancellationToken.IsCancellationRequested) do
                let! logLine = stdout.ReadLineAsync()
                LogStream.logStream.Trigger logLine
        }

    new V2rayProcess(v2rayProcessInfo, createV2rayLoggerTask)
