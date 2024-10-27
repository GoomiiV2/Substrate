using Substrate.Scene3D.Components;
using Veldrid;

namespace Substrate.Scene3D
{
    public class MeshActor : Actor
    {
        public MeshComponent Mesh;

        public override void Init(World world)
        {
            base.Init(world);

            Mesh = AddComponet<MeshComponent>();
        }

        public void LoadFromObj(string objPath)
        {
            var model = SimpleModel.CreateFromObj(objPath);
            Mesh.SetModel(model);

            return;
        }

        public override unsafe void Render(CommandList cmdList)
        {
            base.Render(cmdList);
        }
    }
}
