void main()
{
	// Transform coordinates
	gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
	// Assign texture coordinates
	gl_TexCoord[0] = gl_TextureMatrix[0] * gl_MultiTexCoord0;
	// Assign color
	gl_FrontColor = gl_Color;
}
