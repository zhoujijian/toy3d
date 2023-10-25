using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Toy3d.Game;

namespace Toy3d.Core {
    public class Skybox {
        private int vao;
        private int cubemap;
        private Shader shader;
        private uint[] indices;

        public Skybox(Shader shader, int cubemap) {
            this.shader = shader;
            this.cubemap = cubemap;

            var vertices = new float[] {
                -1f, -1f, -1f,
                 1f, -1f, -1f,
                 1f,  1f, -1f,
                -1f,  1f, -1f,
                -1f, -1f,  1f,
                 1f, -1f,  1f,
                 1f,  1f,  1f,
                -1f,  1f,  1f
            };

            indices = new uint[] {
                2, 3, 1, 3, 0, 1, // 1, 3, 2, 1, 0, 3,
                6, 2, 5, 2, 1, 5, // 5, 2, 6, 5, 1, 2,
                7, 6, 5, 4, 7, 5, // 5, 6, 7, 5, 7, 4,
                3, 7, 4, 0, 3, 4, // 4, 7, 3, 4, 3, 0,
                7, 3, 6, 3, 2, 6, // 6, 3, 7, 6, 2, 3,
                5, 0, 4, 5, 1, 0  // 4, 0, 5, 0, 1, 5
            };

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            
            var vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            var ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            var ELEMENTS = 3;
            var STRIDE = ELEMENTS * sizeof(float);
            GL.VertexAttribPointer(0, ELEMENTS, VertexAttribPointerType.Float, false, STRIDE, 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
        }

        public void Draw(IGameWorld world) {
            GL.UseProgram(shader.program);

            var view = new Matrix4(new Matrix3(world.Camera.ViewMatrix)); // 去除位移影响
            var projection = world.Camera.ProjectionMatrix;
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "view"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "projection"), false, ref projection);

            GL.BindVertexArray(vao);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, cubemap);
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }
}