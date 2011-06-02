using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Entities;
using MHGameWork.TheWizards.Physics;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Entity.Client
{
    public class MeshPhysicsActorBuilder
    {
        public MeshPhysicsPool MeshPhysicsPool { get; private set; }

        public MeshPhysicsActorBuilder(MeshPhysicsPool meshPhysicsPool)
        {
            MeshPhysicsPool = meshPhysicsPool;
        }

        public Actor CreateActorStatic(StillDesign.PhysX.Scene scene, MeshCollisionData data, Matrix globalPose)
        {


            ActorDescription actorDesc = createActorDesc(data, scene, globalPose);

            if (actorDesc.Shapes.Count == 0) return null;
            return scene.CreateActor(actorDesc);


        }
        public Actor CreateActorDynamic(StillDesign.PhysX.Scene scene, MeshCollisionData data, Matrix globalPose)
        {
            ActorDescription actorDesc = createActorDesc(data, scene, globalPose);
            actorDesc.BodyDescription = new BodyDescription(10f); //TODO mass

            if (actorDesc.Shapes.Count == 0) return null;
            return scene.CreateActor(actorDesc);


        }

        /// <summary>
        /// You can put a scale (-1) operation in the global pose, it works! (not for convex meshes though!)
        /// NOTE: WARNING: scaling not supported!!! (still not supported, it has been delayed for several reasons)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="scene"></param>
        /// <param name="globalPose"></param>
        /// <returns></returns>
        private ActorDescription createActorDesc(MeshCollisionData data, StillDesign.PhysX.Scene scene, Matrix globalPose)
        {
            // From PhysX SDK:
            //There are some performance implications of compound shapes that the user should be aware of: 
            //You should avoid static actors being compounds; there's a limit to the number of triangles allowed in one actor's mesh shapes and subshapes exceeding the limit will be ignored. 
            //TODO: is this about triangle meshes only? EDIT: i dont think so



            // Pull scaling out of the transformation
            Vector3 scale, translation;
            Quaternion rotation;
            globalPose.Decompose(out scale, out rotation, out translation);

            //globalPose = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(translation);
            var scaleMat = Matrix.Identity;// Matrix.CreateScale(scale);


            ActorDescription actorDesc = new ActorDescription();

            for (int i = 0; i < data.Boxes.Count; i++)
            {
                var box = data.Boxes[i];
                var shape = new BoxShapeDescription(box.Dimensions);
                shape.LocalPose = box.Orientation * scaleMat;
                actorDesc.Shapes.Add(shape);
            }

            for (int i = 0; i < data.ConvexMeshes.Count; i++)
            {
                var convex = data.ConvexMeshes[i];
                var shape = new ConvexShapeDescription();
                shape.ConvexMesh = MeshPhysicsPool.CreateConvexMesh(scene, convex);

                shape.Flags = ShapeFlag.Visualization;
                actorDesc.Shapes.Add(shape);
            }




            if (data.TriangleMesh != null)
            {
                TriangleMesh triangleMesh;
                triangleMesh = MeshPhysicsPool.CreateTriangleMesh(scene, data.TriangleMesh);

                TriangleMeshShapeDescription shapeDesc = new TriangleMeshShapeDescription();
                shapeDesc.TriangleMesh = triangleMesh;
                shapeDesc.Flags = ShapeFlag.Visualization; // Vizualization enabled, obviously (not obviously enabled, obvious that this enables visualization)

                actorDesc.Shapes.Add(shapeDesc);
            }

            actorDesc.GlobalPose = globalPose;
            return actorDesc;
        }


        public BoundingBox CalculateBoundingBox(MeshCollisionData data)
        {

            var boundingBox = new BoundingBox();

            for (int i = 0; i < data.Boxes.Count; i++)
            {
                var box = data.Boxes[i];
                var shape = new BoxShapeDescription(box.Dimensions);
                shape.LocalPose = box.Orientation;

                //WARNING: this is slow , really slow
                Vector3[] corners = (new BoundingBox(-box.Dimensions * Vector3.One * 0.5f, box.Dimensions * Vector3.One * 0.5f)).GetCorners();
                var transform = new Vector3[corners.Length];

                Vector3.Transform(corners, ref box.Orientation, transform);

                var bs = BoundingBox.CreateFromPoints(transform);



                boundingBox = boundingBox.MergeWith(bs);
            }

            //TODO: convex boundingbox not implemented!

            if (data.TriangleMesh != null)
                boundingBox = boundingBox.MergeWith(calculateBBTriangleMesh(data.TriangleMesh));

            return boundingBox;
        }

        private BoundingBox calculateBBTriangleMesh(MeshCollisionData.TriangleMeshData data)
        {
            return BoundingBox.CreateFromPoints(data.Positions);
        }

        public BoundingSphere CalculateBoundingSphere(MeshCollisionData data)
        {
            return BoundingSphere.CreateFromBoundingBox(CalculateBoundingBox(data));
        }
    }
}
