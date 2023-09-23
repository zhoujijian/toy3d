using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Toy3d.Core {
    public struct Vertex {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 texcoord;
    };

    public struct MeshTexture {
        public uint id;
        public string type;
    }

    public class Mesh {
        private Vertex[] vertices;
        private uint[] indices;
        private List<MeshTexture> textures;
        private int vao, vbo, ebo;

        public Mesh(Vertex[] vertices, uint[] indices, List<MeshTexture> textures) {
            this.vertices = vertices;
            this.indices = indices;
            this.textures = textures;
        }

        private void setup() {
            var attrs = new List<float>();
            foreach (var vertex in vertices) {
                attrs.Add(vertex.position.X);
                attrs.Add(vertex.position.Y);
                attrs.Add(vertex.position.Z);
                attrs.Add(vertex.normal.X);
                attrs.Add(vertex.normal.Y);
                attrs.Add(vertex.normal.Z);
                attrs.Add(vertex.texcoord.X);
                attrs.Add(vertex.texcoord.Y);
            }
            var datas = attrs.ToArray();
            var stride = 8 * sizeof(float);
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, datas.Length * sizeof(float), datas, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
        
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, 6);
            GL.EnableVertexAttribArray(2);
            GL.BindVertexArray(0);
        }

        public void Draw(Shader shader) {
            var diffuseN = 1;
            var specularN = 1;
            var normalN = 1;
            var tangentN = 1;
        
            for (var i = 0; i < textures.Count; i++) {
                GL.ActiveTexture(TextureUnit.Texture0 + i);
        
                string name = null;
                var type = textures[i].type;
                if (type == "diffuse") name = type + diffuseN++;
                else if (type == "specular") name = type + specularN++;
                else if (type == "normal") name = type + normalN++;
                else if (type == "tangent") name = type + tangentN++;

                GL.Uniform1(GL.GetUniformLocation(shader.program, "material." + name), i);
                GL.BindTexture(TextureTarget.Texture2D, textures[i].id);
            }
            GL.ActiveTexture(TextureUnit.Texture0);

            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }
}
