// Lit Brush Shader

// Global Uniforms
uniform sampler2D uTexture0;

uniform vec3 light_position;
uniform vec3 light_color;
uniform float light_intensity;

uniform vec3 light_attenuation; // We store attenuation in a Vector3 that goes Constant, Linear, Quadratic.

// Fragment Code
void fragment() 
{
    // Input attributes from the vertex shader
    in vec2 Uv;
    in vec3 Normal;  // Normal information passed from vertex shader
    in vec3 FragPos; // Fragment position passed from vertex shader

    out vec4 FragColor;

    void main()
    {
        // Sample the texture using the UV coordinates
        vec4 texColor = texture(uTexture0, Uv);

        // Light Variables
        vec3 norm = normalize(Normal);
        vec3 lightDir = normalize(light_position - FragPos);  

        // Ambient
        float ambientStrength = 0.4;
        vec3 ambient = ambientStrength * light_color;

        // Diffuse
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = diff * light_color;

        // Specular
        float specularStrength = 0.5;
        vec3 viewDir = normalize(VIEW_POS - FragPos);
        vec3 reflectDir = reflect(-lightDir, norm);  

        float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
        vec3 specular = specularStrength * spec * light_color;

        // Distance to light
        float distance = length(light_position - FragPos);

        // Calculate attenuation
        float attenuation = 1.0 / (light_attenuation.x + light_attenuation.y * distance + light_attenuation.z * (distance * distance));

        // Final lighting calculations
        vec3 finalLight = (ambient + diffuse + specular) * attenuation;

        // Final result with texture
        vec3 result = finalLight * texColor.rgb; // texColor.rgb to exclude alpha

        // Apply final color by multiplying the texture color by the ambient color
        FragColor = vec4(result, texColor.a);
    }
}
