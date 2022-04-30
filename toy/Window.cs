using Toy3d.Common;
using Toy3d.Core;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

using System.Collections.Generic;

namespace Toy3d.Window {
    public class Window : GameWindow {
        private SpriteRenderer renderer;
        private OrthogonalCamera camera;
        private List<GameObject> gameObjects = new List<GameObject>();

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
	    : base(gameWindowSettings, nativeWindowSettings) { }

        protected override void OnLoad() {
            base.OnLoad();

            var shader = new Shader("Shaders/sprite.vert", "Shaders/sprite.frag");
            renderer = new SpriteRenderer(shader);
            camera = new OrthogonalCamera(800, 600, new Vector3(0f, 0f, 1f));

	    System.Diagnostics.Debug.Print("Orthogonal Camera Projection Matrix:" + System.Environment.NewLine + camera.ProjectionMatrix.ToString());

	    LoadScene();		
        }

        private void LoadScene() {
            var scene = new int[3, 6] {
		{ 1, 1, 1, 1, 1, 1 },
		{ 2, 2, 0, 0, 2, 2 },
		{ 3, 3, 4, 4, 3, 3 }
	    };

            for (var r = 0; r < 3; ++r) {
                for (var c = 0; c < 6; ++c) {
                    Color4 color;
                    string imagePath = null;

		    if (scene[r, c] < 1) { continue; }
                    else if (scene[r, c] == 1) {
                        color = Color4.White;
                        imagePath = "Images/block_solid.png";
                    }
		    else {
                        switch (scene[r, c]) {
                            case 3: color = new Color4(0.2f, 0.6f, 1.0f, 1.0f); break;
                            case 4: color = new Color4(0.0f, 0.7f, 0.0f, 1.0f); break;
                            case 5: color = new Color4(1.0f, 0.5f, 0.0f, 1.0f); break;
                            default: color = Color4.White; break;
                        }
                        imagePath = "Images/block.png";
                    }
                    var sprite = new Sprite(Texture.LoadFromFile(imagePath), color);
                    var gameObject = new GameObject(sprite);
                    gameObjects.Add(gameObject);
                }
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);
	    GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            foreach (var gameObject in gameObjects) {
                gameObject.Draw(renderer, camera);
            }
    
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
