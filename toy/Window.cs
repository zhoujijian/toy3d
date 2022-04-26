using Toy3d.Common;
using Toy3d.Core;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace Toy3d.Window {
    public class Window : GameWindow {
        private Sprite sprite;
        private SpriteRenderer renderer;
        private OrthogonalCamera camera;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
	    : base(gameWindowSettings, nativeWindowSettings) { }

        protected override void OnLoad() {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            var vertices = new float[] {
		0.0f, 1.0f, 0.0f, 1.0f,
		1.0f, 0.0f, 1.0f, 0.0f,
		0.0f, 0.0f, 0.0f, 0.0f,
		0.0f, 1.0f, 0.0f, 1.0f,
		1.0f, 1.0f, 1.0f, 1.0f,
		1.0f, 0.0f, 1.0f, 0.0f
            };

	    /*
            var vertices = new float[] {
		0.0f, 0.5f, 0.0f, 1.0f,
		0.5f, 0.0f, 1.0f, 1.0f,
		0.0f, 0.0f, 0.0f, 0.0f,
		0.0f, 0.5f, 0.0f, 1.0f,
		0.5f, 0.5f, 1.0f, 1.0f,
		0.5f, 0.0f, 1.0f, 0.0f
	    };
	    */

            var shader = new Shader("Shaders/sprite.vert", "Shaders/sprite.frag");
            renderer = new SpriteRenderer(shader);
            sprite = new Sprite("Images/face.png", vertices);
            camera = new OrthogonalCamera(800, 600, new Vector3(0f, 0f, 1f));
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            renderer.Draw(sprite, camera);
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
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
