using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Entities;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Entity.Client
{
    /// <summary>
    /// This class uses a EntityFullData as input to produce a PhysX actor object that represents collision for the given entity. 
    /// Currently this is done by creating a trianglemeshshape using the entity’s rendering triangles. 
    /// This should be extended later on to allow simplified versions of the model or a collision model composed of boxes, spheres, convexmesh. 
    /// These simplified versions should be build in the editor
    /// TODO: Only loads first (sub)model into physics atm.
    ///       This is because the definition of the entity/object/model structure is not yet consolidated in this design.
    /// </summary>
    [Obsolete("This is now moved to the meshes.")]
    public class EntityPhysicsActorBuilder
    {
        public EntityPhysicsActorBuilder()
        {

        }

        /// <summary>
        /// Note that the mesh is cooked in this procedure. This should be preprocessed and stored so that loading is faster.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="fullData"></param>
        /// <returns></returns>
        public Actor CreateActorForEntity(StillDesign.PhysX.Scene scene, EntityFullData fullData)
        {
            // From PhysX SDK:
            //There are some performance implications of compound shapes that the user should be aware of: 
            //You should avoid static actors being compounds; there's a limit to the number of triangles allowed in one actor's mesh shapes and subshapes exceeding the limit will be ignored. 



            ActorDescription actorDesc = new ActorDescription();

            for ( int i = 0; i < fullData.ObjectFullData.Models.Count; i++ )
            {
                ModelFullData model = fullData.ObjectFullData.Models[ i ];


                TriangleMesh triangleMesh = CreateTriangleMesh( model, fullData, scene );

                TriangleMeshShapeDescription shapeDesc = new TriangleMeshShapeDescription();
                shapeDesc.TriangleMesh = triangleMesh;
                shapeDesc.Flags = ShapeFlag.Visualization; // Vizualization enabled, obviously (not obviously enabled, obvious that this enables visualization)

                actorDesc.Shapes.Add( shapeDesc );
            }

            Transformation transformNoScale = fullData.Transform;
            transformNoScale.Scaling = Vector3.One;

            actorDesc.GlobalPose = transformNoScale.CreateMatrix();

            return scene.CreateActor( actorDesc );


        }


        private static TriangleMesh CreateTriangleMesh(ModelFullData model, EntityFullData ent, StillDesign.PhysX.Scene scene)
        {
            TriangleMeshDescription triangleMeshDesc = new TriangleMeshDescription();

            triangleMeshDesc.AllocateVertices<Vector3>( model.Positions.Length );
            triangleMeshDesc.AllocateTriangles<int>( model.Positions.Length ); // int indices, should be short but whatever, EDIT: should model.position not be model.indices?

            Vector3[] transformedPositions = new Vector3[ model.Positions.Length ];
            Matrix transform = model.ObjectMatrix * Matrix.CreateScale( ent.Transform.Scaling );
            Vector3.Transform( model.Positions, ref transform, transformedPositions );

            triangleMeshDesc.VerticesStream.SetData( transformedPositions );

            int[] indices = new int[ model.Positions.Length ];
            for ( int i = 0; i < indices.Length; i++ )
            {
                indices[ i ] = i;
            }
            triangleMeshDesc.TriangleStream.SetData( indices );

            triangleMeshDesc.VertexCount = model.Positions.Length;
            triangleMeshDesc.TriangleCount = model.Positions.Length / 3;

            System.IO.MemoryStream stream = new System.IO.MemoryStream();

            Cooking.InitializeCooking();
            Cooking.CookTriangleMesh( triangleMeshDesc, stream );
            Cooking.CloseCooking();

            stream.Position = 0;

            return scene.Core.CreateTriangleMesh( stream );
        }
    }
}
