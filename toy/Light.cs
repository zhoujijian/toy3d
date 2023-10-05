using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Toy3d.Core {
    public class Light {
        private Cube cube;
        private Shader shader;
        private Vector3 position;

	    public Vector3 Position => position;

        public Light(Shader shader, Vector3 position) {
            this.shader = shader;
            this.position = position;
            cube = Primitive.CreateCube();
        }

        public void Draw(PerspectiveCamera camera) {
            GL.UseProgram(shader.program);
            var model = Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(position);
            var view = camera.ViewMatrix;
            var projection = camera.ProjectionMatrix;
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uModel"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uProjection"), false, ref projection);
            cube.Draw(shader);
        }
    }
}
