using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using MHGameWork.TheWizards.ServerClient;
using TreeGenerator.TreeEngine;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using TreeGenerator.Imposter;
using TangentVertex = MHGameWork.TheWizards.Graphics.Xna.Graphics.TODO.TangentVertex;

namespace TreeGenerator.LodEngine
{
    public class ImposterLodLayer : ITreeLodLayer
    {
        private TWRenderer renderer;
        private EngineTreeRenderDataGenerater treeRenderGenerater;

        private Dictionary<TreeLodEntity, ImposterEntityStruct> imposterEnitiesIndex = new Dictionary<TreeLodEntity, ImposterEntityStruct>();
        private List<ImposterEntityStruct> imposterEnities = new List<ImposterEntityStruct>();
        private bool[] freePlaces;

        public TangentVertex[] Vertices;
        public VertexBuffer vertexBuffer;
        public VertexDeclaration decl;
        public int vertexCount;
        public int triangleCount;
        public int vertexStride;
        public ColladaShader Shader;
        XNAGame game;
        GraphicsDevice device;

        private DepthStencilBuffer depthBuffer;
        SpriteBatch batch;
        public RenderTarget2D RenderTarget;
        public Texture2D texture;
        public int PixelSizeOfOneImposter;
        public int NumberOfImposters;
        public int NumberOfImpostersOnOneLine;
        private int imposterPixelSize;
        public float Time = 0;
        public float TimeForUpdate = 0.5f;

        public float ImposterAngleThreshold = 4f;
        public int ImposterUpdatesPerFrame = 4;

        public ImposterLodLayer(EngineTreeRenderDataGenerater treeRenderGenerater, XNAGame game, int pixelSizeOfOneImposter, int numberOfImpostersOnOneLine, float timeForUpdate, float imposterAngleThreshold, int imposterUpdatesPerFrame)
        {
            this.renderer = renderer;
            this.treeRenderGenerater = treeRenderGenerater;
            this.game = game;
            device = game.GraphicsDevice;
            PixelSizeOfOneImposter = pixelSizeOfOneImposter;
            NumberOfImpostersOnOneLine = numberOfImpostersOnOneLine;
            NumberOfImposters = numberOfImpostersOnOneLine * numberOfImpostersOnOneLine;
            TimeForUpdate = timeForUpdate;
            ImposterAngleThreshold = imposterAngleThreshold;
            ImposterUpdatesPerFrame = imposterUpdatesPerFrame;
            imposterPixelSize = pixelSizeOfOneImposter*numberOfImpostersOnOneLine;
            RenderTarget = new RenderTarget2D(device,imposterPixelSize, imposterPixelSize, 0, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            vertexCount = numberOfImpostersOnOneLine * numberOfImpostersOnOneLine * 6;
            Vertices = new TangentVertex[vertexCount];
            Shader = new ColladaShader(game, new Microsoft.Xna.Framework.Graphics.EffectPool());
            freePlaces = new bool[NumberOfImposters];

            Shader.Technique = ColladaShader.TechniqueType.Textured;

            Shader.ViewInverse = Matrix.Identity;
            Shader.ViewProjection = Matrix.Identity;
            Shader.World = Matrix.Identity;
            Shader.LightDirection = Vector3.Normalize(new Vector3(0.6f, 1f, 0.6f));
            Shader.LightColor = new Vector3(0, 0, 0);

            Shader.AmbientColor = new Vector4(1f, 1f, 1f, 1f);
            Shader.DiffuseColor = new Vector4(1f, 1f, 1f, 1f);
            Shader.SpecularColor = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);

            Shader.Technique = ColladaShader.TechniqueType.Textured;
            Shader.DiffuseTexture = texture;
            decl = TangentVertexExtensions.CreateVertexDeclaration(game);
            vertexStride = TangentVertex.SizeInBytes;
            vertexBuffer = new DynamicVertexBuffer(device, typeof(TangentVertex), vertexCount, BufferUsage.WriteOnly);//TODO DYnamic vertexbuffer performance error 
            depthBuffer = new DepthStencilBuffer(device,imposterPixelSize, imposterPixelSize, device.DepthStencilBuffer.Format);

            for (int i = 0; i < freePlaces.Length; i++)
                freePlaces[i] = true;


            //testing
            batch = new SpriteBatch(device);
        }


