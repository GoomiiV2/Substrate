#version 450

layout (lines) in;                              // now we can access 2 vertices
layout (triangle_strip, max_vertices = 4) out;  // always (for now) producing 2 triangles (so 4 vertices)

layout(location = 0) in vec4 Color[];
layout(location = 1) in float Thickness[];

layout (location = 0) out vec4 outColor;

void main()
{

}