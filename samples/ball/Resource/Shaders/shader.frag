#version 330 core

struct Material {
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
};

struct DirectionLight {
    vec3 direction;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct PointLight {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float constant;
    float linear;
    float quadratic;
};

#define NR_POINT_LIGHTS 4

out vec4 outputColor;

uniform vec3 viewWorldPosition;
uniform Material material;
uniform DirectionLight directionLight;
uniform PointLight pointLights[NR_POINT_LIGHTS];

in vec3 fragWorldNormal;
in vec3 fragWorldPosition;
in vec2 fragTextureCoord;

vec3 CalculateDirectionLight(DirectionLight light) {
    vec3 ambient = light.ambient * texture(material.diffuse, fragTextureCoord).rgb;

    vec3 lightDirection = normalize(-light.direction);
    float factorDiffuse = max(dot(lightDirection, normalize(fragWorldNormal)), 0.0);
    vec3 diffuse = factorDiffuse * light.diffuse * texture(material.diffuse, fragTextureCoord).rgb;

    vec3 viewDirection = normalize(viewWorldPosition - fragWorldPosition);
    vec3 reflectLightDirection = reflect(-lightDirection, fragWorldNormal);
    float factorSpecular = pow(max(dot(viewDirection, reflectLightDirection), 0.0), material.shininess);
    vec3 specular = factorSpecular * light.specular * texture(material.specular, fragTextureCoord).rgb;

    return ambient + diffuse + specular;
}

vec3 CalculatePointLight(PointLight light) {
    vec3 ambient = light.ambient * texture(material.diffuse, fragTextureCoord).rgb;
    
    vec3 lightDirection = normalize(light.position - fragWorldPosition);
    float factorDiffuse = max(dot(lightDirection, normalize(fragWorldNormal)), 0.0);
    vec3 diffuse = factorDiffuse * light.diffuse * texture(material.diffuse, fragTextureCoord).rgb;

    vec3 viewDirection = normalize(viewWorldPosition - fragWorldPosition);
    vec3 reflectLightDirection = reflect(-lightDirection, fragWorldNormal);
    float factorSpecular = pow(max(dot(viewDirection, reflectLightDirection), 0.0), material.shininess);
    vec3 specular = factorSpecular * light.specular * texture(material.specular, fragTextureCoord).rgb;

    float d = length(light.position - fragWorldPosition);
    float attenuation = 1.0 / (light.constant + light.linear * d + light.quadratic * (d * d));
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;

    return ambient + diffuse + specular;
}

void main() {
    vec3 color = CalculateDirectionLight(directionLight);
    for (int i = 0; i < NR_POINT_LIGHTS; i++) {
        color += CalculatePointLight(pointLights[i]);
    }
    outputColor = vec4(color, 1.0);
}