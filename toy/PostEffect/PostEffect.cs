using OpenTK.Graphics.OpenGL4;

namespace Toy3d.Core {
    public class PostEffect {
        private int vao;
        private int textureId;
        private int framebufferId;
        private Shader shader;

	public int FramebufferId { get { return framebufferId; } }

        public PostEffect() {
            // framebuffer
            textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 800, 600, 0, PixelFormat.Rgba, PixelType.Byte, System.IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            framebufferId = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferId);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, textureId, 0);
	    if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete) {
                throw new System.Exception("[ShakeScreen](Initialize)FramebufferTarget not complete!");
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

	    // vbo, vao
            var vertices = new float[] {
		-0.5f, 0.5f, -0.5f, -0.5f, 0.5f, 0.5f,
		0.5f, 0.5f, -0.5f, -0.5f, 0.5f, -0.5f
	    };
            var texcoords = new float[] {
		0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f,
		1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f
	    };
            var vbo1 = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo1);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);
            var vbo2 = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo2);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * texcoords.Length, texcoords, BufferUsageHint.StaticDraw);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo1);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo2);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.BindVertexArray(0);

	    // shader
            shader = new Shader("Shaders/PostEffect/posteffect.vert", "Shaders/PostEffect/posteffect.frag");
        }

	public void Draw() {
            shader.UseProgram();
            GL.BindVertexArray(vao);
            GL.Disable(EnableCap.DepthTest);
	    GL.ActiveTexture(0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }
    }
}
