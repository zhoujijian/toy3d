using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using Toy3d.Core;

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

            Sprite.CreateVertexObject();
            ParticleEmitter.CreateVertexObject();

            // 注意参与运算的width/height要和窗口的Size保持一致，否则会导致变形
            world = new GameWorld2D(Size.X, Size.Y);
            game.OnLoad(world);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            game.OnRenderFrame((float)e.Time);
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
            game.OnUpdateFrame((float)e.Time);
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
