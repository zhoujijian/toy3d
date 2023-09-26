#version 330 core
in vec2 TexCoords;
out vec4 color;

uniform sampler2D text;
uniform vec3 textColor;

void main()
{
    float r = texture(text, TexCoords).r;
    // color = vec4(textColor.rgb, 1.0);
    // color = vec4(textColor.rgb, r);
    // vec4 sampled = vec4(1.0, 1.0, 1.0, r);
    // color = vec4(textColor, 1.0) * sampled;
    color = vec4(textColor.rgb * r, r);
}