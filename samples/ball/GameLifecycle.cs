using Toy3d.Game;
using Toy3d.Core;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class GameLifecycle : IGameLifecycle {
    private Shader shader;
    private BallGameObject ball;
    private GameWorld2D world;

    public void OnLoad(IGameWorld world)
    {
        GL.Enable(EnableCap.Blend);

        shader = Toy3dCore.CreateShader("Resource/Shaders/sprite.vert", "Resource/Shaders/sprite.frag");
        this.world = (GameWorld2D)world;

        LoadScene();
        LoadBall();

        // DebugLoadScene();
    }

    public void OnRenderFrame() {
        // GL.BindFramebuffer(FramebufferTarget.Framebuffer, shakeScreen.FramebufferId);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        world.Draw();
    }

    public void OnUpdateFrame() { }

    private void DebugLoadScene() {
        var sprite = new Sprite(Toy3dCore.CreateTexture("Resource/Images/block.png"), shader);
        var gameObject = new GameObject(sprite);
        gameObject.transform.position = new Vector3(50.0f, 50.0f, 0.0f);
        world.AddGameObject(gameObject);

        // System.Diagnostics.Debug.Print("Model matrix:" + gameObject.transform.ModelMatrix);
    }

    private void LoadBall() {
        var sprite = new Sprite(Toy3dCore.CreateTexture("Resource/Images/face.png"), shader);
        ball = new BallGameObject(sprite, Vector2.One);
        ball.transform.scale = new Vector3(sprite.texture.width, sprite.texture.height, 1);
        ball.transform.position = new Vector3(400, 300, 0);
        world.AddGameObject(ball);
    }

    private void LoadScene() {
        var scene = new int[3, 8] {
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 2, 2, 2, 0, 0, 2, 2, 2 },
            { 3, 3, 3, 4, 4, 3, 3, 3 }
        };

        for (var r = 0; r < 3; ++r) {
            for (var c = 0; c < 8; ++c) {
                Color4 color;
                string imagePath;

                if (scene[r, c] < 1) { continue; }
                else if (scene[r, c] == 1) {
                    color = Color4.White;
                    imagePath = "Resource/Images/block_solid.png";
                }
                else {
                    switch (scene[r, c]) {
                        case 3: color = new Color4(0.2f, 0.6f, 1.0f, 1.0f); break;
                        case 4: color = new Color4(0.0f, 0.7f, 0.0f, 1.0f); break;
                        case 5: color = new Color4(1.0f, 0.5f, 0.0f, 1.0f); break;
                        default: color = Color4.White; break;
                    }
                    imagePath = "Resource/Images/block.png";
                }

                var texture = Toy3dCore.CreateTexture(imagePath);
                var sprite = new Sprite(texture, shader);
                sprite.color = color;
                var gameObject = new GameObject(sprite);
                var x = (c + 0.5f) * world.width / 8;
                var y = (r + 0.5f) * world.height / 8;
                gameObject.transform.scale = new Vector3(sprite.texture.width, sprite.texture.height, 1);
                gameObject.transform.position = new Vector3(x, y, 0.0f);
                world.AddGameObject(gameObject);
            }
        }
    }
}