using System;
using System.Collections.Generic;
using System.Text;
using NovodexWrapper;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Server;
using Wereld = MHGameWork.TheWizards.Server.Wereld;

namespace MHGameWork.TheWizards.ServerClient.Entities
{
    public class TestPlayer : Wereld.IServerClientEntity
    {
        private Wereld.ServerClientBody body;
        private Engine.Model model;

        private NxCapsuleController controller;
        private TestPlayerHitReport hitReport;

        public Vector3 cameraAngles;
        private float jumpTime;

        //private XNAGeoMipMap.TerrainBlock activeBlock;
        public XNAGeoMipMap.Terrain activeTerrain;




        public TestPlayer()
        {
            body = new MHGameWork.TheWizards.ServerClient.Wereld.ServerClientBody();



            model = new Engine.Model(ProgramOud.SvClMain, "Content\\Models\\p1_wedge");


            body.Positie = new Vector3(0, 5, 0);
            body.Scale = new Vector3(10);

            /*NxActorDesc actorDesc = new NxActorDesc();
            NxBodyDesc bodyDesc = new NxBodyDesc();
            NxSphereShapeDesc sphereDesc = new NxSphereShapeDesc();


            sphereDesc.radius = 1f / 2f;
            actorDesc.addShapeDesc( sphereDesc );


            actorDesc.BodyDesc = bodyDesc;
            actorDesc.density = 10;
            actorDesc.globalPose = Matrix.CreateTranslation( new Vector3( 0, 100, 0 ) );

            NxActor actor;
            actor = ServerMainNew.Instance.PhysicsScene.createActor( actorDesc );
            while ( actor == null )
            {
                actor = ServerMainNew.Instance.PhysicsScene.createActor( actorDesc );

            }

            body.Actor = actor;*/









            hitReport = new TestPlayerHitReport();

            NxCapsuleControllerDesc controllerDesc = NxCapsuleControllerDesc.Default;
            controllerDesc.position = body.Positie;
            controllerDesc.upDirection = NxHeightFieldAxis.NX_Y;
            controllerDesc.radius = 2 * 10;
            controllerDesc.height = 1 * 10;
            controllerDesc.stepOffset = 1 * 10;
            controllerDesc.Callback = hitReport;
            controllerDesc.slopeLimit = 0;
            controller = (NxCapsuleController)ControllerManager.createController(ServerMainNew.Instance.PhysicsScene, controllerDesc);
            /*NxCapsuleShapeDesc shapeDesc = NxCapsuleShapeDesc.Default;
            shapeDesc.height = 2.1f * 10;
            shapeDesc.radius = 2 * 10;
            shapeDesc.localPose = Matrix.CreateTranslation( 0, 0, -5 );*/



            /*'If (Not controller Is Nothing) Then
            '    controller.getActor.FlagFrozenRot = True
            '    controller.getActor.setName("CapsuleController")
            '    'If addArrowShapeFlag Then
            '    'controller.getActor.createShape(Me.createSpikeShapeDesc((capsuleRadius / 4.0!), (capsuleRadius / 4.0!), (capsuleRadius * 2.0!), New Vector3(0.0!, ((capsuleTotalHeight - (capsuleRadius * 2.0!)) / 2.0!), 0.0!)))
            '    controller.getActor.createShape(Me.createSpikeShapeDesc(2 / 4.0!, 2 / 4.0!, 2 / 4.0!, New Vector3(0.0!, 2 / 0.2!, 0.0!)))
            '    controller.getActor.getLastShape.FlagDisableCollision = True
            '    'End If
            'End If*/




            body.serverBody.Actor = controller.getActor();
            //body.Actor.createShape( shapeDesc );


            /*'Dim actorDesc As New NxActorDesc
            'Dim bodyDesc As New NxBodyDesc
            'Dim sphereDesc As New NxSphereShapeDesc
            'sphereDesc.radius = 1
            'actorDesc.addShapeDesc(sphereDesc)

            ''Dim boxDesc As New NxBoxShapeDesc
            ''boxDesc.dimensions = New Vector3(1, 1, 1)
            ''actorDesc.addShapeDesc(boxDesc)

            'actorDesc.BodyDesc = bodyDesc
            'actorDesc.density = 10
            'actorDesc.globalPose = Matrix.Translation(Me.functions.Positie)
            'Me.functions.setActor(Me.BaseMain.PhysicsScene.createActor(actorDesc))*/









        }

