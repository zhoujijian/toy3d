using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using Toy3d.Core;

namespace Toy3d.Samples {
    public class SampleWindow : GameWindow {
        private int vao;
        private int vbo;
        // private int ebo;
        private Shader shader;

        private PerspectiveCamera camera;
        private bool mouseDown = false;
        private float wheelOffsetY;
        private bool firstWheel = true;

        // private Light light;
        private Light[] pointLights;
        private int textureIdDiffuse;
        private int textureIdSpecular;

        private const float YAW = -90f;
        private const float PITCH = 0f;

        public SampleWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings) { }

        protected override void OnLoad() {
            base.OnLoad();

            vao = GL.GenVertexArray();

	        var vertices = new float[] {
                // positions          // normals           // texture coords
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  0.0f,
                0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  0.0f,
                0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  1.0f,
                0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  1.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  0.0f,

                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  0.0f,
                0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  0.0f,
                0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  1.0f,
                0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  1.0f,
                -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  0.0f,

                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  0.0f,
                -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  1.0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
                -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  0.0f,
                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  0.0f,

                0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f,
                0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  1.0f,
                0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
                0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
                0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  0.0f,
                0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f,

                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  1.0f,
                0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  1.0f,
                0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  0.0f,
                0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  0.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  0.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  1.0f,

                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  1.0f,
                0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  1.0f,
                0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  0.0f,
                0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  0.0f,
                -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  0.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  1.0f          
	    };

	    /*
            var indices = new uint[] { 0, 2, 1 };
            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StaticDraw);
	    */

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(vao);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);
            GL.BindVertexArray(0);

            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            var position = new Vector3(0.0f, 2.0f, 3.0f);
            var front = new Vector3(0.0f, 0.0f, -1.0f);
            var up = new Vector3(0.0f, 1.0f, 0.0f);
            camera = new PerspectiveCamera(position, front, up, 0.3f, 0.1f, 100.0f, Size.X / Size.Y, 60.0f);
            camera.Yaw = YAW;
            camera.Pitch = PITCH;

            // light = new Light(new Vector3(0.0f, 1.0f, -1.0f));
            
            pointLights = new Light[] {
                new Light(new Vector3( 0.7f,  0.2f,  2.0f)),
                new Light(new Vector3( 2.3f, -3.3f, -4.0f)),
                new Light(new Vector3(-4.0f,  2.0f, -12.0f)),
                new Light(new Vector3( 0.0f,  0.0f, -3.0f))
            };

