using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Toy3d.Core {
    public class SpriteRenderer {
        private ShaderInfo shader;

        public SpriteRenderer(ShaderInfo shader) {
            this.shader = shader;
        }

        public void Draw(Sprite sprite, Matrix4 matrix, OrthogonalCamera2D camera) {
            var model = matrix;
            var projection = camera.ProjectionMatrix;

            GL.UseProgram(shader.program);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uModel"), true, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uProjection"), true, ref projection);
            GL.Uniform4(GL.GetUniformLocation(shader.program, "uSpriteColor"), (Vector4)sprite.color);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, sprite.Texture.Handle);
            GL.BindVertexArray(sprite.VertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
        }
    }
}
