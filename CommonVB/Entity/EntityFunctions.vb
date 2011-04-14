Public Class EntityFunctions
    Inherits SpelObject

    Public Sub New(ByVal nParent As Entity)
        MyBase.New(nParent)

        Me.mPositie = Vector3.Empty
        Me.mSnelheid = Vector3.Empty
        Me.mRotatieMatrix = Matrix.Identity
        Me.mRotatieSnelheid = Vector3.Empty
        Me.mScale = New Vector3(1, 1, 1)
    End Sub


    Public Overrides Sub OnParentChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        MyBase.OnParentChanged(sender, e)
        Me.setParentEntity(CType(Me.Parent, Entity))
    End Sub

    Private mParentEntity As Entity
    Public ReadOnly Property ParentEntity() As Entity
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mParentEntity
        End Get
    End Property
    Private Sub setParentEntity(ByVal value As Entity)
        Me.mParentEntity = value
    End Sub




    Private mVersie As Integer
    Public Overridable ReadOnly Property Versie() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mVersie
        End Get
    End Property
    Public Overridable Sub setVersie(ByVal value As Integer)
        Me.mVersie = value
    End Sub





    Private mActor As NxActor
    Public ReadOnly Property Actor() As NxActor
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mActor
        End Get
    End Property
    Public Sub setActor(ByVal value As NxActor)
        Me.mActor = value
    End Sub

    Protected mPositie As Vector3
    Public Overridable Property Positie() As Vector3
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return mPositie
        End Get
        Set(ByVal value As Vector3)
            If Me.mPositie <> value Then
                mPositie = value
                Me.OnPositieVeranderd(Me, New PositieVeranderdEventArgs())
            End If
        End Set
    End Property

    Protected mSnelheid As Vector3
    Public Overridable Property Snelheid() As Vector3
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mSnelheid
        End Get
        Set(ByVal value As Vector3)
            If Me.mSnelheid <> value Then
                Me.mSnelheid = value
                Me.OnSnelheidVeranderd(Me, New SnelheidVeranderdEventArgs)
            End If
        End Set
    End Property



    Protected mRotatieMatrix As Matrix
    Public Overridable Property RotatieMatrix() As Matrix
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mRotatieMatrix
        End Get
        Set(ByVal value As Matrix)
            If Me.mRotatieMatrix <> value Then
                Me.mRotatieMatrix = value
                Me.mRotatieQuaternion = Quaternion.RotationMatrix(Me.RotatieMatrix)
                Me.OnRotatieVeranderd(Me, New RotatieVeranderdEventArgs())
            End If
        End Set
    End Property


    Protected mRotatieQuaternion As Quaternion
    Public Property RotatieQuaternion() As Quaternion
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mRotatieQuaternion
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Quaternion)
            If Me.mRotatieQuaternion <> value Then
                Me.mRotatieQuaternion = value
                Me.mRotatieMatrix = Matrix.RotationQuaternion(Me.RotatieQuaternion)
                Me.OnRotatieVeranderd(Me, New RotatieVeranderdEventArgs())
            End If
        End Set
    End Property


    Protected mRotatieSnelheid As Vector3
    Public Overridable Property RotatieSnelheid() As Vector3
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mRotatieSnelheid
        End Get
        Set(ByVal value As Vector3)
            If Me.mRotatieSnelheid <> value Then
                Me.mRotatieSnelheid = value
                Me.OnRotatieSnelheidVeranderd(Me, New RotatieSnelheidVeranderdEventArgs())
            End If
        End Set
    End Property




    Protected mScale As Vector3
    ''' <summary>
    ''' TODO
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Property Scale() As Vector3
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mScale
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Vector3)
            Me.mScale = value
            Me.OnScaleVeranderd(Me, New ScaleVeranderdEventArgs)
        End Set
    End Property



    Public Overridable Sub OnPositieVeranderd(ByVal sender As Object, ByVal e As PositieVeranderdEventArgs)
        If Me.Actor IsNot Nothing Then Me.Actor.GlobalPosition = Me.Positie

        Me.ParentEntity.OnPositieVeranderd(sender, e)

    End Sub

    Public Overridable Sub OnSnelheidVeranderd(ByVal sender As Object, ByVal e As SnelheidVeranderdEventArgs)
        If Me.Actor IsNot Nothing Then Me.Actor.LinearVelocity = Me.Snelheid
        Me.ParentEntity.OnSnelheidVeranderd(sender, e)
    End Sub

    Public Overridable Sub OnRotatieVeranderd(ByVal sender As Object, ByVal e As RotatieVeranderdEventArgs)
        If Me.Actor IsNot Nothing Then Me.Actor.GlobalOrientation = Me.RotatieMatrix
        Me.ParentEntity.OnRotatieVeranderd(sender, e)
    End Sub

    Public Overridable Sub OnRotatieSnelheidVeranderd(ByVal sender As Object, ByVal e As RotatieSnelheidVeranderdEventArgs)
        If Me.Actor IsNot Nothing Then Me.Actor.AngularVelocity = Me.RotatieSnelheid
        Me.ParentEntity.OnRotatieSnelheidVeranderd(sender, e)
    End Sub

    Public Overridable Sub OnScaleVeranderd(ByVal sender As Object, ByVal e As ScaleVeranderdEventArgs)
        Me.ParentEntity.OnScaleVeranderd(sender, e)
    End Sub



    Public Overridable Sub GetData(ByVal BW As ByteWriter)

    End Sub

    Public Overridable Sub Update(ByVal BR As ByteReader)


    End Sub

    Public Overridable Sub ReadSerializedData(ByVal nDS As DataSerializer)

    End Sub

    Public Overridable Sub WriteSerializeData(ByVal nDS As DataSerializer)

    End Sub

End Class

