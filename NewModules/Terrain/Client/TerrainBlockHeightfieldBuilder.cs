using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Terrain;
using MHGameWork.TheWizards.ServerClient.Terrain.Rendering;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;


namespace MHGameWork.TheWizards.Terrain.Client
{
    public class TerrainBlockHeightfieldBuilder
    {
        public TerrainBlockHeightfieldBuilder()
        {

        }

        

        public Actor BuildHeightfieldActor( StillDesign.PhysX.Scene scene, TerrainFullData data, int blockX, int blockZ, float blockHeight )
        {

            // Calculate block vertical height
            //float blockHeight = block.BoundingBox.Max.Y*block.BoundingBox.Min.Y;
            float heightScale = blockHeight / ( short.MaxValue - 1 ); //To convert from the short range(0->(65536-1)) to the 0->blockheight
            float inverseHeightScale = 1 / heightScale;



            HeightFieldDescription desc = new HeightFieldDescription();

            desc.NumberOfColumns = data.BlockSize + 1;
            desc.NumberOfRows = data.BlockSize + 1;
            desc.Thickness = -20;
            //desc.ConvexEdgeThreshold = 0;


            HeightFieldSample[] samples = new HeightFieldSample[ desc.NumberOfColumns * desc.NumberOfRows ];
            //desc.SampleStrideSize = sizeof(NxU32);

            HeightFieldSample sample = new HeightFieldSample();

            int wx = blockX * data.BlockSize;
            int wz = blockZ * data.BlockSize;
            int iSample = 0;

            for ( int ix = 0; ix < data.BlockSize + 1; ix++ )
            {
                for ( int iz = 0; iz < data.BlockSize + 1; iz++ )
                {



                    sample.Height = (short)( data.HeightMap[ wx, wz ] * inverseHeightScale ); // TODO: fix the scaling
                    sample.MaterialIndex0 = 0;
                    sample.MaterialIndex1 = 1;
                    sample.TessellationFlag = 0;
                    samples[ iSample ] = sample;

                    wz++;
                    iSample++;
                }

                wx++;
                wz = blockZ * data.BlockSize;
            }

            desc.Samples = samples;

            HeightField heightField = scene.Core.CreateHeightField( desc );
            //Supposed to free up memory
            desc.Samples = null; // delete[] heightFieldDesc.samples;



            HeightFieldShapeDescription shapeDescription = new HeightFieldShapeDescription();

            shapeDescription.HeightField = heightField;
            shapeDescription.HeightScale = heightScale; // I am guessing this is incorrect , read next comment from SDK
            //In other words, the scale factors are applied to the heightfield as if the sample points were mapped into the cube (0,0,0)=>(1,1,1). 
            shapeDescription.RowScale = 1;
            shapeDescription.ColumnScale = 1;
            //shapeDescription.MaterialIndexHighBits = 0;
            shapeDescription.HoleMaterial = 2;

            ActorDescription actorDescription = new ActorDescription();
            actorDescription.Shapes.Add( shapeDescription );
            actorDescription.GlobalPose = Matrix.CreateTranslation( data.Position + new Vector3( blockX * data.BlockSize, 0, blockZ * data.BlockSize ) );

            return scene.CreateActor( actorDescription );



        }
    }
}
