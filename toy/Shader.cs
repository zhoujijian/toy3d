using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace Toy3d.Core
{
    public struct ShaderInfo {
        public int program;
        public Dictionary<string, int> uniformLocations;
    }

    public class Shader {
        public static ShaderInfo Create(string pathVertex, string pathFragment) {
            var vertexSource = File.ReadAllText(pathVertex);
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            GL.CompileShader(vertexShader);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out var codeVertexShader);
            if (codeVertexShader != (int)All.True) {
                var infoLog = GL.GetShaderInfoLog(vertexShader);
                throw new Exception($"Error occurred whilst compiling vertex shader({vertexShader}).\n{infoLog}");
            }

            var fragmentSource = File.ReadAllText(pathFragment);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            GL.CompileShader(fragmentShader);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out var codeFragmentShader);
            if (codeFragmentShader != (int)All.True) {
                var infoLog = GL.GetShaderInfoLog(fragmentShader);
                throw new Exception($"Error occured whilst compiling fragment shader({fragmentShader}).\n{infoLog}");
            }

            var info = new ShaderInfo();	    
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

            /*
            GL.GetProgram(info.program, GetProgramParameterName.ActiveUniforms, out var uniformsCount);
            if (uniformsCount > 0) {
                info.uniformLocations = new Dictionary<string, int>();
                for (var i = 0; i < uniformsCount; i++) {
                    int size;
                    ActiveUniformType type;
                    var name = GL.GetActiveUniform(info.program, i, out size, out type);

                    info.uniformLocations.Add(name, GL.GetUniformLocation(info.program, name));
                    var uniformLocation = GL.GetUniformLocation(program, uniformName);
                    uniformLocations.Add(uniformName, uniformLocation);
                    System.Diagnostics.Debug.Print($"uniform: {uniformName} => {uniformLocation}");
                }
            }
            */

            return info;
        }
    }
}
