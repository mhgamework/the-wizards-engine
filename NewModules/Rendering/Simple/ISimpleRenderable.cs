using System;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Culling;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.Rendering
{
    public interface ISimpleRenderable :ICullable
    {
        void Initialize(IXNAGame game);
        void Update();
        void Render();
    }
}