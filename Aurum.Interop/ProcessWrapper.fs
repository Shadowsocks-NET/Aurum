// fsharplint:disable FL0065
module Aurum.Interop.ProcessWrapper

open System.Diagnostics
open System.Threading

type CoreProcess
    (
        coreProcessInfo: ProcessStartInfo,
        loggerTaskBuilder: System.IO.StreamReader -> CancellationToken -> Tasks.Task<unit>
    ) =
    let disposed = false
    let coreProcessInfo = coreProcessInfo
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
        (this.Process = Process.Start(coreProcessInfo))
        |> ignore

        let loggerTask =
            loggerTaskBuilder this.Process.StandardOutput cancelLogging.Token

        loggerTask.Start()
        ()

    member this.Stop() =
        cancelLogging.Cancel()
        this.Process.Kill(true)

let startCoreProcess executablePath (configPath: string) =
    let coreProcessInfo =
        ProcessStartInfo(executablePath, $"-config={configPath} -format=json")

    coreProcessInfo.UseShellExecute = false |> ignore
    coreProcessInfo.CreateNoWindow = false |> ignore

    coreProcessInfo.RedirectStandardError = true
    |> ignore

    coreProcessInfo.RedirectStandardOutput = true
    |> ignore

    let createCoreLoggerTask (stdout: System.IO.StreamReader) (cancellationToken: CancellationToken) =
        backgroundTask {
            while (not cancellationToken.IsCancellationRequested) do
                let! logLine = stdout.ReadLineAsync()
                LogStream.logStream.Trigger logLine
        }

    new CoreProcess(coreProcessInfo, createCoreLoggerTask)
