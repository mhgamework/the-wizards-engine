Public Interface IBlock
    ReadOnly Property Pointer() As BlockPointer
    ReadOnly Property Size() As Short
    Function ToBytes() As Byte()


End Interface


