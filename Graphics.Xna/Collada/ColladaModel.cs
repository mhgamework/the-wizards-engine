using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MHGameWork.TheWizards.Common.Core;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics.TODO;
using MHGameWork.TheWizards.Graphics.Xna.XML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics.Xna.Collada
{
    public class ColladaModel
    {
        GameFile sourceFile;

        public GameFile File
        {
            get { return sourceFile; }
        }

        public ColladaAsset Asset = new ColladaAsset();
        public List<ColladaTexture> textures = new List<ColladaTexture>();
        public List<ColladaMaterial> materials = new List<ColladaMaterial>();
        private List<ColladaMesh> meshes = new List<ColladaMesh>();
        private ColladaScene scene;
        public List<ColladaBone> bones = new List<ColladaBone>();



        private ColladaModel()
        {


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">This GameFile will probably always be a Debug Gamefile, that is a Gamefile not managed by the engine</param>
        /// <returns></returns>
        public static ColladaModel FromFile( GameFile file )
        {
            ColladaModel colladaModel = new ColladaModel();

            colladaModel.Load( file );


            return colladaModel;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">This GameFile will probably always be a Debug Gamefile, that is a Gamefile not managed by the engine</param>
        /// <returns></returns>
        public static ColladaModel FromStream( Stream strm )
        {
            ColladaModel colladaModel = new ColladaModel();

            colladaModel.Load( strm );


            return colladaModel;
        }


        public void Load( GameFile nFile )
        {
            sourceFile = nFile;

            using ( FileStream fs = new FileStream( sourceFile.GetFullFilename(), FileMode.Open ) )
            {
                Load( fs );
            }

        }
        public void Load( Stream inputStream )
        {
            if ( inputStream == null ) throw new ArgumentNullException( "Stream can not be null!" );

            string colladaXml = null;
            XmlNode colladaFile = null;

            using ( StreamReader strm = new StreamReader( inputStream ) )
            {
                colladaXml = strm.ReadToEnd();
            }
            colladaFile = XmlHelper.LoadXmlFromText( colladaXml );

            LoadTextures( colladaFile );

            // Load Materials
            LoadMaterials( colladaFile );

            // Load bones
            LoadBones( colladaFile );

            // Load mesh (vertices data, combine with bone weights)
            // Now working with inputs?
            LoadMeshes( colladaFile );

            // And finally load bone animation data
            LoadAnimation( colladaFile );

            // Load the scene
            LoadScene( colladaFile );
        }



        private void LoadAsset( XmlNode colladaFile )
        {
            TWXmlNode colladaNode = TWXmlNode.FromXmlNode(colladaFile );
            TWXmlNode assetNode = colladaNode.FindChildNode( "asset" );
            if ( assetNode == null ) return;

            Asset.Load( assetNode );
        }
        private void LoadMeshes( XmlNode colladaFile )
        {
            XmlNode geometrys =
               XmlHelper.GetChildNode( colladaFile, "library_geometries" );
            if ( geometrys == null )
                throw new InvalidOperationException(
                    "library_geometries node not found in collada file " + filename );

            ColladaMesh mesh;

            foreach ( XmlNode geometry in geometrys )
                if ( geometry.Name == "geometry" )
                {
                    XmlNode meshNode = XmlHelper.GetChildNode( geometry, "mesh" );
                    if ( meshNode == null )
                    {
                        //This geometry is not a mesh! skip!
                        continue;
                    }
                    mesh = new ColladaMesh( this );

                    mesh.Name = XmlHelper.GetXmlAttribute( geometry, "id" );




                    // Load everything from the mesh node
                    mesh.LoadMesh( colladaFile, meshNode );


                    //if ( mesh.verticesSkinned.Count > 0 )
                    //{
                    //    mesh.GenerateVertexAndIndexBuffersSkinned();
                    //}
                    //else
                    //{
                    //    // Generate vertex buffer for rendering
                    //    mesh.GenerateVertexAndIndexBuffers();
                    //}



                    //( Get outa here, we currently only support one single mesh!)
                    //MHGW: Not Anymore! Multiple meshes supported

                    meshes.Add( mesh );
                } // foreach if (geometry.name)
        }
        private void LoadTextures( XmlNode colladaFile )
        {
            XmlNode library_images = XmlHelper.GetChildNode( colladaFile, "library_images" );
            if ( library_images == null ) return;
            ColladaTexture tex;

            foreach ( XmlNode image in library_images )
            {
                if ( image.Name == "image" )
                {
                    tex = new ColladaTexture( this );
                    tex.Name = XmlHelper.GetXmlAttribute( image, "id" );
                    tex.Filename = XmlHelper.GetChildNode( image, "init_from" ).InnerText;


                    textures.Add( tex );

                }
            }
        }
        private void LoadMaterials( XmlNode colladaFile )
        {
            XmlNode library_materials = XmlHelper.GetChildNode( colladaFile, "library_materials" );

            ColladaMaterial mat;

            foreach ( XmlNode material in library_materials )
            {
                if ( material.Name == "material" )
                {
                    // Load everything from the mesh node
                    mat = LoadMaterial( colladaFile, material );

                    materials.Add( mat );

                }
            }
        }
        private void LoadScene( XmlNode colladaFile )
        {
            XmlNode library_visual_scenes = XmlHelper.GetChildNode( colladaFile, "library_visual_scenes" );
            XmlNode visual_scene = XmlHelper.GetChildNode( library_visual_scenes, "visual_scene" );

            scene = new ColladaScene( this );
            scene.LoadScene( visual_scene );


        }

        private ColladaMaterial LoadMaterial( XmlNode colladaFile, XmlNode materialNode )
        {
            ColladaMaterial mat = new ColladaMaterial();
            foreach ( XmlNode node in materialNode )
            {
                if ( node.Name == "instance_effect" )
                {
                    string effectName = node.Attributes[ "url" ].Value.Substring( 1 );
                    XmlNode effect = null;
                    XmlNode library_effects = XmlHelper.FindNode( colladaFile, "library_effects" );
                    foreach ( XmlNode iEffect in library_effects )
                    {
                        if ( iEffect.Name == "effect" && iEffect.Attributes[ "id" ].Value == effectName )
                        {
                            effect = iEffect;
                            break;
                        }
                    }



                    mat.Name = materialNode.Attributes[ "id" ].Value;

                    XmlNode profile_COMMON = XmlHelper.GetChildNode( effect, "profile_COMMON" );

                    XmlNode technique = XmlHelper.GetChildNode( effect, "technique" );
                    if ( XmlHelper.GetXmlAttribute( technique, "sid" ) != "common" )
                        throw new Exception( "Unsupported feature!" );

                    foreach ( XmlNode subNode in technique )
                    {

                        if ( subNode.Name == "phong" || subNode.Name == "blinn" )
                        {
                            switch ( subNode.Name )
                            {
                                case "phong:":
                                    mat.Type = ColladaMaterial.MaterialType.Phong;
                                    break;
                                case "blinn":
                                    mat.Type = ColladaMaterial.MaterialType.Blinn;
                                    break;

                            }

                            //XmlNode phong = XmlHelper.GetChildNode( effect, "phong" );

                            float[] vec;
                            XmlNode colorNode;

                            foreach ( XmlNode param in subNode )
                            {
                                switch ( param.Name )
                                {
                                    case "ambient":
                                        colorNode = XmlHelper.GetChildNode( param, "color" );
                                        if ( colorNode != null )
                                        {
                                            vec = StringHelper.ConvertStringToFloatArray( colorNode.InnerText );
                                            mat.Ambient = ColorHelper.ColorFromFloatArray( vec );
                                        }
                                        break;

                                    case "diffuse":
                                        colorNode = XmlHelper.GetChildNode( param, "color" );
                                        if ( colorNode != null )
                                        {
                                            vec = StringHelper.ConvertStringToFloatArray( colorNode.InnerText );
                                            mat.Diffuse = ColorHelper.ColorFromFloatArray( vec );

                                            break;
                                        }
                                        XmlNode textureNode = XmlHelper.GetChildNode( param, "texture" );
                                        if ( textureNode != null )
                                        {
                                            //Get Texcoord channel
                                            string texcoordStr = XmlHelper.GetXmlAttribute( textureNode, "texcoord" );
                                            mat.DiffuseTexcoordSemantic = texcoordStr;

                                            //Get sampler
                                            string samplerName = XmlHelper.GetXmlAttribute( textureNode, "texture" );
                                            XmlNode samplerNode = XmlHelper.GetChildNode( profile_COMMON, "sid", samplerName );
                                            if ( samplerNode == null ) break;
                                            XmlNode sampler2DNode = XmlHelper.GetChildNode( samplerNode, "sampler2D" );
                                            if ( sampler2DNode == null ) break;
                                            string surfaceName = XmlHelper.GetChildNode( sampler2DNode, "source" ).InnerText;

                                            //Get surface
                                            XmlNode surfaceNode = XmlHelper.GetChildNode( profile_COMMON, "sid", surfaceName );
                                            string textureName = XmlHelper.GetChildNode( surfaceNode, "init_from" ).InnerText;

                                            XmlNode techniqueMaya = XmlHelper.GetChildNode( textureNode, "profile", "MAYA" );
                                            if ( techniqueMaya != null )
                                            {
                                                mat.DiffuseTextureRepeatU = StringHelper.StringToFloat(
                                                    XmlHelper.GetChildNode( techniqueMaya, "repeatU" ).InnerText );
                                                mat.DiffuseTextureRepeatV = StringHelper.StringToFloat(
                                                   XmlHelper.GetChildNode( techniqueMaya, "repeatV" ).InnerText );
                                            }

                                            mat.DiffuseTexture = GetTexture( textureName );

                                            break;


                                        }


                                        break;

                                    case "specular":
                                        colorNode = XmlHelper.GetChildNode( param, "color" );
                                        if ( colorNode != null )
                                        {
                                            vec = StringHelper.ConvertStringToFloatArray( colorNode.InnerText );
                                            mat.Specular = ColorHelper.ColorFromFloatArray( vec );
                                        }
                                        break;

                                    case "shininess":
                                        mat.Shininess = 20f;
                                        //mat.Shininess = Convert.ToSingle( XmlHelper.GetChildNode( param, "float" ).InnerText,
                                        //       CultureInfo.InvariantCulture );
                                        break;

                                }

                            }

                        }
                        else if ( subNode.Name == "extra" )
                        {

                            XmlNode techniqueNode = XmlHelper.GetChildNode( subNode, "profile", "FCOLLADA" );
                            if ( techniqueNode == null ) continue;
                            XmlNode bumpNode = XmlHelper.GetChildNode( techniqueNode, "bump" );
                            if ( bumpNode == null ) continue;

                            XmlNode textureNode = XmlHelper.GetChildNode( bumpNode, "texture" );
                            if ( textureNode != null )
                            {
                                //Get Texcoord channel
                                string texcoordStr = XmlHelper.GetXmlAttribute( textureNode, "texcoord" );
                                mat.NormalTexcoordSemantic = texcoordStr;

                                //Get sampler
                                string samplerName = XmlHelper.GetXmlAttribute( textureNode, "texture" );
                                XmlNode samplerNode = XmlHelper.GetChildNode( profile_COMMON, "sid", samplerName );
                                if ( samplerNode == null ) break;
                                XmlNode sampler2DNode = XmlHelper.GetChildNode( samplerNode, "sampler2D" );
                                if ( sampler2DNode == null ) break;
                                string surfaceName = XmlHelper.GetChildNode( sampler2DNode, "source" ).InnerText;

                                //Get surface
                                XmlNode surfaceNode = XmlHelper.GetChildNode( profile_COMMON, "sid", surfaceName );
                                string textureName = XmlHelper.GetChildNode( surfaceNode, "init_from" ).InnerText;

                                XmlNode techniqueMaya = XmlHelper.GetChildNode( textureNode, "profile", "MAYA" );
                                if ( techniqueMaya != null )
                                {
                                    mat.NormalTextureRepeatU = StringHelper.StringToFloat(
                                        XmlHelper.GetChildNode( techniqueMaya, "repeatU" ).InnerText );
                                    mat.NormalTextureRepeatV = StringHelper.StringToFloat(
                                       XmlHelper.GetChildNode( techniqueMaya, "repeatV" ).InnerText );
                                }

                                mat.NormalTexture = GetTexture( textureName );

                                break;


                            }

                        }

                    }





                }
            }

            return mat;
        }








        public ColladaTexture GetTexture( string name )
        {
            for ( int i = 0; i < textures.Count; i++ )
            {
                if ( textures[ i ].Name == name ) return textures[ i ];
            }
            return null;
        }
        public ColladaMaterial GetMaterial( string name )
        {
            for ( int i = 0; i < materials.Count; i++ )
            {
                if ( materials[ i ].Name == name ) return materials[ i ];
            }
            return null;
        }
        public ColladaMesh GetMesh( string name )
        {
            for ( int i = 0; i < meshes.Count; i++ )
            {
                if ( meshes[ i ].Name == name ) return meshes[ i ];
            }
            return null;
        }



        public List<ColladaMesh> Meshes
        {
            get { return meshes; }
        }
        public ColladaScene Scene
        {
            get { return scene; }
        }

        public static Matrix LoadColladaMatrix( XmlNode matrixNode )
        {
            float[] matrixData = StringHelper.ConvertStringToFloatArray( matrixNode.InnerText );
            return LoadColladaMatrix( matrixData, 0 );
        }

        public static Matrix LoadColladaMatrix( float[] mat, int offset )
        {
            return new Matrix(
                mat[ offset + 0 ], mat[ offset + 4 ], mat[ offset + 8 ], mat[ offset + 12 ],
                mat[ offset + 1 ], mat[ offset + 5 ], mat[ offset + 9 ], mat[ offset + 13 ],
                mat[ offset + 2 ], mat[ offset + 6 ], mat[ offset + 10 ], mat[ offset + 14 ],
                mat[ offset + 3 ], mat[ offset + 7 ], mat[ offset + 11 ], mat[ offset + 15 ] );
        }


        #region Cleaning Up


        private string filename;
        //List<SkinnedTangentVertex> vertices = new List<SkinnedTangentVertex>();


        //VertexBuffer vertexBuffer = null;
        //IndexBuffer indexBuffer = null;



        //TEMP
        /// <summary>
        /// Flat list of bones, the first bone is always the root bone, all
        /// children can be accessed from here. The main reason for having a flat
        /// list is easy access to all bones for showing bone previous and of
        /// course to quickly access all animation matrices.
        /// </summary>




        //public ColladaMaterial material;




        public void LoadOud( string nFilename )
        {
            //Model mod;


            filename = nFilename;
            // Set name to identify this model and build the filename
            //name = setName;
            //string filename = Path.Combine( ColladaDirectory,
            //    StringHelper.ExtractFilename( name, true ) + "." +
            //    ColladaExtension );

            // Load file
            Stream file = System.IO.File.OpenRead( filename );
            string colladaXml = new StreamReader( file ).ReadToEnd();
            XmlNode colladaFile = XmlHelper.LoadXmlFromText( colladaXml );

            LoadTextures( colladaFile );

            //( Load material (we only support one))
            // Not Anymore! multiple material support!
            LoadMaterials( colladaFile );

            // Load bones
            LoadBones( colladaFile );

            // Load mesh (vertices data, combine with bone weights)
            LoadMesh( colladaFile );

            // And finally load bone animation data
            LoadAnimation( colladaFile );

            LoadSceneOud( colladaFile );

            // Close file, we are done.
            file.Close();
        }






        /// <summary>
        /// Load bones
        /// </summary>
        /// <param name="colladaFile">Collada file</param>
        private void LoadBones( XmlNode colladaFile )
        {
            // We need to find the bones in the visual scene
            XmlNode visualSceneNode = XmlHelper.GetChildNode(
                XmlHelper.GetChildNode( colladaFile, "library_visual_scenes" ),
                "visual_scene" );

            // Just go through library_visual_scenes and collect all bones
            // +hierachy from there!
            FillBoneNodes( null, visualSceneNode );
        } // LoadBones(colladaFile)

        /// <summary>
        /// Fill bone nodes helper method for LoadBones.
        /// </summary>
        /// <param name="parentBone">Parent bone</param>
        /// <param name="boneNodes">Bone nodes as XmlNodes</param>
        private void FillBoneNodes( ColladaBone parentBone, XmlNode boneNodes )
        {
            foreach ( XmlNode boneNode in boneNodes )
                if ( boneNode.Name == "node" &&
                    ( XmlHelper.GetXmlAttribute( boneNode, "id" ).Contains( "Bone" ) ||
                    XmlHelper.GetXmlAttribute( boneNode, "type" ).Contains( "JOINT" ) ) )
                {
                    Matrix matrix = Matrix.Identity;

                    // Get all sub nodes for the matrix, sorry translate and rotate nodes
                    // are not supported here yet. Reconstructing the matrices is a
                    // little bit complicated because we have to reconstruct the animation
                    // data too and I don't want to overcomplicate this test project.
                    foreach ( XmlNode subnode in boneNode )
                    {
                        switch ( subnode.Name )
                        {
                            case "translate":
                            case "rotate":
                                throw new InvalidOperationException(
                                    "Unsupported bone data found for bone " + bones.Count +
                                    ". Please make sure you save the collada file with baked " +
                                    "matrices!" );
                            case "matrix":
                                matrix = LoadColladaMatrixOud(
                                    StringHelper.ConvertStringToFloatArray( subnode.InnerText ), 0 );
                                break;
                        } // switch
                    } // foreach (subnode)

                    // Create this node, use the current number of bones as number.
                    ColladaBone newBone = new ColladaBone( matrix, parentBone, bones.Count,
                        XmlHelper.GetXmlAttribute( boneNode, "id" ), XmlHelper.GetXmlAttribute( boneNode, "sid" ) );

                    // Add to our global bones list
                    bones.Add( newBone );
                    // And to our parent, this way we have a tree and a flat list in
                    // the bones list :)
                    if ( parentBone != null )
                        parentBone.children.Add( newBone );

                    // Create all children (will do nothing if there are no sub bones)
                    FillBoneNodes( newBone, boneNode );
                } // foreach if (boneNode.Name)
        } // FillBoneNodes(parentBone, boneNodes)


        /// <summary>
        /// Number of values in the animationMatrices in each bone.
        /// TODO: Split the animations up into several states (stay, stay to walk,
        /// walk, fight, etc.), but not required here in this test app yet ^^
        /// </summary>
        public int numOfAnimations = 1;

        /// <summary>
        /// Get frame rate from Collada file, should always be 30, but sometimes
        /// test models might have different times (like 24).
        /// </summary>
        public float frameRate = 30;
        #region Load animation
        #region Load animation targets
        /// <summary>
        /// Animation target values to help us loading animations a little.
        /// Thanks to this dictionary we are able to load all float arrays
        /// at once and then use them wherever we need them later to fill in
        /// the animation matrices for every animated bone.
        /// </summary>
        Dictionary<string, float[]> animationTargetValues =
            new Dictionary<string, float[]>();

        /// <summary>
        /// Load Animation data from a collada file, ignoring timesteps,
        /// interpolation and multiple animations
        /// </summary>
        /// <param name="colladaFile"></param>
        /// <param name="AnimationTargetValue"></param>
        private void LoadAnimationTargets( XmlNode colladaFile )
        {
            // Get global frame rate
            try
            {
                frameRate = Convert.ToSingle(
                    XmlHelper.GetChildNode( colladaFile, "frame_rate" ).InnerText );
            } // try
            catch { } // ignore if that fails

            XmlNode libraryanimation = XmlHelper.GetChildNode( colladaFile,
                "library_animations" );
            if ( libraryanimation == null )
                return;

            LoadAnimationHelper( libraryanimation );
        } // LoadAnimation(colladaFile, AnimationTargetValues)

        /// <summary>
        /// Load animation helper, goes over all animations in the animationnode,
        /// calls itself recursively for sub-animation-nodes
        /// </summary>
        /// <param name="animationnode">Animationnode</param>
        private void LoadAnimationHelper( XmlNode animationnode )
        {
            // go over all animation elements
            foreach ( XmlNode node in animationnode )
            {
                if ( node.Name == "animation" )
                //not a channel but another animation node
                {
                    LoadAnimationHelper( node );
                    continue;
                } // if (animation.name)

                if ( node.Name != "channel" )
                    continue;
                string samplername =
                    XmlHelper.GetXmlAttribute( node, "source" ).Substring( 1 );
                string targetname = XmlHelper.GetXmlAttribute( node, "target" );

                // Find the sampler for the animation values.
                XmlNode sampler = XmlHelper.GetChildNode( animationnode, "id",
                    samplername );

                // Find value xml node
                string valuename = XmlHelper.GetXmlAttribute(
                    XmlHelper.GetChildNode( sampler, "semantic", "OUTPUT" ),
                    "source" ).Substring( 1 ) + "-array";
                XmlNode valuenode = XmlHelper.GetChildNode( animationnode, "id",
                    valuename );

                // Parse values and add to dictionary
                float[] values =
                    StringHelper.ConvertStringToFloatArray( valuenode.InnerText );
                animationTargetValues.Add( targetname, values );

                // Set number of animation we will use in all bones.
                // Leave last animation value out later, but make filling array
                // a little easier (last matrix is just unused then).
                //numOfAnimations = values.Length;
                // If these are matrix values, devide by 16!
                //MHGW edit: the following if to check if these were matrix values is not correct
                //if ( XmlHelper.GetXmlAttribute( valuenode, "id" ).Contains( "transform" ) )
                //    numOfAnimations /= 16;
                int newNumOfAnimations = int.Parse( XmlHelper.GetXmlAttribute( XmlHelper.GetChildNode( valuenode.ParentNode, "accessor" ), "count" ) );

                if ( newNumOfAnimations > numOfAnimations ) numOfAnimations = newNumOfAnimations;
            } // foreach (node)
        } // LoadAnimationHelper(animationnode, AnimationTargetValues)
        #endregion

        #region FillBoneAnimations
        /// <summary>
        /// Fill bone animations, called from LoadAnimation after we got all
        /// animationTargetValues.
        /// </summary>
        /// <param name="colladaFile">Collada file</param>
        private void FillBoneAnimations( XmlNode colladaFile )
        {
            foreach ( ColladaBone bone in bones )
            {
                // Loads animation data from bone node sid, links them
                // automatically, also generates animation matrices.
                // Note: We only support the transform node here, "RotX", "RotY",
                // etc. will only be used if we don't got baked matrices, but that
                // is catched already when loading the initial bone matrices above.

                // Build sid the way it is used in the collada file.
                string sid = bone.id + "/" + "transform";

                int framecount = 0;
                if ( animationTargetValues.ContainsKey( sid ) )
                    // Transformation contains whole matrix (always 4x4).
                    framecount = animationTargetValues[ sid ].Length / 16;

                // Expand array and duplicate the initial matrix in case
                // there is no animation data present (often the case).
                for ( int i = 0; i < numOfAnimations; i++ )
                    bone.animationMatrices.Add( bone.initialMatrix );

                if ( framecount > 0 )
                {
                    float[] mat = animationTargetValues[ sid ];
                    // Load all absolute animation matrices. If you want relative
                    // data here you can use the invBoneMatrix (invert initialMatrix),
                    // but this won't be required here because all animations are
                    // already computed. Maybe you need it when doing your own animations.
                    for ( int num = 0; num < bone.animationMatrices.Count &&
                        num < framecount; num++ )
                    {
                        bone.animationMatrices[ num ] = LoadColladaMatrixOud( mat, num * 16 );
                    } // for (num)
                } // if (framecount)
            } // foreach (bone)
        } // FillBoneAnimations(colladaFile)
        #endregion

        #region CalculateAbsoluteBoneMatrices
        /// <summary>
        /// Calculate absolute bone matrices for finalMatrix. Not really required,
        /// but this way we can start using the bones without having to call
        /// UpdateAnimation (maybe we don't want to animate yet).
        /// </summary>
        private void CalculateAbsoluteBoneMatrices()
        {
            foreach ( ColladaBone bone in bones )
            {
                // Get absolute matrices and also use them for the initial finalMatrix
                // of each bone, which is used for rendering.
                bone.finalMatrix = bone.GetMatrixRecursively();
            } // foreach (bone)
        } // CalculateAbsoluteBoneMatrices()
        #endregion

        /// <summary>
        /// Load animation
        /// </summary>
        /// <param name="colladaFile">Collada file</param>
        private void LoadAnimation( XmlNode colladaFile )
        {
            // Little helper to load and store all animation values
            // before assigning them to the bones where they are used.
            LoadAnimationTargets( colladaFile );

            // Fill animation matrices in each bone (if used or not, always fill).
            FillBoneAnimations( colladaFile );

            // Calculate all absolute matrices. We only got relative ones in
            // initialMatrix for each bone right now.
            CalculateAbsoluteBoneMatrices();
        } // LoadAnimation(colladaFile)
        #endregion





        /// <summary>
        /// Load mesh, must be called after we got all bones. Will also create
        /// the vertex and index buffers and optimize the vertices as much as
        /// we can.
        /// </summary>
        /// <param name="colladaFile">Collada file</param>
        private void LoadMesh( XmlNode colladaFile )
        {
            XmlNode geometrys =
                XmlHelper.GetChildNode( colladaFile, "library_geometries" );
            if ( geometrys == null )
                throw new InvalidOperationException(
                    "library_geometries node not found in collada file " + filename );

            ColladaMesh mesh;

            foreach ( XmlNode geometry in geometrys )
                if ( geometry.Name == "geometry" )
                {
                    XmlNode meshNode = XmlHelper.GetChildNode( geometry, "mesh" );
                    if ( meshNode == null )
                    {
                        //This geometry is not a mesh! skip!
                        continue;
                    }

                    mesh = new ColladaMesh( this );

                    mesh.Name = XmlHelper.GetXmlAttribute( geometry, "id" );




                    // Load everything from the mesh node
                    mesh.LoadMeshGeometry( colladaFile, meshNode );


                    //TODO: no rendering support in this class! just loading of xml files to easy accessible format.
                    if ( mesh.verticesSkinned.Count > 0 )
                    {
                        mesh.GenerateVertexAndIndexBuffersSkinned();
                    }
                    else
                    {
                        // Generate vertex buffer for rendering
                        mesh.GenerateVertexAndIndexBuffers();
                    }



                    //( Get outa here, we currently only support one single mesh!)
                    //MHGW: Not Anymore! Multiple meshes supported

                    meshes.Add( mesh );
                } // foreach if (geometry.name)
        } // LoadMesh(colladaFile)


        private void LoadSceneOud( XmlNode colladaFile )
        {
            XmlNode library_visual_scenes = XmlHelper.GetChildNode( colladaFile, "library_visual_scenes" );
            XmlNode visual_scene = XmlHelper.GetChildNode( library_visual_scenes, "visual_scene" );

            foreach ( XmlNode node in visual_scene )
            {
                XmlNode instance_geometry = XmlHelper.GetChildNode( node, "instance_geometry" );
                if ( instance_geometry == null ) continue;
                string meshName = XmlHelper.GetXmlAttribute( instance_geometry, "url" ).Substring( 1 );
                ColladaMesh mesh = GetMesh( meshName );
                if ( mesh != null )
                {
                    XmlNode matrixNode = XmlHelper.GetChildNode( node, "matrix" );
                    float[] matrixData = StringHelper.ConvertStringToFloatArray( matrixNode.InnerText );
                    Matrix objectMatrix = LoadColladaMatrixOud( matrixData, 0 );

                    mesh.SetObjectMatrix( objectMatrix );
                }

            }
        }


        public static Matrix LoadColladaMatrixOud( float[] mat, int offset )
        {
            return new Matrix(
                mat[ offset + 0 ], mat[ offset + 4 ], mat[ offset + 8 ], mat[ offset + 12 ],
                mat[ offset + 1 ], mat[ offset + 5 ], mat[ offset + 9 ], mat[ offset + 13 ],
                mat[ offset + 2 ], mat[ offset + 6 ], mat[ offset + 10 ], mat[ offset + 14 ],
                mat[ offset + 3 ], mat[ offset + 7 ], mat[ offset + 11 ], mat[ offset + 15 ] );
        } // LoadColladaMatrix(mat, offset)










        #endregion


        #region Oud

        #region Update animation
        /// <summary>
        /// Was this animation data already constructed last time we called
        /// UpdateAnimation? Will not optimize much if you render several models
        /// of this type (maybe use a instance class that holds this animation
        /// data and just calls this class for rendering to optimize it further),
        /// but if you just render a single model, it gets a lot faster this way.
        /// </summary>
        private int lastAniMatrixNum = -1;

        /// <summary>
        /// Update animation. Will do nothing if animation stayed the same since
        /// last time we called this method.
        /// </summary>
        /// <param name="renderMatrix">Render matrix just for adding some
        /// offset value to the animation time, remove if you allow moving
        /// objects, this is just for testing!</param>
        private void UpdateAnimation( Matrix renderMatrix )
        {
            UpdateAnimation( game.Elapsed, renderMatrix );
        } // UpdateAnimation()
        /// <summary>
        /// Update animation. Will do nothing if animation stayed the same since
        /// last time we called this method.
        /// </summary>
        /// <param name="renderMatrix">Render matrix just for adding some
        /// offset value to the animation time, remove if you allow moving
        /// objects, this is just for testing!</param>
        private void UpdateAnimation( float elapsed, Matrix renderMatrix )
        {
            // Add some time to the animation depending on the position.
            int aniMatrixNum = ( (int)( 100 +
                //renderMatrix.Translation.X / 2.35f +
                //renderMatrix.Translation.Y / 1.05f +
                //( engine.ProcessEventArgs.Time / 1000f ) * frameRate ) ) % numOfAnimations;
                ( elapsed ) * frameRate ) ) % numOfAnimations;
            if ( aniMatrixNum < 0 )
                aniMatrixNum = 0;
            // No need to update if everything stayed the same
            if ( aniMatrixNum == lastAniMatrixNum )
                return;
            lastAniMatrixNum = aniMatrixNum;

            foreach ( ColladaBone bone in bones )
            {
                // Just assign the final matrix from the animation matrices.
                bone.finalMatrix = bone.animationMatrices[ aniMatrixNum ];

                // Also use parent matrix if we got one
                // This will always work because all the bones are in order.
                if ( bone.parent != null )
                    bone.finalMatrix *=
                        bone.parent.finalMatrix;
            } // foreach
        } // UpdateAnimation()
        #endregion

        /// <summary>
        /// Get bone matrices for the shader. We have to apply the invBoneSkinMatrix
        /// to each final matrix, which is the recursively created matrix from
        /// all the animation data (see UpdateAnimation).
        /// </summary>
        /// <returns></returns>
        public Matrix[] GetBoneMatrices( Matrix renderMatrix )
        {
            // Update the animation data in case it is not up to date anymore.
            UpdateAnimation( renderMatrix );

            // And get all bone matrices, we support max. 80 (see shader).
            Matrix[] matrices = new Matrix[ Math.Min( 80, bones.Count ) ];
            for ( int num = 0; num < matrices.Length; num++ )
                // The matrices are constructed from the invBoneSkinMatrix and
                // the finalMatrix, which holds the recursively added animation matrices
                // and finally we add the render matrix too here.
                matrices[ num ] =
                     bones[ num ].invBoneSkinMatrix * bones[ num ].finalMatrix * renderMatrix;
            //bones[ num ].invBoneSkinMatrix * renderMatrix;

            return matrices;
        } // GetBoneMatrices()

        private IXNAGame game;


        ///// <summary>
        ///// Load mesh geometry
        ///// </summary>
        ///// <param name="geometry"></param>
        //private void LoadMeshGeometrySkinned( XmlNode colladaFile, XmlNode meshNode )
        //{
        //    ColladaMesh mesh = new ColladaMesh( engine );



        //    // Load all source nodes
        //    Dictionary<string, List<float>> sources = new Dictionary<string,
        //        List<float>>();
        //    foreach ( XmlNode node in meshNode )
        //    {
        //        if ( node.Name != "source" )
        //            continue;
        //        XmlNode floatArray = XmlHelper.GetChildNode( node, "float_array" );
        //        List<float> floats = new List<float>(
        //            StringHelper.ConvertStringToFloatArray( floatArray.InnerText ) );

        //        // Fill the array up
        //        int count = Convert.ToInt32( XmlHelper.GetXmlAttribute( floatArray,
        //            "count" ), NumberFormatInfo.InvariantInfo );
        //        while ( floats.Count < count )
        //            floats.Add( 0.0f );

        //        sources.Add( XmlHelper.GetXmlAttribute( node, "id" ), floats );

        //        ColladaMesh.Source source = new ColladaMesh.Source();
        //        source.ID = XmlHelper.GetXmlAttribute( node, "id" );
        //        source.Data = floats;
        //    } // foreach


        //    // Vertices
        //    // Also add vertices node, redirected to position node into sources
        //    XmlNode verticesNode = XmlHelper.GetChildNode( meshNode, "vertices" );
        //    XmlNode posInput = XmlHelper.GetChildNode( verticesNode, "input" );
        //    if ( XmlHelper.GetXmlAttribute( posInput, "semantic" ).ToLower(
        //        CultureInfo.InvariantCulture ) != "position" )
        //        throw new InvalidOperationException(
        //            "unsupported feature found in collada \"vertices\" node" );
        //    string verticesValueName = XmlHelper.GetXmlAttribute( posInput,
        //        "source" ).Substring( 1 );
        //    sources.Add( XmlHelper.GetXmlAttribute( verticesNode, "id" ),
        //        sources[ verticesValueName ] );
        //    //

        //    // Load helper matrixes (bind shape and all bind poses matrices)
        //    //// We only support 1 skinning, so just load the first skin node!
        //    //XmlNode skinNode = XmlHelper.GetChildNode(
        //    //    XmlHelper.GetChildNode(
        //    //    XmlHelper.GetChildNode( colladaFile, "library_controllers" ),
        //    //    "controller" ), "skin" );
        //    //objectMatrix = LoadColladaMatrix(
        //    //    StringHelper.ConvertStringToFloatArray(
        //    //    XmlHelper.GetChildNode( skinNode, "bind_shape_matrix" ).InnerText ), 0 );

        //    //// Get the order of the bones used in collada (can be different than ours)
        //    //int[] boneArrayOrder = new int[ bones.Count ];
        //    //int[] invBoneArrayOrder = new int[ bones.Count ];
        //    //string boneNameArray =
        //    //    XmlHelper.GetChildNode( skinNode, "Name_array" ).InnerText;
        //    //int arrayIndex = 0;
        //    //foreach ( string boneName in boneNameArray.Split( ' ' ) )
        //    //{
        //    //    boneArrayOrder[ arrayIndex ] = -1;
        //    //    foreach ( Bone bone in bones )
        //    //        if ( bone.id == boneName )
        //    //        {
        //    //            boneArrayOrder[ arrayIndex ] = bone.num;
        //    //            invBoneArrayOrder[ bone.num ] = arrayIndex;
        //    //            break;
        //    //        } // foreach

        //    //    if ( boneArrayOrder[ arrayIndex ] == -1 )
        //    //        throw new InvalidOperationException(
        //    //            "Unable to find boneName=" + boneName +
        //    //            " in our bones array for skinning!" );
        //    //    arrayIndex++;
        //    //} // foreach
        //    ////

        //    //// Load weights
        //    //float[] weights = null;
        //    //foreach ( XmlNode sourceNode in skinNode )
        //    //{
        //    //    // Get all inv bone skin matrices
        //    //    if ( sourceNode.Name == "source" &&
        //    //        XmlHelper.GetXmlAttribute( sourceNode, "id" ).Contains( "bind_poses" ) )
        //    //    {
        //    //        // Get inner float array
        //    //        float[] mat = StringHelper.ConvertStringToFloatArray(
        //    //            XmlHelper.GetChildNode( sourceNode, "float_array" ).InnerText );
        //    //        for ( int boneNum = 0; boneNum < bones.Count; boneNum++ )
        //    //            if ( mat.Length / 16 > boneNum )
        //    //            {
        //    //                bones[ boneArrayOrder[ boneNum ] ].invBoneSkinMatrix =
        //    //                    LoadColladaMatrix( mat, boneNum * 16 );
        //    //            } // for if
        //    //    } // if

        //    //    // Get all weights
        //    //    if ( sourceNode.Name == "source" &&
        //    //        XmlHelper.GetXmlAttribute( sourceNode, "id" ).Contains( "skin-weights" ) )
        //    //    {
        //    //        // Get inner float array
        //    //        weights = StringHelper.ConvertStringToFloatArray(
        //    //            XmlHelper.GetChildNode( sourceNode, "float_array" ).InnerText );
        //    //    } // if
        //    //} // foreach

        //    //if ( weights == null )
        //    //    throw new InvalidOperationException(
        //    //        "No weights were found in our skin, unable to continue!" );
        //    //

        //    // Prepare weights and joint indices, we only want the top 3!
        //    //// Helper to access the bones (first index) and weights (second index).
        //    //// If we got more than 2 indices for an entry here, there are multiple
        //    //// weights used (usually 1 to 3, if more, we only use the strongest).
        //    //XmlNode vertexWeightsNode =
        //    //    XmlHelper.GetChildNode( skinNode, "vertex_weights" );
        //    //int[] vcountArray = StringHelper.ConvertStringToIntArray(
        //    //    XmlHelper.GetChildNode( skinNode, "vcount" ).InnerText );
        //    //int[] vArray = StringHelper.ConvertStringToIntArray(
        //    //    XmlHelper.GetChildNode( skinNode, "v" ).InnerText );

        //    //// Build vertexSkinJoints and vertexSkinWeights for easier access.
        //    //List<Vector3> vertexSkinJoints = new List<Vector3>();
        //    //List<Vector3> vertexSkinWeights = new List<Vector3>();
        //    //int vArrayIndex = 0;
        //    //for ( int num = 0; num < vcountArray.Length; num++ )
        //    //{
        //    //    int vcount = vcountArray[ num ];
        //    //    List<int> jointIndices = new List<int>();
        //    //    List<int> weightIndices = new List<int>();
        //    //    for ( int i = 0; i < vcount; i++ )
        //    //    {
        //    //        // Make sure we convert the internal number to our bone numbers!
        //    //        jointIndices.Add( boneArrayOrder[ vArray[ vArrayIndex ] ] );
        //    //        weightIndices.Add( vArray[ vArrayIndex + 1 ] );
        //    //        vArrayIndex += 2;
        //    //    } // for

        //    //    // If we got less than 3 values, add until we have enough,
        //    //    // this makes the computation easier below
        //    //    while ( jointIndices.Count < 3 )
        //    //        jointIndices.Add( 0 );
        //    //    while ( weightIndices.Count < 3 )
        //    //        weightIndices.Add( -1 );

        //    //    // Find out top 3 weights
        //    //    float[] weightValues = new float[ weightIndices.Count ];
        //    //    int[] bestWeights = { 0, 1, 2 };
        //    //    for ( int i = 0; i < weightIndices.Count; i++ )
        //    //    {
        //    //        // Use weight of zero for invalid indices.
        //    //        if ( weightIndices[ i ] < 0 ||
        //    //            weightValues[ i ] >= weights.Length )
        //    //            weightValues[ i ] = 0;
        //    //        else
        //    //            weightValues[ i ] = weights[ weightIndices[ i ] ];

        //    //        // Got 4 or more weights? Then just select the top 3!
        //    //        if ( i >= 3 )
        //    //        {
        //    //            float lowestWeight = 1.0f;
        //    //            int lowestWeightOverride = 2;
        //    //            for ( int b = 0; b < bestWeights.Length; b++ )
        //    //                if ( lowestWeight > weightValues[ bestWeights[ b ] ] )
        //    //                {
        //    //                    lowestWeight = weightValues[ bestWeights[ b ] ];
        //    //                    lowestWeightOverride = b;
        //    //                } // for if
        //    //            // Replace lowest weight
        //    //            bestWeights[ lowestWeightOverride ] = i;
        //    //        } // if
        //    //    } // for

        //    //    // Now build 2 vectors from the best weights
        //    //    Vector3 boneIndicesVec = new Vector3(
        //    //        jointIndices[ bestWeights[ 0 ] ],
        //    //        jointIndices[ bestWeights[ 1 ] ],
        //    //        jointIndices[ bestWeights[ 2 ] ] );
        //    //    Vector3 weightsVec = new Vector3(
        //    //        weightValues[ bestWeights[ 0 ] ],
        //    //        weightValues[ bestWeights[ 1 ] ],
        //    //        weightValues[ bestWeights[ 2 ] ] );
        //    //    // Renormalize weight, important if we got more entries before
        //    //    // and always good to do!
        //    //    float totalWeights = weightsVec.X + weightsVec.Y + weightsVec.Z;
        //    //    if ( totalWeights == 0 )
        //    //        weightsVec.X = 1.0f;
        //    //    else
        //    //    {
        //    //        weightsVec.X /= totalWeights;
        //    //        weightsVec.Y /= totalWeights;
        //    //        weightsVec.Z /= totalWeights;
        //    //    } // else

        //    //    vertexSkinJoints.Add( boneIndicesVec );
        //    //    vertexSkinWeights.Add( weightsVec );
        //    //} // for
        //    //

        //    // Construct all triangle polygons from the vertex data
        //    // Construct and generate vertices lists. Every 3 vertices will
        //    // span one triangle polygon, but everything is optimized later.
        //    foreach ( XmlNode trianglenode in meshNode )
        //    {
        //        if ( trianglenode.Name != "triangles" )
        //            continue;

        //        // Find data source nodes
        //        XmlNode positionsnode = XmlHelper.GetChildNode( trianglenode,
        //            "semantic", "VERTEX" );
        //        XmlNode normalsnode = XmlHelper.GetChildNode( trianglenode,
        //            "semantic", "NORMAL" );
        //        XmlNode texcoordsnode = XmlHelper.GetChildNode( trianglenode,
        //            "semantic", "TEXCOORD" );
        //        XmlNode tangentsnode = XmlHelper.GetChildNode( trianglenode,
        //            "semantic", "TEXTANGENT" );

        //        // Get the data of the sources
        //        List<float> positions = sources[ XmlHelper.GetXmlAttribute(
        //            positionsnode, "source" ).Substring( 1 ) ];
        //        List<float> normals = sources[ XmlHelper.GetXmlAttribute( normalsnode,
        //            "source" ).Substring( 1 ) ];
        //        List<float> texcoords = sources[ XmlHelper.GetXmlAttribute(
        //            texcoordsnode, "source" ).Substring( 1 ) ];
        //        List<float> tangents = sources[ XmlHelper.GetXmlAttribute( tangentsnode,
        //            "source" ).Substring( 1 ) ];

        //        // Find the Offsets
        //        int positionsoffset = Convert.ToInt32( XmlHelper.GetXmlAttribute(
        //            positionsnode, "offset" ), NumberFormatInfo.InvariantInfo );
        //        int normalsoffset = Convert.ToInt32( XmlHelper.GetXmlAttribute(
        //            normalsnode, "offset" ), NumberFormatInfo.InvariantInfo );
        //        int texcoordsoffset = Convert.ToInt32( XmlHelper.GetXmlAttribute(
        //            texcoordsnode, "offset" ), NumberFormatInfo.InvariantInfo );
        //        int tangentsoffset = Convert.ToInt32( XmlHelper.GetXmlAttribute(
        //            tangentsnode, "offset" ), NumberFormatInfo.InvariantInfo );

        //        // Get the indexlist
        //        XmlNode p = XmlHelper.GetChildNode( trianglenode, "p" );
        //        int[] pints = StringHelper.ConvertStringToIntArray( p.InnerText );
        //        int trianglecount = Convert.ToInt32( XmlHelper.GetXmlAttribute(
        //            trianglenode, "count" ), NumberFormatInfo.InvariantInfo );
        //        // The number of ints that form one vertex:
        //        int vertexcomponentcount = pints.Length / trianglecount / 3;

        //        // Construct data
        //        vertices.Clear();
        //        // Initialize reuseVertexPositions and reverseReuseVertexPositions
        //        // to make it easier to use them below
        //        reuseVertexPositions = new int[ trianglecount * 3 ];
        //        reverseReuseVertexPositions = new List<int>[ positions.Count / 3 ];
        //        for ( int i = 0; i < reverseReuseVertexPositions.Length; i++ )
        //            reverseReuseVertexPositions[ i ] = new List<int>();

        //        // We have to use int indices here because we often have models
        //        // with more than 64k triangles (even if that gets optimized later).
        //        for ( int i = 0; i < trianglecount * 3; i++ )
        //        {
        //            // Position
        //            int pos = pints[ i * vertexcomponentcount + positionsoffset ] * 3;
        //            Vector3 position = new Vector3(
        //                positions[ pos ], positions[ pos + 1 ], positions[ pos + 2 ] );

        //            //// Get vertex blending stuff (uses pos too)
        //            //Vector3 blendWeights = vertexSkinWeights[ pos / 3 ];
        //            //Vector3 blendIndices = vertexSkinJoints[ pos / 3 ];



        //            //// Pre-multiply all indices with 3, this way the shader
        //            //// code gets a little bit simpler and faster
        //            //blendIndices = new Vector3(
        //            //    blendIndices.X * 3, blendIndices.Y * 3, blendIndices.Z * 3 );

        //            // Normal
        //            int nor = pints[ i * vertexcomponentcount + normalsoffset ] * 3;
        //            Vector3 normal = new Vector3(
        //                normals[ nor ], normals[ nor + 1 ], normals[ nor + 2 ] );

        //            // Texture Coordinates
        //            int tex = pints[ i * vertexcomponentcount + texcoordsoffset ] * 3;
        //            float u = texcoords[ tex ];
        //            // V coordinate is inverted in max
        //            float v = 1.0f - texcoords[ tex + 1 ];

        //            // Tangent
        //            int tan = pints[ i * vertexcomponentcount + tangentsoffset ] * 3;
        //            Vector3 tangent = new Vector3(
        //                tangents[ tan ], tangents[ tan + 1 ], tangents[ tan + 2 ] );

        //            // Set the vertex
        //            //vertices.Add( new SkinnedTangentVertex(
        //            //    position, blendWeights, blendIndices, u, v, normal, tangent ) );

        //            vertices.Add( new TangentVertex( position, u, v, normal, tangent ) );

        //            // Remember pos for optimizing the vertices later more easily.
        //            reuseVertexPositions[ i ] = pos / 3;
        //            reverseReuseVertexPositions[ pos / 3 ].Add( i );
        //        } // for (ushort)

        //        // Only support one mesh for now, get outta here.
        //        return;
        //    } // foreach (trianglenode)

        //    throw new InvalidOperationException(
        //        "No mesh found in this collada file, unable to continue!" );
        //    //
        //} // LoadMeshGeometry(geometry)

        ///// <summary>
        ///// Load mesh geometry
        ///// </summary>
        ///// <param name="geometry"></param>
        //private void LoadMeshGeometryOrig( XmlNode colladaFile, XmlNode meshNode )
        //{
        //    ColladaMesh mesh = new ColladaMesh( engine );

        //    ColladaMesh.Source source;

        //    // Load all source nodes
        //    Dictionary<string, List<float>> sources = new Dictionary<string,
        //        List<float>>();
        //    foreach ( XmlNode node in meshNode )
        //    {
        //        if ( node.Name != "source" )
        //            continue;
        //        XmlNode floatArray = XmlHelper.GetChildNode( node, "float_array" );
        //        List<float> floats = new List<float>(
        //            StringHelper.ConvertStringToFloatArray( floatArray.InnerText ) );

        //        // Fill the array up
        //        int count = Convert.ToInt32( XmlHelper.GetXmlAttribute( floatArray,
        //            "count" ), NumberFormatInfo.InvariantInfo );
        //        while ( floats.Count < count )
        //            floats.Add( 0.0f );

        //        sources.Add( XmlHelper.GetXmlAttribute( node, "id" ), floats );

        //        source = new ColladaMesh.Source();
        //        source.ID = XmlHelper.GetXmlAttribute( node, "id" );
        //        source.Data = floats;


        //        //mesh.Sources.Add( source.ID, source );
        //    } // foreach


        //    // Vertices
        //    // Also add vertices node, redirected to position node into sources
        //    XmlNode verticesNode = XmlHelper.GetChildNode( meshNode, "vertices" );
        //    XmlNode posInput = XmlHelper.GetChildNode( verticesNode, "input" );
        //    if ( XmlHelper.GetXmlAttribute( posInput, "semantic" ).ToLower(
        //        CultureInfo.InvariantCulture ) != "position" )
        //        throw new InvalidOperationException(
        //            "unsupported feature found in collada \"vertices\" node" );
        //    string verticesValueName = XmlHelper.GetXmlAttribute( posInput,
        //        "source" ).Substring( 1 );
        //    sources.Add( XmlHelper.GetXmlAttribute( verticesNode, "id" ),
        //        sources[ verticesValueName ] );

        //    //source = mesh.Sources[ verticesValueName ];

        //    //source.ID = XmlHelper.GetXmlAttribute( verticesNode, "id" );

        //    //mesh.Sources.Add( source.ID, source );


        //    // Construct all triangle polygons from the vertex data
        //    // Construct and generate vertices lists. Every 3 vertices will
        //    // span one triangle polygon, but everything is optimized later.
        //    foreach ( XmlNode trianglenode in meshNode )
        //    {
        //        if ( trianglenode.Name != "triangles" )
        //            continue;

        //        // Find data source nodes
        //        XmlNode positionsnode = XmlHelper.GetChildNode( trianglenode,
        //            "semantic", "VERTEX" );
        //        XmlNode normalsnode = XmlHelper.GetChildNode( trianglenode,
        //            "semantic", "NORMAL" );
        //        XmlNode texcoordsnode = XmlHelper.GetChildNode( trianglenode,
        //            "semantic", "TEXCOORD" );
        //        XmlNode tangentsnode = XmlHelper.GetChildNode( trianglenode,
        //            "semantic", "TEXTANGENT" );

        //        // Get the data of the sources
        //        List<float> positions = sources[ XmlHelper.GetXmlAttribute(
        //            positionsnode, "source" ).Substring( 1 ) ];
        //        List<float> normals = sources[ XmlHelper.GetXmlAttribute( normalsnode,
        //            "source" ).Substring( 1 ) ];
        //        List<float> texcoords = sources[ XmlHelper.GetXmlAttribute(
        //            texcoordsnode, "source" ).Substring( 1 ) ];
        //        List<float> tangents = sources[ XmlHelper.GetXmlAttribute( tangentsnode,
        //            "source" ).Substring( 1 ) ];

        //        /*mesh.SetPositionsSource( mesh.Sources[ XmlHelper.GetXmlAttribute(
        //            positionsnode, "source" ).Substring( 1 ) ] );*/



        //        // Find the Offsets
        //        int positionsoffset = Convert.ToInt32( XmlHelper.GetXmlAttribute(
        //            positionsnode, "offset" ), NumberFormatInfo.InvariantInfo );
        //        int normalsoffset = Convert.ToInt32( XmlHelper.GetXmlAttribute(
        //            normalsnode, "offset" ), NumberFormatInfo.InvariantInfo );
        //        int texcoordsoffset = Convert.ToInt32( XmlHelper.GetXmlAttribute(
        //            texcoordsnode, "offset" ), NumberFormatInfo.InvariantInfo );
        //        int tangentsoffset = Convert.ToInt32( XmlHelper.GetXmlAttribute(
        //            tangentsnode, "offset" ), NumberFormatInfo.InvariantInfo );

        //        // Get the indexlist
        //        XmlNode p = XmlHelper.GetChildNode( trianglenode, "p" );
        //        int[] pints = StringHelper.ConvertStringToIntArray( p.InnerText );
        //        int trianglecount = Convert.ToInt32( XmlHelper.GetXmlAttribute(
        //            trianglenode, "count" ), NumberFormatInfo.InvariantInfo );
        //        // The number of ints that form one vertex:
        //        int vertexcomponentcount = pints.Length / trianglecount / 3;

        //        // Construct data
        //        vertices.Clear();
        //        // Initialize reuseVertexPositions and reverseReuseVertexPositions
        //        // to make it easier to use them below
        //        reuseVertexPositions = new int[ trianglecount * 3 ];
        //        reverseReuseVertexPositions = new List<int>[ positions.Count / 3 ];
        //        for ( int i = 0; i < reverseReuseVertexPositions.Length; i++ )
        //            reverseReuseVertexPositions[ i ] = new List<int>();

        //        // We have to use int indices here because we often have models
        //        // with more than 64k triangles (even if that gets optimized later).
        //        for ( int i = 0; i < trianglecount * 3; i++ )
        //        {
        //            // Position
        //            int pos = pints[ i * vertexcomponentcount + positionsoffset ] * 3;
        //            Vector3 position = new Vector3(
        //                positions[ pos ], positions[ pos + 1 ], positions[ pos + 2 ] );

        //            //// Get vertex blending stuff (uses pos too)
        //            //Vector3 blendWeights = vertexSkinWeights[ pos / 3 ];
        //            //Vector3 blendIndices = vertexSkinJoints[ pos / 3 ];



        //            //// Pre-multiply all indices with 3, this way the shader
        //            //// code gets a little bit simpler and faster
        //            //blendIndices = new Vector3(
        //            //    blendIndices.X * 3, blendIndices.Y * 3, blendIndices.Z * 3 );

        //            // Normal
        //            int nor = pints[ i * vertexcomponentcount + normalsoffset ] * 3;
        //            Vector3 normal = new Vector3(
        //                normals[ nor ], normals[ nor + 1 ], normals[ nor + 2 ] );

        //            // Texture Coordinates
        //            int tex = pints[ i * vertexcomponentcount + texcoordsoffset ] * 3;
        //            float u = texcoords[ tex ];
        //            // V coordinate is inverted in max
        //            float v = 1.0f - texcoords[ tex + 1 ];

        //            // Tangent
        //            int tan = pints[ i * vertexcomponentcount + tangentsoffset ] * 3;
        //            Vector3 tangent = new Vector3(
        //                tangents[ tan ], tangents[ tan + 1 ], tangents[ tan + 2 ] );

        //            // Set the vertex
        //            //vertices.Add( new SkinnedTangentVertex(
        //            //    position, blendWeights, blendIndices, u, v, normal, tangent ) );

        //            vertices.Add( new TangentVertex( position, u, v, normal, tangent ) );

        //            // Remember pos for optimizing the vertices later more easily.
        //            reuseVertexPositions[ i ] = pos / 3;
        //            reverseReuseVertexPositions[ pos / 3 ].Add( i );
        //        } // for (ushort)

        //        // Only support one mesh for now, get outta here.
        //        return;
        //    } // foreach (trianglenode)

        //    throw new InvalidOperationException(
        //        "No mesh found in this collada file, unable to continue!" );
        //    //
        //} // LoadMeshGeometry(geometry)


        public void Render( Matrix renderMatrix, ShaderEffect effect )
        {
            for ( int i = 0; i < meshes.Count; i++ )
            {
                meshes[ i ].Render( renderMatrix, effect );
            }
        }



        public IXNAGame Game { get { return game; } }




        public ColladaModel( IXNAGame _game )
        {
            game = _game;
        }
        #endregion










        public static ColladaModel LoadWall001()
        {
            using ( Stream strm = EmbeddedFile.GetStream(
                "MHGameWork.TheWizards.Collada.Files.Wall001.DAE",
                "Wall001.DAE" ) )
            {
                return ColladaModel.FromStream( strm );
            }
        }

        public static ColladaModel LoadSimpleCharacterAnim001()
        {
            using ( Stream strm = EmbeddedFile.GetStream(
                "MHGameWork.TheWizards.Collada.Files.SimpleCharacterAnim001.DAE",
                "SimpleCharacterAnim001.DAE" ) )
            {
                return ColladaModel.FromStream( strm );
            }
        }

        public static ColladaModel LoadSimpleBones001()
        {
            using ( Stream strm = EmbeddedFile.GetStream(
                "MHGameWork.TheWizards.Collada.Files.SimpleBones001.DAE",
                "SimpleBones001.DAE" ) )
            {
                return ColladaModel.FromStream( strm );
            }
        }

        public static ColladaModel LoadTriangleBones001()
        {
            using ( Stream strm = EmbeddedFile.GetStream(
                "MHGameWork.TheWizards.Collada.Files.TriangleBones001.DAE",
                "TriangleBones001.DAE" ) )
            {
                return ColladaModel.FromStream( strm );
            }
        }


        public static void TestLoadColladaModel()
        {
            LoadWall001();
            LoadSimpleCharacterAnim001();
            LoadSimpleBones001();
            LoadTriangleBones001();

        }

        public static void TestColladaModelSkinnedSimple()
        {


            TestXNAGame main = null;

            ColladaModel model = null;

            ShaderEffect effect = null;

            bool started = false;

            VertexDeclaration vertexDeclaration = null;

            TestXNAGame.Start( "TestColladaModel001",
                delegate
                {
                    main = TestXNAGame.Instance;

                    model = new ColladaModel( main );

                    ColladaMaterial mat = new ColladaMaterial();
                    ColladaMesh mesh = new ColladaMesh( main, model );
                    model.meshes.Add( mesh );
                    mesh.material = mat;

                    List<SkinnedTangentVertex> verts = new List<SkinnedTangentVertex>();
                    SkinnedTangentVertex v;
                    v = new SkinnedTangentVertex( new Vector3( 0, 0, 0 )
                        , new Vector3( 1, 0.2f, 0 )
                        , new Vector3( 0, 3, 0 )
                        , new Vector2( 0, 0 )
                        , new Vector3( 0, 0, 1 )
                        , new Vector3( 1, 0, 0 ) );
                    verts.Add( v );

                    v = new SkinnedTangentVertex( new Vector3( 1, 0, 0 )
                      , new Vector3( 1, 0, 0 )
                      , new Vector3( 0, 0, 0 )
                      , new Vector2( 0, 0 )
                      , new Vector3( 0, 0, 1 )
                      , new Vector3( 1, 0, 0 ) );
                    verts.Add( v );

                    v = new SkinnedTangentVertex( new Vector3( 0, 1, 0 )
                      , new Vector3( 0.5f, 0.5f, 0 )
                      , new Vector3( 0, 3, 0 )
                      , new Vector2( 0, 0 )
                      , new Vector3( 0, 0, 1 )
                      , new Vector3( 1, 0, 0 ) );
                    verts.Add( v );


                    ushort[] indices = new ushort[] { 0, 1, 2 };



                    // Create the vertex buffer from our vertices.
                    mesh.vertexBuffer = new VertexBuffer(
                        main.GraphicsDevice,
                        //typeof( SkinnedTangentVertex ),
                        typeof( SkinnedTangentVertex ),
                        verts.Count, BufferUsage.WriteOnly );
                    //ResourceUsage.WriteOnly,
                    //ResourceManagementMode.Automatic );
                    mesh.vertexBuffer.SetData( verts.ToArray() );
                    mesh.numOfVertices = verts.Count;


                    // Create the index buffer from our indices (Note: While the indices
                    // will point only to 16bit (ushort) vertices, we can have a lot
                    // more indices in this list than just 65535).
                    mesh.indexBuffer = new IndexBuffer(
                        main.GraphicsDevice,
                        typeof( ushort ),
                        indices.Length, BufferUsage.WriteOnly );
                    //ResourceUsage.WriteOnly, 
                    //ResourceManagementMode.Automatic );
                    mesh.indexBuffer.SetData( indices );
                    mesh.numOfIndices = indices.Length;




                    ColladaBone bone = new ColladaBone( Matrix.Identity, null, model.bones.Count, "Bone1", "Bone1" );
                    model.bones.Add( bone );


                    bone.animationMatrices.Add( Matrix.Identity );
                    bone.animationMatrices.Add( Matrix.CreateTranslation( new Vector3( 1, 0, 0 ) ) );
                    model.numOfAnimations = bone.animationMatrices.Count;
                    bone.invBoneSkinMatrix = Matrix.Identity;



                    ColladaBone boneOld;
                    boneOld = bone;
                    bone = new ColladaBone( Matrix.Identity, null, model.bones.Count, "Bone2", "Bone2" );
                    model.bones.Add( bone );
                    boneOld.children.Add( bone );
                    bone.parent = boneOld;

                    bone.animationMatrices.Add( Matrix.Identity );
                    bone.animationMatrices.Add( Matrix.CreateTranslation( new Vector3( 0, 1, 0 ) ) );

                    model.numOfAnimations = bone.animationMatrices.Count;
                    bone.invBoneSkinMatrix = Matrix.Identity;

                    model.frameRate = 1;
                },
                delegate
                {
                    if ( !started || main.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.L ) )
                    {
                        started = true;
                        //effect = new ShaderEffect( main, main.EngineFiles.RootDirectory + @"\Content\SimpleShader.fx" );
                        effect = new ShaderEffect( main, main.EngineFiles.RootDirectory + @"\Content\SkinnedNormalMapping.fx" );
                        effect.Load( main.Content );
                    }

                    main.GraphicsDevice.RenderState.CullMode = CullMode.None;
                    //main.GraphicsDevice.RenderState.AlphaBlendEnable = false;
                    //main.GraphicsDevice.RenderState.AlphaTestEnable = false;


                    //effect.Effect.View = main.ActiveCamera.CameraInfo.ViewMatrix;
                    //effect.Effect.Projection = main.ActiveCamera.CameraInfo.ProjectionMatrix;
                    //effect.Effect.World = 

                    Matrix worldMatrix = Matrix.Identity;//Matrix.CreateFromYawPitchRoll( -MathHelper.PiOver2, -MathHelper.PiOver2, 0 );

                    //effect.Effect.Parameters[ "worldViewProj" ].SetValue( worldMatrix * main.ActiveCamera.CameraInfo.ViewProjectionMatrix );
                    //effect.Effect.Parameters[ "world" ].SetValue( worldMatrix );
                    //effect.Effect.Parameters[ "viewInverse" ].SetValue( main.ActiveCamera.CameraInfo.InverseViewMatrix );
                    //effect.Effect.Parameters[ "ambientColor" ].SetValue( model.material.Ambient.ToVector4() );
                    //effect.Effect.Parameters[ "diffuseColor" ].SetValue( model.material.Ambient.ToVector4() );
                    //effect.Effect.Parameters[ "specularColor" ].SetValue( model.material.Ambient.ToVector4() );
                    //effect.Effect.Parameters[ "shininess" ].SetValue( model.material.Shininess );

                    model.UpdateAnimation( worldMatrix );

                    //effect.Effect.CurrentTechnique = effect.GetTechnique( "SpecularPerPixelColored" );

                    //effect.Effect.Begin();
                    //effect.Effect.CurrentTechnique.Passes[ 0 ].Begin();





                    //                Matrix worldMatrix =

                    ////Matrix.CreateFromYawPitchRoll( -MathHelper.PiOver2, -MathHelper.PiOver2, 0 ) *
                    //                    //Matrix.CreateFromYawPitchRoll( 0, -MathHelper.PiOver2, 0 ) 
                    //objectMatrix * renderMatrix;
                    //                //* Matrix.CreateFromYawPitchRoll( -MathHelper.PiOver2, -MathHelper.PiOver2, 0 );










                    //
                    //
                    //
                    //
                    //TODO: commented out !!!!!!!!
                    //
                    //      effect.SetParametersCollada( model.meshes[ 0 ].material );
                    //
                    //














                    //effect.ViewProjMatrix = model.Game.Camera.ViewProjection;


                    //Microsoft.Xna.Framework.Graphics.SamplerState s = new SamplerState();
                    //object o = effect.Effect.Parameters[ "DiffuseTextureSampler " ];


                    //if ( verticesSkinned.Count > 0 )
                    //{
                    if ( vertexDeclaration == null ) vertexDeclaration = SkinnedTangentVertex.CreateVertexDeclaration( main );
                    main.GraphicsDevice.VertexDeclaration = vertexDeclaration;
                    model.meshes[ 0 ].SetBoneMatrices( effect, model.GetBoneMatrices( worldMatrix ) );
                    effect.RenderCollada( "DiffuseSpecular20", model.meshes[ 0 ].RenderVerticesSkinned );
                    //}
                    //else
                    //{
                    //    effect.WorldMatrix = worldMatrix;
                    //    effect.WorldViewProjMatrix = worldMatrix * model.Game.Camera.ViewProjection;
                    //    game.GraphicsDevice.VertexDeclaration = SkinnedTangentVertex.VertexDeclaration;//TangentVertex.VertexDeclaration;

                    //    if ( material.DiffuseTexture == null )
                    //    {

                    //        effect.RenderCollada( "SpecularPerPixelColored", RenderVerticesSkinned ); //RenderVertices );
                    //    }
                    //    else if ( material.NormalTexture == null || game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Y ) )
                    //    {
                    //        effect.RenderCollada( "SpecularPerPixel", RenderVertices );
                    //    }
                    //    else
                    //    {
                    //        //Normal Mapping
                    //        effect.RenderCollada( "SpecularPerPixelNormalMapping", RenderVertices );
                    //    }
                    //}























                    //model.Render( worldMatrix, effect );
                    //effect.Effect.CurrentTechnique.Passes[ 0 ].End();

                    //effect.Effect.End();

                    main.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 2000, 0, 0 ), Color.Red );
                    main.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 0, 0, 2000 ), Color.Blue );


                } );
        }


        public static void TestColladaModel001()
        {


            TestXNAGame main = null;

            ColladaModel model = null;

            ShaderEffect effect = null;

            bool started = false;

            TestXNAGame.Start( "TestColladaModel001",
                delegate
                {
                    main = TestXNAGame.Instance;

                    /*main.Graphics.IsFullScreen = false;
                    main.Graphics.PreferredBackBufferWidth = 1024;
                    main.Graphics.PreferredBackBufferHeight = 768;
                    main.Graphics.ApplyChanges();*/

                    model = new ColladaModel( main );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\TestBox.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\TeapotConstructie.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\AdvancedScene001.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\TownOfHopeNoTex.DAE" );
                    model.LoadOud( main.EngineFiles.RootDirectory + @"\Content\Wall001.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\HuisjeInBos.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\SimpleBones001.DAE" ); 
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\TriangleBones001.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\TownOfHope002.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\TownOfHope003.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\TownOfHope004.DAE" );

                    //effect = new ShaderEffect( main, main.EngineFiles.RootDirectory + @"\Content\SimpleShader.fx" );
                    //effect = new ShaderEffect( main, main.EngineFiles.RootDirectory + @"\Content\SkinnedNormalMapping.fx" );
                },
                delegate
                {
                    if ( !started || main.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.L ) )
                    {
                        started = true;
                        effect = new ShaderEffect( main, main.EngineFiles.RootDirectory + @"\Content\SimpleShader.fx" );
                        //effect = new ShaderEffect( main, main.EngineFiles.RootDirectory + @"\Content\SkinnedNormalMapping.fx" );
                        effect.Load( main.Content );
                    }

                    //main.GraphicsDevice.RenderState.CullMode = CullMode.None;
                    //main.GraphicsDevice.RenderState.AlphaBlendEnable = false;
                    //main.GraphicsDevice.RenderState.AlphaTestEnable = false;


                    //effect.Effect.View = main.ActiveCamera.CameraInfo.ViewMatrix;
                    //effect.Effect.Projection = main.ActiveCamera.CameraInfo.ProjectionMatrix;
                    //effect.Effect.World = 

                    Matrix worldMatrix = Matrix.CreateFromYawPitchRoll( -MathHelper.PiOver2, -MathHelper.PiOver2, 0 );

                    //effect.Effect.Parameters[ "worldViewProj" ].SetValue( worldMatrix * main.ActiveCamera.CameraInfo.ViewProjectionMatrix );
                    //effect.Effect.Parameters[ "world" ].SetValue( worldMatrix );
                    //effect.Effect.Parameters[ "viewInverse" ].SetValue( main.ActiveCamera.CameraInfo.InverseViewMatrix );
                    //effect.Effect.Parameters[ "ambientColor" ].SetValue( model.material.Ambient.ToVector4() );
                    //effect.Effect.Parameters[ "diffuseColor" ].SetValue( model.material.Ambient.ToVector4() );
                    //effect.Effect.Parameters[ "specularColor" ].SetValue( model.material.Ambient.ToVector4() );
                    //effect.Effect.Parameters[ "shininess" ].SetValue( model.material.Shininess );


                    //effect.Effect.CurrentTechnique = effect.GetTechnique( "SpecularPerPixelColored" );

                    //effect.Effect.Begin();
                    //effect.Effect.CurrentTechnique.Passes[ 0 ].Begin();
                    model.Render( worldMatrix, effect );
                    //effect.Effect.CurrentTechnique.Passes[ 0 ].End();

                    //effect.Effect.End();

                    main.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 2000, 0, 0 ), Color.Red );
                    main.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 0, 0, 2000 ), Color.Blue );


                } );
        }
        public static void TestColladaModelSkinned()
        {


            TestXNAGame main = null;

            ColladaModel model = null;

            ShaderEffect effect = null;

            bool started = false;
            // Bone colors for displaying bone lines.
            Color[] BoneColors = new Color[]
				{ Color.DarkBlue, Color.DarkRed, Color.Yellow, Color.White, Color.Teal,
				Color.RosyBrown, Color.Orange, Color.Olive, Color.Maroon, Color.Lime,
				Color.LightBlue, Color.LightGreen, Color.Lavender, Color.Green,
				Color.Firebrick, Color.DarkKhaki, Color.BlueViolet, Color.Beige };
            TestXNAGame.Start( "TestColladaModel001",
                delegate
                {
                    main = TestXNAGame.Instance;

                    model = new ColladaModel( main );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\TestBox.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\TeapotConstructie.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\AdvancedScene001.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\TownOfHopeNoTex.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\Wall001.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\HuisjeInBos.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\SimpleBones001.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\test_man_baked.DAE" );
                    model.Load( new GameFile( main.EngineFiles.RootDirectory + @"\Content\SimpleCharacterAnim001.DAE" ) );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\VerySimpleBones001.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\TriangleBones001.DAE" );
                    //model.numOfAnimations = 40;
                    //model.frameRate = 20f;
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\Goblin.DAE" );


                    //effect = new ShaderEffect( main, main.EngineFiles.RootDirectory + @"\Content\SimpleShader.fx" );
                    //effect = new ShaderEffect( main, main.EngineFiles.RootDirectory + @"\Content\SkinnedNormalMapping.fx" );
                },
                delegate
                {
                    if ( !started || main.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.L ) )
                    {
                        started = true;
                        //effect = new ShaderEffect( main, main.EngineFiles.RootDirectory + @"\Content\SimpleShader.fx" );
                        effect = new ShaderEffect( main, main.EngineFiles.RootDirectory + @"\Content\SkinnedNormalMapping.fx" );
                        effect.Load( main.Content );
                    }

                    main.GraphicsDevice.RenderState.CullMode = CullMode.None;
                    //main.GraphicsDevice.RenderState.AlphaBlendEnable = false;
                    //main.GraphicsDevice.RenderState.AlphaTestEnable = false;


                    //effect.Effect.View = main.ActiveCamera.CameraInfo.ViewMatrix;
                    //effect.Effect.Projection = main.ActiveCamera.CameraInfo.ProjectionMatrix;
                    //effect.Effect.World = 

                    Matrix worldMatrix = Matrix.CreateFromYawPitchRoll( -MathHelper.PiOver2, -MathHelper.PiOver2, 0 );
                    //Matrix worldMatrix = Matrix.Identity;


                    //effect.Effect.Parameters[ "worldViewProj" ].SetValue( worldMatrix * main.ActiveCamera.CameraInfo.ViewProjectionMatrix );
                    //effect.Effect.Parameters[ "world" ].SetValue( worldMatrix );
                    //effect.Effect.Parameters[ "viewInverse" ].SetValue( main.ActiveCamera.CameraInfo.InverseViewMatrix );
                    //effect.Effect.Parameters[ "ambientColor" ].SetValue( model.material.Ambient.ToVector4() );
                    //effect.Effect.Parameters[ "diffuseColor" ].SetValue( model.material.Ambient.ToVector4() );
                    //effect.Effect.Parameters[ "specularColor" ].SetValue( model.material.Ambient.ToVector4() );
                    //effect.Effect.Parameters[ "shininess" ].SetValue( model.material.Shininess );

                    model.UpdateAnimation( worldMatrix );

                    //effect.Effect.CurrentTechnique = effect.GetTechnique( "SpecularPerPixelColored" );

                    //effect.Effect.Begin();
                    //effect.Effect.CurrentTechnique.Passes[ 0 ].Begin();
                    model.Render( worldMatrix, effect );
                    //effect.Effect.CurrentTechnique.Passes[ 0 ].End();

                    //effect.Effect.End();

                    foreach ( ColladaBone bone in model.bones )
                    {
                        foreach ( ColladaBone childBone in bone.children )
                            main.LineManager3D.AddLine(
                                ( bone.finalMatrix * worldMatrix ).Translation,
                                ( childBone.finalMatrix * worldMatrix ).Translation,
                                //( bone.animationMatrices[0] * worldMatrix ).Translation,
                                //( childBone.animationMatrices[0] * worldMatrix ).Translation,
                                BoneColors[ bone.num % BoneColors.Length ] );
                        //BaseGame.DrawLine(
                        //    bone.finalMatrix.Translation,
                        //    childBone.finalMatrix.Translation,
                        //    BoneColors[ bone.num % BoneColors.Length ] );
                    } // foreach (bone)

                    main.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 2000, 0, 0 ), Color.Red );
                    main.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 0, 0, 2000 ), Color.Blue );


                } );
        }

        public static void TestShowBones()
        {


            TestXNAGame main = null;

            ColladaModel model = null;

            ShaderEffect effect = null;

            bool started = false;

            // Bone colors for displaying bone lines.
            Color[] BoneColors = new Color[]
				{ Color.DarkBlue, Color.DarkRed, Color.Yellow, Color.White, Color.Teal,
				Color.RosyBrown, Color.Orange, Color.Olive, Color.Maroon, Color.Lime,
				Color.LightBlue, Color.LightGreen, Color.Lavender, Color.Green,
				Color.Firebrick, Color.DarkKhaki, Color.BlueViolet, Color.Beige };

            TestXNAGame.Start( "TestShowBones",
                delegate
                {
                    main = TestXNAGame.Instance;

                    Stream strm = TheWizards.Common.Core.EmbeddedFile.GetStreamFullPath(
                        "MHGameWork.TheWizards.Common.Core.Collada.Files.test_man_baked.DAE",
                        main.EngineFiles.RootDirectory + @"\DebugFiles\test_man_baked.DAE" );

                    model = ColladaModel.FromStream( strm );
                    //model = new ColladaModel( main );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\TestBox.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\TeapotConstructie.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\AdvancedScene001.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\TownOfHopeNoTex.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\Wall001.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\HuisjeInBos.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\SimpleBones001.DAE" );
                    //model.LoadOud( main.EngineFiles.RootDirectory + @"\Content\test_man_baked.DAE" );
                    //model.Load( main.EngineFiles.RootDirectory + @"\Content\SimpleCharacterAnim001.DAE" );


                    //effect = new ShaderEffect( main, main.EngineFiles.RootDirectory + @"\Content\SimpleShader.fx" );
                },
                delegate
                {
                    if ( !started || main.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.L ) )
                    {
                        started = true;
                        effect = new ShaderEffect( main, main.EngineFiles.RootDirectory + @"\Content\SimpleShader.fx" );
                        effect.Load( main.Content );
                    }

                    //main.GraphicsDevice.RenderState.CullMode = CullMode.None;
                    //main.GraphicsDevice.RenderState.AlphaBlendEnable = false;
                    //main.GraphicsDevice.RenderState.AlphaTestEnable = false;


                    //effect.Effect.View = main.ActiveCamera.CameraInfo.ViewMatrix;
                    //effect.Effect.Projection = main.ActiveCamera.CameraInfo.ProjectionMatrix;
                    //effect.Effect.World = 

                    Matrix worldMatrix = Matrix.CreateFromYawPitchRoll( -MathHelper.PiOver2, -MathHelper.PiOver2, 0 );

                    //effect.Effect.Parameters[ "worldViewProj" ].SetValue( worldMatrix * main.ActiveCamera.CameraInfo.ViewProjectionMatrix );
                    //effect.Effect.Parameters[ "world" ].SetValue( worldMatrix );
                    //effect.Effect.Parameters[ "viewInverse" ].SetValue( main.ActiveCamera.CameraInfo.InverseViewMatrix );
                    //effect.Effect.Parameters[ "ambientColor" ].SetValue( model.material.Ambient.ToVector4() );
                    //effect.Effect.Parameters[ "diffuseColor" ].SetValue( model.material.Ambient.ToVector4() );
                    //effect.Effect.Parameters[ "specularColor" ].SetValue( model.material.Ambient.ToVector4() );
                    //effect.Effect.Parameters[ "shininess" ].SetValue( model.material.Shininess );


                    //effect.Effect.CurrentTechnique = effect.GetTechnique( "SpecularPerPixelColored" );

                    //effect.Effect.Begin();
                    //effect.Effect.CurrentTechnique.Passes[ 0 ].Begin();
                    //model.Render( effect );
                    //effect.Effect.CurrentTechnique.Passes[ 0 ].End();

                    //effect.Effect.End();

                    model.UpdateAnimation( main.Elapsed, worldMatrix );


                    foreach ( ColladaBone bone in model.bones )
                    {
                        foreach ( ColladaBone childBone in bone.children )
                            main.LineManager3D.AddLine(
                                ( bone.finalMatrix * worldMatrix ).Translation,
                                ( childBone.finalMatrix * worldMatrix ).Translation,
                                BoneColors[ bone.num % BoneColors.Length ] );
                        //BaseGame.DrawLine(
                        //    bone.finalMatrix.Translation,
                        //    childBone.finalMatrix.Translation,
                        //    BoneColors[ bone.num % BoneColors.Length ] );
                    } // foreach (bone)
                    main.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 2000, 0, 0 ), Color.Red );
                    main.LineManager3D.AddLine( new Vector3( 0, 0, 0 ), new Vector3( 0, 0, 2000 ), Color.Blue );

                } );
        }
        ///// <summary>
        ///// TestShowBones
        ///// </summary>
        //public static void TestShowBonesOld()
        //{
        //    ColladaModel model = null;
        //    PlaneRenderer groundPlane = null;
        //    // Bone colors for displaying bone lines.
        //    Color[] BoneColors = new Color[]
        //        { Color.Blue, Color.Red, Color.Yellow, Color.White, Color.Teal,
        //        Color.RosyBrown, Color.Orange, Color.Olive, Color.Maroon, Color.Lime,
        //        Color.LightBlue, Color.LightGreen, Color.Lavender, Color.Green,
        //        Color.Firebrick, Color.DarkKhaki, Color.BlueViolet, Color.Beige };

        //    TestGame.Start( "TestLoadColladaModel",
        //        delegate
        //        {
        //            // Load our goblin here, you can also load one of my test models!
        //            model = new ColladaModel(
        //                //"Goblin");
        //                //"test_bones_simple_baked");
        //                //"test_bones_advanced_baked");
        //                "test_man_baked" );

        //            // And load ground plane
        //            groundPlane = new PlaneRenderer(
        //                new Vector3( 0, 0, -0.001f ),
        //                new Plane( new Vector3( 0, 0, 1 ), 0 ),
        //                new Material(
        //                    "GroundStone", "GroundStoneNormal", "GroundStoneHeight" ),
        //                50 );
        //        },
        //        delegate
        //        {
        //            // Show ground
        //            groundPlane.Render( ShaderEffect.parallaxMapping, "DiffuseSpecular20" );

        //            // Show bones without rendering the model itself
        //            if ( model.bones.Count == 0 )
        //                return;

        //            // Update bone animation.
        //            model.UpdateAnimation( Matrix.Identity );

        //            // Show bones (all endpoints)
        //            foreach ( Bone bone in model.bones )
        //            {
        //                foreach ( Bone childBone in bone.children )
        //                    BaseGame.DrawLine(
        //                        bone.finalMatrix.Translation,
        //                        childBone.finalMatrix.Translation,
        //                        BoneColors[ bone.num % BoneColors.Length ] );
        //            } // foreach (bone)
        //        } );
        //} // TestLoadColladaModel()
    }


}


