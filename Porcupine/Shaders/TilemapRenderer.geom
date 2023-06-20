#version 410 core

uniform mat4 projection;
uniform gs_rendering {
    uint[] textures;
};

in VS_OUT {
    uint id;
} gs_in[];

out vec2 uv;

layout (points) in;
layout (triangle_strip, max_vertices = 4) out;

void main() {
    uint id = gs_in[0].id;
    vec4 position = gl_in[0].gl_Position;

    // Spritesheet is 16x16 tiles of 16x16 pixels for a total of 256x256 pixels
    //TODO: Implement indirect IDs
    float u = float(id & 15u) / 16.0;
    float v = float((id >> 4u) & 15u) / 16.0;

    // Prepare paddings for UVs
    const float B = 1 / 256.0;
    const float S = 1 / 16.0;

    gl_Position = projection * position;
    uv = vec2(u + B, v + B);
    EmitVertex();

    gl_Position = projection * (position + vec4(1.0, 0.0, 0.0, 0.0));
    uv = vec2(u + S - B, v + B);
    EmitVertex();

    gl_Position = projection * (position + vec4(0.0, 1.0, 0.0, 0.0));
    uv = vec2(u + B, v + S - B);
    EmitVertex();

    gl_Position = projection * (position + vec4(1.0, 1.0, 0.0, 0.0));
    uv = vec2(u + S - B, v + S - B);
    EmitVertex();

    EndPrimitive();
}  