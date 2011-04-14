using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NovodexWrapper;
using MHGameWork.TheWizards.Common.GeoMipMap;

namespace MHGameWork.TheWizards.Server.GeoMipMap
{
    public class TerrainBlock : Common.GeoMipMap.TerrainBlock, Wereld.IEntityHolder
    {
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        //private Vector3 center;
        private int totalVertexes = 0;
        private NxActor actor;

        private int version;

        public int Version
        {
            get { return version; }
            set { version = value; }
        }

        public TerrainBlock( Terrain terrain, int x, int z )
            : base( terrain, x, z )
        {

        }








        public virtual void LoadGraphicsContent( GraphicsDevice device, bool loadAllContent )
        {
            //BuildVertexBufferFromFile( device );
            //BuildVertexBufferFromHeightmap();
            //CalculateMinDistances( Terrain.Engine.ActiveCamera.CameraInfo.ProjectionMatrix );
            //BuildIndexBuffer( device );
        }


        //public void BuildVertexBufferFromHeightmap()
        //{
        //    if ( vertexBuffer != null )
        //        vertexBuffer.Dispose();

        //    VertexMultitextured[] vertexes;

        //    Vector3 min;
        //    Vector3 max;

        //    vertexes = GenerateVerticesFromHeightmap( out min, out max );

        //    center = ( min + max ) * 0.5f;
        //    LocalBoundingBox = new BoundingBox( min, max );

        //    totalVertexes = vertexes.Length;

        //    if ( vertexBuffer != null )
        //        vertexBuffer.Dispose();

        //    vertexBuffer = new VertexBuffer( Terrain.Device, typeof( VertexPositionNormalTexture ),
        //        vertexes.Length, BufferUsage.None );
        //    vertexBuffer.SetData<VertexMultitextured>( vertexes );

        //    totalVertexes = vertexes.Length;
        //    vertexes = null;

        //    vertexBufferDirty = false;
        //}

        protected VertexMultitextured[] GenerateVerticesFromHeightmap( out Vector3 min, out Vector3 max )
        {
            VertexMultitextured[] vertexes = new VertexMultitextured[ ( terrain.BlockSize + 1 ) * ( terrain.BlockSize + 1 ) ];

            min = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );
            max = new Vector3( float.MinValue, float.MinValue, float.MinValue );



            // build vectors and normals
            for ( int x = 0; x <= terrain.BlockSize; x++ )
            {
                for ( int z = 0; z <= terrain.BlockSize; z++ )
                {
                    int cx = this.x + x;
                    int cz = this.z + z;

                    VertexMultitextured vert = new VertexMultitextured();

                    vert.Position = terrain.GetLocalPosition( new Vector3( cx, terrain.HeightMap[ cx, cz ], cz ) );


                    //TODO: klopt deze texcoord wel?
                    vert.TextureCoordinate = new Vector2( (float)cx / (float)terrain.SizeX, (float)cz / (float)terrain.SizeY );

                    vert.Normal = terrain.GetAveragedNormal( cx, cz );

                    min = Vector3.Min( min, vert.Position );
                    max = Vector3.Max( max, vert.Position );

                    vertexes[ IndexFromCoords( x, z ) ] = vert;
                }
            }


            return vertexes;

        }

        protected VertexMultitextured[] GenerateVerticesFromHeightmap()
        {
            Vector3 min;
            Vector3 max;
            return GenerateVerticesFromHeightmap( out min, out max );
        }





        protected override void Dispose( bool disposing )
        {

            base.Dispose( disposing );
        }



        public virtual void UnloadGraphicsContent( bool unloadAllContent )
        {
            if ( vertexBuffer != null )
                vertexBuffer.Dispose();

            if ( indexBuffer != null )
                indexBuffer.Dispose();


            vertexBuffer = null;
            indexBuffer = null;

        }

        public void ChangeDetailLevel( int mipLevel, bool force )
        {
            if ( mipLevel > terrain.MaxDetailLevel )
                mipLevel = terrain.MaxDetailLevel;

            if ( mipLevel == detailLevel && force != true )
                return;

            detailLevel = mipLevel;

            BuildBaseIndexes();
            BuildEdgeIndexes();

            if ( West != null )
                West.BuildEdgeIndexes();

            if ( East != null )
                East.BuildEdgeIndexes();

            if ( North != null )
                North.BuildEdgeIndexes();

            if ( South != null )
                South.BuildEdgeIndexes();
        }


        public override void OnBoundingChanged()
        {
            base.OnBoundingChanged();
            //Terrain.Engine.Wereld.Tree.OrdenEntity (
            //QuadTreeNode.OnTerreinBlockBoundingChanged();
        }

