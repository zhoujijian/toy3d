using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Toy3d.Core {
    public class Sprite {
        public Color4 color = Color4.White;
        public Transform transform;

        private int vao;
        private int vbo;
        private Shader shader;
        private Texture texture;        

        public Sprite(Texture texture, Shader shader, Color4 color) {	        
            this.texture = texture;
            this.shader = shader;
            this.color = color;
	    
            var xr = texture.width * 0.5f;
	        var xl = -xr;
            var yt = texture.height * 0.5f;
	        var yb = -yt;

            var vertices = new float[] {
                xl, yt, 0.0f, 1.0f,
                xr, yb, 1.0f, 0.0f,
                xl, yb, 0.0f, 0.0f,
                xl, yt, 0.0f, 1.0f,
                xr, yt, 1.0f, 1.0f,
                xr, yb, 1.0f, 0.0f
            };

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.DynamicDraw);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindVertexArray(0);
        }

        public void Draw(Matrix4 matrix, OrthogonalCamera2D camera) {
            var model = matrix;
            var projection = camera.ProjectionMatrix;

            GL.UseProgram(shader.program);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uModel"), true, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uProjection"), true, ref projection);
            GL.Uniform4(GL.GetUniformLocation(shader.program, "uSpriteColor"), (Vector4)color);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture.id);
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
        }
    }
}