        protected void ComputeBoundingBoxFromPoints(Vector3[] vertices, out Vector3 min, out Vector3 max)
        {
            min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < vertices.Length; i++)
            {
                min = Vector3.Min(min, vertices[i]);
                max = Vector3.Max(max, vertices[i]);
            }
        }

        void UpdateImposterLod()
        {
            int count = 0;
            for (int i = 0; i < imposterEnities.Count; i++)
            {

                if (!imposterEnities[i].NeedsUpdate)
                    count++;
                if (count > ImposterUpdatesPerFrame)
                    break;
                if (imposterEnities[i].UrgeForUpdate < ImposterAngleThreshold && !imposterEnities[i].NeedsUpdate) break;
               
                Vector3[] impostorVerts = new Vector3[8];

                Vector3 cameraPosition = game.Camera.ViewInverse.Translation;
                ImposterCamera camera = new ImposterCamera();

                Vector3[] corners = new Vector3[8];

                corners = imposterEnities[i].BoundingBoxData;

                camera.LookAt(cameraPosition, imposterEnities[i].MeshCenter);
                Vector3 CameraDirection = cameraPosition - imposterEnities[i].MeshCenter;
                imposterEnities[i].Normal = CameraDirection;
            
                camera.BuildView();


              
                Vector3[] screenVerts = new Vector3[8];
                for (int j = 0; j < 8; j++)
                {
                    screenVerts[j] = device.Viewport.Project(corners[j], camera.Projection, camera.View, Matrix.Identity);

                }

                // compute the screen space AABB
                Vector3 min, max;
                ComputeBoundingBoxFromPoints(screenVerts, out min, out max);

                // construct the quad that will represent our diffuse mesh
                Vector3[] screenQuadVerts = new Vector3[4];
                screenQuadVerts[0] = new Vector3(min.X, min.Y, min.Z);
                screenQuadVerts[1] = new Vector3(max.X, min.Y, min.Z);
                screenQuadVerts[2] = new Vector3(max.X, max.Y, min.Z);
                screenQuadVerts[3] = new Vector3(min.X, max.Y, min.Z);


                //----------------------------------------------------------------------
                // Step 3. Unproject the screen quad vertices to form a 3D quad that 
                //         represents our impostor. We will use this to both draw the 
                //         impostor quad and for intersecting the impostor for reflection.

                //now unproject the screen space quad and save the
                //vertices for when we render the impostor quad
                for (int j = 0; j < 4; j++)
                {
                    impostorVerts[j] = device.Viewport.Unproject(screenQuadVerts[j], camera.Projection, camera.View, Matrix.Identity);
                }
                //impostorCenter = impostorVerts[0] + impostorVerts[1] + impostorVerts[2] + impostorVerts[3];
                //impostorCenter *= .25f;

                float width = (impostorVerts[1] - impostorVerts[0]).Length() * 1.2f;
                float height = (impostorVerts[3] - impostorVerts[0]).Length() * 1.2f;

             
                camera.Projection = Matrix.CreateOrthographic(width, height, .1f, 1000);
                camera.BuildView();
                imposterEnities[i].cam = camera;
                
                CreatePlainLod(imposterEnities[i], impostorVerts);
                imposterEnities[i].NeedsUpdate = false;
                imposterEnities[i].NeedsImposterRender = true;
                imposterEnities[i].CalculatedUrgeForUpdate(game.Camera.ViewInverse.Translation);
               

            }


            //NOTE: BART ik weet waarom het zo traag gaat (waarschijnlijk). 
            //      Ge kopieerd elke frame u volledige lijst vertices, dus mss wa minder voor de performance
            //      een oplossing hiervoor is mss gewoon direct setdata doen ipv ze eerst in de VertOne op te slagen
            //      of een double buffer maken: 2 vertexbuffer's, ene met de current en ene met de nieuwe, maar ik denk dat
            //      BufferUsage.Dynamic ervoor zorgt dat u videokaart al zo iets doet.
            SetupForRenderLod();


        }
        private void CreatePlainLod(ImposterEntityStruct imp,Vector3[] impostorVerts)
        {
             Vector3 tangent = Vector3.Up;
            Vector2 uvStart, uvEnd;

            float tempSize = 1;

            tempSize = 1f / NumberOfImpostersOnOneLine;//clean-up texturesizeone and tow will always be the same



                uvStart = new Vector2(imp.XCoord * tempSize, imp.YCoord * tempSize);
                uvEnd = new Vector2( ( imp.XCoord + 1 ) * tempSize, ( imp.YCoord + 1 ) * tempSize );

            

                Vertices[ imp.GlobalIndexOfPositionOnTexture * 6 + 0 ] = new TangentVertex( impostorVerts[ 0 ], uvStart.X, uvStart.Y, imp.Normal, tangent );
                Vertices[ imp.GlobalIndexOfPositionOnTexture * 6 + 1 ] = new TangentVertex( impostorVerts[ 3 ], uvStart.X, uvEnd.Y, imp.Normal, tangent );
                Vertices[ imp.GlobalIndexOfPositionOnTexture * 6 + 2 ] = new TangentVertex( impostorVerts[ 2 ], uvEnd.X, uvEnd.Y, imp.Normal, tangent );
                //triangle 2
                Vertices[ imp.GlobalIndexOfPositionOnTexture * 6 + 3 ] = new TangentVertex( impostorVerts[ 0 ], uvStart.X, uvStart.Y, imp.Normal, tangent );
                Vertices[ imp.GlobalIndexOfPositionOnTexture * 6 + 4 ] = new TangentVertex( impostorVerts[ 1 ], uvEnd.X, uvStart.Y, imp.Normal, tangent );
                Vertices[ imp.GlobalIndexOfPositionOnTexture * 6 + 5 ] = new TangentVertex( impostorVerts[ 2 ], uvEnd.X, uvEnd.Y, imp.Normal, tangent );



        }
        public void RenderImpostersLod()
        {


            Viewport oldViewPort = device.Viewport;
            DepthStencilBuffer oldDepthBuffer = device.DepthStencilBuffer;
            ICamera camera = game.Camera;
            
         
            device.SetRenderTarget(0, RenderTarget);
            device.DepthStencilBuffer = depthBuffer;
            if (texture==null)
            {
                device.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, new Color(0, 0, 0, 0), 1f, 0);
            }
            
            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.AlphaTestEnable = false;
           
            
         

