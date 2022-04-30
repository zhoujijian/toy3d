using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Toy3d.Common;

namespace Toy3d.Core {
    public class Sprite {
        public Color4 color = Color4.White;
        public Texture Texture { get; private set; }

        public int VertexArrayObject { get; private set; }
        public int VertexBufferObject { get; private set; }
        public float[] Vertices { get; private set; }

        public Sprite(Texture texture) : this(texture, Color4.White) { }
	public Sprite(Texture texture, Color4 color) : this(texture, texture.ImageWidth, texture.ImageHeight, color) { }
        public Sprite(Texture texture, float width, float height, Color4 color) {
	    this.color = color;
            this.Texture = texture;
	    
            var xr = width * 0.5f;
	    var xl = -xr;
            var yt = height * 0.5f;
	    var yb = -yt;

            Vertices = new float[] {
		xl, yt, 0.0f, 1.0f,
		xr, yb, 1.0f, 0.0f,
		xl, yb, 0.0f, 0.0f,
		xl, yt, 0.0f, 1.0f,
		xr, yt, 1.0f, 1.0f,
		xr, yb, 1.0f, 0.0f
	    };

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * Vertices.Length, Vertices, BufferUsageHint.StaticDraw);

            // System.Diagnostics.Debug.Print($"[Sprite]Create vertex buffer object: {VertexBufferObject}");

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindVertexArray(0);

            // System.Diagnostics.Debug.Print($"[Sprite]Create vertex array object: {VertexArrayObject}");
        }
    }
}
