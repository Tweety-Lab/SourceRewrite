// Unlit Object Shader

uniform samplerCube uSkybox; // Cubemap texture for the skybox
uniform vec4 color; // Optional tint

// Fragment Code
void fragment()
{
    in vec3 ViewDirection; // This should be passed from the vertex shader

    void main()
    {
        vec4 defaultColor = vec4(1.0, 1.0, 1.0, 1.0); // Default white

        // Sample from the skybox cubemap using the view direction
        vec4 skyColor = texture(uSkybox, normalize(ViewDirection));

        // Apply optional tint
        vec4 finalColor = skyColor * (color.a > 0.0 ? color : defaultColor);

        // Output the color
        FRAG_COLOR = finalColor;
    }
}