using Toy3d.Game;
using Toy3d.Core;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class GameLifecycle : IGameLifecycle {
    private Shader shaderSprite;
    private Shader shaderParticle;
    private BallSprite ball;
    private GameWorld world;

    private float intervalLogicFrame = 0f;

    private const float GAME_LOGIC_FRAME_INTERVAL = 0.03f;

    public void OnLoad(IGameWorld world) {
        GL.Enable(EnableCap.Blend);
        shaderSprite = Toy3dCore.CreateSpriteShader();
        shaderParticle = Toy3dCore.CreateParticleShader();

        this.world = (GameWorld)world;

        LoadScene();
        LoadBall();

        // DebugLoadScene();
    }

    public void OnRenderFrame(float elapsed) {
        // GL.BindFramebuffer(FramebufferTarget.Framebuffer, shakeScreen.FramebufferId);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        world.Draw2D(elapsed);
    }

    public void OnUpdateFrame(float elapsed) {
        intervalLogicFrame += elapsed;
        while (intervalLogicFrame >= GAME_LOGIC_FRAME_INTERVAL) {
            world.Update(GAME_LOGIC_FRAME_INTERVAL);
            intervalLogicFrame -= GAME_LOGIC_FRAME_INTERVAL;
        }
    }

    private void LoadBall() {
        ball = new BallSprite(Toy3dCore.CreateTexture("Resource/Images/face.png"), shaderSprite, Vector2.One);
        ball.transform.scale = new Vector3(ball.texture.width, ball.texture.height, 1);
        // ball.transform.rotation = new Vector3(0, 0, 1f);
        ball.transform.position = new Vector3(400, 300, 0);
        ball.emitter = new ParticleEmitter(Toy3dCore.CreateTexture("Resource/Images/face.png"), shaderParticle);

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
                var block = new Sprite(texture, shaderSprite);
                var w = world.width / 8;
                var h = w * 0.5f;
                // gameObject.transform.scale = new Vector3(sprite.texture.width, sprite.texture.height, 1);
                block.transform.scale = new Vector3(w, h, 1);
                block.transform.position = new Vector3(c * w, r * h, 0.0f);
                block.additive = color;

                world.AddGameObject(block);
            }
        }
    }
}