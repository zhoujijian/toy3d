using System;
using System.Drawing;
using System.Drawing.Imaging;

using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Toy3d.Core {
    public struct Shader {
        public int program;
    }

    public struct Texture {
        public int id;
        public int width;
        public int height;
        public string type;
    }

    public struct Transform {
        public OpenTK.Mathematics.Vector3 scale;
        public OpenTK.Mathematics.Vector3 rotation;
        public OpenTK.Mathematics.Vector3 position;
    }

    public static class Toy3dCore {
        public static Shader CreateSpriteShader() {
            const string vertex = @"
                // First non-comment line should always be a #version statement; this just tells the GLSL compiler what version it should use.
                #version 330 core

                // GLSL's syntax is somewhat like C, but it has a few differences.

                // There are four different types of variables in GLSL: input, output, uniform, and internal.
                // - Input variables are sent from the buffer, in a way defined by GL.VertexAttribPointer.
                // - Output variables are sent from this shader to the next one in the chain (which will be the fragment shader most of the time).
                // - Uniforms will be touched on in the next tutorial.
                // - Internal variables are defined in the shader file and only used there.

                layout(location = 0) in vec4 aPosition;

                out vec2 aTexCoords;

                uniform mat4 uModel;
                uniform mat4 uProjection;

                // Like C, we have an entrypoint function. In this case, it takes void and returns void, and must be named main.
                // You can do all sorts of calculations here to modify your vertices, but right now, we don't need to do any of that.
                // gl_Position is the final vertex position; pass a vec4 to it and you're done.

                void main(void)
                {
                    aTexCoords = aPosition.zw;
                    gl_Position = uProjection * uModel * vec4(aPosition.x, aPosition.y, 0.0, 1.0);
                    // gl_Position = vec4(aPosition.x, aPosition.y, 0.0, 1.0) * uModel * uProjection;
                }
            ";

            const string fragment = @"
                #version 330 core

                in vec2 aTexCoords;
                out vec4 color;

                uniform sampler2D uTexture;
                uniform vec4 uSpriteColor;

                void main()
                {
                    color = texture(uTexture, aTexCoords) * uSpriteColor;
                }
            ";

            return CreateShader(vertex, fragment);
        }

        public static Shader CreateParticleShader() {
            const string vertex = @"
                #version 330 core

                layout(location = 0) in vec4 aPosition;

                out vec2 texCoords;
                out vec4 particleColor;

                uniform mat4 uProjection;
                uniform mat4 uModel;
                uniform vec4 uColor;

                void main() {
                    particleColor = uColor;
                    texCoords = aPosition.zw;
                    gl_Position = uProjection * uModel * vec4(aPosition.x, aPosition.y, 0.0f, 1.0f);
                }
            ";

            const string fragment = @"
                #version 330

                in vec2 texCoords;
                in vec4 particleColor;

                out vec4 color;

                uniform sampler2D uTexture;

                void main() {
                    vec4 texel = texture(uTexture, texCoords);
                    color = texel * particleColor;
                }
            ";

            return CreateShader(vertex, fragment);
        }

        public static Shader CreateFramebufferShader() {
            const string vertex = @"
                #version 330 core

                layout(location = 0) in vec2 aPosition;
                layout(location = 1) in vec2 aTexCoord;

                out vec2 texCoords;

                void main() {
                    texCoords = aTexCoord;
                    gl_Position = vec4(aPosition.x, aPosition.y, 0, 1.0);
                }
            ";

            const string fragment = @"
                #version 330

                in vec2 texCoords;
                uniform sampler2D uTexture;
                out vec4 color;

                void main() {
                    float depth = texture(uTexture, texCoords).r;
                    color = vec4(vec3(depth), 1.0);
                    // color = texture(uTexture, texCoords);

                    // 1)inversion(反相)
                    // color = vec4(vec3(1 - texture(uTexture, texCoords)), 1.0);

                    // 2)grayscale(加权灰度)
                    // float average = 0.2126 * color.r + 0.7152 * color.g + 0.0722 * color.b;
                    // color = vec4(average, average, average, 1.0);
                }
            ";

            return CreateShader(vertex, fragment);
        }

        public static Shader CreateShadowShader() {
            const string vertex = @"
                #version 330 core

                layout(location = 0) in vec3 position;

                uniform mat4 projectionLight;
                uniform mat4 viewLight;
                uniform mat4 model;

                void main() {
                    gl_Position = projectionLight * viewLight * model * vec4(position, 1.0f);
                }
            ";

            const string fragment = @"
                #version 330

                void main() { }
            ";

            return CreateShader(vertex, fragment);
        }

        public static Shader CreateShader(string vertexSource, string fragmentSource) {
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            GL.CompileShader(vertexShader);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out var codeVertexShader);
            if (codeVertexShader != (int)All.True) {
                var infoLog = GL.GetShaderInfoLog(vertexShader);
                throw new Exception($"Error occurred whilst compiling vertex shader({vertexShader}).\n{infoLog}");
            }

            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            GL.CompileShader(fragmentShader);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out var codeFragmentShader);
            if (codeFragmentShader != (int)All.True) {
                var infoLog = GL.GetShaderInfoLog(fragmentShader);
                throw new Exception($"Error occured whilst compiling fragment shader({fragmentShader}).\n{infoLog}");
            }

            var info = new Shader();	    
            info.program = GL.CreateProgram();
            GL.AttachShader(info.program, vertexShader);
            GL.AttachShader(info.program, fragmentShader);
            GL.LinkProgram(info.program);
            GL.GetProgram(info.program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True) {
                throw new Exception($"Error occured whilst linking program({info.program})");
            }

            // When the shader program is linked, it no longer needs the individual shaders attached to it;
            // the compiled code is copied into the shader program. Detach them, and then delete them.
            GL.DetachShader(info.program, vertexShader);
            GL.DetachShader(info.program, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return info;
        }

        public static int CreateCubemap(string[] paths) {
            var id = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, id);

            for (var i=0; i<6; ++i) {
                using (var image = new Bitmap(paths[i])) {
                    // image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    var data = image.LockBits(
                        new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                }
            }
            
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            return id;
        }

        public static Texture CreateTexture(string path) {
            Texture info = new Texture();
            info.id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, info.id);

            using (var image = new Bitmap(path)) {
                info.width = image.Width;
                info.height = image.Height;
		
                // Our Bitmap loads from the top-left pixel, whereas OpenGL loads from the bottom-left, causing the texture to be flipped vertically.
                // This will correct that, making the texture display properly.
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                // First, we get our pixels from the bitmap we loaded.
                // Arguments:
                //   The pixel area we want. Typically, you want to leave it as (0,0) to (width,height), but you can
                //   use other rectangles to get segments of textures, useful for things such as spritesheets.
                //   The locking mode. Basically, how you want to use the pixels. Since we're passing them to OpenGL,
                //   we only need ReadOnly.
                //   Next is the pixel format we want our pixels to be in. In this case, ARGB will suffice.
                //   We have to fully qualify the name because OpenTK also has an enum named PixelFormat.
                var data = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                // Now that our pixels are prepared, it's time to generate a texture. We do this with GL.TexImage2D.
                // Arguments:
                //   The type of texture we're generating. There are various different types of textures, but the only one we need right now is Texture2D.
                //   Level of detail. We can use this to start from a smaller mipmap (if we want), but we don't need to do that, so leave it at 0.
                //   Target format of the pixels. This is the format OpenGL will store our image with.
                //   Width of the image
                //   Height of the image.
                //   Border of the image. This must always be 0; it's a legacy parameter that Khronos never got rid of.
                //   The format of the pixels, explained above. Since we loaded the pixels as ARGB earlier, we need to use BGRA.
                //   Data type of the pixels.
                //   And finally, the actual pixels.
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            }

            // First, we set the min and mag filter. These are used for when the texture is scaled down and up, respectively.
            // Here, we use Linear for both. This means that OpenGL will try to blend pixels, meaning that textures scaled too far will look blurred.
            // You could also use (amongst other options) Nearest, which just grabs the nearest pixel, which makes the texture look pixelated if scaled too far.
            // NOTE: The default settings for both of these are LinearMipmap. If you leave these as default but don't generate mipmaps,
            // your image will fail to render at all (usually resulting in pure black instead).
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Now, set the wrapping mode. S is for the X axis, and T is for the Y axis.
            // We set this to Repeat so that textures will repeat when wrapped. Not demonstrated here since the texture coordinates exactly match
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            // Next, generate mipmaps.
            // Mipmaps are smaller copies of the texture, scaled down. Each mipmap level is half the size of the previous one
            // Generated mipmaps go all the way down to just one pixel.
            // OpenGL will automatically switch between mipmaps when an object gets sufficiently far away.
            // This prevents moiré effects, as well as saving on texture bandwidth.
            // Here you can see and read about the morié effect https://en.wikipedia.org/wiki/Moir%C3%A9_pattern
            // Here is an example of mips in action https://en.wikipedia.org/wiki/File:Mipmap_Aliasing_Comparison.png
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return info;
        }
    }
}