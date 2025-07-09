using Hexa.NET.ImGui;
using Substrate.Scene3D;
using Substrate.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Substrate.Test
{
    public class SceneView : WindowBase
    {
        public Scene3dWidget Scene;
        public MeshActor MeshTestActor;
        public MeshActor MeshModelTestActor;
        public MeshActor MeshModelTestActor2;

        public SceneView()
        {
            Scene = new Scene3dWidget();
            Scene.WorldScene.DebugShapes.AddCube(new System.Numerics.Vector3(0, 0, 0), new System.Numerics.Vector3(10, 10, 10));

            MeshTestActor = Scene.WorldScene.CreateActor<MeshActor>();
            MeshTestActor.Mesh.SetModel(SimpleModel.CreateFromCube());

            
            MeshModelTestActor = Scene.WorldScene.CreateActor<MeshActor>();
            MeshModelTestActor.Mesh.SetModel(SimpleModel.CreateFromObj("D:\\TestModels\\Neon\\neon.obj"));
            MeshModelTestActor.Transform.Position = new System.Numerics.Vector3(2, 0, 0);

            MeshModelTestActor2 = Scene.WorldScene.CreateActor<MeshActor>();
            MeshModelTestActor2.Mesh.SetModel(SimpleModel.CreateFromObj("D:\\TestModels\\Evelynn\\Evelynn.obj"));
            MeshModelTestActor2.Transform.Position = new System.Numerics.Vector3(5, 0, 0);
            MeshModelTestActor2.Transform.Scale = new System.Numerics.Vector3(0.01f);
        }

        public override void Draw(float dt)
        {
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, 0);
            if (ImGui.Begin("Scene View"))
            {
                var size = ImGui.GetContentRegionAvail();
                Scene.Draw(size);
            }
            ImGui.End();
            //ImGui.PopStyleVar();
        }
    }
}