            for (int i = 0; i < imposterEnities.Count; i++)
            {

                if (imposterEnities[i].NeedsImposterRender )
                {
                    if (Vertices[imposterEnities[i].GlobalIndexOfPositionOnTexture * 6].pos.LengthSquared() < 0.1f)
                        throw new InvalidOperationException();


                    device.Viewport = imposterEnities[i].VPort;
                    device.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, new Color(0,0,0,0), 1f, 0);
                    game.SetCamera(imposterEnities[i].cam);
                    imposterEnities[i].renderdata.SetWorldMatrix(imposterEnities[i].ent.WorldMatrix);
                    imposterEnities[i].renderdata.Render(game);

                    imposterEnities[i].NeedsImposterRender = false;

                }
            }
            device.Viewport = oldViewPort;
            device.SetRenderTarget(0, null);
            texture = RenderTarget.GetTexture();
            device.DepthStencilBuffer = oldDepthBuffer;
            game.SetCamera(camera);
            device.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.CornflowerBlue, 1f, 0);


        }
        public void SetupForRenderLod()
        {
            vertexBuffer.SetData<TangentVertex>(Vertices);
            triangleCount = Vertices.Length/3;
            Shader.DiffuseTexture = texture;
        }

        public void Update()
       {
           bool calculateUrge = false;
           Time += game.Elapsed;
           if (Time > TimeForUpdate)
           {
               calculateUrge = true;
               Time = 0;
           }
            for (int i = 0; i < imposterEnities.Count; i++)
            {
                if (calculateUrge)
                {
                    imposterEnities[i].CalculatedUrgeForUpdate(game.Camera.ViewInverse.Translation);
                }
            }
            sort();
            UpdateImposterLod();
       }

        private int sortingIndex = -1;
        public void sort()
        {
            // Bart, ge had al een manier om de imposters te vergelijken gemaakt. 
            //   Ik vermoed dat er iets mis is met de methode die ge nu doet, vandaar ik het verander
            //   Als er toch geen fout in zit is het alleszins duidelijker



            int i = 0;
            bool swap;
            do
            {
                swap = false;
                for (int j = i + 1; j <= imposterEnities.Count-1; j++)
                {
                    if (imposterEnities[j].CompareTo(imposterEnities[i]) < 0)
                    {
                        ImposterEntityStruct tmp = imposterEnities[i];
                        imposterEnities[i] = imposterEnities[j];
                        imposterEnities[j] = tmp;

                        swap = true;
                    }
                    
                }
                i++;

            }
            while (swap);


        }
        public void Render()
        {
           
            //RenderImpostersLod();

            //device.RenderState.AlphaBlendEnable = false;
            device.RenderState.CullMode = CullMode.None;
            device.RenderState.AlphaTestEnable = true;
            //device.RenderState.ReferenceAlpha = 10;
            //device.RenderState.AlphaFunction = CompareFunction.GreaterEqual;

            device.RenderState.SourceBlend = Blend.One;
            device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;


            Shader.ViewProjection = game.Camera.ViewProjection;
            Shader.ViewInverse = game.Camera.ViewInverse;

            Shader.Shader.RenderMultipass(renderPrimitives);
            //batch.Begin();
            //batch.Draw(texture, Vector2.Zero, Color.White);
            //batch.End();
            //for (int i = 0; i < imposterEnities.Count; i++)
            //{

            //    imposterEnities[i].renderdata.SetWorldMatrix(imposterEnities[i].ent.WorldMatrix);
            //    imposterEnities[i].renderdata.Render();


            //    //game.LineManager3D.AddBox(BoundingBox.CreateFromPoints(imposterEnities[i].BoundingBoxData), Color.Red);

            //    //game.LineManager3D.AddCenteredBox(imposterEnities[i].BoundingBoxPosition, 0.5f, Color.Red);
            //    //game.LineManager3D.AddCenteredBox(imposterEnities[i].MeshCenter, 0.5f, Color.Green);

            //}

            //for (int i = 0; i < Vertices.Length/3; i++)
            //{
            //    game.LineManager3D.AddTriangle(Vertices[i].pos, Vertices[i + 1].pos, Vertices[i + 2].pos, Color.Black);
            //}
           
        }

        private void renderPrimitives()
        {
            device.Vertices[0].SetSource(vertexBuffer, 0, TangentVertex.SizeInBytes);
            device.VertexDeclaration = decl;
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, triangleCount);
        }
        private int ReturnFreePlace()
        {
            //NOTE: Wat de free places betreft
            //      speedup: sla de index eerste lege plaats op in een int. Als een plaats wordt geleegd voor die eerste plaats
            //               wordt dat de eerste plaats. Is die eerste plaats niet meer leeg, zoek dan de eerstvolgende lege.
            //               Start dus met zoeken vanaf de index in die variabele naar een lege plaats
            for (int i = 0; i < freePlaces.Length; i++)
            {
                if (freePlaces[i])
                {
                    freePlaces[i] = false;
                    return i;
                    
                }
            }
            return -1;

        }

        public bool AddEntity(TreeLodEntity ent)
        {
            int freePlace = ReturnFreePlace();
            if (freePlace<0)
            {
                return false;
            }
            EngineTreeRenderData renderData = treeRenderGenerater.GetRenderData(ent.TreeStructure, game, 1);
            renderData.Initialize();
            ImposterEntityStruct structure = new ImposterEntityStruct(ent,renderData , freePlace);
            structure.CalculateXYCoord(NumberOfImpostersOnOneLine);
            structure.CreateViewPort(PixelSizeOfOneImposter);
            imposterEnities.Add(structure);
            imposterEnitiesIndex.Add(ent, structure);
            Update();
            return true;
        }

        public void RemoveEntity(TreeLodEntity ent)
        {
            ImposterEntityStruct structure =imposterEnitiesIndex[ent];
            imposterEnitiesIndex.Remove(ent);
            imposterEnities.Remove(structure);
            freePlaces[structure.GlobalIndexOfPositionOnTexture] = true;
            for (int i = 0; i < 6; i++)
            {
                Vertices[structure.GlobalIndexOfPositionOnTexture*6 + i].pos = Vector3.Zero;
            }
        }

      
      
        private float minDistance;
        public float MinDistance
        {
            get { return minDistance; }
            set { minDistance = value; }
        }


        #region ITreeLodLayer Members


        public bool IsFull()
        {
            if (imposterEnities.Count>=freePlaces.Length)
                return true;
            return false;
        }

        #endregion
    }

    class ImposterEntityStruct:IComparable<ImposterEntityStruct>
    {
        public TreeLodEntity ent;
        public EngineTreeRenderData renderdata;
        public float UrgeForUpdate;
        public bool NeedsImposterRender;
        public bool NeedsUpdate;
        public Vector3 Normal;
        public Vector3 MeshCenter;
        public int GlobalIndexOfPositionOnTexture;
        public int XCoord, YCoord;
        public ImposterCamera cam;
        public Vector3 BoundingBoxPosition;
        public Vector3[] BoundingBoxData = new Vector3[8];
        public ImposterEntityStruct(TreeLodEntity ent, EngineTreeRenderData renderdata, int globalIndexOfPositionOnTexture)
        {
            this.ent = ent;
            this.renderdata = renderdata;
            GlobalIndexOfPositionOnTexture = globalIndexOfPositionOnTexture;
            NeedsUpdate = true;
            UrgeForUpdate = 1000;
            for (int i = 0; i < renderdata.BoundingBoxData.Length; i++)
            {
                BoundingBoxData[i] = renderdata.BoundingBoxData[i]+ ent.WorldMatrix.Translation;

            }
            CalculateCenterAndBoundingBoxPosition();
            
            
        }
        public float PixelPerMeter;
        private float time = 0;

     
        public float Time
        {
            get { return time; }
            set { time = value; }
        }

        public Viewport VPort;

        public void CalculatedUrgeForUpdate(Vector3 cameraPosition)
        {
            if (Normal.LengthSquared() < 0.00001f)
            {
                UrgeForUpdate = 1000;
                return;
            }

            UrgeForUpdate = Math.Abs(AngleBetweenTwoV3(cameraPosition - BoundingBoxPosition, Normal));
            if (float.IsNaN(UrgeForUpdate)) throw new InvalidOperationException();
        }
        public float AngleBetweenTwoV3(Vector3 v1, Vector3 v2)
        {
            v1.Normalize();
            v2.Normalize();
            float angle = (float)Math.Acos(MathHelper.Clamp(Vector3.Dot(v1, v2), -1, 1));
            angle = MathHelper.ToDegrees(angle);

            return angle;
        }// in radians
        public void CalculateCenterAndBoundingBoxPosition()
        {

            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < BoundingBoxData.Length; i++)
            {
                min = Vector3.Min(min, BoundingBoxData[i]);
                max = Vector3.Max(max, BoundingBoxData[i]);
            }

            MeshCenter = (min + max) * 0.5f;
            BoundingBoxPosition = new Vector3((BoundingBoxData[1].X + BoundingBoxData[0].X) * 0.5f, BoundingBoxData[0].Y, (BoundingBoxData[2].Z + BoundingBoxData[0].Z) * 0.5f);
        }
        public void CalculateXYCoord(int imposterCount)
        {
            //changed textureindex to index
            //float step = 1f / imposterCount;
            XCoord = (GlobalIndexOfPositionOnTexture - (int)(GlobalIndexOfPositionOnTexture / imposterCount) * imposterCount); //* step;
            YCoord = ((int)(GlobalIndexOfPositionOnTexture / imposterCount));// *step;
           
        }
        public void CreateViewPort(int resSize)
        {
            VPort = new Viewport();
            VPort.X = XCoord * resSize;
            if (VPort.X> 4000)
            {
                int a = 0;
            }
            VPort.Y = YCoord * resSize;
            VPort.Width = resSize;
            VPort.Height = resSize;
            VPort.MaxDepth = 1.0f;
            VPort.MinDepth = 0.0f;

            PixelPerMeter = resSize/(BoundingBoxData[2].X - BoundingBoxData[0].X);
        }
        #region IComparable<ImposterEntityStruct> Members

        public int CompareTo(ImposterEntityStruct other)
        {
            if (NeedsUpdate && other.NeedsUpdate) return 0;
            if (NeedsUpdate) return -1;
            if (other.NeedsUpdate) return 1;
            if (UrgeForUpdate > other.UrgeForUpdate) return -1;
            if (UrgeForUpdate < other.UrgeForUpdate) return 1;
            return 0;

        }

        #endregion
    }
}
