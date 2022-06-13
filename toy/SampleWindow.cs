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
        private int ebo;
        private Shader shader;

        private PerspectiveCamera camera;
        private bool mouseDown = false;
        private float wheelOffsetY;
        private bool firstWheel = true;

        private Light light;

        private const float YAW = -90f;
        private const float PITCH = 0f;

        public SampleWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings) { }

        protected override void OnLoad() {
            base.OnLoad();

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            var vertices = new float[] {
                -0.5f, -0.5f, 0.0f,
                 0.0f,  0.5f, 0.0f,
                 0.5f, -0.5f, 0.0f
            };
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            var indices = new uint[] { 0, 2, 1 };
            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);

            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            var position = new Vector3(0.0f, 0.0f, 3.0f);
            var front = new Vector3(0.0f, 0.0f, -1.0f);
            var up = new Vector3(0.0f, 1.0f, 0.0f);
            camera = new PerspectiveCamera(position, front, up, 0.3f, 0.1f, 100.0f, Size.X / Size.Y, 60.0f);
            camera.Yaw = YAW;
            camera.Pitch = PITCH;

            light = new Light(new Vector3(2.0f, 2.0f, -1.0f));
        }

        protected override void OnRenderFrame(FrameEventArgs args) {
            base.OnRenderFrame(args);

            var model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
            var view = camera.ViewMatrix;
            var projection = camera.ProjectionMatrix;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.BindVertexArray(vao);
            GL.UseProgram(shader.ProgramId);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.ProgramId, "uModel"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.ProgramId, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.ProgramId, "uProjection"), false, ref projection);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "objectColor"), 1.0f, 0.5f, 0.31f);
            GL.Uniform3(GL.GetUniformLocation(shader.ProgramId, "lightColor"), 1.0f, 1.0f, 1.0f);
            GL.DrawElements(PrimitiveType.Triangles, 3, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            light.Draw(camera);

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
