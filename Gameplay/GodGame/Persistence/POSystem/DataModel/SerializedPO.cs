using System;

namespace MHGameWork.TheWizards.GodGame.Persistence.POSystem
{
    /// <summary>
    /// Represents a serialized form of a PO, aka a reference object
    /// </summary>
    public class SerializedPO
    {
        public POIdentifier Identifier; // Use name or smth?
        public SerializedType Type;
        public SerializedAttribute[] Attributes;

        public override string ToString()
        {
            return String.Format("Identifier: {0}, Type: {1}, Attributes: {2}", Identifier, Type, Attributes);
        }
    }
}