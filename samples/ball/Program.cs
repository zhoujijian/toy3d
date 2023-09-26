using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using System.Diagnostics;
using Toy3d.Game;

public static class Program {
    private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args) {
        var e = (Exception)args.ExceptionObject;
        Debug.Print("Unhandled exception:" + e);
    }

    private static void Main() {
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);

        // https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.defaulttracelistener?view=net-6.0
        var listener = new DefaultTraceListener();
        listener.LogFileName = "message.log";
        Trace.Listeners.Clear();
        Trace.Listeners.Add(listener);

        Debug.Print($"==== {System.DateTime.Now} ====================================");

        var nativeWindowSettings = new NativeWindowSettings() {
            Size = new Vector2i(800, 600),
            Title = "Toy3d",
            Flags = ContextFlags.ForwardCompatible // This is needed to run on macos
        };

        var game = new GameLifecycle();
        using (var window = new Toy2dWindow(GameWindowSettings.Default, nativeWindowSettings, game)) {
            window.Run();
        }
    }
}