        public float CalculateWeight( float height, float min, float max )
        {
            //oorspronkelijke formule
            // 1 - Math.Abs( ( height - min - ( ( max - min ) / 2 ) ) / ( ( max - min ) / 2 ) )
            float test = MathHelper.Clamp( 1 - Math.Abs( ( height - min - ( ( max - min ) / 2 ) ) / ( ( max - min ) / 2 ) ), 0, 1 );

            //uitgerekende formule

            float result = MathHelper.Clamp( 1 - Math.Abs( ( 2 * height - min - max ) / ( max - min ) ), 0, 1 );

            if ( test != result ) throw new Exception();
            return result;
        }

        //public void PaintWeight(int x, int y, int range, int texNum)
        //{
        //    BoundingSphere sphere = new BoundingSphere( new Vector3( x, 0, y ), range );
        //    BoundingBox box = new BoundingBox( new Vector3( this.x, 0, this.z ), new Vector3( this.x + 16, 0, this.z + 16 ) );
        //    if ( !sphere.Intersects( box ) ) return;
        //    x -= this.x;
        //    y -= this.z;
        //    //if ( x < 0 || x > 16 ) return;
        //    //if ( y < 0 || y > 16 ) return;
        //    Color[] data = new Color[ 17 * 17 ];
        //    weightTexture.GetData<Color>( data, 0, 17 * 17 );


        //    for ( int ix = 0; ix < 17; ix++ )
        //    {
        //        for ( int iy = 0; iy < 17; iy++ )
        //        {
        //            Vector2 diff = new Vector2( ix, iy ) - new Vector2( x, y );
        //            float dist = diff.Length();
        //            if ( dist < range )
        //            {
        //                float factor = 1 - ( dist / range );
        //                factor *= 255;
        //                Color c = data[ IndexFromCoords( ix, iy ) ];
        //                float a = c.A;
        //                float r = c.R;
        //                float g = c.G;
        //                float b = c.B;

        //                //Deel elke kleur door het nieuwe totaal * 255
        //                a = a / ( 255 + factor ) * 255;
        //                r = r / ( 255 + factor ) * 255;
        //                g = g / ( 255 + factor ) * 255;
        //                b = b / ( 255 + factor ) * 255;

        //                a = (float)Math.Floor( a );
        //                r = (float)Math.Floor( r );
        //                g = (float)Math.Floor( g );
        //                b = (float)Math.Floor( b );

        //                //Zorgt dat de som exact 255 is, de overschot gaat naar de gekozen weight

        //                switch ( texNum )
        //                {
        //                    case 0:
        //                        r = 255 - g - b - a;
        //                        break;
        //                    case 1:
        //                        g = 255 - r - b - a;
        //                        break;
        //                    case 2:
        //                        b = 255 - r - g - a;
        //                        break;
        //                    case 3:
        //                        a = 255 - r - g - b;
        //                        break;
        //                }




        //                //r = (byte)Math.Floor( 255 * factor );
        //                //a = (byte)( 1 - r );
        //                data[ IndexFromCoords( ix, iy ) ] = new Color( (byte)r, (byte)g, (byte)b, (byte)a );

        //            }
        //        }
        //    }

        //    /*byte a;
        //    byte r;
        //    byte g;
        //    byte b;
        //    Color c;*/
        //    //c = data[ IndexFromCoords( x, y ) ];
        //    //data[ IndexFromCoords( x, y ) ] = new Color( 255, 0, 0, 0 );

        //    /*data[ IndexFromCoords( x, y ) ].A = 0;*/

        //    weightTexture.SetData<Color>( data );


        //}

        /*public byte[] ToBytes()
        {
            MHGameWork.TheWizards.Common.ByteWriter BW = new MHGameWork.TheWizards.Common.ByteWriter();

            VertexPositionNormalTexture[] vertexes = new VertexPositionNormalTexture[ ( terrain.BlockSize + 1 ) * ( terrain.BlockSize + 1 ) ];


            Vector3 min = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );
            Vector3 max = new Vector3( float.MinValue, float.MinValue, float.MinValue );

            float ox = terrain.HeightMap.Width * 0.5f;
            float oz = terrain.HeightMap.Length * 0.5f;

            // build vectors and normals
            for ( int x = 0; x <= terrain.BlockSize; x++ )
            {
                for ( int z = 0; z <= terrain.BlockSize; z++ )
                {
                    int cx = this.x + x;
                    int cz = this.z + z;

                    Vector3 position = new Vector3(
                        ( cx - ox ) * terrain.Scale,
                        terrain.HeightMap[ cx, cz ] * terrain.HeightScale,
                        ( cz - oz ) * terrain.Scale );

                    Vector2 textureCoords = new Vector2(
                        (float)x / ( (float)terrain.BlockSize + 0.51f ),
                        (float)z / ( (float)terrain.BlockSize + 0.51f ) );

                    Vector3 normal = terrain.GetAveragedNormal( cx, cz );

                    min = Vector3.Min( min, position );
                    max = Vector3.Max( max, position );

                    vertexes[ IndexFromCoords( x, z ) ] = new VertexPositionNormalTexture( position, normal, textureCoords );
                }
            }

            center = ( min + max ) * 0.5f;
            BoundingBox = new BoundingBox( min, max );

            totalVertexes = vertexes.Length;


            BW.Write( totalVertexes );
            BW.Write( min );
            BW.Write( max );
            for ( int i = 0; i < vertexes.Length; i++ )
            {
                BW.Write( vertexes[ i ].Position );
                BW.Write( vertexes[ i ].Normal );
                BW.Write( vertexes[ i ].TextureCoordinate );
            }

            return BW.ToBytesAndClose();
        }*/


