using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Toy3d.Core {
    public class Light {
        private Cube cube;
        private Shader shader;

	public Vector3 LocalPosition { get; set; }

        public Light(Vector3 localPosition) {
            LocalPosition = localPosition;
            cube = Primitive.CreateCube();
            shader = new Shader("Shaders/shader.vert", "Shaders/light.frag");
        }

        public void Draw(PerspectiveCamera camera) {
            GL.UseProgram(shader.ProgramId);
            var model = Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(LocalPosition);
            var view = camera.ViewMatrix;
            var projection = camera.ProjectionMatrix;
            GL.UniformMatrix4(GL.GetUniformLocation(shader.ProgramId, "uModel"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.ProgramId, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.ProgramId, "uProjection"), false, ref projection);
            cube.Draw(shader);
        }
    }
}
