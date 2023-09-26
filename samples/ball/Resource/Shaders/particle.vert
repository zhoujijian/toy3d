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
    gl_Position = vec4(aPosition.x, aPosition.y, 0.0f, 1.0f) * uModel * uProjection;
}