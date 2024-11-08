﻿using Veldrid;
using Veldrid.Utilities;

namespace Substrate.Scene3D
{
    public class Component
    {
        public string Name;
        public Actor Owner;
        public ActorFlags Flags;
        public Transform Transform;
        public BoundingBox BoundingBox;

        public virtual void Init(Actor owner)
        {
            Transform = new Transform();
            Transform.OnChange = OnTransformChanged;
            Owner = owner;
        }

        public virtual void OnTransformChanged()
        {
            Owner.OnTransformChanged(false);
        }

        public virtual void Update(double dt)
        {

        }

        public virtual void Render(CommandList cmdList)
        {

        }

        public virtual void DrawInspector()
        {

        }
    }
}
