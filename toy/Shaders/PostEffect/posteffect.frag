#version 330 core

in vec2 frag_aTextureCoords;
out vec4 FragColor;

uniform sampler2D uScreenTexture;

void main()
{ 
    FragColor = texture(uScreenTexture, frag_aTextureCoords);
}