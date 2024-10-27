#version 450

layout(location = 0) in vec2 Uvs;
layout(location = 1) in vec3 Norm;
layout(location = 2) in vec3 FragPos;

layout(set = 1, binding = 0) uniform WorldBuffer
{
    mat4 World;
    uint SelectionId;
    uint Flags;
};

layout(location = 2) out vec4 selectedMask;

void main() {
    selectedMask.r = 1;
}