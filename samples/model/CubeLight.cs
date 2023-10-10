using OpenTK.Graphics.OpenGL4;
using Toy3d.Game;
using Toy3d.Core;

namespace Toy3d.Samples {
    public class CubeLight: GameObject {
        private Cube cube;
        private readonly Shader shader;

        public CubeLight(Shader shader) {
            this.shader = shader;
            this.cube = Primitive.CreateCube();
        }

        public override void Draw(IGameWorld world) {
            var model = GetModelMatrix();
            var view = world.GetCamera().ViewMatrix;
            var projection = world.GetCamera().ProjectionMatrix;

            GL.UseProgram(shader.program);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uModel"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uProjection"), false, ref projection);
            cube.Draw(shader);
        }
    }
}
