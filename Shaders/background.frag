#version 330 core
out vec4 FragColor;

in vec2 texCoord;

uniform sampler2D uTexture;
uniform float uOpacity;

void main()
{
    vec4 videoColor = texture(uTexture, texCoord);
    // Ensure proper color range and apply opacity
    FragColor = vec4(videoColor.rgb, uOpacity);
}
