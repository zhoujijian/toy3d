using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Toy3d.Common;

namespace Toy3d.Core {
    public class Sprite {
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Transform { get; set; }
	public Texture Texture { get; private set; }

	public int VertexArrayObject { get; private set; }
	public int VertexBufferObject { get; private set; }
        public float[] Vertices { get; private set; }

        public Matrix4 ModelMatrix {
            get {
                var transform = Matrix4.CreateTranslation(Transform);
                var scale = Matrix4.CreateScale(Scale);
                var rotation = Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationZ(Rotation.Z);
                return transform * scale * rotation;
            }
        }

        public Sprite(string texturePath, float[] vertices) {
            Scale = Vector3.One;
            Rotation = Vector3.Zero;
            Transform = Vector3.Zero;
	    
            Texture = Texture.LoadFromFile(texturePath);
            Vertices = vertices;

            System.Diagnostics.Debug.Print($"[Sprite]Load texture from {texturePath}: {Texture.Handle}");

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            System.Diagnostics.Debug.Print($"[Sprite]Create vertex buffer object: {VertexBufferObject}");

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindVertexArray(0);

            System.Diagnostics.Debug.Print($"[Sprite]Create vertex array object: {VertexArrayObject}");
        }
    }
}
