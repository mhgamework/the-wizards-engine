Public MustInherit Class ModelManager
    Inherits SpelObject

    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)

    End Sub



    Public MustOverride ReadOnly Property ModelData(ByVal nModelID As Integer) As ModelData


    Protected MustOverride ReadOnly Property StaticEntityFunctionsEnumerable() As IEnumerable


    Public MustOverride Sub AddStaticEntityFunctions(ByVal nSFunc As StaEntFunctions)

    Public MustOverride Sub RemoveStaticEntityFunctions(ByVal nSFunc As StaEntFunctions)


    Public Sub ReloadStaticEntities()

        For Each S As StaEntFunctions In Me.StaticEntityFunctionsEnumerable
            S.ReloadModel()
        Next
    End Sub

End Class
