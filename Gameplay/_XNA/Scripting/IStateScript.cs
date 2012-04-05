using System;

namespace MHGameWork.TheWizards._XNA.Scripting
{
    [Obsolete("Currently obsolete, something similar could be reimplemented later")]
    public interface IStateScript
    {

        void Init();
        void Destroy();

        void Update();
        void Draw();

    }
}
