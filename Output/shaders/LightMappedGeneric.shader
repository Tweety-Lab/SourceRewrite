// Lit Brush Shader

// Global Uniforms
uniform sampler2D uTexture0;

struct PointLight {
    vec3 position;
    vec3 color;
    float intensity;
};

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
        // Create a test pointlight
        PointLight test_light = PointLight(vec3(1.0, 1.0, 1.0), vec3(1.0, 1.0, 1.0), 1.0);

        // Sample the texture using the UV coordinates
        vec4 texColor = texture(uTexture0, Uv);

        // Light Variables
        vec3 norm = normalize(Normal);
        vec3 lightDir = normalize(test_light.position - FragPos);  

        // Ambient
        float ambientStrength = 0.1;
        vec3 ambient = ambientStrength * test_light.color;

        // Diffuse
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = diff * test_light.color;

        // Specular
        float specularStrength = 0.5;
        vec3 viewDir = normalize(VIEW_POS - FragPos);
        vec3 reflectDir = reflect(-lightDir, norm);  

        float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
        vec3 specular = specularStrength * spec * test_light.color;  

        // Final color result: (ambient + diffuse) and multiply by texture color
        vec3 result = (ambient + diffuse + specular) * texColor.rgb;  // texColor.rgb to exclude alpha

        // Apply final color by multiplying the texture color by the ambient color
        FragColor = vec4(result, texColor.a);
    }
}
