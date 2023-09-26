using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Toy3d.Core {
    public class Sprite {        
        public readonly Texture texture;
        public readonly Shader shader;
        public Color4 color = Color4.White;

        private int vao;
        private int vbo;        

        public Sprite(Texture texture, Shader shader) {
            this.texture = texture;
            this.shader = shader;

            // 这里是以(0,0)为原点，所在的Model空间坐标系，来设置各点坐标的
            // 每个点Vertex*Model*View*Projection转换到NDC空间，再根据转换后的坐标对纹理采样
            // vertex shader中会变换每个顶点，并且传递相应的uv坐标，转换为fragment shader后，依据NDC的顶点坐标与uv坐标的对应关系，对像素点采样
            var vertices = new float[] {
                /*x,y*/0.0f, 1.0f, /*u,v*/0.0f, 1.0f,
                /*x,y*/1.0f, 0.0f, /*u,v*/1.0f, 0.0f,
                /*x,y*/0.0f, 0.0f, /*u,v*/0.0f, 0.0f,
                /*x,y*/1.0f, 0.0f, /*u,v*/1.0f, 0.0f,
                /*x,y*/0.0f, 1.0f, /*u,v*/0.0f, 1.0f,
                /*x,y*/1.0f, 1.0f, /*u,v*/1.0f, 1.0f
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

        public void Draw(Matrix4 model, Matrix4 projection) {
            GL.UseProgram(shader.program);
            // parameter [transpose]: shader中是否使用左乘(转置矩阵), false表示右乘
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uModel"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uProjection"), false, ref projection);
            GL.Uniform4(GL.GetUniformLocation(shader.program, "uSpriteColor"), (Vector4)color);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture.id);
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
        }
    }
}
