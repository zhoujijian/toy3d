using Toy3d.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Toy3d.Core {
    public class SpriteRenderer {
        private Shader shader;

        public SpriteRenderer(Shader shader) {
            this.shader = shader;
        }

        public void Draw(Sprite sprite, OrthogonalCamera camera) {
            shader.UseProgram();
            shader.SetMatrix4("uModel", sprite.ModelMatrix);
            shader.SetMatrix4("uProjection", camera.ProjectionMatrix);
            // shader.SetMatrix4("uModel", Matrix4.Identity);
            // shader.SetMatrix4("uProjection", Matrix4.Identity);
            shader.SetVector4("uSpriteColor", new Vector4(1.0f, 1.0f, 1.0f, 1.0f));

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, sprite.Texture.Handle);
            GL.BindVertexArray(sprite.VertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
        }

        public void DebugDraw(Sprite sprite, OrthogonalCamera camera) {
        }
    }
}
