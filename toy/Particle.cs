using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Toy3d.Core {
    public struct Particle {
        public Color4 color;        
        public Vector3 position;
        public Vector3 velocity;
        public float lifetime;
    }

    public class ParticleEmitter {
        private static int vao;
        private static int vbo;

        private Shader shader;
        private Texture texture;
        private Random random = new Random();
        private LinkedList<Particle> particles = new LinkedList<Particle>();        

        public ParticleEmitter(Texture texture, Shader shader) {
            this.texture = texture;
            this.shader = shader;
        }

        public static void CreateVertexObject() {
            var vertices = new float[] {
                -0.5f,  0.5f, /*x,y*/ 0.0f, 1.0f, /*u,v*/
                 0.5f, -0.5f, /*x,y*/ 1.0f, 0.0f, /*u,v*/
                -0.5f, -0.5f, /*x,y*/ 0.0f, 0.0f, /*u,v*/
                 0.5f, -0.5f, /*x,y*/ 1.0f, 0.0f, /*u,v*/
                -0.5f,  0.5f, /*x,y*/ 0.0f, 1.0f, /*u,v*/
                 0.5f,  0.5f, /*x,y*/ 1.0f, 1.0f  /*u,v*/
            };
            
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindVertexArray(0);
        }

	    public void Update(Vector3 target, float det, bool moving) {
            var node = particles.First;

	        while (node != null) {
                var c = node.ValueRef.color;
                node.ValueRef.lifetime -= det;
                node.ValueRef.position -= node.ValueRef.velocity * det;
                node.ValueRef.color = new Color4(c.R, c.G, c.B, c.A - det);
                node = node.Next;
            }

            particles.Where(x => x.lifetime <= 0).ToList().ForEach(x => particles.Remove(x));

            if (moving) {
                particles.AddFirst(RespawnParticle(target, 10.0f));
            }
        }

        public void Draw(Matrix4 model, Matrix4 projection) {
            if (particles.Count <= 0) { return; }
	    
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            // GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.UseProgram(shader.program);

            var node = particles.Last;
	        while (node != null) {

                // 1)game object是先进行祖先结点的model运算，再进行当前节点的translate运算的，右乘形式为:
                //   (Local)Translation * (Ancestor)Model * Rotation * Scale * Vertex
                // 2)OpenTK创建的矩阵为OpenGL矩阵的转置形式，故运算顺序为R * S * (Ancestor)Model * (Local)T
                // 3)缩放、旋转运算在祖先结点及当前结算的矩阵运算之前!

                var local = Matrix4.CreateScale(0.05f) * model * Matrix4.CreateTranslation(node.ValueRef.position);
                GL.Uniform4(GL.GetUniformLocation(shader.program, "uColor"), (Vector4)node.ValueRef.color);
                GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uModel"), false, ref local);
                GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "uProjection"), false, ref projection);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, texture.id);
                GL.BindVertexArray(vao);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                GL.BindVertexArray(0);

                node = node.Previous;
            }

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

	    private Particle RespawnParticle(Vector3 target, float lifetime) {
            var rand1 = (random.Next() % 50) - 25;
            var rand2 = (random.Next() % 50) - 25;
            var r = (float)random.NextDouble();
            var g = (float)random.NextDouble();
            var b = (float)random.NextDouble();
            return new Particle {
                position = target + new Vector3(rand1, rand2, 0),
                color = new Color4(r, g, b, 1.0f),
                lifetime = lifetime,
                velocity = new Vector3(0f, 100.0f, 0f)
            };
        }
    }
}
