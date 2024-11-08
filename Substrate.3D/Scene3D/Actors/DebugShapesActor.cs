﻿using Substrate.Scene3D.Components;
using System.Numerics;
using Veldrid.Utilities;
using static Substrate.Scene3D.Components.DebugShapesComp;

namespace Substrate.Scene3D
{
    public class DebugShapesActor : Actor
    {
        public DebugShapesComp DebugShapes;

        public override void Init(World world)
        {
            base.Init(world);

            DebugShapes = AddComponet<DebugShapesComp>();
            BoundingBox = new Veldrid.Utilities.BoundingBox(Vector3.One * float.MinValue, Vector3.One * float.MaxValue);
        }

        public void Recreate()
        {
            DebugShapes.RecreateBuffers();
        }

        public Cube AddCube(Vector3 pos, Vector3? size = null, Vector4? color = null, float thickness = 2f) => DebugShapes.AddCube(pos, size, color, thickness);
        public Line AddLine(Vector3 start, Vector3 end, Vector4? color = null, float thickness = 2f) => DebugShapes.AddLine(start, end, color, thickness);
        public Fustrum AddFustrum(BoundingFrustum fustrum, Vector3? size = null, Vector4? color = null, float thickness = 2f) => DebugShapes.AddFustrum(fustrum, size, color, thickness);
        public DebugShapesComp.Rect AddRect(Vector3 pos, Vector2 size, Vector4? color = null, float thickness = 2f) => DebugShapes.AddRect(pos, size, color, thickness);
        public Cricle AddCricle(Vector3 pos, float radius = 1f, ushort sides = 16, Vector4? color = null, float thickness = 2f) => DebugShapes.AddCricle(pos, radius, sides, color, thickness);
        public Cylnder AddCylnder(Vector3 pos, float radius = 1f, float height = 2f, ushort sides = 16, Vector4? color = null, float thickness = 2f) => DebugShapes.AddCylnder(pos, radius, height, sides, color, thickness);
        public Sphere AddSphere(Vector3 pos, float radius = 1f, ushort sides = 16, Vector4? color = null, float thickness = 2f) => DebugShapes.AddSphere(pos, radius, sides, color, thickness);
        public Arrow AddArrow(Vector3 pos, Vector3 dir, ushort sides = 16, Vector4? color = null, float thickness = 2f) => DebugShapes.AddArrow(pos, dir, sides, color, thickness);
    }
}
