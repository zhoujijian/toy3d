using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Toy3d.Game;
using System.Linq;

namespace Toy3d.Core {
    internal struct VertexData {
        public int vao;
        public int vbo;
        public int ebo;
        public uint[] indices;
    }

    internal struct RenderParams {
        public int width;
        public int height;
        public int framebufferId;
        public Shader shader;
        public Texture texture;
        public VertexData vdata;
    }

    public struct RenderContext {
        public int id;
        public Matrix4 parent;

        public const int NORMAL = 0;
        public const int SHADOW = 1;
        public const int DEPTH = 2;
    }

    public class Renderer {
        public readonly int width;
        public readonly int height;        
        private RenderParams renderTarget;
        private RenderParams renderShadow;
        private Shader shadowMapShader;

        private const int SHADOW_WIDTH  = 1024;
        private const int SHADOW_HEIGHT = 1024;
        private const float SHADOW_LIGHT_NEAR = 0.5f;
        private const float SHADOW_LIGHT_FAR = 2.5f;

        public bool EnableShadow { get; set; }

        public Renderer(int width, int height) {
            this.width = width;
            this.height = height;
        }

        private VertexData CreateFramebufferVertexData() {
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

            return new VertexData {
                vao = vao,
                vbo = vbo,
                ebo = ebo,
                indices = indices
            };
        }

        public void Draw(IGameWorld world, float elapsed) {
            if (renderShadow.framebufferId <= 0) {
                AddRenderDepthMap();
            }
            DrawShadowDepthMap(world);
            GL.Viewport(0, 0, width, height);
            // DrawRenderTarget(renderShadow);
            DrawWorldShadow(world);

            /*
            if (renderTarget.id <= 0) {
                AddRenderTarget();
            }
            DrawWorld(world);
            DrawRenderTarget(renderTarget);
            */
        }

        public void DrawMesh(IGameWorld world, Mesh mesh, RenderContext context) {
            if (context.id == RenderContext.DEPTH) {
                DrawMeshDepth(world, mesh, context);
            } else if (context.id == RenderContext.NORMAL) {
                DrawMeshNormal(world, mesh, context);
            } else if (context.id == RenderContext.SHADOW) {
                DrawMeshShadow(world, mesh, context);
            }
        }

        private void DrawGameObjects(IGameWorld world, RenderContext context) {
            foreach (var gameObject in world.GetGameObjects()) {
                gameObject.Draw(world, context);
            }
        }

