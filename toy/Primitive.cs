using OpenTK.Graphics.OpenGL4;

namespace Toy3d.Core {
    public class Primitive {
        public static Cube CreateCube() { return new Cube(); }
        }

        public class Cube {
            private int vao;
            private int vbo;
            private int ebo;

            private float[] vertices = new float[] {
                -0.5f, -0.5f, -0.5f,
                0.5f, -0.5f, -0.5f,
                0.5f,  0.5f, -0.5f,
                -0.5f,  0.5f, -0.5f,
                -0.5f, -0.5f,  0.5f,
                0.5f, -0.5f,  0.5f,
                0.5f,  0.5f,  0.5f,
                -0.5f,  0.5f,  0.5f
            };

            private uint[] indices = new uint[] {
                1, 3, 2, 1, 0, 3,
                5, 2, 6, 5, 1, 2,
                5, 6, 7, 5, 7, 4,
                4, 7, 3, 4, 3, 0,
                6, 3, 7, 6, 2, 3,
                5, 0, 4, 5, 1, 0
            };

            public Cube() {
                vao = GL.GenVertexArray();
                GL.BindVertexArray(vao);
                vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
                ebo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);
                GL.BindVertexArray(0);
            }

            public void Draw(ShaderInfo shader) {
                GL.BindVertexArray(vao);
                GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
            }
    }
}
