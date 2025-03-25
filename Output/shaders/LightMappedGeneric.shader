// Lit Brush Shader - Multi-Light Version

// Global Uniforms
uniform sampler2D uTexture0;

struct Light {
    vec3 position;
    vec4 color; // RGB = color, A = intensity
    vec3 attenuation; // Constant, Linear, Quadratic
};

uniform Light lights[10];
uniform int activeLights; // Number of active lights in the scene

// Vertex Code
void vertex() 
{
    // Input attributes
    layout(location = 0) in vec3 vPos;    // Vertex position
    layout(location = 1) in vec3 vNormal; // Vertex normal
    layout(location = 2) in vec2 vUv;     // UV coordinates

    out vec2 Uv;
    out vec3 VERT_NORMAL;
    out vec3 FragPos;

    void main()
    {
        mat4 model = MODEL_MATRIX;
        vec3 scale = vec3(length(model[0] * 100), length(model[1] * 100), length(model[2] * 100));
        vec2 scaledUv = vUv * scale.xy;

        gl_Position = PROJECTION_MATRIX * VIEW_MATRIX * model * vec4(vPos, 1.0);
    
        Uv = scaledUv;
        VERT_NORMAL = normalize(mat3(transpose(inverse(model))) * vNormal);
        FragPos = vec3(model * vec4(vPos, 1.0));
    }
}

// Fragment Code
void fragment() 
{
    in vec2 Uv;
    in vec3 VERT_NORMAL;
    in vec3 FragPos;
    
    uniform float specular = 0.5;
    uniform float ambientStrength = 0.2;

    void main()
    {
        vec4 texColor = texture(uTexture0, Uv);
        vec3 norm = normalize(VERT_NORMAL);
        vec3 viewDir = normalize(VIEW_POS - FragPos);
        
        vec3 totalLight = vec3(0.0);
        
        // Iterate through all active lights
        for (int i = 0; i < activeLights; i++) {
            // Light properties
            vec3 lightDir = normalize(lights[i].position - FragPos);
            float intensity = lights[i].color.a;
            vec3 color = lights[i].color.rgb;
            
            // Calculate distance and attenuation
            float distance = length(lights[i].position - FragPos);
            float attenuation = 1.0 / (lights[i].attenuation.x + 
                                      lights[i].attenuation.y * distance + 
                                      lights[i].attenuation.z * (distance * distance));
            
            // Ambient component
            vec3 ambient = ambientStrength * color * intensity;
            
            // Diffuse component
            float diff = max(dot(norm, lightDir), 0.0);
            vec3 diffuse = diff * color * intensity;
            
            // Specular component
            vec3 reflectDir = reflect(-lightDir, norm);  
            float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
            vec3 specularOutput = specular * spec * color * intensity;
            
            // Combine all components for this light
            totalLight += (ambient + diffuse + specularOutput) * attenuation;
        }
        
        // Final result with texture
        vec3 result = totalLight * texColor.rgb;
        
        // Output final color
        FRAG_COLOR = vec4(result, texColor.a);
    }
}