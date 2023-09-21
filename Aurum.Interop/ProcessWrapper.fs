// fsharplint:disable FL0065
module Aurum.Interop.ProcessWrapper

open System.Diagnostics
open System.Threading

type CoreProcess
  (coreProcessInfo: ProcessStartInfo, loggerTaskBuilder: System.IO.StreamReader -> CancellationToken -> Tasks.Task<unit>)
  =
  let mutable disposed = false
  let coreProcessInfo = coreProcessInfo
  let cancelLogging = new CancellationTokenSource()

  let mutable internalProcess = new Process()

  interface System.IDisposable with
    member this.Dispose() =
      if (not disposed) then
        cancelLogging.Cancel()
        cancelLogging.Dispose()
        this.Process.Dispose()
        disposed <- false

  member this.Process
    with get () = internalProcess
    and set (value) = internalProcess <- value

  member this.Start() =
    (this.Process <- Process.Start(coreProcessInfo))

    let loggerTask = loggerTaskBuilder this.Process.StandardOutput cancelLogging.Token

    loggerTask.Start()
    ()

  member this.Stop() =
    cancelLogging.Cancel()
    this.Process.Kill(true)

let startCoreProcess (executablePath, configPath: string, assetPath) =
  let coreProcessInfo =
    ProcessStartInfo(executablePath, $"-config={configPath} -format=json")

  coreProcessInfo.UseShellExecute <- false
  coreProcessInfo.CreateNoWindow <- false

  coreProcessInfo.RedirectStandardError <- true

  coreProcessInfo.RedirectStandardOutput <- true

  assetPath
  |> Option.iter (fun a -> coreProcessInfo.Environment.["v2ray.location.asset"] <- a)

  let createCoreLoggerTask (stdout: System.IO.StreamReader) (cancellationToken: CancellationToken) =
    backgroundTask {
      while (not cancellationToken.IsCancellationRequested) do
        let! logLine = stdout.ReadLineAsync()
        LogStream.logStream.Trigger logLine
    }

  new CoreProcess(coreProcessInfo, createCoreLoggerTask)
