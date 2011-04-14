using System;
using MHGameWork.TheWizards.Graphics;
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