        public override byte[] ToBytes()
        {
            MHGameWork.TheWizards.Common.ByteWriter BW = new MHGameWork.TheWizards.Common.ByteWriter();

            VertexMultitextured[] vertexes;

            Vector3 min;
            Vector3 max;

            vertexes = GenerateVerticesFromHeightmap( out min, out max );


            totalVertexes = vertexes.Length;


            BW.Write( totalVertexes );
            BW.Write( min );
            BW.Write( max );
            for ( int i = 0; i < vertexes.Length; i++ )
            {
                BW.Write( vertexes[ i ].Position );
                BW.Write( vertexes[ i ].Normal );
                BW.Write( vertexes[ i ].TextureCoordinate );
            }

            return BW.ToBytesAndClose();
        }


        public void BuildIndexBuffer( GraphicsDevice device )
        {
            if ( indexBuffer != null )
                indexBuffer.Dispose();

            // allocate index buffer for maximum amount of room needed
            indexBuffer = new IndexBuffer( device, typeof( ushort ),
                ( terrain.BlockSize + 1 ) * ( terrain.BlockSize + 1 ) * 6, BufferUsage.None );

            List<ushort> indexes = new List<ushort>();

            BuildBaseIndexes( indexes );
            BuildEdgeIndexes( indexes );

            if ( indexes.Count > 0 )
                indexBuffer.SetData<ushort>( indexes.ToArray() );
        }

        public void BuildBaseIndexes()
        {
            List<ushort> indexes = new List<ushort>();

            BuildBaseIndexes( indexes );

            if ( indexes.Count > 0 )
                indexBuffer.SetData<ushort>( indexes.ToArray() );
        }



        public void BuildEdgeIndexes()
        {
            List<ushort> indexes = new List<ushort>();

            BuildEdgeIndexes( indexes );

            if ( indexes.Count > 0 )
            {
                indexBuffer.SetData<ushort>( totalEdgeOffset * sizeof( ushort ),
                    indexes.ToArray(), 0, indexes.Count );
                //TODO: CHECK IF THIS EDIT IS OK!!
                /*indexBuffer.SetData<ushort>( totalEdgeOffset * sizeof( ushort ),
                    indexes.ToArray(), 0, indexes.Count, SetDataOptions.None );*/
            }
        }



        public void LoadHeightField()
        {
            UnloadHeightField();
            actor = CreateHeightField();
        }

        public void UnloadHeightField()
        {
            if ( actor != null ) actor.destroy();
            actor = null;

        }
        //The data range is between 0 and maxHeight
        public float[] createHeightFieldData( int gridXsubdivisions, int gridZsubdivisions, float gridWidth, float gridDepth, float maxHeight, int seed, int numSmooths, float smoothWeight )
        {
            int numXverts = gridXsubdivisions + 1;
            int numZverts = gridZsubdivisions + 1;

            NovodexUtil.seedRandom( seed );

            //Create a random array of heights
            float[] heightArray = new float[ numXverts * numZverts ];
            for ( int z = 0; z < numZverts; z++ )
            {
                for ( int x = 0; x < numXverts; x++ )
                { heightArray[ x + ( z * numXverts ) ] = NovodexUtil.randomFloat( 0, maxHeight ); }
            }

            //Smooth the random values so the terrain isn't so jagged
            for ( int i = 0; i < numSmooths; i++ )
            {
                for ( int z = 0; z < numZverts; z++ )
                {
                    int z0 = z;
                    int z1 = NovodexUtil.clampInt( z + 1, 0, numZverts - 1 );

                    for ( int x = 0; x < numXverts; x++ )
                    {
                        int x0 = x;
                        int x1 = NovodexUtil.clampInt( x + 1, 0, numXverts - 1 );

                        float a = heightArray[ x0 + ( z0 * numXverts ) ];
                        float b = heightArray[ x1 + ( z0 * numXverts ) ];
                        float c = heightArray[ x1 + ( z1 * numXverts ) ];
                        float d = heightArray[ x0 + ( z1 * numXverts ) ];

                        heightArray[ x + ( z * numXverts ) ] = ( ( a * smoothWeight ) + b + c + d ) / ( smoothWeight + 3 );
                    }
                }
            }

            return heightArray;
        }

