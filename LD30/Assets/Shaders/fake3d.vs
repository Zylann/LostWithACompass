void main()
{
    // transform the vertex position
    gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;

    // forward the vertex color
    gl_FrontColor = gl_Color;
}

