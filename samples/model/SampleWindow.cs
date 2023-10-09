using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using Toy3d.Core;

namespace Toy3d.Samples {
    public class SampleWindow : GameWindow {
        private Material material;
        private Camera camera;
        private Light[] pointLights;
        private Light directionLight;
        private uint[] indices;
        private Model box;

        private bool mouseDown = false;
        private float wheelY;
        private bool wheelFirst = true;

        public SampleWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings) { }

        protected override void OnLoad() {
            base.OnLoad();

            AddCube();
            AddCamera();
            AddLights();
        }

        private void AddCube() {
	        var vertices = new float[] {
                // xyz               // normals           // uv
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  0.0f,
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  0.0f,                
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  0.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  1.0f,

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

            // 只使用8个顶点的数据，然后使用索引绘制是不足够的
            //   顶点绑定的UV坐标，在绘制更多面时，因为UV坐标不能合理对应，不能正确采样
            // => 这里使用冗余的全顶点数据

            var indices = new uint[] {
                0, 1, 2, 3, 4, 5,
                6, 7, 8, 9, 10, 11,
                12, 13, 14, 15, 16, 17,
                20, 19, 18, 23, 22, 21,
                24, 25, 26, 27, 28, 29,
                32, 31, 30, 35, 34, 33
            };

            // var mesh = new Mesh(vertices, indices, null);

            Material material;

            material.vao = GL.GenVertexArray();

            var vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(material.vao);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            var ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);

            var vertex = File.ReadAllText("Resource/Shaders/shader.vert");
            var fragment = File.ReadAllText("Resource/Shaders/shader.frag");
            material.shader = Toy3dCore.CreateShader(vertex, fragment);
            material.diffuse = Toy3dCore.CreateTexture("Resource/Images/container2.png");
            material.specular = Toy3dCore.CreateTexture("Resource/Images/container2_specular.png");

            material.shininess = 32f;
        }

        private void AddCamera() {
            var front = new Vector3(0.0f, 0f, -1.0f);
            var up = new Vector3(0.0f, 1.0f, 0.0f);
            camera = new Camera(front, up, 0.3f, 0.1f, 100.0f, Size.X / Size.Y, 60.0f);
            camera.position = new Vector3(0.0f, 2.0f, 3.0f);

            camera.Yaw = -90f;
            camera.Pitch = -45f;
            camera.ResetFront();
        }

        private void AddLights() {
            var vertex = File.ReadAllText("Resource/Shaders/light.vert");
            var fragment = File.ReadAllText("Resource/Shaders/light.frag");
            var shader = Toy3dCore.CreateShader(vertex, fragment);
            pointLights = new Light[] {
                new Light(LightType.Point, shader),
                new Light(LightType.Point, shader),
                new Light(LightType.Point, shader),
                new Light(LightType.Point, shader)
            };
            pointLights[0].transform.position = new Vector3( 0.7f, 0.2f, 2.0f );
            pointLights[1].transform.position = new Vector3( 2.3f, -3.3f, -4.0f );
            pointLights[2].transform.position = new Vector3( -4.0f, 2.0f, -12.0f );
            pointLights[3].transform.position = new Vector3( 0.0f, 0.0f, -3.0f );
            foreach (var point in pointLights) {
                point.ambient = new Vector3(0.05f, 0.05f, 0.05f);
                point.diffuse = new Vector3(0.8f, 0.8f, 0.8f);
                point.specular = new Vector3(1.0f, 1.0f, 1.0f);
                point.constant = 1.0f;
                point.linear = 0.09f;
                point.quadratic = 0.032f;
            }

            directionLight = new Light(LightType.Direction, shader);
            directionLight.direction = new Vector3(-0.2f, -0.1f, -0.3f);
            directionLight.ambient = new Vector3(0.05f, 0.05f, 0.05f);
            directionLight.diffuse = new Vector3(0.4f, 0.4f, 0.4f);
            directionLight.specular = new Vector3(0.5f, 0.5f, 0.5f);
        }