        public NxActor CreateHeightField()
        {


            NxHeightFieldDesc heightFieldDesc = NxHeightFieldDesc.Default;
            //heightFieldDesc.nbColumns = (uint)terrain.BlockSize + 1;
            //heightFieldDesc.nbRows = (uint)terrain.BlockSize + 1;
            heightFieldDesc.verticalExtent = -1000;
            heightFieldDesc.convexEdgeThreshold = 0;
            //heightFieldDesc.sampleStride = 3 * 4;
            heightFieldDesc.setSampleDimensions( terrain.BlockSize + 1, terrain.BlockSize + 1 );

            float ox = terrain.HeightMap.Width * 0.5f;
            float oz = terrain.HeightMap.Length * 0.5f;

            for ( int ix = 0; ix < terrain.BlockSize + 1; ix++ )
            {
                for ( int iz = 0; iz < terrain.BlockSize + 1; iz++ )
                {
                    int cx = this.x + ix;
                    int cz = this.z + iz;
                    //int cx = ix;
                    //int cz = iz;
                    bool flag = ( ix % 2 ) == 0;
                    if ( iz % 2 != 0 ) flag = !flag;
                    flag = false;
                    heightFieldDesc.setSample( ix, iz, new NxHeightFieldSample( (short)( terrain.HeightMap[ cx, cz ] * 10 ), 1, flag, 1 ) );
                    //heightFieldDesc.setSample( ix, iz, new NxHeightFieldSample( (short)( terrain.HeightMap[ cx, cz ] * 5000 ), 0, flag, 0 ) );
                }
            }
            NxHeightField heightField = Terrain.Engine.PhysicsSDK.createHeightField( heightFieldDesc );

            heightFieldDesc = null;

            NxHeightFieldShapeDesc shapeDesc = new NxHeightFieldShapeDesc();

            shapeDesc.HeightField = heightField;
            shapeDesc.heightScale = terrain.HeightScale * 0.1f;
            shapeDesc.rowScale = terrain.Scale;
            shapeDesc.columnScale = terrain.Scale;
            shapeDesc.materialIndexHighBits = 0;
            shapeDesc.holeMaterial = 2;

            NxActorDesc actorDesc = new NxActorDesc();
            actorDesc.addShapeDesc( shapeDesc );

            Vector3 position = new Vector3( 0, 0, 0 );
            position.X = Terrain.QuadTreeNode.BoundingBox.Min.X + this.x;
            position.Z = Terrain.QuadTreeNode.BoundingBox.Min.Z + this.z;

            actorDesc.globalPose = Matrix.CreateTranslation( position );
            // Matrix.CreateTranslation( new Vector3( ( this.x - ox ) * terrain.Scale, 0, ( this.z - oz ) * terrain.Scale ) );

            return Terrain.Engine.PhysicsScene.createActor( actorDesc );

        }


        public new Terrain Terrain
        {
            get { return (Terrain)terrain; }

        }
        public new TerrainBlock West
        {
            get { return (TerrainBlock)base.West; }
            set { base.West = value; }
        }

        public new TerrainBlock East
        {
            get { return (TerrainBlock)base.East; }
            set { base.East = value; }
        }

        public new TerrainBlock North
        {
            get { return (TerrainBlock)base.North; }
            set { base.North = value; }
        }

        public new TerrainBlock South
        {
            get { return (TerrainBlock)base.South; }
            set { base.South = value; }
        }

        public new Wereld.QuadTreeNode QuadTreeNode
        {
            get { return (Wereld.QuadTreeNode)base.QuadTreeNode; }
            set { base.QuadTreeNode = value; }
        }

        #region IEntityHolder Members

        private int _id;


        public void SetID( int nID )
        {
            _id = nID;
        }

        public void MoveToNode( MHGameWork.TheWizards.Server.Wereld.QuadTreeNode nNode )
        {
            //dont change node

        }

        public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {

        }

        public void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {

        }
        public void EnablePhysics()
        {
            LoadHeightField();
        }

        public void DisablePhysics()
        {
            UnloadHeightField();

        }

        public BoundingSphere BoundingSphere
        {
            get { return BoundingSphere.CreateFromBoundingBox( BoundingBox ); }
        }

        public bool Static
        {
            get
            {
                return true;
            }
            set
            {

            }
        }

        public int ID
        {
            get { return _id; }
        }
        public MHGameWork.TheWizards.Server.Wereld.QuadTreeNode ContainingNode
        {
            get { return QuadTreeNode; }
        }
        #endregion




    }
}

