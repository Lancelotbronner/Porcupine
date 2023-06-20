#version 410 core

uniform mat4 projection;
uniform ivec2 chunk;

layout (location = 0) in uint id;

out VS_OUT {
    uint id;
} vs_out;

void main() {
    int i = gl_VertexID;

    // Chunks are 16x16, so the position is encoded in nibbles
    int x = i & 0xF;
    int y = (i >> 4) & 0xF;

    // Offset by chunk position
    x += chunk.x * 16;
    y += chunk.y * 16;

    gl_Position = vec4(float(x), float(y), 0, 1);
    
    vs_out.id = id;
}