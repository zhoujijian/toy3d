using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using Toy3d.Samples;

public static class Program {
    private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args) {
        Console.WriteLine("Unhandled exception:" + (Exception)args.ExceptionObject);
    }

    private static void Main() {
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);

        var nativeWindowSettings = new NativeWindowSettings() {
            Size = new Vector2i(800, 600),
            Title = "Toy3d",
            Flags = ContextFlags.ForwardCompatible // This is needed to run on macos
        };

        using (var window = new SampleWindow(GameWindowSettings.Default, nativeWindowSettings)) {
            window.Run();
        }
    }
}