using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Toy3d.Core {
    public class Light {
        private Cube cube;
        private ShaderInfo shader;

	    public Vector3 LocalPosition { get; set; }

        public Light(Vector3 localPosition) {
            LocalPosition = localPosition;
            cube = Primitive.CreateCube();
            shader = Shader.Create("Shaders/light.vert", "Shaders/light.frag");
        }

        public void Draw(PerspectiveCamera camera) {
            GL.UseProgram(shader.program);
            var model = Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(LocalPosition);
            var view = camera.ViewMatrix;
            var projection = camera.ProjectionMatrix;
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uModel"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uProjection"), false, ref projection);
            cube.Draw(shader);
        }
    }
}