        #region IClientEntity Members



        void MHGameWork.TheWizards.ServerClient.Wereld.IClientEntity.Process(MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e)
        {

            //Game3DPlay.Core.Elements.ProcessEventArgs nE = ServerClientMain.instance.ProcessEventArgs;
            Game3DPlay.Core.Elements.ProcessEventArgs nE = e;
            if (nE.Keyboard.IsKeyStateDown(Microsoft.Xna.Framework.Input.Keys.G))
            {
                Shuriken002 shur = new Shuriken002();

                Wereld.ClientEntityHolder nClientEntH;
                Server.Wereld.ServerEntityHolder nServerEntH;
                Shuriken002.CreateShuriken002Entity(shur, out nClientEntH, out nServerEntH);

                ServerClientMainOud.instance.Wereld.AddEntity(nClientEntH);
                ServerClientMainOud.instance.ServerMain.Wereld.AddEntity(nServerEntH);

                shur.Body.Positie = body.Positie + new Vector3(0, 10 * 10, 0);
            }


            if (nE.Keyboard.IsKeyStateDown(Microsoft.Xna.Framework.Input.Keys.RightControl))
            {
                jumpTime = 1000;
            }
            if (nE.Keyboard.IsKeyStateUp(Microsoft.Xna.Framework.Input.Keys.RightControl))
            {
                jumpTime = 0;
            }
            if (nE.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                cameraAngles.Y += 1 * e.Elapsed;
            }
            if (nE.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                cameraAngles.Y -= 1 * e.Elapsed;
            }
        }

