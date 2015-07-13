using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics.TODO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics.Xna.Collada
{
    public class ColladaMesh
    {
        private Dictionary<string, ColladaSource> sources = new Dictionary<string, ColladaSource>();
        private ColladaModel model;
        private List<PrimitiveList> parts = new List<PrimitiveList>();

        public List<PrimitiveList> Parts
        {
            get { return parts; }
        }

        private VertexDeclaration vertexDeclaration;


        public ColladaMesh( ColladaModel nModel )
        {
            model = nModel;
        }

        /// <summary>
        /// Load mesh geometry
        /// </summary>
        /// <param name="geometry"></param>
        public void LoadMesh( XmlNode colladaFile, XmlNode meshNode )
        {


            // Load all sources
            foreach ( XmlNode node in meshNode )
            {
                if ( node.Name == "source" )
                    AddSource( ColladaSource.FromXmlNode( node ) );

            }


            // Vertices
            // Currently, the importer only supports meshes where the vertices node only contains position input

            // Also add vertices node, redirected to position node into sources


            XmlNode verticesNode = XmlHelper.GetChildNode( meshNode, "vertices" );
            ColladaSource source;



            XmlNode posInput = XmlHelper.GetChildNode( verticesNode, "input" );
            //if ( XmlHelper.GetXmlAttribute( posInput, "semantic" ).ToLower(
            //    CultureInfo.InvariantCulture ) != "position" )
            //    throw new InvalidOperationException(
            //        "unsupported feature found in collada \"vertices\" node" );
            if ( XmlHelper.GetXmlAttribute( posInput, "semantic" ).ToLower() != "position" )
                throw new InvalidOperationException(
                    "unsupported feature found in collada \"vertices\" node" );

            string verticesValueName = XmlHelper.GetXmlAttribute( posInput,
                "source" ).Substring( 1 );

            source = GetSource( verticesValueName );

            source.ID = XmlHelper.GetXmlAttribute( verticesNode, "id" );

            AddSource( source );


            // Find all primitive lists
            // We currently only support triangles

            foreach ( XmlNode trianglenode in meshNode )
            {


                if ( trianglenode.Name == "triangles" )
                {
                    PrimitiveList primitives = new PrimitiveList();

                    // Type
                    primitives.Type = PrimitiveList.PrimitiveListType.triangles;

                    // PrimitiveCount
                    string countString = XmlHelper.GetXmlAttribute( trianglenode, "count" );
                    primitives.PrimitiveCount = int.Parse( countString );

                    // Material
                    string materialString = XmlHelper.GetXmlAttribute( trianglenode, "material" );
                    primitives.Material = model.GetMaterial( materialString );

                    // Inputs and primitives
                    foreach ( XmlNode inputNode in trianglenode )
                    {
                        if ( inputNode.Name == "input" )
                        {
                            Input input = new Input();
                            input.SetSemantic( XmlHelper.GetXmlAttribute( inputNode, "semantic" ) );

                            //TODO: Currently we ignore what is in the 'vertices' node, and we assume that the semantic VERTEX 
                            //simply refers to the POSITION source
                            //To implement collada correctly, we should add each input specified in the 'vertices node'
                            if ( input.Semantic == Input.InputSemantics.Vertex ) input.Semantic = Input.InputSemantics.Position;


                            string sourceString = XmlHelper.GetXmlAttribute( inputNode, "source" ).Substring( 1 );
                            input.Source = GetSource( sourceString );

                            string offset = XmlHelper.GetXmlAttribute( inputNode, "offset" );
                            string set = XmlHelper.GetXmlAttribute( inputNode, "set" );

                            input.Offset = offset == "" ? -1 : int.Parse( offset );
                            input.Set = set == "" ? -1 : int.Parse( set );

                            primitives.Inputs.Add( input );

                        }
                    }

                    // Indices
                    XmlNode p = XmlHelper.GetChildNode( trianglenode, "p" );
                    int[] pints = StringHelper.ConvertStringToIntArray( p.InnerText );
                    primitives.Indices.AddRange( pints );

                    // Indices per Vertex
                    int indicesPerVertex = pints.Length / primitives.PrimitiveCount / 3;
                    primitives.IndicesPerVertex = indicesPerVertex;


                    parts.Add( primitives );


                }

                continue;

                //material = model.GetMaterial( XmlHelper.GetXmlAttribute( trianglenode, "material" ) );



                // Get the indexlist

                //int trianglecount = Convert.ToInt32( XmlHelper.GetXmlAttribute(
                //    trianglenode, "count" ), NumberFormatInfo.InvariantInfo );


                //LoadSkinData( colladaFile, meshNode );

                //if ( vertexSkinJoints.Count > 0 )
                //{
                //    CreateSkinnedVertices( trianglecount );
                //}
                //else
                //{
                //CreateVertices( trianglecount );
                //}

            }


            LoadSkinData( colladaFile, meshNode );

        }



        public void AddSource( ColladaSource nSource )
        {
            sources.Add( nSource.ID, nSource );

        }
        public ColladaSource GetSource( string id )
        {
            return sources[ id ];
        }


        public class PrimitiveList
        {
            /// <summary>
            /// Check the collada specs of the mesh node for more info
            /// </summary>
            public enum PrimitiveListType
            {
                /// <summary>
                /// This variable has not yet been initialized
                /// </summary>
                Empty = 0,
                /// <summary>
                /// Not supported yet
                /// </summary>
                lines,
                /// <summary>
                /// Not supported yet
                /// </summary>
                linestrips,
                /// <summary>
                /// Not supported yet
                /// </summary>
                polygons,
                /// <summary>
                /// Not supported yet
                /// </summary>
                polylist,
                triangles,
                /// <summary>
                /// Not supported yet
                /// </summary>
                trifans,
                /// <summary>
                /// Not supported yet
                /// </summary>
                tristrips

            }

            public PrimitiveListType Type;

            public int PrimitiveCount;
            public ColladaMaterial Material;
            public int IndicesPerVertex;

            public List<Input> Inputs = new List<Input>();
            public List<int> Indices = new List<int>();

            public bool ContainsInput( Input.InputSemantics semantic )
            { return GetInput( semantic ) != null; }

            public bool ContainsInput( Input.InputSemantics semantic, int set )
            { return GetInput( semantic, set ) != null; }


            /// <summary>
            /// This requires that the source used by the input with the correct semantic is a float_array
            /// </summary>
            /// <param name="semantic"></param>
            /// <param name="vertexIndex"></param>
            /// <returns></returns>
            public Vector3 GetVector3( Input.InputSemantics semantic, int vertexIndex )
            {
                return GetVector3( semantic, -1, vertexIndex );
            }

            /// <summary>
            /// This requires that the source used by the input with the correct semantic is a float_array
            /// </summary>
            /// <param name="semantic"></param>
            /// <param name="vertexIndex"></param>
            /// <returns></returns>
            public Vector3 GetVector3( Input.InputSemantics semantic, int set, int vertexIndex )
            {
                Input input = GetInput( semantic, set );
                if ( input == null ) throw new InvalidOperationException( "There is not input available for this semantic" );



                int sourceIndex = GetSourceIndex( input, vertexIndex );

                List<float> Data = ( (ColladaSourceFloat)input.Source ).Data;

                return new Vector3( Data[ sourceIndex * 3 ], Data[ sourceIndex * 3 + 1 ], Data[ sourceIndex * 3 + 2 ] );
            }

            /// <summary>
            /// Returns the input that has given semantic and doesn't have a 'set'.
            /// Returns null when not found.
            /// </summary>
            /// <param name="semantic"></param>
            /// <returns></returns>
            public Input GetInput( Input.InputSemantics semantic )
            { return GetInput( semantic, -1 ); }

            /// <summary>
            /// Returns the input with given semantic and set.
            /// Returns null when not found.
            /// </summary>
            /// <param name="semantic"></param>
            /// <param name="set"></param>
            /// <returns></returns>
            public Input GetInput( Input.InputSemantics semantic, int set )
            {
                for ( int i = 0; i < Inputs.Count; i++ )
                {
                    if ( Inputs[ i ].Semantic == semantic && Inputs[ i ].Set == set )
                    {
                        return Inputs[ i ];
                    }
                }
                return null;
            }

            public int GetSourceIndex( Input input, int vertexIndex )
            {
                int startIndex = vertexIndex * IndicesPerVertex;
                int index = startIndex + input.Offset;

                return Indices[ index ];
            }


            public override string ToString()
            {
                return "PrimitiveList: " + System.Enum.GetName( typeof( PrimitiveListType ), Type ) + " - count: " + PrimitiveCount.ToString();
            }

        }

        public class Input
        {
            /// <summary>
            /// A list of semantics used by the importer
            /// </summary>
            public enum InputSemantics
            {
                Position = 1,
                Vertex,
                Normal,
                Texcoord,
                TexTangent,
                TexBinormal

            }

            public InputSemantics Semantic;
            public ColladaSource Source;
            public int Offset;
            public int Set;

            public void SetSemantic( string semanticString )
            {
                switch ( semanticString.ToLower() )
                {
                    case "position":
                        Semantic = InputSemantics.Position;
                        break;
                    case "vertex":
                        Semantic = InputSemantics.Vertex;
                        break;
                    case "normal":
                        Semantic = InputSemantics.Normal;
                        break;
                    case "texcoord":
                        Semantic = InputSemantics.Texcoord;
                        break;
                    case "textangent":
                        Semantic = InputSemantics.TexTangent;
                        break;
                    case "texbinormal":
                        Semantic = InputSemantics.TexBinormal;
                        break;
                }
            }

            public override string ToString()
            {
                string setstr = "";
                if ( Set != -1 ) setstr = Set.ToString();
                return "Collada Input: " + System.Enum.GetName( typeof( InputSemantics ), Semantic ) + " " + setstr + " - Source: " + Source.ID;
            }

        }




        // Oud
        //
        //
        // Oud
        //
        //
        // Oud


        public void AddSourceOud( SourceOud nSource )
        {
            sourcesOud.Add( nSource.ID, nSource );

        }







        private Dictionary<string, SourceOud> sourcesOud = new Dictionary<string, SourceOud>();

        private IXNAGame game;

        public IndexBuffer indexBuffer;
        public VertexBuffer vertexBuffer;
        public ColladaMaterial material;

        public string Name;

        public Matrix objectMatrix = Matrix.Identity;


        public List<Vector3> vertexSkinJoints = new List<Vector3>();
        public List<Vector3> vertexSkinWeights = new List<Vector3>();




        public Vector3[] Positions;
        private int positionsOffset;
        public Vector3[] Normals;
        private int normalsOffset;

        public Vector3[] TexCoords;
        private int texcoordsOffset;
        public Vector3[] Tangents;
        private int tangentsOffset;

        private int[] triangleIndices;
        private int vertexComponentCount;

        public ColladaMesh( IXNAGame _game, ColladaModel parentModel )
        {
            model = parentModel;
            game = _game;

        }


        public int GetVertexComponentIndex( int numVertex, int inputOffset )
        {
            return triangleIndices[ numVertex * vertexComponentCount + inputOffset ];
        }

        public void SetTriangleIndices( int[] indices, int nVertexComponentCount )
        {
            triangleIndices = indices;
            vertexComponentCount = nVertexComponentCount;
        }



        public void SetPositionsSource( SourceOud nSource, int offset )
        {
            Positions = new Vector3[ nSource.Data.Count / 3 ];
            for ( int i = 0; i < ( Positions.Length ); i++ )
            {
                Positions[ i ] = nSource.GetVector3( i );
            }

            positionsOffset = offset;
        }
        public void SetNormalsSource( SourceOud nSource, int offset )
        {
            Normals = new Vector3[ nSource.Data.Count / 3 ];
            for ( int i = 0; i < ( Normals.Length ); i++ )
            {
                Normals[ i ] = nSource.GetVector3( i );
            }
            normalsOffset = offset;
        }

        public void SetTexCoordsSource( SourceOud nSource, int offset )
        {
            TexCoords = new Vector3[ nSource.Data.Count / 3 ];
            for ( int i = 0; i < ( TexCoords.Length ); i++ )
            {
                TexCoords[ i ] = nSource.GetVector3( i );
            }

            texcoordsOffset = offset;
        }
        public void SetTangentsSource( SourceOud nSource, int offset )
        {
            Tangents = new Vector3[ nSource.Data.Count / 3 ];
            for ( int i = 0; i < ( Tangents.Length ); i++ )
            {
                Tangents[ i ] = nSource.GetVector3( i );
            }
            tangentsOffset = offset;
        }


        public int GetPositionIndex( int vertexIndex )
        {
            return GetVertexComponentIndex( vertexIndex, positionsOffset );
        }
        public Vector3 GetPosition( int vertexIndex )
        {
            return Positions[ GetVertexComponentIndex( vertexIndex, positionsOffset ) ];
        }
        public Vector3 GetNormal( int vertexIndex )
        {
            return Normals[ GetVertexComponentIndex( vertexIndex, normalsOffset ) ];
        }
        public Vector3 GetTexCoord( int vertexIndex )
        {
            return TexCoords[ GetVertexComponentIndex( vertexIndex, texcoordsOffset ) ];
        }
        public Vector3 GetTangent( int vertexIndex )
        {
            return Tangents[ GetVertexComponentIndex( vertexIndex, tangentsOffset ) ];
        }


        public SourceOud GetSourceOud( string id )
        {
            return sourcesOud[ id ];
        }

        public void SetObjectMatrix( Matrix value )
        {
            objectMatrix = value;

        }


        public List<TheWizards.Graphics.TangentVertex> vertices = new List<TheWizards.Graphics.TangentVertex>();
        public List<SkinnedTangentVertex> verticesSkinned = new List<SkinnedTangentVertex>();

        public int numOfVertices = 0,
            numOfIndices = 0;

        /// <summary>
        /// Helpers to remember how we can reuse vertices for OptimizeVertexBuffer.
        /// See below for more details.
        /// </summary>
        int[] reuseVertexPositions;
        /// <summary>
        /// Reverse reuse vertex positions, this one is even more important because
        /// this way we can get a list of used vertices for a shared vertex pos.
        /// </summary>
        List<int>[] reverseReuseVertexPositions;

        /// <summary>
        /// Load mesh geometry
        /// </summary>
        /// <param name="geometry"></param>
        public void LoadMeshGeometry( XmlNode colladaFile, XmlNode meshNode )
        {
            ColladaMesh.SourceOud source;

            // Load all source nodes
            //Dictionary<string, List<float>> sources = new Dictionary<string,
            //    List<float>>();
            foreach ( XmlNode node in meshNode )
            {
                if ( node.Name != "source" )
                    continue;
                XmlNode floatArray = XmlHelper.GetChildNode( node, "float_array" );
                int count = Convert.ToInt32( XmlHelper.GetXmlAttribute( floatArray,
                    "count" ), NumberFormatInfo.InvariantInfo );

                source = new ColladaMesh.SourceOud();
                source.ID = XmlHelper.GetXmlAttribute( node, "id" );

                if ( count == 0 )
                {
                    //No data available
                    source.Data = null;
                }
                else
                {
                    List<float> floats = new List<float>(
                        StringHelper.ConvertStringToFloatArray( floatArray.InnerText ) );

                    // Fill the array up

                    while ( floats.Count < count )
                        floats.Add( 0.0f );

                    source.Data = floats;
                }
                AddSourceOud( source );


            } // foreach


            // Vertices
            // Also add vertices node, redirected to position node into sources
            XmlNode verticesNode = XmlHelper.GetChildNode( meshNode, "vertices" );
            XmlNode posInput = XmlHelper.GetChildNode( verticesNode, "input" );
            if ( XmlHelper.GetXmlAttribute( posInput, "semantic" ).ToLower(
                CultureInfo.InvariantCulture ) != "position" )
                throw new InvalidOperationException(
                    "unsupported feature found in collada \"vertices\" node" );
            string verticesValueName = XmlHelper.GetXmlAttribute( posInput,
                "source" ).Substring( 1 );
            //sources.Add( XmlHelper.GetXmlAttribute( verticesNode, "id" ),
            //    sources[ verticesValueName ] );

            source = GetSourceOud( verticesValueName );

            source.ID = XmlHelper.GetXmlAttribute( verticesNode, "id" );

            AddSourceOud( source );


            // Construct all triangle polygons from the vertex data
            // Construct and generate vertices lists. Every 3 vertices will
            // span one triangle polygon, but everything is optimized later.



            foreach ( XmlNode trianglenode in meshNode )
            {
                if ( trianglenode.Name != "triangles" )
                    continue;

                material = model.GetMaterial( XmlHelper.GetXmlAttribute( trianglenode, "material" ) );


                // Find data source nodes
                XmlNode positionsnode = XmlHelper.GetChildNode( trianglenode,
                    "semantic", "VERTEX" );
                XmlNode normalsnode = XmlHelper.GetChildNode( trianglenode,
                    "semantic", "NORMAL" );
                XmlNode texcoordsnode = XmlHelper.GetChildNode( trianglenode,
                    "semantic", "TEXCOORD" );
                XmlNode tangentsnode = XmlHelper.GetChildNode( trianglenode,
                    "semantic", "TEXTANGENT" );

                if ( positionsnode == null ||
                    normalsnode == null ||
                    texcoordsnode == null ||
                    tangentsnode == null ) return;

                /*// Get the data of the sources
                List<float> positions = sources[ XmlHelper.GetXmlAttribute(
                    positionsnode, "source" ).Substring( 1 ) ];
                List<float> normals = sources[ XmlHelper.GetXmlAttribute( normalsnode,
                    "source" ).Substring( 1 ) ];
                List<float> texcoords = sources[ XmlHelper.GetXmlAttribute(
                    texcoordsnode, "source" ).Substring( 1 ) ];
                List<float> tangents = sources[ XmlHelper.GetXmlAttribute( tangentsnode,
                    "source" ).Substring( 1 ) ];*/




                // Find the Offsets
                int positionsoffset = Convert.ToInt32( XmlHelper.GetXmlAttribute(
                    positionsnode, "offset" ), NumberFormatInfo.InvariantInfo );
                int normalsoffset = Convert.ToInt32( XmlHelper.GetXmlAttribute(
                    normalsnode, "offset" ), NumberFormatInfo.InvariantInfo );
                int texcoordsoffset = Convert.ToInt32( XmlHelper.GetXmlAttribute(
                    texcoordsnode, "offset" ), NumberFormatInfo.InvariantInfo );
                int tangentsoffset = Convert.ToInt32( XmlHelper.GetXmlAttribute(
                    tangentsnode, "offset" ), NumberFormatInfo.InvariantInfo );


                SetPositionsSource( GetSourceOud( XmlHelper.GetXmlAttribute(
                     positionsnode, "source" ).Substring( 1 ) ), positionsoffset );
                SetNormalsSource( GetSourceOud( XmlHelper.GetXmlAttribute(
                        normalsnode, "source" ).Substring( 1 ) ), normalsoffset );

                SetTexCoordsSource( GetSourceOud( XmlHelper.GetXmlAttribute(
                        texcoordsnode, "source" ).Substring( 1 ) ), texcoordsoffset );
                SetTangentsSource( GetSourceOud( XmlHelper.GetXmlAttribute(
                        tangentsnode, "source" ).Substring( 1 ) ), tangentsoffset );

                // Get the indexlist
                XmlNode p = XmlHelper.GetChildNode( trianglenode, "p" );
                int[] pints = StringHelper.ConvertStringToIntArray( p.InnerText );
                int trianglecount = Convert.ToInt32( XmlHelper.GetXmlAttribute(
                    trianglenode, "count" ), NumberFormatInfo.InvariantInfo );
                // The number of ints that form one vertex:
                int vertexcomponentcount = pints.Length / trianglecount / 3;

                SetTriangleIndices( pints, vertexcomponentcount );

                LoadSkinData( colladaFile, meshNode );

                if ( vertexSkinJoints.Count > 0 )
                {
                    CreateSkinnedVertices( trianglecount );
                }
                else
                {
                    CreateVertices( trianglecount );
                }

                // Only support one mesh for now, get outta here.
                return;
            } // foreach (trianglenode)

            throw new InvalidOperationException(
                "No mesh found in this collada file, unable to continue!" );
            //
        } // LoadMeshGeometry(geometry)

        public void CreateVertices( int trianglecount )
        {
            // Construct data
            vertices.Clear();
            // Initialize reuseVertexPositions and reverseReuseVertexPositions
            // to make it easier to use them below


            reuseVertexPositions = new int[ trianglecount * 3 ];
            reverseReuseVertexPositions = new List<int>[ Positions.Length ];


            for ( int i = 0; i < reverseReuseVertexPositions.Length; i++ )
                reverseReuseVertexPositions[ i ] = new List<int>();

            // We have to use int indices here because we often have models
            // with more than 64k triangles (even if that gets optimized later).
            for ( int i = 0; i < trianglecount * 3; i++ )
            {
                Vector3 position = GetPosition( i );
                Vector3 normal = GetNormal( i );

                //// Get vertex blending stuff (uses pos too)
                //Vector3 blendWeights = vertexSkinWeights[ pos / 3 ];
                //Vector3 blendIndices = vertexSkinJoints[ pos / 3 ];



                //// Pre-multiply all indices with 3, this way the shader
                //// code gets a little bit simpler and faster
                //blendIndices = new Vector3(
                //    blendIndices.X * 3, blendIndices.Y * 3, blendIndices.Z * 3 );




                //// Texture Coordinates
                //int tex = pints[ i * vertexcomponentcount + texcoordsoffset ] * 3;
                //float u = texcoords[ tex ];
                //// V coordinate is inverted in max
                //float v = 1.0f - texcoords[ tex + 1 ];
                Vector3 texcoord = GetTexCoord( i );
                texcoord.Y = 1.0f - texcoord.Y;

                Vector3 tangent = GetTangent( i );

                //// Tangent
                //int tan = pints[ i * vertexcomponentcount + tangentsoffset ] * 3;
                //Vector3 tangent = new Vector3(
                //    tangents[ tan ], tangents[ tan + 1 ], tangents[ tan + 2 ] );

                //// Set the vertex
                ////vertices.Add( new SkinnedTangentVertex(
                ////    position, blendWeights, blendIndices, u, v, normal, tangent ) );

                //vertices.Add( new TangentVertex( position, u, v, normal, tangent ) );
                vertices.Add( new TheWizards.Graphics.TangentVertex( position, texcoord.X, texcoord.Y, normal, tangent ) );
                // Remember pos for optimizing the vertices later more easily.
                reuseVertexPositions[ i ] = GetPositionIndex( i );// pos / 3;
                reverseReuseVertexPositions[ GetPositionIndex( i ) ].Add( i );
                //reverseReuseVertexPositions[ pos / 3 ].Add( i );
            } // for (ushort)
        }
        public void CreateSkinnedVertices( int trianglecount )
        {
            // Construct data
            verticesSkinned.Clear();
            // Initialize reuseVertexPositions and reverseReuseVertexPositions
            // to make it easier to use them below


            reuseVertexPositions = new int[ trianglecount * 3 ];
            reverseReuseVertexPositions = new List<int>[ Positions.Length ];


            for ( int i = 0; i < reverseReuseVertexPositions.Length; i++ )
                reverseReuseVertexPositions[ i ] = new List<int>();

            // We have to use int indices here because we often have models
            // with more than 64k triangles (even if that gets optimized later).
            for ( int i = 0; i < trianglecount * 3; i++ )
            {
                Vector3 position = GetPosition( i );
                Vector3 normal = GetNormal( i );

                // Get vertex blending stuff (uses pos too)
                //Vector3 blendWeights = vertexSkinWeights[ pos / 3 ];
                //Vector3 blendIndices = vertexSkinJoints[ pos / 3 ];
                Vector3 blendWeights = vertexSkinWeights[ GetPositionIndex( i ) ];
                Vector3 blendIndices = vertexSkinJoints[ GetPositionIndex( i ) ];


                // Pre-multiply all indices with 3, this way the shader
                // code gets a little bit simpler and faster
                blendIndices = new Vector3(
                    blendIndices.X * 3, blendIndices.Y * 3, blendIndices.Z * 3 );




                //// Texture Coordinates
                //int tex = pints[ i * vertexcomponentcount + texcoordsoffset ] * 3;
                //float u = texcoords[ tex ];
                //// V coordinate is inverted in max
                //float v = 1.0f - texcoords[ tex + 1 ];
                Vector3 texcoord = GetTexCoord( i );
                texcoord.Y = 1.0f - texcoord.Y;

                Vector3 tangent = GetTangent( i );

                //// Tangent
                //int tan = pints[ i * vertexcomponentcount + tangentsoffset ] * 3;
                //Vector3 tangent = new Vector3(
                //    tangents[ tan ], tangents[ tan + 1 ], tangents[ tan + 2 ] );

                //// Set the vertex
                verticesSkinned.Add( new SkinnedTangentVertex(
                    position, blendWeights, blendIndices, texcoord.X, texcoord.Y, normal, tangent ) );

                //vertices.Add( new TangentVertex( position, u, v, normal, tangent ) );
                //vertices.Add( new TangentVertex( position, texcoord.X, texcoord.Y, normal, tangent ) );
                // Remember pos for optimizing the vertices later more easily.
                reuseVertexPositions[ i ] = GetPositionIndex( i );// pos / 3;
                reverseReuseVertexPositions[ GetPositionIndex( i ) ].Add( i );
                //reverseReuseVertexPositions[ pos / 3 ].Add( i );
            } // for (ushort)
        }

        public void LoadSkinData( XmlNode colladaFile, XmlNode meshNode )
        {
            XmlNode library_controllers = XmlHelper.GetChildNode( colladaFile, "library_controllers" );
            if ( library_controllers == null ) return;
            XmlNode skinNode = null;
            foreach ( XmlNode controller in library_controllers )
            {
                if ( controller.Name != "controller" ) continue;
                skinNode = XmlHelper.GetChildNode( controller, "skin" );
                if ( skinNode == null ) continue;
                if ( XmlHelper.GetXmlAttribute( skinNode, "source" ).Substring( 1 ) == Name )
                {
                    break;
                }
                else
                {
                    skinNode = null;
                    continue;
                }
            }

            if ( skinNode == null ) return;

            // Load helper matrixes (bind shape and all bind poses matrices)
            // We only support 1 skinning, so just load the first skin node!
            /*XmlNode skinNode = XmlHelper.GetChildNode(
                XmlHelper.GetChildNode(
                XmlHelper.GetChildNode( colladaFile, "library_controllers" ),
                "controller" ), "skin" );*/
            objectMatrix = ColladaModel.LoadColladaMatrixOud(
                StringHelper.ConvertStringToFloatArray(
                XmlHelper.GetChildNode( skinNode, "bind_shape_matrix" ).InnerText ), 0 );

            // Get the order of the bones used in collada (can be different than ours)
            int[] boneArrayOrder = new int[ model.bones.Count ];
            int[] invBoneArrayOrder = new int[ model.bones.Count ];
            string boneNameArray =
                XmlHelper.GetChildNode( skinNode, "Name_array" ).InnerText;
            int arrayIndex = 0;
            foreach ( string boneName in boneNameArray.Split( ' ' ) )
            {
                boneArrayOrder[ arrayIndex ] = -1;
                invBoneArrayOrder[ arrayIndex ] = -1;
                foreach ( ColladaBone bone in model.bones )
                    if ( bone.sid == boneName )
                    {
                        boneArrayOrder[ arrayIndex ] = bone.num;
                        invBoneArrayOrder[ bone.num ] = arrayIndex;
                        break;
                    } // foreach

                if ( boneArrayOrder[ arrayIndex ] == -1 )
                    throw new InvalidOperationException(
                        "Unable to find boneName=" + boneName +
                        " in our bones array for skinning!" );
                arrayIndex++;
            } // foreach
            //

            // Load weights
            float[] weights = null;
            foreach ( XmlNode sourceNode in skinNode )
            {
                // Get all inv bone skin matrices
                if ( sourceNode.Name == "source" &&
                    XmlHelper.GetXmlAttribute( sourceNode, "id" ).Contains( "bind_poses" ) )
                {
                    // Get inner float array
                    float[] mat = StringHelper.ConvertStringToFloatArray(
                        XmlHelper.GetChildNode( sourceNode, "float_array" ).InnerText );
                    for ( int boneNum = 0; boneNum < model.bones.Count; boneNum++ )
                        if ( mat.Length / 16 > boneNum )
                        {
                            model.bones[ boneArrayOrder[ boneNum ] ].invBoneSkinMatrix =
                                ColladaModel.LoadColladaMatrixOud( mat, boneNum * 16 );
                        } // for if
                } // if

                // Get all weights
                if ( sourceNode.Name == "source" &&
                    XmlHelper.GetXmlAttribute( sourceNode, "id" ).Contains( "skin-weights" ) )
                {
                    // Get inner float array
                    weights = StringHelper.ConvertStringToFloatArray(
                        XmlHelper.GetChildNode( sourceNode, "float_array" ).InnerText );
                } // if
            } // foreach

            if ( weights == null )
                throw new InvalidOperationException(
                    "No weights were found in our skin, unable to continue!" );
            //

            // Prepare weights and joint indices, we only want the top 3!
            // Helper to access the bones (first index) and weights (second index).
            // If we got more than 2 indices for an entry here, there are multiple
            // weights used (usually 1 to 3, if more, we only use the strongest).
            XmlNode vertexWeightsNode =
                XmlHelper.GetChildNode( skinNode, "vertex_weights" );
            int[] vcountArray = StringHelper.ConvertStringToIntArray(
                XmlHelper.GetChildNode( skinNode, "vcount" ).InnerText );
            int[] vArray = StringHelper.ConvertStringToIntArray(
                XmlHelper.GetChildNode( skinNode, "v" ).InnerText );

            // Build vertexSkinJoints and vertexSkinWeights for easier access.

            int vArrayIndex = 0;
            for ( int num = 0; num < vcountArray.Length; num++ )
            {
                int vcount = vcountArray[ num ];
                List<int> jointIndices = new List<int>();
                List<int> weightIndices = new List<int>();
                for ( int i = 0; i < vcount; i++ )
                {
                    // Make sure we convert the internal number to our bone numbers!
                    jointIndices.Add( boneArrayOrder[ vArray[ vArrayIndex ] ] );
                    weightIndices.Add( vArray[ vArrayIndex + 1 ] );
                    vArrayIndex += 2;
                } // for

                // If we got less than 3 values, add until we have enough,
                // this makes the computation easier below
                while ( jointIndices.Count < 3 )
                    jointIndices.Add( 0 );
                while ( weightIndices.Count < 3 )
                    weightIndices.Add( -1 );

                // Find out top 3 weights
                float[] weightValues = new float[ weightIndices.Count ];
                int[] bestWeights = { 0, 1, 2 };
                for ( int i = 0; i < weightIndices.Count; i++ )
                {
                    // Use weight of zero for invalid indices.
                    if ( weightIndices[ i ] < 0 ||
                        weightValues[ i ] >= weights.Length )
                        weightValues[ i ] = 0;
                    else
                        weightValues[ i ] = weights[ weightIndices[ i ] ];

                    // Got 4 or more weights? Then just select the top 3!
                    if ( i >= 3 )
                    {
                        float lowestWeight = 1.0f;
                        int lowestWeightOverride = 2;
                        for ( int b = 0; b < bestWeights.Length; b++ )
                            if ( lowestWeight > weightValues[ bestWeights[ b ] ] )
                            {
                                lowestWeight = weightValues[ bestWeights[ b ] ];
                                lowestWeightOverride = b;
                            } // for if
                        // Replace lowest weight
                        bestWeights[ lowestWeightOverride ] = i;
                    } // if
                } // for

                // Now build 2 vectors from the best weights
                Vector3 boneIndicesVec = new Vector3(
                    jointIndices[ bestWeights[ 0 ] ],
                    jointIndices[ bestWeights[ 1 ] ],
                    jointIndices[ bestWeights[ 2 ] ] );
                Vector3 weightsVec = new Vector3(
                    weightValues[ bestWeights[ 0 ] ],
                    weightValues[ bestWeights[ 1 ] ],
                    weightValues[ bestWeights[ 2 ] ] );
                // Renormalize weight, important if we got more entries before
                // and always good to do!
                float totalWeights = weightsVec.X + weightsVec.Y + weightsVec.Z;
                if ( totalWeights == 0 )
                    weightsVec.X = 1.0f;
                else
                {
                    weightsVec.X /= totalWeights;
                    weightsVec.Y /= totalWeights;
                    weightsVec.Z /= totalWeights;
                } // else

                vertexSkinJoints.Add( boneIndicesVec );
                vertexSkinWeights.Add( weightsVec );
            } // for
            //
        }

        /// <summary>
        /// Little helper method to flip indices from 0, 1, 2 to 0, 2, 1.
        /// This way we can render with CullClockwiseFace (default for XNA).
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <returns></returns>
        private int FlipIndexOrder( int oldIndex )
        {
            int polygonIndex = oldIndex % 3;
            if ( polygonIndex == 0 )
                return oldIndex;
            else if ( polygonIndex == 1 )
                return (ushort)( oldIndex + 1 );
            else //if (polygonIndex == 2)
                return (ushort)( oldIndex - 1 );
        } // FlipIndexOrder(oldIndex)

        /// <summary>
        /// Optimize vertex buffer. Note: The vertices list array will be changed
        /// and shorted quite a lot here. We are also going to create the indices
        /// for the index buffer here (we don't have them yet, they are just
        /// sequential from the loading process above).
        /// 
        /// Note: This method is highly optimized for speed, it performs
        /// hundred of times faster than OptimizeVertexBufferSlow, see below!
        /// </summary>
        /// <returns>ushort array for the optimized indices</returns>
        private ushort[] OptimizeVertexBuffer()
        {
            List<TheWizards.Graphics.TangentVertex> newVertices =
                new List<TheWizards.Graphics.TangentVertex>();
            List<ushort> newIndices = new List<ushort>();

            // Helper to only search already added newVertices and for checking the
            // old position indices by transforming them into newVertices indices.
            List<int> newVerticesPositions = new List<int>();

            // Go over all vertices (indices are currently 1:1 with the vertices)
            for ( int num = 0; num < vertices.Count; num++ )
            {
                // Get current vertex
                TheWizards.Graphics.TangentVertex currentVertex = vertices[ num ];
                bool reusedExistingVertex = false;

                // Find out which position index was used, then we can compare
                // all other vertices that share this position. They will not
                // all be equal, but some of them can be merged.
                int sharedPos = reuseVertexPositions[ num ];
                foreach ( int otherVertexIndex in reverseReuseVertexPositions[ sharedPos ] )
                {
                    // Only check the indices that have already been added!
                    if ( otherVertexIndex != num &&
                        // Make sure we already are that far in our new index list
                        otherVertexIndex < newIndices.Count &&
                        // And make sure this index has been added to newVertices yet!
                        newIndices[ otherVertexIndex ] < newVertices.Count &&
                        // Then finally compare vertices (this call is slow, but thanks to
                        // all the other optimizations we don't have to call it that often)
                        TheWizards.Graphics.TangentVertex.NearlyEquals(
                        currentVertex, newVertices[ newIndices[ otherVertexIndex ] ] ) )
                    {
                        // Reuse the existing vertex, don't add it again, just
                        // add another index for it!
                        newIndices.Add( (ushort)newIndices[ otherVertexIndex ] );
                        reusedExistingVertex = true;
                        break;
                    } // if (TangentVertex.NearlyEquals)
                } // foreach (otherVertexIndex)

                if ( reusedExistingVertex == false )
                {
                    // Add the currentVertex and set it as the current index
                    newIndices.Add( (ushort)newVertices.Count );
                    newVertices.Add( currentVertex );
                } // if (reusedExistingVertex)
            } // for (num)

            // Finally flip order of all triangles to allow us rendering
            // with CullCounterClockwiseFace (default for XNA) because all the data
            // is in CullClockwiseFace format right now!
            for ( int num = 0; num < newIndices.Count / 3; num++ )
            {
                ushort swap = newIndices[ num * 3 + 1 ];
                newIndices[ num * 3 + 1 ] = newIndices[ num * 3 + 2 ];
                newIndices[ num * 3 + 2 ] = swap;
            } // for

            // Reassign the vertices, we might have deleted some duplicates!
            vertices = newVertices;

            // And return index list for the caller
            return newIndices.ToArray();
        } // OptimizeVertexBuffer()
        /// <summary>
        /// Generate vertex and index buffers
        /// </summary>
        public void GenerateVertexAndIndexBuffers()
        {
            if ( vertices.Count == 0 ) return;

            // Optimize vertices first and build index buffer from that!
            ushort[] indices = OptimizeVertexBuffer();
            // For testing, this one is REALLY slow (see method summary)!
            //ushort[] indices = OptimizeVertexBufferSlow();

            // Create the vertex buffer from our vertices.
            vertexBuffer = new VertexBuffer(
                game.GraphicsDevice,
                //typeof( SkinnedTangentVertex ),
                typeof( TheWizards.Graphics.TangentVertex ),
                vertices.Count, BufferUsage.WriteOnly );
            //ResourceUsage.WriteOnly,
            //ResourceManagementMode.Automatic );
            vertexBuffer.SetData( vertices.ToArray() );
            numOfVertices = vertices.Count;

            // We only support max. 65535 optimized vertices, which is really a
            // lot, but more would require a int index buffer (twice as big) and
            // I have never seen any realtime 3D model that needs more vertices ^^
            if ( vertices.Count > ushort.MaxValue )
                throw new InvalidOperationException(
                    "Too much vertices to index, optimize vertices or use " +
                    "fewer vertices. Vertices=" + vertices.Count +
                    ", Max Vertices for Index Buffer=" + ushort.MaxValue );

            // Create the index buffer from our indices (Note: While the indices
            // will point only to 16bit (ushort) vertices, we can have a lot
            // more indices in this list than just 65535).
            indexBuffer = new IndexBuffer(
                game.GraphicsDevice,
                typeof( ushort ),
                indices.Length, BufferUsage.WriteOnly );
            //ResourceUsage.WriteOnly, 
            //ResourceManagementMode.Automatic );
            indexBuffer.SetData( indices );
            numOfIndices = indices.Length;
        } // GenerateVertexAndIndexBuffers()


        /// <summary>
        /// Optimize vertex buffer. Note: The vertices list array will be changed
        /// and shorted quite a lot here. We are also going to create the indices
        /// for the index buffer here (we don't have them yet, they are just
        /// sequential from the loading process above).
        /// 
        /// Note: This method is highly optimized for speed, it performs
        /// hundred of times faster than OptimizeVertexBufferSlow, see below!
        /// </summary>
        /// <returns>ushort array for the optimized indices</returns>
        private ushort[] OptimizeVertexBufferSkinned()
        {
            List<SkinnedTangentVertex> newVertices =
                new List<SkinnedTangentVertex>();
            List<ushort> newIndices = new List<ushort>();

            // Helper to only search already added newVertices and for checking the
            // old position indices by transforming them into newVertices indices.
            List<int> newVerticesPositions = new List<int>();

            // Go over all vertices (indices are currently 1:1 with the vertices)
            for ( int num = 0; num < verticesSkinned.Count; num++ )
            {
                // Get current vertex
                SkinnedTangentVertex currentVertex = verticesSkinned[ num ];
                bool reusedExistingVertex = false;

                // Find out which position index was used, then we can compare
                // all other vertices that share this position. They will not
                // all be equal, but some of them can be merged.
                int sharedPos = reuseVertexPositions[ num ];
                foreach ( int otherVertexIndex in reverseReuseVertexPositions[ sharedPos ] )
                {
                    // Only check the indices that have already been added!
                    if ( otherVertexIndex != num &&
                        // Make sure we already are that far in our new index list
                        otherVertexIndex < newIndices.Count &&
                        // And make sure this index has been added to newVertices yet!
                        newIndices[ otherVertexIndex ] < newVertices.Count &&
                        // Then finally compare vertices (this call is slow, but thanks to
                        // all the other optimizations we don't have to call it that often)
                        SkinnedTangentVertex.NearlyEquals(
                        currentVertex, newVertices[ newIndices[ otherVertexIndex ] ] ) )
                    {
                        // Reuse the existing vertex, don't add it again, just
                        // add another index for it!
                        newIndices.Add( (ushort)newIndices[ otherVertexIndex ] );
                        reusedExistingVertex = true;
                        break;
                    } // if (TangentVertex.NearlyEquals)
                } // foreach (otherVertexIndex)

                if ( reusedExistingVertex == false )
                {
                    // Add the currentVertex and set it as the current index
                    newIndices.Add( (ushort)newVertices.Count );
                    newVertices.Add( currentVertex );
                } // if (reusedExistingVertex)
            } // for (num)

            // Finally flip order of all triangles to allow us rendering
            // with CullCounterClockwiseFace (default for XNA) because all the data
            // is in CullClockwiseFace format right now!
            for ( int num = 0; num < newIndices.Count / 3; num++ )
            {
                ushort swap = newIndices[ num * 3 + 1 ];
                newIndices[ num * 3 + 1 ] = newIndices[ num * 3 + 2 ];
                newIndices[ num * 3 + 2 ] = swap;
            } // for

            // Reassign the vertices, we might have deleted some duplicates!
            verticesSkinned = newVertices;

            // And return index list for the caller
            return newIndices.ToArray();
        } // OptimizeVertexBuffer()



        /// <summary>
        /// Generate vertex and index buffers
        /// </summary>
        public void GenerateVertexAndIndexBuffersSkinned()
        {
            if ( verticesSkinned.Count == 0 ) return;

            // Optimize vertices first and build index buffer from that!
            ushort[] indices = OptimizeVertexBufferSkinned();
            // For testing, this one is REALLY slow (see method summary)!
            //ushort[] indices = OptimizeVertexBufferSlow();

            // Create the vertex buffer from our vertices.
            vertexBuffer = new VertexBuffer(
                game.GraphicsDevice,
                //typeof( SkinnedTangentVertex ),
                typeof( SkinnedTangentVertex ),
                verticesSkinned.Count, BufferUsage.WriteOnly );
            //ResourceUsage.WriteOnly,
            //ResourceManagementMode.Automatic );
            vertexBuffer.SetData( verticesSkinned.ToArray() );
            numOfVertices = verticesSkinned.Count;

            // We only support max. 65535 optimized vertices, which is really a
            // lot, but more would require a int index buffer (twice as big) and
            // I have never seen any realtime 3D model that needs more vertices ^^
            if ( verticesSkinned.Count > ushort.MaxValue )
                throw new InvalidOperationException(
                    "Too much vertices to index, optimize vertices or use " +
                    "fewer vertices. Vertices=" + verticesSkinned.Count +
                    ", Max Vertices for Index Buffer=" + ushort.MaxValue );

            // Create the index buffer from our indices (Note: While the indices
            // will point only to 16bit (ushort) vertices, we can have a lot
            // more indices in this list than just 65535).
            indexBuffer = new IndexBuffer(
                game.GraphicsDevice,
                typeof( ushort ),
                indices.Length, BufferUsage.WriteOnly );
            //ResourceUsage.WriteOnly, 
            //ResourceManagementMode.Automatic );
            indexBuffer.SetData( indices );
            numOfIndices = indices.Length;
        } // GenerateVertexAndIndexBuffers()



        /// <summary>
        /// Render the animated model (will call UpdateAnimation internally,
        /// but if you do that yourself before calling this method, it gets
        /// optimized out). Rendering always uses the skinnedNormalMapping shader
        /// with the DiffuseSpecular20 technique.
        /// </summary>
        /// <param name="renderMatrix">Render matrix</param>
        public void Render( Matrix renderMatrix, ShaderEffect effect )
        {
            if ( vertexBuffer == null || indexBuffer == null ) return;

            // Make sure we use the correct vertex declaration for our shader.
            //game.GraphicsDevice.VertexDeclaration = TangentVertex.VertexDeclaration;

            // Set the world matrix for this object (often Identity).
            // The renderMatrix is directly applied to the matrices we use
            // as bone matrices for the shader (has to be done this way because
            // the bone matrices are transmitted transposed and we could lose
            // important render matrix translation data if we do not apply it there).
            //BaseGame.WorldMatrix = objectMatrix;
            // And set all bone matrices (Goblin has 40, but we support up to 80).
            //ShaderEffect.skinnedNormalMapping.SetBoneMatrices(
            //    GetBoneMatrices( renderMatrix ) );

            // Rendering is pretty straight forward (if you know how anyway).
            //ShaderEffect.skinnedNormalMapping.Render(
            //    material, "DiffuseSpecular20",
            //    RenderVertices );

            Matrix worldMatrix =

                //Matrix.CreateFromYawPitchRoll( -MathHelper.PiOver2, -MathHelper.PiOver2, 0 ) *
                //Matrix.CreateFromYawPitchRoll( 0, -MathHelper.PiOver2, 0 ) 
                renderMatrix;
            //* Matrix.CreateFromYawPitchRoll( -MathHelper.PiOver2, -MathHelper.PiOver2, 0 );

















            //
            //
            //
            //TODO: commented out
            //
            //          effect.SetParametersCollada( material );
            //
            //
            //

























            //effect.ViewProjMatrix = model.Game.Camera.ViewProjection;


            //Microsoft.Xna.Framework.Graphics.SamplerState s = new SamplerState();
            //object o = effect.Effect.Parameters[ "DiffuseTextureSampler " ];


            if ( verticesSkinned.Count > 0 )
            {
                /*worldMatrix = Matrix.Identity;
                worldMatrix = model.bones[ 0 ].invBoneSkinMatrix * model.bones[ 0 ].initialMatrix;*/
                /*worldMatrix = objectMatrix;
                effect.WorldMatrix = worldMatrix;
                effect.WorldViewProjMatrix = worldMatrix * model.Game.Camera.ViewProjection;
                worldMatrix = Matrix.Identity;*/
                /*game.GraphicsDevice.VertexDeclaration = SkinnedTangentVertex.VertexDeclaration;


                //SetBoneMatrices( effect, model.GetBoneMatrices( worldMatrix ) );



                effect.RenderCollada( "SpecularPerPixelColored", RenderVerticesSkinned );*/


                effect.Effect.Parameters[ "viewProj" ].SetValue( model.Game.Camera.ViewProjection );
                effect.Effect.Parameters[ "world" ].SetValue( objectMatrix );
                //Matrix mat = Matrix.Identity;
                //mat.Translation = new Vector3( 1, 0, 0 );
                //effect.Effect.Parameters[ "world" ].SetValue( mat  );
                effect.Effect.Parameters[ "cameraPos" ].SetValue( model.Game.Camera.ViewInverse.Translation );



                if ( vertexDeclaration == null ) vertexDeclaration = SkinnedTangentVertex.CreateVertexDeclaration( game );
                game.GraphicsDevice.VertexDeclaration = vertexDeclaration;
                SetBoneMatrices( effect, model.GetBoneMatrices( renderMatrix ) );

                if ( material.DiffuseTexture == null )
                {

                    effect.RenderCollada( "DiffuseSpecularColored20", RenderVerticesSkinned );
                }
                else
                {
                    effect.RenderCollada( "DiffuseSpecular20", RenderVerticesSkinned );
                }

            }
            else
            {
                worldMatrix = objectMatrix * worldMatrix;
                effect.WorldMatrix = worldMatrix;
                effect.WorldViewProjMatrix = worldMatrix * model.Game.Camera.ViewProjection;
                if (vertexDeclaration == null) vertexDeclaration = TangentVertexExtensions.CreateVertexDeclaration(game);
                game.GraphicsDevice.VertexDeclaration = vertexDeclaration;

                if ( material.DiffuseTexture == null )
                {

                    effect.RenderCollada( "SpecularPerPixelColored", RenderVertices );
                }
                else if ( material.NormalTexture == null || game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Y ) )
                {
                    effect.RenderCollada( "SpecularPerPixel", RenderVertices );
                }
                else
                {
                    if ( game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.N ) )
                    {
                        effect.RenderCollada( "SpecularPerPixel", RenderVertices );
                    }
                    else
                    {
                        //Normal Mapping
                        effect.RenderCollada( "SpecularPerPixelNormalMapping", RenderVertices );
                    }
                }
            }
        } // Render(renderMatrix)

        public void SetBoneMatrices( ShaderEffect effect, Matrix[] matrices )
        {

            Vector4[] values = new Vector4[ matrices.Length * 3 ];
            for ( int i = 0; i < matrices.Length; i++ )
            {
                //if ( !( matrices[ i ].M14 == 0 && matrices[ i ].M24 == 0 && matrices[ i ].M34 == 0 && matrices[ i ].M44 == 1 ) ) throw new Exception();



                // Note: We use the transpose matrix here.
                // This has to be reconstructed in the shader, but this is not
                // slower than directly using matrices and this is the only way
                // we can store 80 matrices with ps2.0.
                values[ i * 3 + 0 ] = new Vector4(
                    matrices[ i ].M11, matrices[ i ].M21, matrices[ i ].M31, matrices[ i ].M41 );
                values[ i * 3 + 1 ] = new Vector4(
                    matrices[ i ].M12, matrices[ i ].M22, matrices[ i ].M32, matrices[ i ].M42 );
                values[ i * 3 + 2 ] = new Vector4(
                    matrices[ i ].M13, matrices[ i ].M23, matrices[ i ].M33, matrices[ i ].M43 );
            } // for
            effect.Effect.Parameters[ "skinnedMatricesVS20" ].SetValue( values );
        } // SetBoneMatrices(matrices)

        /// <summary>
        /// Render vertices
        /// </summary>
        private void RenderVertices()
        {


            game.GraphicsDevice.Vertices[ 0 ].SetSource( vertexBuffer, 0,
                TheWizards.Graphics.TangentVertex.SizeInBytes );
            game.GraphicsDevice.Indices = indexBuffer;
            game.GraphicsDevice.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0, 0, numOfVertices,
                0, numOfIndices / 3 );
        } // RenderVertices()
        public void RenderVerticesSkinned()
        {


            game.GraphicsDevice.Vertices[ 0 ].SetSource( vertexBuffer, 0,
                SkinnedTangentVertex.SizeInBytes );
            game.GraphicsDevice.Indices = indexBuffer;
            game.GraphicsDevice.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0, 0, numOfVertices,
                0, numOfIndices / 3 );
        } // RenderVertices()


        public ColladaMaterial Material { get { return material; } }



        public class SourceOud
        {
            public string ID;
            public List<float> Data;


            public Vector3 GetVector3( int index )
            {
                return new Vector3( Data[ index * 3 ], Data[ index * 3 + 1 ], Data[ index * 3 + 2 ] );
            }

        }


    }
}
