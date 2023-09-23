using OpenTK.Graphics.OpenGL4;

namespace Toy3d.Core {
    public class PostEffect {
        private int vao;
        private int textureId;
        private int framebufferId;
        private ShaderInfo shader;

        public bool Chaos { get; set; }
        public bool Confuse { get; set; }
        public bool Shake { get; set; }
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
                0.5f,  0.5f, -0.5f, -0.5f, 0.5f, -0.5f
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
            shader = Shader.Create("Shaders/PostEffect/posteffect.vert", "Shaders/PostEffect/posteffect.frag");

	        // You must specify <program> by <GL.ProgramUniform1/2> to set uniform variables here,
	        // otherwise the uniform variables donot work. Or you can specify it in function <Draw> by call <GL.UseProgram(program)>

            int[] edgeKernel = {
                -1, -1, -1,
                -1,  8, -1,
                -1, -1, -1
            };
            // glUniform1iv
            GL.ProgramUniform1(shader.program, GL.GetUniformLocation(shader.program, "edge_kernel"), edgeKernel.Length, edgeKernel);

            float[] blurKernel = {
                1.0f/16, 2.0f/16, 1.0f/16,
                2.0f/16, 4.0f/16, 2.0f/16,
                1.0f/16, 2.0f/16, 1.0f/16
            };
            // glUniform1fv
            GL.ProgramUniform1(shader.program, GL.GetUniformLocation(shader.program, "blur_kernel"), blurKernel.Length, blurKernel);

            float offset = 1.0f / 300.0f;
            float[] offsets = {
                -offset, offset,
                0,       offset,
                offset,  offset,
                -offset, 0,
                0,       0,
                offset,  0,
                -offset, -offset,
                0,       -offset,
                offset,  -offset
            };
            // glUniform2fv
            GL.ProgramUniform2(shader.program, GL.GetUniformLocation(shader.program, "offsets"), offsets.Length / 2, offsets);
        }

	    public void Draw(float time) {
            GL.UseProgram(shader.program);

            GL.Uniform1(GL.GetUniformLocation(shader.program, "chaos"), Chaos ? 1 : 0);
            GL.Uniform1(GL.GetUniformLocation(shader.program, "confuse"), Confuse ? 1 : 0);
            GL.Uniform1(GL.GetUniformLocation(shader.program, "shake"), Shake ? 1 : 0);
            GL.Uniform1(GL.GetUniformLocation(shader.program, "time"), time);

            // System.Diagnostics.Debug.Print($"[PostEffect](Draw)time: {time}, sintime: {System.Math.Sin(time)}, costime: {System.Math.Cos(time)}");

            GL.BindVertexArray(vao);
            GL.Disable(EnableCap.DepthTest);
	        GL.ActiveTexture(0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }
    }
}
