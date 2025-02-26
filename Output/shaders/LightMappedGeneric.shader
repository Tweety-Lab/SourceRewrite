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

        // Sample the texture using the UV coordinates
        vec4 texColor = texture(uTexture0, fUv);

        // Apply tint by multiplying the texture color by the tint color
            FragColor = texColor * vec4(test_light.color, 1.0);
    }
}
