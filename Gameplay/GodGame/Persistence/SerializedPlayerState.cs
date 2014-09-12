using System;

namespace MHGameWork.TheWizards.GodGame.Persistence
{
    /// <summary>
    /// Configuration of how the playerstate has to be serialized
    /// </summary>
    [Serializable]
    public class SerializedPlayerState
    {
        public string Name;
        public string ActiveToolName;
        public int HeightToolSize;
        public ChangeHeightToolPerPlayer.HeightToolState HeightToolState;

        public SerializedPlayerState()
        {
        }

        public void Set(PlayerState state, GameplayObjectsSerializer objectSerializer)
        {
            Name = state.Name;
            ActiveToolName = objectSerializer.Serialize(state.ActiveTool);
            HeightToolSize = (state.HeightToolSize);
            HeightToolState = (state.HeightToolState);
        }
        public void Apply(PlayerState state, GameplayObjectsSerializer objectSerializer)
        {
            if (state.Name != Name) throw new InvalidOperationException("Deserializing on wrong or changed name player!");
            state.ActiveTool = objectSerializer.GetPlayerTool(ActiveToolName);
            state.HeightToolSize = HeightToolSize;
            state.HeightToolState = HeightToolState;
        }
    }
}