        void MHGameWork.TheWizards.ServerClient.Wereld.IClientEntity.Tick(MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e)
        {

            Game3DPlay.Core.Elements.ProcessEventArgs nE = ServerClientMainOud.instance.ProcessEventArgs;





            Vector3 totalDisp = new Vector3();  //Total Displacement
            Vector3 localMov = new Vector3();  //Local Movement
            Vector3 worldMov = new Vector3();   //World Movement
            //If Me.LastPlayerCommand.NoClip Then
            if (false)
            {

                //    /*With Me.LastPlayerCommand
                //        If .Vooruit Then
                //            LocalMov += New Vector3(0, 0, 1)
                //        End If
                //        If .Achteruit Then
                //            LocalMov += New Vector3(0, 0, -1)
                //        End If
                //        If .StrafeLinks Then
                //            LocalMov += New Vector3(-1, 0, 0)
                //        End If
                //        If .StrafeRechts Then
                //            LocalMov += New Vector3(1, 0, 0)
                //        End If
                //        If .Jump Then
                //            WorldMov += New Vector3(0, 1, 0)
                //        End If
                //        If .Crouch Then
                //            WorldMov += New Vector3(0, -1, 0)
                //        End If

                //    End With*/



                //    /*If Me.LastPlayerCommand.PrimaryAttack Then
                //        WorldMov *= 100
                //    End If
                //    If Me.LastPlayerCommand.Run Then
                //        WorldMov *= 5
                //    End If*/





                //if (nE.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                //{
                //    localMov += new Vector3(0, 0, 1);
                //}
                //if (nE.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                //{
                //    localMov += new Vector3(0, 0, -1);
                //}
                //if (nE.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift))
                //{
                //    localMov += new Vector3(0, 1, 0);
                //}
                //if (nE.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl))
                //{
                //    localMov += new Vector3(0, -1, 0);
                //}



                //worldMov += Vector3.Transform(localMov, Matrix.CreateFromYawPitchRoll(cameraAngles.Y, -cameraAngles.X, cameraAngles.Z));
                //if (worldMov.Length() != 0) worldMov.Normalize();

                ///*If Me.LastPlayerCommand.Run Then
                //    WorldMov *= 5
                //End If*/



                //totalDisp += worldMov * 10;//CType(Me.HoofdObj, BaseMain).ClientSpeed




                //if (body.serverBody.Actor.getGlobalPosition().Y - 1.5f + totalDisp.Y < 0)
                //{ totalDisp.Y = -(body.serverBody.Actor.getGlobalPosition().Y - 1.5f); }











            }
            else
            {

                /*With Me.LastPlayerCommand
                    If .Vooruit Then
                        LocalMov += New Vector3(0, 0, 1)
                    End If
                    If .Achteruit Then
                        LocalMov += New Vector3(0, 0, -1)
                    End If
                    If .StrafeLinks Then
                        LocalMov += New Vector3(-1, 0, 0)
                    End If
                    If .StrafeRechts Then
                        LocalMov += New Vector3(1, 0, 0)
                    End If
                    If .Jump Then
                        If Me.JumpTime = 0 Then Me.JumpTime = 1000
                    Else
                        Me.JumpTime = 0
                    End If
                    If .Crouch Then
                        Me.LinkedPlayerEntity.Controller.Height = 0.2
                    Else
                        Me.LinkedPlayerEntity.Controller.Height = 2
        '    WorldMov += New Vector3(0, -1, 0)
                    End If

                End With*/
                //LocalDisp.Normalize()

                if (nE.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                {
                    localMov += new Vector3(0, 0, 1);
                }
                if (nE.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                {
                    localMov += new Vector3(0, 0, -1);
                }
                if (nE.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                {
                    cameraAngles.Y += 1 * e.Elapsed;
                }
                if (nE.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                {
                    cameraAngles.Y -= 1 * e.Elapsed;
                }


                worldMov += Vector3.Transform(localMov, Matrix.CreateFromYawPitchRoll(cameraAngles.Y, -cameraAngles.X, cameraAngles.Z));
                if (worldMov.Length() != 0) worldMov.Normalize();

                /*If Me.LastPlayerCommand.Run Then
                    WorldMov *= 5
                End If*/

                if (jumpTime > 0)
                {
                    jumpTime -= e.Elapsed * 1000f;
                    worldMov += new Vector3(0, 1, 0);
                }


                totalDisp += worldMov * 10;//CType(Me.HoofdObj, BaseMain).ClientSpeed


                if (jumpTime <= 0)
                {
                    jumpTime = 0;
                    totalDisp += new Vector3(0, -9.81f, 0); //CType(Me.HoofdObj, BaseMain).ClientGravity
                }



                if (body.serverBody.Actor.getGlobalPosition().Y - 1.5f + totalDisp.Y < 0)
                { totalDisp.Y = -(body.serverBody.Actor.getGlobalPosition().Y - 1.5f); }




            }

            uint collisionFlags;

            totalDisp *= 10f;

            controller.move(totalDisp * e.Elapsed, uint.MaxValue, 0.001f, out collisionFlags, 1.0f);

            body.serverBody.Actor.GlobalOrientation = Matrix.CreateFromYawPitchRoll(cameraAngles.Y, -cameraAngles.X, cameraAngles.Z);








        }


        #endregion

        #region IServerEntity Members

        void MHGameWork.TheWizards.Server.Wereld.IServerEntity.Process(MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e)
        {

        }

        void MHGameWork.TheWizards.Server.Wereld.IServerEntity.Tick(MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e)
        {
            //if (activeTerrain.Trunk == null) return;

            //XNAGeoMipMap.TerrainBlock block = FindContainingBlock(activeTerrain.Trunk);

            //if (block != activeBlock)
            //{
            //    if (activeBlock != null)
            //    {
            //        //if ( tempHeightFieldActor != null ) tempHeightFieldActor.destroy();
            //        //tempHeightFieldActor = null;
            //        activeBlock.UnloadHeightField();
            //        activeBlock = null;
            //    }

            //    activeBlock = block;

            //    if (activeBlock != null)
            //    {
            //        //tempHeightFieldActor = CreateHeightField( activeTerrain, activeBlock );
            //        activeBlock.LoadHeightField();
            //    }

            //}
        }

        //XNAGeoMipMap.TerrainBlock FindContainingBlock(XNAGeoMipMap.TreeNode node)
        //{
        //    if (!node.BoundingBox.Intersects(BoundingSphere)) return null;

        //    XNAGeoMipMap.TerrainBlock block = null;

        //    Type type = node.GetType();

        //    if (type == typeof(XNAGeoMipMap.QuadTreeNode))
        //    {
        //        XNAGeoMipMap.QuadTreeNode tree = (XNAGeoMipMap.QuadTreeNode)node;

        //        if (block == null && tree.UpperLeft != null)
        //            //totalTriangles += DrawTreeNode( frustum, tree.UpperLeft );
        //            block = FindContainingBlock(tree.UpperLeft);

        //        if (block == null && tree.UpperRight != null)
        //            block = FindContainingBlock(tree.UpperRight);

        //        if (block == null && tree.LowerLeft != null)
        //            block = FindContainingBlock(tree.LowerLeft);

        //        if (block == null && tree.LowerRight != null)
        //            block = FindContainingBlock(tree.LowerRight);
        //    }
        //    else if (type == typeof(XNAGeoMipMap.TerrainBlock))
        //    {
        //        block = (XNAGeoMipMap.TerrainBlock)node;

        //    }

        //    return block;
        //}


        #endregion


        public void Render()
        {
            model.TempRender(Matrix.CreateScale(body.Scale)
                * Matrix.CreateScale(2)
                * body.Rotatie
                * Matrix.CreateRotationY(MathHelper.Pi)
                * Matrix.CreateTranslation(0, 0, -5)
                * Matrix.CreateTranslation(body.Positie));
        }


        public Microsoft.Xna.Framework.BoundingSphere BoundingSphere
        {
            get { return new Microsoft.Xna.Framework.BoundingSphere(body.Positie, 1 * 2 * 10); }
        }

        public Wereld.ServerClientBody Body { get { return body; } }















        /*public static Server.Wereld.ServerEntityHolder CreateTestPlayerEntity(Entities.TestPlayer nPL)
        {
            Server.Wereld.ServerEntityHolder entH;
            entH = new Server.Wereld.ServerEntityHolder( nPL );

            entH.AddElement( nPL.body );

            return entH;

        }*/


        public static void CreateTestPlayerEntity(Entities.TestPlayer nEnt, out Wereld.ClientEntityHolder nClientEntH, out Server.Wereld.ServerEntityHolder nServerEntH)
        {
            nClientEntH = new Wereld.ClientEntityHolder(nEnt);
            nServerEntH = new Server.Wereld.ServerEntityHolder(nEnt);

            nClientEntH.AddElement(nEnt.body);
            nServerEntH.AddElement(nEnt.body);

        }











        #region IClientEntity Members

        void MHGameWork.TheWizards.ServerClient.Wereld.IClientEntity.Render()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        BoundingSphere MHGameWork.TheWizards.ServerClient.Wereld.IClientEntity.BoundingSphere
        {
            get { throw new Exception( "The method or operation is not implemented." ); }
        }

        #endregion

        #region IServerEntity Members


        BoundingSphere MHGameWork.TheWizards.Server.Wereld.IServerEntity.BoundingSphere
        {
            get { throw new Exception( "The method or operation is not implemented." ); }
        }

        void MHGameWork.TheWizards.Server.Wereld.IServerEntity.EnablePhysics()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        void MHGameWork.TheWizards.Server.Wereld.IServerEntity.DisablePhysics()
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion
    }
}
