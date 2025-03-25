// Lit Brush Shader

// Global Uniforms
uniform sampler2D uTexture0;

struct Light {
    vec3 position;
    vec4 color; // RGB = color, A = intensity
    vec3 attenuation; // Constant, Linear, Quadratic
};

uniform Light lights[10];

// Vertex Code
void vertex() 
{
    // Input attributes
    layout(location = 0) in vec3 vPos;    // Vertex position
    layout(location = 1) in vec3 vNormal; // Vertex normal (from location 1)
    layout(location = 2) in vec2 vUv;     // UVs (from location 2)

    out vec2 Uv;
    out vec3 VERT_NORMAL; // Pass the normal to the fragment shader
    out vec3 FragPos; // Pass the fragment position to the fragment shader

    void main()
    {
        // Model matrix for transforming positions and normals
        mat4 model = MODEL_MATRIX;

        // Compute the scale factor based on the model matrix
        vec3 scale = vec3(length(model[0] * 100), length(model[1] * 100), length(model[2] * 100));

        // Normalize the UVs to map them based on the size of the object
        vec2 scaledUv = vUv * scale.xy;

        // Transform the vertex position to clip space
        gl_Position = PROJECTION_MATRIX * VIEW_MATRIX * model * vec4(vPos, 1.0);
    
        // Pass the scaled UV and normal to the fragment shader
        Uv = scaledUv;
        VERT_NORMAL = normalize(mat3(transpose(inverse(model))) * vNormal); // Transform normal
        FragPos = vec3(model * vec4(vPos, 1.0)); // Calculate world space fragment position
    }
}

// Fragment Code
void fragment() 
{
    // Input attributes from the vertex shader
    in vec2 Uv;
    in vec3 VERT_NORMAL;  // Normal information passed from vertex shader
    in vec3 FragPos; // Fragment position passed from vertex shader
    
    uniform float specular = 0.5;

    void main()
    {
        // Sample the texture using the UV coordinates
        vec4 texColor = texture(uTexture0, Uv);

        // Light Variables
        vec3 norm = normalize(VERT_NORMAL);
        vec3 lightDir = normalize(lights[0].position - FragPos);

        // Extract intensity from light.color.a
        float intensity = lights[0].color.a;
        vec3 color = lights[0].color.rgb; // Extract RGB component

        // Ambient
        float ambientStrength = 0.2;
        vec3 ambient = ambientStrength * color * intensity;

        // Diffuse
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = diff * color * intensity;

        vec3 viewDir = normalize(VIEW_POS - FragPos);
        vec3 reflectDir = reflect(-lightDir, norm);  
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
        vec3 specularOutput = specular * spec * color * intensity;

        // Distance to light
        float distance = length(lights[0].position - FragPos);

        // Calculate attenuation
        float attenuation = 1.0 / (lights[0].attenuation.x + lights[0].attenuation.y * distance + lights[0].attenuation.z * (distance * distance));

        // Final lighting calculations
        vec3 finalLight = (ambient + diffuse + specularOutput) * attenuation;

        // Final result with texture
        vec3 result = finalLight * texColor.rgb; // texColor.rgb to exclude alpha

        // Apply final color by multiplying the texture color by the ambient color
        FRAG_COLOR = vec4(result, texColor.a);
    }
}