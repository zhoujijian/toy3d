using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

namespace Toy3d.Game {
    public class Toy2dWindow : GameWindow {
        private IGameLifecycle game;
        private IGameWorld world;

        public Toy2dWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, IGameLifecycle game)
	    : base(gameWindowSettings, nativeWindowSettings) {
            this.game = game;
        }

        protected override void OnLoad() {
            base.OnLoad();

            world = new GameWorld2D(800, 600);
            game.OnLoad(world);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            game.OnRenderFrame();
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);            
            game.OnUpdateFrame();
            if (KeyboardState.IsKeyDown(Keys.Escape)) {
                Close();
            }
        }

        protected override void OnResize(ResizeEventArgs e) {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }
    }
}
