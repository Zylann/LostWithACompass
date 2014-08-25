//uniform sampler2D texture;
uniform sampler2D filterMap;
uniform vec2 lightPos;
uniform vec4 lightColor;
uniform vec2 fragOffset;

void main()
{
	// Hardcoded constants
	float resolution = 256.0;
	//vec3 lightColor = vec3(1.0, 1.0, 1.0);
	float amount = 1.0;
	//float radius = 100;

	// Calculated constants
	float invRes = 1.0 / resolution;
	
	//---

	vec2 fragCoord = gl_FragCoord.xy - vec2(fragOffset.x, fragOffset.y);

	// Distance from the fragment to the light in pixels
	float distancePixels = distance(lightPos, fragCoord);

	// Samples actually needed to do the full raycast
	int neededSamples = int(distancePixels);
	//float invSamples = 1.0 / float(samples);

	// Fixed number of samples the shader will execute (for performance)
	int FIXED_SAMPLES = 150;
	float fixedInvSamples = 1.0 / float(FIXED_SAMPLES);

	// Raycast parameters in [0,1] space
    vec2 rayStart = lightPos * invRes;
	vec2 rayEnd = fragCoord * invRes;
	vec2 rayStep = normalize(rayEnd - rayStart) * invRes;

	vec2 rayPos = rayStart;
	int crossedSamples = 0;

	vec3 filteredLightColor = lightColor.rgb;
	//vec3 filteredLightColor = lightColor * (clamp(1.0 - 0.002*distancePixels, 0.0, 1.0));

	// Note: this loops a fixed number of times because it enables the compiler
	// to unroll the code, which makes the shader a lot faster.
	// Still, we need to check a variable amount of samples, so the step() function 
	// is used to disable light filtering above a certain number of loops.
	for(int i = 0; i < FIXED_SAMPLES; ++i)
	{
		// k==1 on the raycast segment, 0 if out
	    float k = 1.0 - step(float(neededSamples), float(crossedSamples));

	    // Get color filter
	    vec3 filterColor = texture2D(filterMap, vec2(rayPos.x, rayPos.y)).rgb;

	    // Filter our "photon", or not if we got enough samples
	    //filteredLightColor = mix(filteredLightColor, filteredLightColor*filterColor, 0.25*k);
		filteredLightColor = mix(filteredLightColor, min(filteredLightColor, filterColor), k);

	    // Advance the ray
		rayPos += rayStep;

		++crossedSamples;
	}

	filteredLightColor *= (1.0 - 2.0*distancePixels * invRes);

	vec3 finalColor = amount*filteredLightColor + (1.0-amount)*lightColor.rgb;

    gl_FragColor = gl_Color * vec4(finalColor, 1.0);
    //gl_FragColor = vec4(fragCoord.x * invRes, fragCoord.y * invRes, 1.0, 1.0);
    //gl_FragColor = vec4(vec2(fragOffset.x, 2048.0-128.0-fragOffset.y) / 2048.0, 1.0, 1.0);
    //gl_FragColor = vec4(gl_FragCoord.xy / 2048.0, 1.0,1.0);
}

