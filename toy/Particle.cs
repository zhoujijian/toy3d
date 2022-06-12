using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace Toy3d.Core {
    public struct Particle {
        public Vector3 position;
        public Color4 color;
        public float lifetime;

	    public Particle(Vector3 position, Color4 color, float lifetime) {
            this.position = position;
            this.color = color;
            this.lifetime = lifetime;
        }
    }

    public class ParticleGenerator {
        private int vao;
        private Shader shader;
        private ImageTexture texture;
        private LinkedList<Particle> particles = new LinkedList<Particle>();
        private Random random = new Random();

        public ParticleGenerator() {
            texture = ImageTexture.LoadFromFile("Images/face.png");
            var xr = texture.ImageWidth * 0.5f;
            var xl = -xr;
            var yt = texture.ImageHeight * 0.5f;
            var yb = -yt;
            var vertices = new float[] {
                xl, yt, 0.0f, 1.0f,
                xl, yb, 0.0f, 0.0f,
                xr, yb, 1.0f, 0.0f,
                xl, yt, 0.0f, 1.0f,
                xr, yb, 1.0f, 0.0f,
                xr, yt, 1.0f, 1.0f
            };
            var vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindVertexArray(0);

            shader = new Shader("Shaders/particle.vert", "Shaders/particle.frag");
        }

        public void DebugUpdate(Vector3 target, float dt) {
            if (particles.Count <= 0) {
                var particle = new Particle(target, Color4.White, 100.0f);
                particles.AddFirst(particle);
            }
        }

	    public void Update(Vector3 target, float dt, bool moving) {
            var node = particles.First;
	        while (node != null) {
                var next = node.Next;
                var lifetime = node.Value.lifetime - dt;
                if (lifetime <= 0.0f) {
                    particles.Remove(node);
                } else {
                    var c = node.Value.color;
                    var color = new Color4(c.R, c.G, c.B, c.A - dt);
                    node.Value = new Particle(node.Value.position, color, lifetime);
                }
                node = next;
            }

            if (moving) {
                var particle = respawnParticle(target, 10.0f);
                particles.AddFirst(particle);
            }
        }

	    public void Draw(OrthogonalCamera2D camera) {
            if (particles.Count <= 0) { return; }
	    
	        // BlendFunc enabled on Loading Window
            // GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
	        shader.UseProgram();

            var node = particles.Last;
	        while (node != null) {
                var particle = node.Value;
                node = node.Previous;

                shader.SetVector4("uColor", (Vector4)particle.color);
                shader.SetMatrix4("uModel", Matrix4.CreateScale(0.1f) * Matrix4.CreateTranslation(particle.position));
                shader.SetMatrix4("uProjection", camera.ProjectionMatrix);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, texture.Handle);
                GL.BindVertexArray(vao);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                GL.BindVertexArray(0);
            }

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

	    private Particle respawnParticle(Vector3 target, float lifetime) {
            // var randOffset = ((random.Next() % 100) - 50) / 20.0f;
            var randOffset = 0.0f;
            var r = (float)random.NextDouble();
            var g = (float)random.NextDouble();
            var b = (float)random.NextDouble();
            return new Particle(target + new Vector3(randOffset, randOffset, 0), new Color4(r, g, b, 1.0f), lifetime);
        }
    }
}
