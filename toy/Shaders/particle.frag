#version 330

in vec2 texCoords;
in vec4 particleColor;

out vec4 color;

uniform sampler2D uTexture;

void main() {
    vec4 texel = texture(uTexture, texCoords);
    color = texel * particleColor;
}