            textureIdDiffuse = ImageTexture.LoadFromFile("Images/container2.png").Handle;
            textureIdSpecular = ImageTexture.LoadFromFile("Images/container2_specular.png").Handle;
        }

        protected override void OnRenderFrame(FrameEventArgs args) {
            base.OnRenderFrame(args);

            var model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
            var view = camera.ViewMatrix;
            var projection = camera.ProjectionMatrix;

            // GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
	    
            GL.BindVertexArray(vao);
            GL.UseProgram(shader.ProgramId);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.ProgramId, "uModel"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.ProgramId, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.ProgramId, "uProjection"), false, ref projection);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "viewWorldPosition"), camera.Position.X, camera.Position.Y, camera.Position.Z);

            // direction light
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "directionLight.direction"), -0.2f, -0.1f, -0.3f);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "directionLight.ambient"), 0.05f, 0.05f, 0.05f);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "directionLight.diffuse"), 0.4f, 0.4f, 0.4f);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "directionLight.specular"), 0.5f, 0.5f, 0.5f);

            // point light
            var p0 = pointLights[0].LocalPosition;
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "pointLights[0].position"), p0.X, p0.Y, p0.Z);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "pointLights[0].ambient"), 0.05f, 0.05f, 0.05f);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "pointLights[0].diffuse"), 0.8f, 0.8f, 0.8f);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "pointLights[0].specular"), 1.0f, 1.0f, 1.0f);
            GL.Uniform1(GL.GetUniformLocation(shader.ProgramId, "pointLights[0].constant"), 1.0f);
            GL.Uniform1(GL.GetUniformLocation(shader.ProgramId, "pointLights[0].linear"), 0.09f);
            GL.Uniform1(GL.GetUniformLocation(shader.ProgramId, "pointLights[0].quadratic"), 0.032f);

            var p1 = pointLights[1].LocalPosition;
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "pointLights[1].position"), p1.X, p1.Y, p1.Z);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "pointLights[1].ambient"), 0.05f, 0.05f, 0.05f);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "pointLights[1].diffuse"), 0.8f, 0.8f, 0.8f);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "pointLights[1].specular"), 1.0f, 1.0f, 1.0f);
            GL.Uniform1(GL.GetUniformLocation(shader.ProgramId, "pointLights[1].constant"), 1.0f);
            GL.Uniform1(GL.GetUniformLocation(shader.ProgramId, "pointLights[1].linear"), 0.09f);
            GL.Uniform1(GL.GetUniformLocation(shader.ProgramId, "pointLights[1].quadratic"), 0.032f);

            var p2 = pointLights[1].LocalPosition;
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "pointLights[2].position"), p2.X, p2.Y, p2.Z);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "pointLights[2].ambient"), 0.05f, 0.05f, 0.05f);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "pointLights[2].diffuse"), 0.8f, 0.8f, 0.8f);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "pointLights[2].specular"), 1.0f, 1.0f, 1.0f);
            GL.Uniform1(GL.GetUniformLocation(shader.ProgramId, "pointLights[2].constant"), 1.0f);
            GL.Uniform1(GL.GetUniformLocation(shader.ProgramId, "pointLights[2].linear"), 0.09f);
            GL.Uniform1(GL.GetUniformLocation(shader.ProgramId, "pointLights[2].quadratic"), 0.032f);

            var p3 = pointLights[1].LocalPosition;
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "pointLights[3].position"), p3.X, p3.Y, p3.Z);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "pointLights[3].ambient"), 0.05f, 0.05f, 0.05f);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "pointLights[3].diffuse"), 0.8f, 0.8f, 0.8f);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "pointLights[3].specular"), 1.0f, 1.0f, 1.0f);
            GL.Uniform1(GL.GetUniformLocation(shader.ProgramId, "pointLights[3].constant"), 1.0f);
            GL.Uniform1(GL.GetUniformLocation(shader.ProgramId, "pointLights[3].linear"), 0.09f);
            GL.Uniform1(GL.GetUniformLocation(shader.ProgramId, "pointLights[3].quadratic"), 0.032f);

            // material
            GL.Uniform1(GL.GetUniformLocation(shader.ProgramId, "material.shininess"), 32.0f);
            GL.Uniform1(GL.GetUniformLocation(shader.ProgramId, "material.diffuse"), 0);
            GL.Uniform1(GL.GetUniformLocation(shader.ProgramId, "material.specular"), 1);
            
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureIdDiffuse);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, textureIdSpecular);

            // GL.DrawElements(PrimitiveType.Triangles, 3, DrawElementsType.UnsignedInt, 0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);

            // light.Draw(camera);
            foreach (var light in pointLights) {
                light.Draw(camera);
            }

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args) {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape)) {
                Close();
		        return;
            }

            var cameraSpeed = 2.5f * (float)args.Time;
            if (KeyboardState.IsKeyDown(Keys.W)) {
                camera.Position = camera.Position + cameraSpeed * camera.Front;
            }
	        else if (KeyboardState.IsKeyDown(Keys.S)) {
                camera.Position = camera.Position - cameraSpeed * camera.Front;
            }
	        else if (KeyboardState.IsKeyDown(Keys.A)) {
                camera.Position = camera.Position - cameraSpeed * (Vector3.Cross(camera.Front, camera.Up));
            }
	        else if (KeyboardState.IsKeyDown(Keys.D)) {
                camera.Position = camera.Position + cameraSpeed * (Vector3.Cross(camera.Front, camera.Up));
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            mouseDown = true;
        }

        protected override void OnMouseMove(MouseMoveEventArgs e) {
            base.OnMouseMove(e);

	        if (!mouseDown) return;

            camera.Yaw = camera.Yaw + e.DeltaX * camera.Sensitivity;
            camera.Pitch = camera.Pitch + (-e.DeltaY) * camera.Sensitivity;
            if (camera.Pitch >  89.0f) { camera.Pitch =  89.0f; }
            if (camera.Pitch < -89.0f) { camera.Pitch = -89.0f; }
	    
            // 按照pitch俯仰角计算y分量及在xz平面的投影 => 按照yaw偏航角计算xz平面的投影的x/z值
            var radYaw = MathHelper.DegreesToRadians(camera.Yaw);
            var radPitch = MathHelper.DegreesToRadians(camera.Pitch);
            var frontx = MathHelper.Cos(radPitch) * MathHelper.Cos(radYaw);
            var fronty = MathHelper.Sin(radPitch);
            var frontz = MathHelper.Cos(radPitch) * MathHelper.Sin(radYaw);
            camera.Front = Vector3.Normalize(new Vector3((float)frontx, (float)fronty, (float)frontz));
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            mouseDown = false;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            base.OnMouseWheel(e);

            if (firstWheel) {
                firstWheel = false;
                wheelOffsetY = e.OffsetY;
                return;
            }

            camera.Fov = camera.Fov + e.OffsetY - wheelOffsetY;
            if (camera.Fov < 1.0f)   { camera.Fov = 1.0f; }
            if (camera.Fov > 120.0f) { camera.Fov = 120.0f; }
            wheelOffsetY = e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e) {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }
    }
}
