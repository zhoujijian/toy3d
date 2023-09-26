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