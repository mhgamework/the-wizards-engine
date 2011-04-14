Namespace TerrainPreprocessor

    Public Enum CornerDir As Byte
        NorthEast = 0
        NorthWest = 1
        SouthWest = 2
        SouthEast = 3

    End Enum

    Public Enum ChildDir As Byte
        NorthEast = 0
        NorthWest = 1
        SouthWest = 2
        SouthEast = 3


    End Enum

    Public Enum VertexDir As Byte
        Center = 0
        East = 1
        North = 2
        West = 3
        South = 4
        NorthEast = 5
        NorthWest = 6
        SouthWest = 7
        SouthEast = 8

    End Enum

    Public Enum HeightDir As Byte
        Center = 0
        East = 1
        North = 2
        West = 3
        South = 4

    End Enum

    Public Enum ErrorType As Byte
        East = 0
        South = 1
        ChildNorthEast = 2
        ChildNorthWest = 3
        ChildSouthWest = 4
        ChildSouthEast = 5

    End Enum

End Namespace