using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.GodGame
{
    public interface IPlayerInputHandler
    {
        void OnSave();
        void OnRightClick(GameVoxel target);
        void OnLeftClick(GameVoxel target);
        void OnNextTool();
        void OnPreviousTool();
        void OnKeyPressed(GameVoxel target, Key key);
        void OnTargetChanged(GameVoxel target);
    }
}