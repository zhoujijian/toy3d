using Toy3d.Core;
using Toy3d.Samples;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

using System.Collections.Generic;

namespace Toy3d.Window {
    public class Window : GameWindow {
        private SpriteRenderer renderer;
        private OrthogonalCamera2D camera;
        private List<GameObject> gameObjects = new List<GameObject>();

        private BallGameObject ball;
        private ParticleGenerator particleGenerator;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
	    : base(gameWindowSettings, nativeWindowSettings) { }

        protected override void OnLoad() {
            base.OnLoad();

            var shader = new Shader("Shaders/sprite.vert", "Shaders/sprite.frag");
            renderer = new SpriteRenderer(shader);
            camera = new OrthogonalCamera2D(800, 600, new Vector3(0f, 0f, 1f));
            particleGenerator = new ParticleGenerator();

            System.Diagnostics.Debug.Print("Orthogonal Camera Projection Matrix:" + System.Environment.NewLine + camera.ProjectionMatrix.ToString());

            // LoadScene();
            LoadBall();

            // DebugLoadScene();
        }

	private void DebugLoadScene() {
            var sprite = new Sprite(Texture.LoadFromFile("Images/block.png"), Color4.White);
            var gameObject = new GameObject(sprite);
            gameObject.Transform.position = new Vector3(50.0f, 50.0f, 0.0f);
            gameObjects.Add(gameObject);

            System.Diagnostics.Debug.Print("Model matrix:" + gameObject.Transform.ModelMatrix);
        }

	private void LoadBall() {
            var sprite = new Sprite(Texture.LoadFromFile("Images/face.png"), Color4.White);
            ball = new BallGameObject(sprite, Vector2.One);
            ball.Transform.scale = new Vector3(0.1f, 0.1f, 1);
            ball.Transform.position = new Vector3(400, 300, 0);
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
                    var texture = Texture.LoadFromFile(imagePath);
                    var height = texture.ImageHeight * 0.25f;
                    var sprite = new Sprite(texture, width, height, color);
                    var gameObject = new GameObject(sprite);
                    var x = (c + 0.5f) * width;
                    var y = (r + 0.5f) * height;
                    gameObject.Transform.position = new Vector3(x, y, 0.0f);
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

            particleGenerator.Draw(camera);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

	    if (KeyboardState.IsKeyDown(Keys.W)) {
                ball.Move(0.02f, 800, 600);
                particleGenerator.Update(ball.Transform.position, 0.02f, true);
                return;
            }

            particleGenerator.Update(ball.Transform.position, 0.02f, false);

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
