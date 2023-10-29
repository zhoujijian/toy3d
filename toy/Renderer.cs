using OpenTK.Graphics.OpenGL4;
using Toy3d.Game;

namespace Toy3d.Core {
    internal struct RenderTarget {
        public int width;
        public int height;
        public readonly int id;        
        public readonly int vao;
        public readonly uint[] indices;
        public readonly Shader shader;
        public readonly Texture texture;

        public RenderTarget(int id, int vao, uint[] indices, Shader shader, Texture texture) {
            this.id = id;
            this.vao = vao;
            this.indices = indices;
            this.shader = shader;
            this.texture = texture;
        }
    }

    public class Renderer {
        public readonly int width;
        public readonly int height;        
        private RenderTarget renderTarget;

        public Renderer(int width, int height) {
            this.width = width;
            this.height = height;
        }

        public void AddRenderTarget() {
            var texture = new Texture();
            texture.id = GL.GenTexture();
            texture.width = width;
            texture.height = height;            
            GL.BindTexture(TextureTarget.Texture2D, texture.id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, 0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            var id = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, id);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture.id, 0);

            var rbo = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, width, height);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo);

            var code = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (code != FramebufferErrorCode.FramebufferComplete) {
                throw new System.Exception("Framebuffer status Error:" + code);
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            var vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            var vertices = new float[] {
                -1f, -1f,/*xy*/ 0, 0,/*uv*/
                 1f, -1f,/*xy*/ 1, 0,/*uv*/
                 1f,  1f,/*xy*/ 1, 1,/*uv*/
                -1f,  1f,/*xy*/ 0, 1 /*uv*/
            };
            var vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            var indices = new uint[] { 0, 1, 2, 0, 2, 3 };
            var ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StaticDraw);

            var STRIDE = 4 * sizeof(float);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, STRIDE, 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, STRIDE, 2 * sizeof(float));
            GL.BindVertexArray(0);

            var shader = Toy3dCore.CreateFramebufferShader();
            renderTarget = new RenderTarget(id, vao, indices, shader, texture) {
                width = width,
                height = height
            };
        }

        public void Draw(GameWorld world, float elapsed) {
            if (renderTarget.id <= 0) {
                DrawWorld(world, elapsed);
            } else {
                DrawRenderTarget(world, elapsed);
            }
        }

        private void DrawWorld(GameWorld world, float elapsed) {
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            // 待分析：若为Less则天空盒无法绘制（在设定深度值为1的情况下：gl_Position = pos.xyww;)
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            world.Draw();
        }

        private void DrawRenderTarget(GameWorld world, float elapsed) {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, renderTarget.id);
            DrawWorld(world, elapsed);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Disable(EnableCap.DepthTest);
            // GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            // GL.LineWidth(4.0f);
            GL.UseProgram(renderTarget.shader.program);
            GL.BindVertexArray(renderTarget.vao);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, renderTarget.texture.id);
            GL.DrawElements(BeginMode.Triangles, renderTarget.indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }
}