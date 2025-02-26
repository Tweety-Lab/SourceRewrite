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
    in vec2 fUv;
    out vec4 FragColor;

    void main()
    {
        // Create a test pointlight
        PointLight test_light = PointLight(vec3(0.0, 0.0, 0.0), vec3(1.0, 1.0, 1.0), 1.0);

        // Global light variables
        float ambientStrength = 0.1;
        vec3 ambient = ambientStrength * test_light.color;

        // Sample the texture using the UV coordinates
        vec4 texColor = texture(uTexture0, fUv);

        // Apply final color by multiplying the texture color by the ambient color
        FragColor = texColor * vec4(ambient, 1.0);
    }
}
