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

    public struct Material {
        public Shader shader;
        public int vao;
        public float shininess;
        public Texture diffuse;
        public Texture specular;
    }

    public class Mesh {
        private uint[] indices;
        private List<Texture> textures;
        private Material material;
        private Model parent;

        public readonly Transform transform;

        public Mesh(Model parent, float[] vertices, uint[] indices, List<Texture> textures) {
            this.parent = parent;
            this.indices = indices;
            this.textures = textures;
            
            material.vao = GL.GenVertexArray();
            GL.BindVertexArray(material.vao);

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
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, 6);
            GL.EnableVertexAttribArray(2);
            
            GL.BindVertexArray(0);
        }

        public void Draw(Camera camera) {
            var model = Matrix4.CreateTranslation(transform.position.X, transform.position.Y, transform.position.Z);
            var view = camera.ViewMatrix;
            var projection = camera.ProjectionMatrix;
            var program = material.shader.program;

            GL.UseProgram(program);
            // matrix
            GL.UniformMatrix4(GL.GetUniformLocation(program, "uModel"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "uProjection"), false, ref projection);
            GL.Uniform3(GL.GetUniformLocation(program, "viewWorldPosition"), camera.position.X, camera.position.Y, camera.position.Z);
            // material
            GL.Uniform1(GL.GetUniformLocation(program, "material.diffuse"), 0);
            GL.Uniform1(GL.GetUniformLocation(program, "material.specular"), 1);
            GL.Uniform1(GL.GetUniformLocation(program, "material.shininess"), material.shininess);
            // texture
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, material.diffuse.id);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, material.specular.id);

            GL.BindVertexArray(material.vao);
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void DrawMany(Camera camera) {
            var diffuseId = 1;
            var specularId = 1;
            var normalId = 1;
            var tangentId = 1;
        
            for (var i = 0; i < textures.Count; i++) {
                GL.ActiveTexture(TextureUnit.Texture0 + i);
        
                var j = 0;
                switch (textures[i].type) {
                    case "diffuse":  j = diffuseId++;  break;
                    case "specular": j = specularId++; break;
                    case "normal":   j = normalId++;   break;
                    case "tangent":  j = tangentId++;  break;
                }
                var name = textures[i].type + j;
                GL.Uniform1(GL.GetUniformLocation(material.shader.program, "material." + name), i);
                GL.BindTexture(TextureTarget.Texture2D, textures[i].id);
            }
            GL.ActiveTexture(TextureUnit.Texture0);

            GL.BindVertexArray(material.vao);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }
}