        #region Shadow
        private Matrix4 GetLightMatrix(IGameWorld world, int program) {
            var L = world.DirectionLight;
            var view = Matrix4.LookAt(L.position, L.position + L.direction, Vector3.UnitY);
            // var projection = Matrix4.CreateOrthographicOffCenter(-1f, 1f, -1f, 1f, SHADOW_LIGHT_NEAR, SHADOW_LIGHT_FAR);
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), 1.0f, SHADOW_LIGHT_NEAR, SHADOW_LIGHT_FAR);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "projectionLight"), false, ref projection);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "viewLight"), false, ref view);
            return projection * view;
        }

        private void DrawMeshShadow(IGameWorld world, Mesh mesh, RenderContext context) {
            var program = mesh.material.shader.program;
            GL.UseProgram(program);

            // uniform
            var position = mesh.transform.position;
            var model = context.parent * Matrix4.CreateTranslation(position.X, position.Y, position.Z);
            var view = world.Camera.ViewMatrix;
            var projection = world.Camera.ProjectionMatrix;
            GL.UniformMatrix4(GL.GetUniformLocation(program, "model"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "view"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "projection"), false, ref projection);
            GL.Uniform3(GL.GetUniformLocation(program, "viewPos"),
                world.Camera.position.X, world.Camera.position.Y, world.Camera.position.Z);
            GL.Uniform3(GL.GetUniformLocation(program, "lightPos"),
                world.DirectionLight.position.X, world.DirectionLight.position.Y, world.DirectionLight.position.Z);
            var light = GetLightMatrix(world, program);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "lightSpaceMatrix"), false, ref light);

            // material
            GL.Uniform1(GL.GetUniformLocation(program, "diffuseTexture"), 0);
            GL.Uniform1(GL.GetUniformLocation(program, "shadowMap"), 1);
            // texture
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, mesh.material.diffuse.id);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, renderShadow.texture.id);

            mesh.Draw();
        }

        private void DrawWorldShadow(IGameWorld world) {
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            // 待分析：若为Less则天空盒无法绘制（在设定深度值为1的情况下：gl_Position = pos.xyww;)
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            DrawGameObjects(world, new RenderContext {
                id = RenderContext.SHADOW,
                parent = Matrix4.Identity
            });
            world.Skybox.Draw(world);
        }
        #endregion

        #region RenderTarget
        private void AddRenderTarget() {
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

            renderTarget = new RenderParams() {
                framebufferId = id,
                vdata = CreateFramebufferVertexData(),
                shader = Toy3dCore.CreateFramebufferShader(),
                texture = texture,
                width = width, height = height
            };
        }

        private void DrawRenderTarget(RenderParams rt) {
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Disable(EnableCap.DepthTest);
            // GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            // GL.LineWidth(4.0f);
            GL.UseProgram(rt.shader.program);
            GL.Uniform1(GL.GetUniformLocation(rt.shader.program, "uTexture"), 0);
            GL.Uniform1(GL.GetUniformLocation(rt.shader.program, "near"), SHADOW_LIGHT_NEAR);
            GL.Uniform1(GL.GetUniformLocation(rt.shader.program, "far"), SHADOW_LIGHT_FAR);
            GL.BindVertexArray(rt.vdata.vao);            
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, rt.texture.id);
            GL.DrawElements(BeginMode.Triangles, rt.vdata.indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
        #endregion

        #region DepthMap
        // 排查记录：
        // 1）测试正交相机在无阴影、无Framebuffer情形下是否正常渲染；（适当调整正交相机区域，避免目标过小而看不到）
        // 2）测试Framebuffer在不读取深度纹理情形下是否正常渲染；
        // 3）测试无Framebuffer情形下，渲染的深度纹理是否正常显示；
        // 4）最终定位到深度测试选项未打开!

        private void DrawShadowDepthMap(IGameWorld world) {
            var program = shadowMapShader.program;
            GL.UseProgram(program);
            
            var L = world.DirectionLight;
            var view = Matrix4.LookAt(L.position, L.position + L.direction, Vector3.UnitY);
            // var projection = Matrix4.CreateOrthographicOffCenter(-1f, 1f, -1f, 1f, SHADOW_LIGHT_NEAR, SHADOW_LIGHT_FAR);
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), 1.0f, SHADOW_LIGHT_NEAR, SHADOW_LIGHT_FAR);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "projectionLight"), false, ref projection);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "viewLight"), false, ref view);
            
            if (renderShadow.framebufferId > 0) {
                GL.Viewport(0, 0, SHADOW_WIDTH, SHADOW_HEIGHT);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, renderShadow.framebufferId);
            }
            // 打开深度测试选项，否则不会写出深度信息
            // 且必须在GL.BindFramebuffer之后打开，否则不会对缓冲区纹理生效！
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            DrawGameObjects(world, new RenderContext {
                id = RenderContext.DEPTH,
                parent = Matrix4.Identity
            });
            if (renderShadow.framebufferId > 0) {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
        }

        private void DrawMeshDepth(IGameWorld world, Mesh mesh, RenderContext context) {
            var model = context.parent * Matrix4.CreateTranslation(mesh.transform.position);
            GL.UniformMatrix4(GL.GetUniformLocation(shadowMapShader.program, "model"), false, ref model);
            mesh.Draw();
        }

        private void AddRenderDepthMap() {
            var texid = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texid);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, SHADOW_WIDTH, SHADOW_HEIGHT, 0, PixelFormat.DepthComponent, PixelType.Float, 0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);            

            var framebufferId = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferId);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, texid, 0);
            // 显式告诉OpenGL不需要任何颜色数据进行渲染(Draw/Read Buffer)
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            shadowMapShader = Toy3dCore.CreateShadowShader();

            renderShadow = new RenderParams() {
                framebufferId = framebufferId,
                shader = Toy3dCore.CreateShadowFramebufferShader(),
                vdata = CreateFramebufferVertexData(),
                texture = new Texture { id = texid, width = SHADOW_WIDTH, height = SHADOW_HEIGHT }
            };
        }

        private void DestroyDepthMap() {
            GL.DeleteFramebuffer(renderShadow.framebufferId);
            GL.DeleteBuffer(renderShadow.vdata.ebo);
            GL.DeleteBuffer(renderShadow.vdata.vbo);
            GL.DeleteBuffer(renderShadow.vdata.vao);            
            renderShadow.framebufferId = 0;
            renderShadow.vdata.ebo = 0;
            renderShadow.vdata.vbo = 0;
            renderShadow.vdata.vao = 0;
        }
        #endregion

        #region Normal
        private void DrawWorld(IGameWorld world) {
            if (renderTarget.framebufferId > 0) {
                // 受到EnableCap影响，需在其之前绑定
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, renderTarget.framebufferId);
            }

            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            // 待分析：若为Less则天空盒无法绘制（在设定深度值为1的情况下：gl_Position = pos.xyww;)
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            DrawGameObjects(world, new RenderContext {
                id = RenderContext.NORMAL,
                parent = Matrix4.Identity
            });
            world.Skybox.Draw(world);

            if (renderTarget.framebufferId > 0) {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
        }

        private void DrawMeshNormal(IGameWorld world, Mesh mesh, RenderContext context) {
            var program = mesh.material.shader.program;
            GL.UseProgram(program);

            SetDirectionLight(program, world.DirectionLight);
            var pointLights = world.GetPointLights();
            for (var i=0; i<pointLights.Count(); ++i) {
                SetPointLight(program, pointLights.ElementAt(i), i);
            }

            // matrix
            var position = mesh.transform.position;
            var model = context.parent * Matrix4.CreateTranslation(position.X, position.Y, position.Z);
            var view = world.Camera.ViewMatrix;
            var projection = world.Camera.ProjectionMatrix;           
            GL.UniformMatrix4(GL.GetUniformLocation(program, "uModel"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(program, "uProjection"), false, ref projection);
            GL.Uniform3(GL.GetUniformLocation(program, "viewWorldPosition"),
                world.Camera.position.X, world.Camera.position.Y, world.Camera.position.Z);
            // material
            GL.Uniform1(GL.GetUniformLocation(program, "material.diffuse"), 0);
            GL.Uniform1(GL.GetUniformLocation(program, "material.specular"), 1);
            GL.Uniform1(GL.GetUniformLocation(program, "material.shininess"), mesh.material.shininess);
            // texture
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, mesh.material.diffuse.id);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, mesh.material.specular.id);

            mesh.Draw();
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
        #endregion
    }
}