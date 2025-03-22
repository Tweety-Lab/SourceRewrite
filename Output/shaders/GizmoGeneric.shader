// Solid Colour Gizmo

uniform vec4 tint; // Color to display

// Fragment Code
void fragment() 
{

    void main()
    {
        // Output the color
        FRAG_COLOR = tint / 255;
    }
}