        private void SetDirectionLight(Light light) {
            var uLocDirection = GL.GetUniformLocation(material.shader.program, "directionLight.direction");
            var uLocAmbient   = GL.GetUniformLocation(material.shader.program, "directionLight.ambient");
            var uLocDiffuse   = GL.GetUniformLocation(material.shader.program, "directionLight.diffuse");
            var uLocSpecular  = GL.GetUniformLocation(material.shader.program, "directionLight.specular");
            GL.Uniform3(uLocDirection, light.direction.X, light.direction.Y, light.diffuse.Z);
            GL.Uniform3(uLocAmbient, light.ambient.X, light.ambient.Y, light.ambient.Z);
            GL.Uniform3(uLocDiffuse, light.diffuse.X, light.diffuse.Y, light.diffuse.Z);
            GL.Uniform3(uLocSpecular, light.specular.X, light.specular.Y, light.specular.Z);
        }

        private void SetPointLight(int index) {
            var light = pointLights[index];
            var PL = "pointLights[" + index + "]";
            var uLocPosition  = GL.GetUniformLocation(material.shader.program, PL + ".position");
            var uLocAmbient   = GL.GetUniformLocation(material.shader.program, PL + ".ambient");
            var uLocDiffuse   = GL.GetUniformLocation(material.shader.program, PL + ".diffuse");
            var uLocSpecular  = GL.GetUniformLocation(material.shader.program, PL + ".specular");
            var uLocConstant  = GL.GetUniformLocation(material.shader.program, PL + ".constant");
            var uLocLinear    = GL.GetUniformLocation(material.shader.program, PL + ".linear");
            var uLocQuadratic = GL.GetUniformLocation(material.shader.program, PL + ".quadratic");
            GL.Uniform3(uLocPosition, light.transform.position.X, light.transform.position.Y, light.transform.position.Z);
            GL.Uniform3(uLocAmbient, light.ambient.X, light.ambient.Y, light.ambient.Z);
            GL.Uniform3(uLocDiffuse, light.diffuse.X, light.diffuse.Y, light.diffuse.Z);
            GL.Uniform3(uLocSpecular, light.specular.X, light.specular.Y, light.specular.Z);
            GL.Uniform1(uLocConstant, light.constant);
            GL.Uniform1(uLocLinear, light.linear);
            GL.Uniform1(uLocQuadratic, light.quadratic);
        }

        protected override void OnRenderFrame(FrameEventArgs args) {
            base.OnRenderFrame(args);

            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            DrawDirect();

            SwapBuffers();
        }

        private void DrawDirect() {
            GL.BindVertexArray(material.vao);

            var program = material.shader.program;
            var model = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
            var view = camera.ViewMatrix;
            var projection = camera.ProjectionMatrix;

            GL.UseProgram(program);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "uModel"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "uProjection"), false, ref projection);
            GL.Uniform3(GL.GetUniformLocation(program, "viewWorldPosition"), camera.position.X, camera.position.Y, camera.position.Z);

            SetDirectionLight(directionLight);
            for (var i=0; i<4; ++i) {
                SetPointLight(i);
            }

            // material
            GL.Uniform1(GL.GetUniformLocation(program, "material.shininess"), material.shininess);
            GL.Uniform1(GL.GetUniformLocation(program, "material.diffuse"), 0);
            GL.Uniform1(GL.GetUniformLocation(program, "material.specular"), 1);
            
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, material.diffuse.id);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, material.specular.id);

            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            foreach (var light in pointLights) {
                light.Draw(camera);
            }            
        }

        protected override void OnUpdateFrame(FrameEventArgs args) {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape)) {
                Close();
		        return;
            }

            var cameraSpeed = 2.5f * (float)args.Time;
            if (KeyboardState.IsKeyDown(Keys.W)) {
                camera.position += cameraSpeed * camera.Front;
            }
	        else if (KeyboardState.IsKeyDown(Keys.S)) {
                camera.position -= cameraSpeed * camera.Front;
            }
	        else if (KeyboardState.IsKeyDown(Keys.A)) {
                camera.position -= cameraSpeed * Vector3.Cross(camera.Front, camera.Up);
            }
	        else if (KeyboardState.IsKeyDown(Keys.D)) {
                camera.position += cameraSpeed * Vector3.Cross(camera.Front, camera.Up);
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

            camera.ResetFront();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            mouseDown = false;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            base.OnMouseWheel(e);

            if (wheelFirst) {
                wheelFirst = false;
                wheelY = e.OffsetY;
                return;
            }

            camera.Fov = camera.Fov + e.OffsetY - wheelY;
            if (camera.Fov < 1.0f)   { camera.Fov = 1.0f; }
            if (camera.Fov > 120.0f) { camera.Fov = 120.0f; }
            wheelY = e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e) {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }
    }
}
