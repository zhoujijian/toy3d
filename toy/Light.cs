using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Toy3d.Core {
    public enum LightType {
        Direction,
        Point,
        Spot
    }

    public class Light: GameObject {
        private Cube cube;
        private readonly Shader shader;

        public Vector3 direction;
        public Vector3 ambient;
        public Vector3 diffuse;
        public Vector3 specular;
        public float constant;
        public float linear;
        public float quadratic;
        public float radius;

        public readonly LightType type;

        public Light(LightType type, Shader shader) {
            this.type = type;
            this.shader = shader;
            this.cube = Primitive.CreateCube();
        }

        public override void Draw(Camera camera) {
            var model = GetModelMatrix();
            var view = camera.ViewMatrix;
            var projection = camera.ProjectionMatrix;

            GL.UseProgram(shader.program);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uModel"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uProjection"), false, ref projection);
            cube.Draw(shader);
        }
    }
}
