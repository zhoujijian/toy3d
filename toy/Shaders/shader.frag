#version 330 core

struct Material {
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
};

struct Light {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

out vec4 outputColor;

uniform vec3 viewWorldPosition;
uniform Material material;
uniform Light light;

in vec3 fragWorldNormal;
in vec3 fragWorldPosition;
in vec2 fragTextureCoord;

void main()
{
    vec3 ambient = light.ambient * texture(material.diffuse, fragTextureCoord).rgb;
    
    vec3 lightDirection = normalize(light.position - fragWorldPosition);
    float factorDiffuse = max(dot(lightDirection, normalize(fragWorldNormal)), 0.0);
    vec3 diffuse = factorDiffuse * light.diffuse * vec3(texture(material.diffuse, fragTextureCoord));

    vec3 viewDirection = normalize(viewWorldPosition - fragWorldPosition);
    vec3 reflectLightDirection = reflect(-lightDirection, fragWorldNormal);
    float factorSpecular = pow(max(dot(viewDirection, reflectLightDirection), 0.0), material.shininess);
    vec3 specular = factorSpecular * light.specular * vec3(texture(material.specular, fragTextureCoord));

    outputColor = vec4(ambient + diffuse + specular, 1.0);
}