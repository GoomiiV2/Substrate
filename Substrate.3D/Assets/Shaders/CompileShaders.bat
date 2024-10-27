glslangvalidator -V 3D/DebugShapes/DebugShapesVert.glsl -o 3D/DebugShapes/DebugShapesVert.spirv -S vert
glslangvalidator -V 3D/DebugShapes/DebugShapesFrag.glsl -o 3D/DebugShapes/DebugShapesFrag.spirv -S frag
glslangvalidator -V 3D/DebugShapes/DebugShapesGeo.glsl -o 3D/DebugShapes/DebugShapesGeo.spirv -S geom

glslangvalidator -V 3D/Grid/GridVert.glsl -o 3D/Grid/GridVert.spirv -S vert
glslangvalidator -V 3D/Grid/GridFrag.glsl -o 3D/Grid/GridFrag.spirv -S frag

glslangvalidator -V 3D/Fullscreen/OutlineVert.glsl -o 3D/Fullscreen/OutlineVert.spirv -S vert
glslangvalidator -V 3D/Fullscreen/OutlineFrag.glsl -o 3D/Fullscreen/OutlineFrag.spirv -S frag

glslangvalidator -V 3D/Mesh/MeshVert.glsl -o 3D/Mesh/MeshVert.spirv -S vert
glslangvalidator -V 3D/Mesh/MeshFrag.glsl -o 3D/Mesh/MeshFrag.spirv -S frag
glslangvalidator -V 3D/Mesh/MeshSolidFrag.glsl -o 3D/Mesh/MeshSolidFrag.spirv -S frag