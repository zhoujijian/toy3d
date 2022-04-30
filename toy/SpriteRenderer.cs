using Toy3d.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Toy3d.Core {
    public class SpriteRenderer {
        private Shader shader;

        public SpriteRenderer(Shader shader) {
            this.shader = shader;
        }

        public void Draw(Sprite sprite, Matrix4 matrix, OrthogonalCamera camera) {
            shader.UseProgram();
            shader.SetMatrix4("uModel", matrix);
            shader.SetMatrix4("uProjection", camera.ProjectionMatrix);
            shader.SetVector4("uSpriteColor", (Vector4)sprite.color);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, sprite.Texture.Handle);
            GL.BindVertexArray(sprite.VertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
        }
    }
}
