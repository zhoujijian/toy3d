using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Linq;

namespace Toy3d.Core {
    public struct Vertex {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 texcoord;
    };

    public class Mesh {
        private Vertex[] vertices;
        private uint[] indices;
        private List<Texture> textures;
        private int vao, vbo, ebo;

        public Mesh(Vertex[] vertices, uint[] indices, List<Texture> textures) {
            this.vertices = vertices;
            this.indices = indices;
            this.textures = textures;

            var ELEMENTS = 8;
            var datas = new float[vertices.Length * ELEMENTS];
            foreach (var vertex in vertices) {
                datas.Append(vertex.position.X);
                datas.Append(vertex.position.Y);
                datas.Append(vertex.position.Z);
                datas.Append(vertex.normal.X);
                datas.Append(vertex.normal.Y);
                datas.Append(vertex.normal.Z);
                datas.Append(vertex.texcoord.X);
                datas.Append(vertex.texcoord.Y);
            }
            
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, datas.Length * sizeof(float), datas, BufferUsageHint.StaticDraw);            
            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
        
            var stride = ELEMENTS * sizeof(float);
            // xyz
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);
            // normal
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3);
            GL.EnableVertexAttribArray(1);
            // uv
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, 6);
            GL.EnableVertexAttribArray(2);
            
            GL.BindVertexArray(0);
        }

        public void Draw(Shader shader) {
            var diffuseId = 1;
            var specularId = 1;
            var normalId = 1;
            var tangentId = 1;
        
            for (var i = 0; i < textures.Count; i++) {
                GL.ActiveTexture(TextureUnit.Texture0 + i);
        
                var id = 0;
                switch (textures[i].type) {
                    case "diffuse":  id = diffuseId++;  break;
                    case "specular": id = specularId++; break;
                    case "normal":   id = normalId++;   break;
                    case "tangent":  id = tangentId++;  break;
                }
                var name = textures[i].type + id;
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
