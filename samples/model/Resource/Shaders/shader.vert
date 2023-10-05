#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTextureCoord;

uniform mat4 uProjection;
uniform mat4 uView;
uniform mat4 uModel;

out vec3 fragWorldNormal;
out vec3 fragWorldPosition;
out vec2 fragTextureCoord;

void main(void)
{
    gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
    fragWorldPosition = vec3(uModel * vec4(aPosition, 1.0));
    fragWorldNormal = aNormal;
    fragTextureCoord = aTextureCoord;
}