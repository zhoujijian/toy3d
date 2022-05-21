using System;
using System.IO;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Toy3d.Core
{
    public class Shader
    {
        private readonly int program;
        private readonly Dictionary<string, int> uniformLocations;

        public int ProgramId { get { return program; } }

        // The GLSL source is compiled *at runtime*, so it can optimize itself for the graphics card it's currently being used on.
        public Shader(string pathVertexShader, string pathFragmentShader) {
            var vertexSource = File.ReadAllText(pathVertexShader);
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            GL.CompileShader(vertexShader);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out var codeVertexShader);
            if (codeVertexShader != (int)All.True) {
                var infoLog = GL.GetShaderInfoLog(vertexShader);
                throw new Exception($"Error occurred whilst compiling vertex shader({vertexShader}).\n{infoLog}");
            }

            var fragmentSource = File.ReadAllText(pathFragmentShader);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            GL.CompileShader(fragmentShader);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out var codeFragmentShader);
            if (codeFragmentShader != (int)All.True) {
                var infoLog = GL.GetShaderInfoLog(fragmentShader);
                throw new Exception($"Error occured whilst compiling fragment shader({fragmentShader}).\n{infoLog}");
            }
	    
            program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True) {
                throw new Exception($"Error occured whilst linking program({program})");
            }
            // When the shader program is linked, it no longer needs the individual shaders attached to it;
            // the compiled code is copied into the shader program. Detach them, and then delete them.
            GL.DetachShader(program, vertexShader);
            GL.DetachShader(program, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            GL.GetProgram(program, GetProgramParameterName.ActiveUniforms, out var uniformsCount);
            uniformLocations = new Dictionary<string, int>();
            for (var i = 0; i < uniformsCount; i++) {
                int size;
                ActiveUniformType type;
                var uniformName = GL.GetActiveUniform(program, i, out size, out type);
                var uniformLocation = GL.GetUniformLocation(program, uniformName);
                uniformLocations.Add(uniformName, uniformLocation);
                System.Diagnostics.Debug.Print($"uniform: {uniformName} => {uniformLocation}");
            }
        }

        public void UseProgram() {
            GL.UseProgram(program);
        }

        // The shader sources provided with this project use hardcoded layout(location)-s. If you want to do it dynamically,
        // you can omit the layout(location=X) lines in the vertex shader, and use this in VertexAttribPointer instead of the hardcoded values.
        public int GetAttribLocation(string attribName) {
            return GL.GetAttribLocation(program, attribName);
        }

        // Setting a uniform is almost always the exact same, so I'll explain it here once, instead of in every method:
        //   1. Bind the program you want to set the uniform on
        //   2. Get a handle to the location of the uniform with GL.GetUniformLocation.
        //   3. Use the appropriate GL.Uniform* function to set the uniform.

        public void SetInt(string name, int data) {
            GL.Uniform1(uniformLocations[name], data);
        }

        public void SetFloat(string name, float data) {
            GL.Uniform1(uniformLocations[name], data);
        }

        public void SetVector4(string name, Vector4 data) {
            GL.Uniform4(uniformLocations[name], data);
        }

        public void SetMatrix4(string name, Matrix4 data) {
            GL.UniformMatrix4(uniformLocations[name], true, ref data);
        }
    }
}
