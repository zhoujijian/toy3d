using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using Toy3d.Core;
using Toy3d.Game;

namespace Toy3d.Samples {
    public class SampleWindow : GameWindow {
        private Camera camera;
        private IGameWorld world;

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
            AddSkybox();
        }

        private void AddSkybox() {
            var vertex = File.ReadAllText("Resource/Shaders/skybox.vert");
            var fragment = File.ReadAllText("Resource/Shaders/skybox.frag");
            var shader = Toy3dCore.CreateShader(vertex, fragment);
            var cubemap = Toy3dCore.CreateCubemap(new string[] {
                "Resource/Images/skybox/right.jpg",
                "Resource/Images/skybox/left.jpg",
                "Resource/Images/skybox/top.jpg",
                "Resource/Images/skybox/bottom.jpg",
                "Resource/Images/skybox/front.jpg",
                "Resource/Images/skybox/back.jpg"
            });
            world.Skybox = new Skybox(shader, cubemap);
        }

        private void AddCube() {
	        var vertices = new float[] {
                // xyz               // normals           // uv
                 0.1f,  0.1f, -0.1f,  0.0f,  0.0f, -1.0f,  1.0f,  1.0f,
                 0.1f, -0.1f, -0.1f,  0.0f,  0.0f, -1.0f,  1.0f,  0.0f,
                -0.1f, -0.1f, -0.1f,  0.0f,  0.0f, -1.0f,  0.0f,  0.0f,                
                -0.1f, -0.1f, -0.1f,  0.0f,  0.0f, -1.0f,  0.0f,  0.0f,
                -0.1f,  0.1f, -0.1f,  0.0f,  0.0f, -1.0f,  0.0f,  1.0f,
                 0.1f,  0.1f, -0.1f,  0.0f,  0.0f, -1.0f,  1.0f,  1.0f,

                -0.1f, -0.1f,  0.1f,  0.0f,  0.0f,  1.0f,  0.0f,  0.0f,
                 0.1f, -0.1f,  0.1f,  0.0f,  0.0f,  1.0f,  1.0f,  0.0f,
                 0.1f,  0.1f,  0.1f,  0.0f,  0.0f,  1.0f,  1.0f,  1.0f,
                 0.1f,  0.1f,  0.1f,  0.0f,  0.0f,  1.0f,  1.0f,  1.0f,
                -0.1f,  0.1f,  0.1f,  0.0f,  0.0f,  1.0f,  0.0f,  1.0f,
                -0.1f, -0.1f,  0.1f,  0.0f,  0.0f,  1.0f,  0.0f,  0.0f,

                -0.1f,  0.1f,  0.1f, -1.0f,  0.0f,  0.0f,  1.0f,  0.0f,
                -0.1f,  0.1f, -0.1f, -1.0f,  0.0f,  0.0f,  1.0f,  1.0f,
                -0.1f, -0.1f, -0.1f, -1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
                -0.1f, -0.1f, -0.1f, -1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
                -0.1f, -0.1f,  0.1f, -1.0f,  0.0f,  0.0f,  0.0f,  0.0f,
                -0.1f,  0.1f,  0.1f, -1.0f,  0.0f,  0.0f,  1.0f,  0.0f,

                 0.1f,  0.1f,  0.1f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f,
                 0.1f,  0.1f, -0.1f,  1.0f,  0.0f,  0.0f,  1.0f,  1.0f,
                 0.1f, -0.1f, -0.1f,  1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
                 0.1f, -0.1f, -0.1f,  1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
                 0.1f, -0.1f,  0.1f,  1.0f,  0.0f,  0.0f,  0.0f,  0.0f,
                 0.1f,  0.1f,  0.1f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f,

                -0.1f, -0.1f, -0.1f,  0.0f, -1.0f,  0.0f,  0.0f,  1.0f,
                 0.1f, -0.1f, -0.1f,  0.0f, -1.0f,  0.0f,  1.0f,  1.0f,
                 0.1f, -0.1f,  0.1f,  0.0f, -1.0f,  0.0f,  1.0f,  0.0f,
                 0.1f, -0.1f,  0.1f,  0.0f, -1.0f,  0.0f,  1.0f,  0.0f,
                -0.1f, -0.1f,  0.1f,  0.0f, -1.0f,  0.0f,  0.0f,  0.0f,
                -0.1f, -0.1f, -0.1f,  0.0f, -1.0f,  0.0f,  0.0f,  1.0f,

                -0.1f,  0.1f, -0.1f,  0.0f,  1.0f,  0.0f,  0.0f,  1.0f,
                 0.1f,  0.1f, -0.1f,  0.0f,  1.0f,  0.0f,  1.0f,  1.0f,
                 0.1f,  0.1f,  0.1f,  0.0f,  1.0f,  0.0f,  1.0f,  0.0f,
                 0.1f,  0.1f,  0.1f,  0.0f,  1.0f,  0.0f,  1.0f,  0.0f,
                -0.1f,  0.1f,  0.1f,  0.0f,  1.0f,  0.0f,  0.0f,  0.0f,
                -0.1f,  0.1f, -0.1f,  0.0f,  1.0f,  0.0f,  0.0f,  1.0f          
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

            var vertex = File.ReadAllText("Resource/Shaders/shader.vert");
            var fragment = File.ReadAllText("Resource/Shaders/shader.frag");
            var diffuse = Toy3dCore.CreateTexture("Resource/Images/container2.png");
            var specular = Toy3dCore.CreateTexture("Resource/Images/container2_specular.png");            
            Material material = new Material();
            material.shader = Toy3dCore.CreateShader(vertex, fragment);;
            material.diffuse = diffuse;
            material.specular = specular;
            material.shininess = 32f;
            material.textures = new List<Texture> { diffuse, specular };            
            
            var box = new Model(new Mesh[] { new Mesh(vertices, indices, material) });
            world = new GameWorld(Size.X, Size.Y);
            world.AddGameObject(box);
        }

        private void AddCamera() {
            var front = new Vector3(0.0f, 0f, -1.0f);
            var up = new Vector3(0.0f, 1.0f, 0.0f);
            camera = new Camera(front, up, 0.3f, 0.1f, 100.0f, Size.X / Size.Y, 60.0f);
            camera.position = new Vector3(0.0f, 0.5f, 0.5f);
            camera.Yaw = -90f;
            camera.Pitch = -45f;
            camera.ResetFront();

            world.Camera = camera;
        }

        private void AddLights() {
            var directionLight = new Light(LightType.Direction);
            directionLight.direction = new Vector3(-0.2f, -0.1f, -0.3f);
            directionLight.ambient = new Vector3(0.05f, 0.05f, 0.05f);
            directionLight.diffuse = new Vector3(0.4f, 0.4f, 0.4f);
            directionLight.specular = new Vector3(0.5f, 0.5f, 0.5f);
            directionLight.position = new Vector3(0.7f, 0.2f, 2.0f); // For ShadowMap
            world.DirectionLight = directionLight;

            var pointLights = new Light[] {
                new Light(LightType.Point),
                new Light(LightType.Point),
                new Light(LightType.Point),
                new Light(LightType.Point)
            };
            pointLights[0].position = new Vector3( 0.7f, 0.2f, 2.0f );
            pointLights[1].position = new Vector3( 2.3f, -3.3f, -4.0f );
            pointLights[2].position = new Vector3( -4.0f, 2.0f, -12.0f );
            pointLights[3].position = new Vector3( 0.0f, 0.0f, -3.0f );
            for (var i=0; i<pointLights.Length; ++i) {
                pointLights[i].ambient = new Vector3(0.05f, 0.05f, 0.05f);
                pointLights[i].diffuse = new Vector3(0.8f, 0.8f, 0.8f);
                pointLights[i].specular = new Vector3(1.0f, 1.0f, 1.0f);
                pointLights[i].constant = 1.0f;
                pointLights[i].linear = 0.09f;
                pointLights[i].quadratic = 0.032f;
                world.AddPointLight(pointLights[i]);
            }

            var vertex = File.ReadAllText("Resource/Shaders/light.vert");
            var fragment = File.ReadAllText("Resource/Shaders/light.frag");
            var shader = Toy3dCore.CreateShader(vertex, fragment);
            foreach (var point in pointLights) {
                var cube = new CubeLight(shader);
                cube.transform.position = point.position;
                world.AddGameObject(cube);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args) {
            base.OnRenderFrame(args);

            world.DrawWindow((float)args.Time);

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
