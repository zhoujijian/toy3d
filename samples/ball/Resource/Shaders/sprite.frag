#version 330 core

in vec2 aTexCoords;
out vec4 color;

uniform sampler2D uTexture;
uniform vec4 uSpriteColor;

void main()
{
    color = texture(uTexture, aTexCoords) * uSpriteColor;
}