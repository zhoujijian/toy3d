using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

using System;
using System.IO;
using System.Collections.Generic;

using SharpFont;

namespace Toy3d.Core {
    public class FontRenderer {
        private struct Character {
            public int textureId;
            public Vector2 size;
            public Vector2 bearing;
            public int advance;
        }

        private int vao;
        private int vbo;
        private ShaderInfo shader;
        private Dictionary<char, Character> characters;

        public FontRenderer() {
            characters = new Dictionary<char, Character>();

            var library = new Library();
            var face = new Face(library, File.ReadAllBytes("Resource/FreeSans.ttf"), 0);
            face.SetPixelSizes(0, 32);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.ActiveTexture(TextureUnit.Texture0);

            for (var c = '\u0000'; c <= '\u00ff'; c++) {
		        face.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);

                var textureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, textureId);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8,
                    face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows, 0, PixelFormat.Red, PixelType.UnsignedByte, face.Glyph.Bitmap.Buffer);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.BindTexture(TextureTarget.Texture2D, 0);

                characters.Add(c, new Character() {
                    textureId = textureId,
                    size = new Vector2(face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows),
                    bearing = new Vector2(face.Glyph.BitmapLeft, face.Glyph.BitmapTop),
                    advance = (int)face.Glyph.Advance.X.Value
                });
            }

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, 6 * 4 * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), IntPtr.Zero);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            shader = Shader.Create("Shaders/font.vert", "Shaders/font.frag");
            GL.ProgramUniform1(shader.program, GL.GetUniformLocation(shader.program, "text"), 0);
            GL.ProgramUniform3(shader.program, GL.GetUniformLocation(shader.program, "textColor"), 0.5f, 0.9f, 0.8f);
        }

	    public void Draw(string text, float x, float y, OrthogonalCamera2D camera) {
            var projection = camera.ProjectionMatrix;
            GL.UseProgram(shader.program);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.program, "projection"), true, ref projection);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindVertexArray(vao);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            // GL.BlendFunc(0, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            foreach (var t in text) {
                var c = characters[t];
                var w = c.size.X;
                var h = c.size.Y;
                var px = x + c.bearing.X;
                var py = y + c.bearing.Y - h;
                var vertices = new float[] {
                    px,   py,   0.0f, 0.0f,
                    px+w, py+h, 1.0f, 1.0f,
                    px,   py+h, 0.0f, 1.0f,
                    px+w, py+h, 1.0f, 1.0f,
                    px,   py,   0.0f, 0.0f,
                    px+w, py,   1.0f, 0.0f
                };
                GL.BindTexture(TextureTarget.Texture2D, c.textureId);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertices.Length * sizeof(float), vertices);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                // GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                // GL.BindTexture(TextureTarget.Texture2D, 0);

                x += (c.advance >> 6);
            }

	        GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindVertexArray(0);
        }
    }
}
