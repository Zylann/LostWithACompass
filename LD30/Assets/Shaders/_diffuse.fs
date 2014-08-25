uniform sampler2D mainTexture;
uniform sampler2D shadowMap;

void main()
{
	// Get base texture color
	vec4 pixel = texture2D(mainTexture, gl_TexCoord[0].xy);
	// Get shadow color
	vec4 shadow = vec4(texture2D(shadowMap, gl_FragCoords.xy).rgb, 1.0);
	// Final color
	gl_FragColor = gl_Color * pixel * shadow;
}

