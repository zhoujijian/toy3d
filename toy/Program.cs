using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using System;
using System.Diagnostics;

using Toy3d.Samples;

namespace Toy3d.Window
{
    public static class Program
    {
	    private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args) {
            var e = (Exception)args.ExceptionObject;
            System.Diagnostics.Debug.Print("Unhandled exception:" + e);
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

            using (var sample = new SampleWindow(GameWindowSettings.Default, nativeWindowSettings)) {
                sample.Run();
            }

            // using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings)) {
            //     window.Run();
            // }
        }
    }
}
