using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Toy3d.Core {
    public struct Vertex {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 texcoord;
    };

    public struct Material {
        public Shader shader;
        public float shininess;
        public Texture diffuse;
        public Texture specular;
        public List<Texture> textures;
    }

    public class Mesh {
        private int vao;
        private uint[] indices;

        public readonly Transform transform;
        public Material material;

        public Mesh(float[] vertices, uint[] indices, Material material) {
            this.indices = indices;
            this.material = material;

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);            
            
            var vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);            
            var ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);            
            var ELEMENTS = 8;
            var stride = ELEMENTS * sizeof(float);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);
            
            GL.BindVertexArray(0);
        }

        public void Draw() {
            GL.BindVertexArray(vao);
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void DrawMany(Camera camera) {
            var diffuseId = 1;
            var specularId = 1;
            var normalId = 1;
            var tangentId = 1;
        
            for (var i = 0; i < material.textures.Count; i++) {
                GL.ActiveTexture(TextureUnit.Texture0 + i);
        
                var j = 0;
                switch (material.textures[i].type) {
                    case "diffuse":  j = diffuseId++;  break;
                    case "specular": j = specularId++; break;
                    case "normal":   j = normalId++;   break;
                    case "tangent":  j = tangentId++;  break;
                }
                var name = material.textures[i].type + j;
                GL.Uniform1(GL.GetUniformLocation(material.shader.program, "material." + name), i);
                GL.BindTexture(TextureTarget.Texture2D, material.textures[i].id);
            }
            GL.ActiveTexture(TextureUnit.Texture0);

            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }
}
