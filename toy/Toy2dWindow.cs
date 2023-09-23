using Toy3d.Core;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

using System.Collections.Generic;

namespace Toy3d.Samples {
    public class Toy2dWindow : GameWindow {
        private Shader shader;
        private OrthogonalCamera2D camera;
        private List<GameObject> gameObjects = new List<GameObject>();

        private BallGameObject ball;

        public Toy2dWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
	    : base(gameWindowSettings, nativeWindowSettings) { }

        protected override void OnLoad() {
            base.OnLoad();

            GL.Enable(EnableCap.Blend);

            shader = Toy3dCore.CreateShader("Shaders/sprite.vert", "Shaders/sprite.frag");
            camera = new OrthogonalCamera2D(800, 600, new Vector3(0f, 0f, 1f));

            LoadScene();
            LoadBall();

            // DebugLoadScene();
        }

	    private void DebugLoadScene() {
            var sprite = new Sprite(Toy3dCore.CreateTexture("Images/block.png"), shader, Color4.White);
            var gameObject = new GameObject(sprite);
            gameObject.transform.position = new Vector3(50.0f, 50.0f, 0.0f);
            gameObjects.Add(gameObject);

            // System.Diagnostics.Debug.Print("Model matrix:" + gameObject.transform.ModelMatrix);
        }

	    private void LoadBall() {
            var sprite = new Sprite(Toy3dCore.CreateTexture("Images/face.png"), shader, Color4.White);
            ball = new BallGameObject(sprite, Vector2.One);
            ball.transform.scale = new Vector3(0.1f, 0.1f, 1);
            ball.transform.position = new Vector3(400, 300, 0);
            gameObjects.Add(ball);
        }

        private void LoadScene() {
            var scene = new int[3, 8] {
                { 1, 1, 1, 1, 1, 1, 1, 1 },
                { 2, 2, 2, 0, 0, 2, 2, 2 },
                { 3, 3, 3, 4, 4, 3, 3, 3 }
            };

            var width = camera.Width / 8;

            for (var r = 0; r < 3; ++r) {
                for (var c = 0; c < 8; ++c) {
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
                    var texture = Toy3dCore.CreateTexture(imagePath);
                    var height = texture.height * 0.25f;
                    var sprite = new Sprite(texture, shader, color);
                    var gameObject = new GameObject(sprite);
                    var x = (c + 0.5f) * width;
                    var y = (r + 0.5f) * height;
                    gameObject.transform.position = new Vector3(x, y, 0.0f);
                    gameObjects.Add(gameObject);
                }
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);

            // GL.BindFramebuffer(FramebufferTarget.Framebuffer, shakeScreen.FramebufferId);
            GL.Clear(ClearBufferMask.ColorBufferBit);
	        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            foreach (var gameObject in gameObjects) {
                gameObject.Draw(camera);
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
