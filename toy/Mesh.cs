using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Toy3d.Game;
using System.Linq;

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
        private Material material;

        public readonly Transform transform;

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

        public void Draw(IGameWorld world) {
            var model = Matrix4.CreateTranslation(transform.position.X, transform.position.Y, transform.position.Z);
            var view = world.GetCamera().ViewMatrix;
            var projection = world.GetCamera().ProjectionMatrix;
            var program = material.shader.program;

            GL.UseProgram(program);

            SetDirectionLight(program, world.GetDirectionLight());
            var pointLights = world.GetPointLights();
            for (var i=0; i<pointLights.Count(); ++i) {
                SetPointLight(program, pointLights.ElementAt(i), i);
            }

            // matrix
            GL.UniformMatrix4(GL.GetUniformLocation(program, "uModel"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "uProjection"), false, ref projection);
            GL.Uniform3(GL.GetUniformLocation(program, "viewWorldPosition"),
                world.GetCamera().position.X, world.GetCamera().position.Y, world.GetCamera().position.Z);
            // material
            GL.Uniform1(GL.GetUniformLocation(program, "material.diffuse"), 0);
            GL.Uniform1(GL.GetUniformLocation(program, "material.specular"), 1);
            GL.Uniform1(GL.GetUniformLocation(program, "material.shininess"), material.shininess);
            // texture
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, material.diffuse.id);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, material.specular.id);

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

        private void SetDirectionLight(int program, Light light) {
            var uLocDirection = GL.GetUniformLocation(program, "directionLight.direction");
            var uLocAmbient   = GL.GetUniformLocation(program, "directionLight.ambient");
            var uLocDiffuse   = GL.GetUniformLocation(program, "directionLight.diffuse");
            var uLocSpecular  = GL.GetUniformLocation(program, "directionLight.specular");
            GL.Uniform3(uLocDirection, light.direction.X, light.direction.Y, light.diffuse.Z);
            GL.Uniform3(uLocAmbient, light.ambient.X, light.ambient.Y, light.ambient.Z);
            GL.Uniform3(uLocDiffuse, light.diffuse.X, light.diffuse.Y, light.diffuse.Z);
            GL.Uniform3(uLocSpecular, light.specular.X, light.specular.Y, light.specular.Z);
        }

        private void SetPointLight(int program, Light light, int index) {
            var PL = "pointLights[" + index + "]";
            var uLocPosition  = GL.GetUniformLocation(program, PL + ".position");
            var uLocAmbient   = GL.GetUniformLocation(program, PL + ".ambient");
            var uLocDiffuse   = GL.GetUniformLocation(program, PL + ".diffuse");
            var uLocSpecular  = GL.GetUniformLocation(program, PL + ".specular");
            var uLocConstant  = GL.GetUniformLocation(program, PL + ".constant");
            var uLocLinear    = GL.GetUniformLocation(program, PL + ".linear");
            var uLocQuadratic = GL.GetUniformLocation(program, PL + ".quadratic");
            GL.Uniform3(uLocPosition, light.position.X, light.position.Y, light.position.Z);
            GL.Uniform3(uLocAmbient, light.ambient.X, light.ambient.Y, light.ambient.Z);
            GL.Uniform3(uLocDiffuse, light.diffuse.X, light.diffuse.Y, light.diffuse.Z);
            GL.Uniform3(uLocSpecular, light.specular.X, light.specular.Y, light.specular.Z);
            GL.Uniform1(uLocConstant, light.constant);
            GL.Uniform1(uLocLinear, light.linear);
            GL.Uniform1(uLocQuadratic, light.quadratic);
        }
    }
}
