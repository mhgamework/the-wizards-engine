using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Wereld
{
    /// <summary>
    /// Dit is de Client wereld.
    /// CMHGW001
    /// </summary>
    public class WereldOud
    {
        private ServerClientMainOud engine;
        private QuadTree tree;
        private int treeSize = (int)Math.Pow( 2, 11 );
        private int treeExtendUp = 4000;
        private int treeExtendDown = 4000;
        private List<ClientEntityHolder> entities;
        private int nextEntityID = 100;

        private Engine.TerrainManager terrainManager;

        public Engine.TerrainManager TerrainManager
        {
            get { return terrainManager; }
            set { terrainManager = value; }
        }
        //private List<XNAGeoMipMap.Terrain> terrains = new List<MHGameWork.TheWizards.ServerClient.XNAGeoMipMap.Terrain>();

        public WereldOud( ServerClientMainOud nMain )
        {
            engine = nMain;
            int half = treeSize / 2;

            tree = new QuadTree( new BoundingBox( new Vector3( -half, -treeExtendDown, -half ), new Vector3( half, treeExtendUp, half ) ) );

            entities = new List<ClientEntityHolder>();

            terrainManager = new MHGameWork.TheWizards.ServerClient.Engine.TerrainManager( engine );

        }


        public void AddEntity( ClientEntityHolder nEntH )
        {
            nEntH.SetID( nextEntityID );
            nextEntityID++;

            entities.Add( nEntH );
            tree.OrdenEntity( nEntH );

            OnEntityAdded( nEntH );

        }

        public void AddEntity( ClientEntityHolder nClientEntH, Server.Wereld.ServerEntityHolder nServerEntH )
        {
            /*entities.Add( nClientEntH );
            tree.OrdenEntity( nClientEntH );*/
            AddEntity( nClientEntH );

            engine.ServerMain.Wereld.AddEntity( nServerEntH );


        }

        //public void AddTerrain( XNAGeoMipMap.Terrain terr )
        //{
        //    terrains.Add( terr );
        //}


        public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            for ( int i = 0; i < terrainManager.Terrains.Count; i++ )
            {
                terrainManager.Terrains[ i ].Frustum = engine.ActiveCamera.CameraInfo.Frustum;
                terrainManager.Terrains[ i ].CameraPostion = engine.ActiveCamera.CameraPosition;
                terrainManager.Terrains[ i ].Update();
                System.IO.FileInfo f = new System.IO.FileInfo( "f" );
                
                
            }
            for ( int i = 0; i < entities.Count; i++ )
            {
                entities[ i ].Process( e );
            }

            OptimizeNodes();

            AddLoadTasks();
            
        }

        public void AddLoadTasks()
        {
            AddLoadTasks( Engine.LoadingTaskType.PreProccesing, new BoundingSphere( engine.GameClient.PlayerEntityHolder.Body.Positie, 2000f ), tree, false );
            AddLoadTasks( Engine.LoadingTaskType.Normal, new BoundingSphere( engine.GameClient.PlayerEntityHolder.Body.Positie, 1900f ), tree, false );
            AddLoadTasks( Engine.LoadingTaskType.Detail, new BoundingSphere( engine.GameClient.PlayerEntityHolder.Body.Positie, 100f ), tree, false );
            AddUnLoadTasks( Engine.LoadingTaskType.Unload, new BoundingSphere( engine.GameClient.PlayerEntityHolder.Body.Positie, 2000f ), tree, false );
        }
        public void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {
            for ( int i = 0; i < entities.Count; i++ )
            {
                entities[ i ].Tick( e );
            }
        }

        public void Render()
        {
            UpdateNodeVisible( engine.ActiveCamera.CameraInfo.Frustum, tree, false );

            RenderEntities();
            RenderTerrains();


        }

        public void AddLoadTasks( Engine.LoadingTaskType taskType, BoundingSphere sphere, QuadTreeNode node, bool skipFrustumCheck )
        {

            if ( skipFrustumCheck != true )
            {

                ContainmentType containment = sphere.Contains( node.BoundingBox );

                if ( containment == ContainmentType.Disjoint )
                    return;

                // if the entire node is contained within, then assume all children are as well
                if ( containment == ContainmentType.Contains )
                    skipFrustumCheck = true;
            }


            if ( node.TerrainBlock != null ) node.TerrainBlock.AddLoadTasks( engine.LoadingManager, taskType );

            if ( node.UpperLeft != null )
                AddLoadTasks( taskType, sphere, node.UpperLeft, skipFrustumCheck );

            if ( node.UpperRight != null )
                AddLoadTasks( taskType, sphere, node.UpperRight, skipFrustumCheck );

            if ( node.LowerLeft != null )
                AddLoadTasks( taskType, sphere, node.LowerLeft, skipFrustumCheck );

            if ( node.LowerRight != null )
                AddLoadTasks( taskType, sphere, node.LowerRight, skipFrustumCheck );


        }

        public void AddUnLoadTasks( Engine.LoadingTaskType taskType, BoundingSphere sphere, QuadTreeNode node, bool skipFrustumCheck )
        {
            ContainmentType containment = ContainmentType.Disjoint;
            if ( skipFrustumCheck != true )
            {

                containment = sphere.Contains( node.BoundingBox );

                if ( containment == ContainmentType.Contains )
                    return;

                // if the entire node is contained within, then assume all children are as well
                if ( containment == ContainmentType.Disjoint )
                    skipFrustumCheck = true;
            }

            if ( containment == ContainmentType.Disjoint )
            {
                if ( node.TerrainBlock != null ) node.TerrainBlock.AddLoadTasks( engine.LoadingManager, taskType );
            }

            if ( node.UpperLeft != null )
                AddUnLoadTasks( taskType, sphere, node.UpperLeft, skipFrustumCheck );

            if ( node.UpperRight != null )
                AddUnLoadTasks( taskType, sphere, node.UpperRight, skipFrustumCheck );

            if ( node.LowerLeft != null )
                AddUnLoadTasks( taskType, sphere, node.LowerLeft, skipFrustumCheck );

            if ( node.LowerRight != null )
                AddUnLoadTasks( taskType, sphere, node.LowerRight, skipFrustumCheck );


        }

        public void RenderEntities()
        {
            RenderNode( tree );
        }

        public void RenderTerrains()
        {
            for ( int i = 0; i < terrainManager.Terrains.Count; i++ )
            {
                terrainManager.Terrains[ i ].Draw();
                //( (Editor.Terrain)terrainManager.Terrains[ i ] ).RenderWeightPaintMode();
            }

            engine.XNAGame.GraphicsDevice.Indices = null;
        }

        public int RenderNode( QuadTreeNode node )
        {

            if ( node.Visible != true )
                return 0;

            int totalTriangles = 0;


            for ( int i = 0; i < node.Entities.Count; i++ )
            {
                node.Entities[ i ].Render();
            }



            if ( node.UpperLeft != null )
                totalTriangles += RenderNode( node.UpperLeft );

            if ( node.UpperRight != null )
                totalTriangles += RenderNode( node.UpperRight );

            if ( node.LowerLeft != null )
                totalTriangles += RenderNode( node.LowerLeft );

            if ( node.LowerRight != null )
                totalTriangles += RenderNode( node.LowerRight );


            return totalTriangles;
        }

        public void UpdateNodeVisible( BoundingFrustum frustum, QuadTreeNode node, bool skipFrustumCheck )
        {
            node.Visible = false;

            if ( skipFrustumCheck != true )
            {

                ContainmentType containment = frustum.Contains( node.EntityBoundingBox );

                if ( containment == ContainmentType.Disjoint )
                    return;

                // if the entire node is contained within, then assume all children are as well
                if ( containment == ContainmentType.Contains )
                    skipFrustumCheck = true;
            }

            node.Visible = true;

            if ( node.UpperLeft != null )
                UpdateNodeVisible( frustum, node.UpperLeft, skipFrustumCheck );

            if ( node.UpperRight != null )
                UpdateNodeVisible( frustum, node.UpperRight, skipFrustumCheck );

            if ( node.LowerLeft != null )
                UpdateNodeVisible( frustum, node.LowerLeft, skipFrustumCheck );

            if ( node.LowerRight != null )
                UpdateNodeVisible( frustum, node.LowerRight, skipFrustumCheck );


        }



        public void OptimizeNode( QuadTreeNode node )
        {
            node.RecalculateEntityBounding();
        }

        /*public void OptimizeNodeResume( ref QuadTreeNode lastNode, int maxNodes )
        {
            //int totalNodeCount = 0;
            int nodeCount = 0;

            while ( startNode.Parent != null )
            {
                OptimizeNodePausable( startNode.Parent, maxNodes, ref lastNode, ref nodeCount );

                maxNodes -= nodeCount;
                nodeCount = 0;

                if ( maxNodes < 0 )
                {
                    //error
                    throw new Exception();
                }
                if ( maxNodes == 0 )
                {
                    //Done processesing or paused
                    if ( lastNode == null )
                    {
                        //Done processing, return.
                        return;
                    }
                    else
                    {
                        //Paused, return with new startNode.
                        return;
                    }
                }
                else
                {
                    //Not done yet, 
                }

            }
        }

        public void OptimizeNodePausable( QuadTreeNode node, int maxNodes, ref QuadTreeNode pausedLastNode )
        {
            int nodeCount = 0;
            OptimizeNodePausable( node, maxNodes, ref pausedLastNode, ref nodeCount );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="maxNodes">Must be >= the quadtree's depth. Otherwise, the loop with never reach the leaves.</param>
        /// <param name="pausedLastNode"></param>
        /// <param name="nodeCount"></param>
        public void OptimizeNodePausable( QuadTreeNode node, int maxNodes, ref QuadTreeNode pausedLastNode, ref int nodeCount )
        {
            if ( node == null ) return;
            if ( nodeCount == 0 )
            {
                //The loop has just started, so we assume this node shouldn't be skipped.
            }
            else
            {
                //We are in the loop. Check if the loop is being resumed.
                if ( pausedLastNode == null )
                {
                    //The loop is not being resumed, continue normally.
                }
                else
                {
                    //The loop has been paused at a past point.
                    //We now check if 'node' is the node where the loop stopped 
                    //in a previous call.
                    if ( pausedLastNode == node )
                    {
                        //We stopped after this node was completed in a previous call.

                        //Reset the paused state, we are now continuing as usual.
                        pausedLastNode = null;
                        return;
                    }
                    else
                    {
                        //The loop has been paused in a previous state, and this
                        //node has already been processed, so we skip this node.
                        return;
                    }
                }
            }




            //This node is going to be processed in this call.

            nodeCount++;

            if ( nodeCount >= maxNodes )
            {
                //When we have processed this node we will have processed the maximum number of nodes.
                //We set that this was the last processed node.
                pausedLastNode = node;
                //return;
            }


            OptimizeNodePausable( node.UpperLeft, maxNodes, ref pausedLastNode, ref nodeCount );
            OptimizeNodePausable( node.UpperRight, maxNodes, ref pausedLastNode, ref nodeCount );
            OptimizeNodePausable( node.LowerLeft, maxNodes, ref pausedLastNode, ref nodeCount );
            OptimizeNodePausable( node.LowerRight, maxNodes, ref pausedLastNode, ref nodeCount );

            OptimizeNode( node );


        }
        
        QuadTreeNode lastOptimizedNode = null;*/


        public bool OptimizeNodePausable( QuadTreeNode node, int optimizeID, int maxNodes )
        {
            int nodeCount = 0;
            return OptimizeNodePausable( node, optimizeID, maxNodes, ref nodeCount );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="maxNodes">Must be >= the quadtree's depth. Otherwise, the loop with never reach the leaves.</param>
        /// <param name="pausedLastNode"></param>
        /// <param name="nodeCount"></param>
        public bool OptimizeNodePausable( QuadTreeNode node, int optimizeID, int maxNodes, ref int nodeCount )
        {
            if ( node == null ) return true;
            if ( node.LastOptimizeID == optimizeID ) return true;

            if ( nodeCount >= maxNodes )
            {
                return false;
            }


            //This node is going to be processed in this call.

            nodeCount++;

            bool ret = true;


            ret &= OptimizeNodePausable( node.UpperLeft, optimizeID, maxNodes, ref nodeCount );
            ret &= OptimizeNodePausable( node.UpperRight, optimizeID, maxNodes, ref nodeCount );
            ret &= OptimizeNodePausable( node.LowerLeft, optimizeID, maxNodes, ref nodeCount );
            ret &= OptimizeNodePausable( node.LowerRight, optimizeID, maxNodes, ref nodeCount );

            OptimizeNode( node );

            if ( ret == true )
            {
                node.LastOptimizeID = optimizeID;
            }


            return ret;

        }

        int lastQuadTreeOptimizeID = 1;
        int maxOptimizeNodes = 11;

        public void OptimizeNodes()
        {
            if ( engine.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.K ) ) return;
            if ( engine.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.J ) )
            {
                maxOptimizeNodes = 80000;
            }
            else
            {
                maxOptimizeNodes = 11;
            }
            if ( OptimizeNodePausable( tree, lastQuadTreeOptimizeID, maxOptimizeNodes ) )
            {
                lastQuadTreeOptimizeID++;
            }
            /*if ( lastOptimizedNode == null )
            {
                OptimizeNodePausable( tree, 20, ref lastOptimizedNode );
            }
            else
            {
                OptimizeNodePausable( tree, 20, ref lastOptimizedNode );
            }*/
        }




        public ClientEntityHolder GetEntity( int nID )
        {
            for ( int i = 0; i < entities.Count; i++ )
            {
                if ( entities[ i ].ID == nID )
                {
                    return entities[ i ];
                }
            }
            return null;
        }



        public void OnEntityAdded( ClientEntityHolder entH )
        {
            if ( engine.GameClient.PlayerEntityHolder == null )
            {
                if ( engine.GameClient.GameClientData != null && engine.GameClient.GameClientData.PlayerEntityID == entH.ID )
                {
                    engine.GameClient.SetPlayerEntityHolder( entH );
                }
            }
        }



































        //public void OnSuccessfulLogin( string LoginKey )
        //{
        //    engine.Server.LinkUDPConnection( LoginKey );
        //}

        public void OnPingReply()
        {

        }

        public void UpdateEntity( Common.Network.UpdateEntityPacket p )
        {
            bool found = false;

            for ( int i = 0; i < entities.Count; i++ )
            {
                /*if (entities[i].ID == p.EntityID)
                {*/
                entities[ i ].UpdateEntity( 0, p );
                found = true;
                //}
            }

            if ( !found )
            {

            }


        }

        public void UpdateWorld( Common.Network.DeltaSnapshotPacket p )
        {
            int entityID;
            int length;

            entityID = p.ReadEntityUpdate( out length );

            while ( entityID != -1 )
            {

                bool found = false;

                for ( int i = 0; i < entities.Count; i++ )
                {
                    if ( entities[ i ].ID == entityID )
                    {

                        entities[ i ].UpdateEntity( p.Tick, p.dataReader, length );
                        found = true;
                        break;
                    }
                }

                if ( !found )
                {
                    p.dataReader.ReadBytes( length );
                    MHGameWork.TheWizards.ServerClient.Entities.Shuriken003 clientEnt = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken003( engine );
                    ServerClient.Wereld.ClientEntityHolder clientEntH = ServerClient.Entities.Shuriken003.CreateShuriken003Entity( clientEnt );

                    engine.Wereld.AddEntity( clientEntH );
                    clientEntH.SetID( entityID );
                }





                entityID = p.ReadEntityUpdate( out length );
            }



        }
        public void UpdateTime( Common.Network.TimeUpdatePacket p )
        {
            engine.SetTime( p.Time );
        }






        public QuadTree Tree
        { get { return tree; } }

        public List<ClientEntityHolder> Entities { get { return entities; } }

    }
}
