using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
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
        private Texture texture;
        private LinkedList<Particle> particles = new LinkedList<Particle>();

        public ParticleGenerator() {
            texture = Texture.LoadFromFile("Images/face.png");
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
                    node.Value = new Particle(node.Value.position, node.Value.color, lifetime);
                }
                node = next;
            }

	    if (moving) {
		var particle = new Particle(target, Color4.White, 30.0f);
		particles.AddFirst(particle);
	    }
        }

	public void Draw(OrthogonalCamera2D camera) {
            if (particles.Count <= 0) { return; }

            foreach (var particle in particles) {
                shader.UseProgram();
                shader.SetVector4("uColor", (Vector4)particle.color);
                shader.SetMatrix4("uModel", Matrix4.CreateScale(0.2f) * Matrix4.CreateTranslation(particle.position));
                shader.SetMatrix4("uProjection", camera.ProjectionMatrix);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, texture.Handle);
                GL.BindVertexArray(vao);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                GL.BindVertexArray(0);
            }
	}
    }
}
