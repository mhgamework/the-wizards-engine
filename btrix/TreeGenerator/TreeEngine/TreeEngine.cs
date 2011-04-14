//using System;
//using System.Collections.Generic;
//using System.Text;
//using MHGameWork.TheWizards.ServerClient;
//using Microsoft.Xna.Framework;
//using TreeGenerator.help;
//using TreeGenerator.Imposter;
//using TreeGenerator.TreeEngine;
//using MHGameWork.TheWizards.Database;
//using MHGameWork.TheWizards.ServerClient.Database;
//using System.Windows.Forms;
//using MHGameWork.TheWizards.ServerClient.Terrain;
//using MHGameWork.TheWizards.Terrain.Editor;
//using MHGameWork.TheWizards.Graphics;

//namespace TreeGenerator.TreeEngine//array not,  minder data over en't weer smijte,  dottrace 3.1
//{
//    public class TreeEngine
//    {
//        private EngineTreeType[] TreeTypes;
//        EngineTreeRenderData[] RenderData;
//        EngineTreeRenderDataGenerater gen =new EngineTreeRenderDataGenerater(20);
//        private EngineTree[] Trees;

//        //ImposterEngine ImpEngine;
//        TreeImposterEngine TreeImpEngine;
//        IXNAGame game;
//        float distanceDiscartData=100;
//        Seeder seeder=new Seeder(698);

//        Vector3 TerrainPosition = Vector3.Zero;
//        TerrainFullData data = null;

//        public TreeEngine(float dist, IXNAGame _game)
//        {
//            distanceDiscartData = dist;
//            game = _game;
//            //ImpEngine = new ImposterEngine(game);
//            TreeImpEngine=new TreeImposterEngine(game);
            
//        }

//        public void Intialize(int numDifferentTrees,int numTrees,Vector3 MinBound,Vector3 MaxBound)
//        {
//            SetUpTerrain();

//            TreeTypes=new EngineTreeType[numDifferentTrees];

//            RenderData = new EngineTreeRenderData[numDifferentTrees];

//            Trees=new EngineTree[(int)(numTrees*1.2f)];

//            //ImpEngine.Initialze();
//            TreeImpEngine.Initialze();
//            for (int i = 0; i < numDifferentTrees; i++)
//            {
//                TreeTypes[i]=new EngineTreeType();
//                //EngineTreeType tree=new EngineTreeType();
//                //tree.seed=i;
//                seeder = new Seeder(i);
//                //TreeTypes.Add(tree);
//                TreeTypes[i].seed = i;
//                gen=new EngineTreeRenderDataGenerater(20);
//                TreeStructure treeStruct = new TreeStructure();
//                TreeRenderManager bodyManager=new TreeRenderManager();
//                TreeRenderManager leafManager=new TreeRenderManager();
//                gen.GetRenderData(treeStruct, game, Vector3.Zero,0);
//                //RenderData.Add(gen.TreeRenderData);
//                RenderData[i] = gen.TreeRenderData;
//                RenderData[i].Initialize();
//                //ImpEngine.AddRenderObject(RenderData[i].draw, RenderData[i].boundingBox);

//            }
//            for (int i = 0; i < numTrees; i++)
//            {   Vector3 temp=seeder.NextVector3(MinBound, MaxBound);
//            Vector3 pos = new Vector3(temp.X, RetreiveHeight(temp.X, temp.Z), temp.Z);
//                Trees[i] = new EngineTree(pos, seeder.NextFloat(0f, 3.14f),seeder.NextInt(0,RenderData.Length-1));
//                RenderData[Trees[i].TreeTypeData].WorldMatrices.Add(Trees[i].WorldMatrix);
//                //RenderData[Trees[i].TreeTypeData].Transform(Trees[i]);
//                //ImpEngine.AddRenderObject(RenderData[Trees[i].TreeTypeData].draw, RenderData[Trees[i].TreeTypeData].boundingBox);
//                int index = RenderData[Trees[i].TreeTypeData].WorldMatrices.Count - 1;
//                //TreeImpEngine.AddRenderObject(RenderData[Trees[i].TreeTypeData].RenderNew, RenderData[Trees[i].TreeTypeData].TransFormBoundingBox(index),index);
//                TreeImpEngine.AddRenderObjectLod(RenderData[Trees[i].TreeTypeData].RenderNew, RenderData[Trees[i].TreeTypeData].TransFormBoundingBox(index), index);
//            }
//        }

//       public void Update()
//       {
           
//           //TreeImpEngine.Update();
//           TreeImpEngine.UpdateLod();
//           //we don't need to discard any render data any more
//           //for (int i = 0; i < TreeImpEngine.Imposters.Count; i++)
//           //{
//           //    if (TreeImpEngine.Imposters[i].DiscartRenderData)
//           //    {
//           //        RenderData[i].DiscartRenderData();
//           //        TreeImpEngine.Imposters[i].DiscartRenderData = false;
//           //    }
//           //ImpEngine.Update();
//           //for (int i = 0; i < ImpEngine.Imposters.Count; i++)
//           //{
//           //     if (ImpEngine.Imposters[i].DiscartRenderData)
//           //        {
//           //            RenderData[i].DiscartRenderData();
//           //            ImpEngine.Imposters[i].DiscartRenderData = false;
//           //        }
//                   //if (ImpEngine.Imposters[i].LoadRenderData)
//                   //{
//                   //    RenderData[i].LoadRenderData(Trees[i], game);
//                   //    ImpEngine.Imposters[i].DiscartRenderData = false;
//                   //}
//           //}
          
//       }

//        public void Render()
//        {
//            //TreeImpEngine.Render();
//            TreeImpEngine.RenderLod();
//        }

//        public float RetreiveHeight(float x, float z)//is not working because of the testing
//        {
//            //return 0;
//             return data.HeightMap.CalculateHeight(x-TerrainPosition.X, z-TerrainPosition.Z);

//        }
//        public void SetUpTerrain()
//        {
//            Database database = loadDatabaseServices();
//            TerrainManagerService tms = new TerrainManagerService(database);
//            TaggedTerrain taggedTerrain = tms.CreateTerrain();


//            data = null;


//            data = taggedTerrain.GetFullData();
//            data.NumBlocksX = 30;
//            data.NumBlocksZ = 30;
//            data.BlockSize = 20; //16;
//            data.SizeX = data.NumBlocksX * data.BlockSize;
//            data.SizeZ = data.NumBlocksZ * data.BlockSize;
//            data.Position =TerrainPosition;//new Vector3(-data.BlockSize * (data.NumBlocksX / 2), 4, -data.BlockSize * (data.NumBlocksZ / 2));
//            data.HeightMap = new TerrainHeightMap(data.NumBlocksX * data.BlockSize, data.NumBlocksZ * data.BlockSize);

//            TerrainRaiseTool.RaiseTerrain(data, 300, 300, 60, 100);
//            TerrainRaiseTool.RaiseTerrain(data, 0,600, 120, 30);
//            TerrainRaiseTool.RaiseTerrain(data, 600, 0, 60, 400);
//            TerrainRaiseTool.RaiseTerrain(data, 78, 112, 18, -5);
//            TerrainRaiseTool.RaiseTerrain(data, -7, 50, 50, 30);
//            TerrainRaiseTool.RaiseTerrain(data, 0, 0, 300, 10);
//            TerrainRaiseTool.RaiseTerrain(data, 0, 0, 100, 40);
//            TerrainRaiseTool.RaiseTerrain(data, 500, 500, 100, 60);



//        }
//        private Database loadDatabaseServices()
//        {
//            Database database = new Database();
//            database.AddService(new DiskSerializerService(database, Application.StartupPath + "\\WizardsEditorSave"));
//            database.AddService(new DiskLoaderService(database));
//            database.AddService(new MHGameWork.TheWizards.ServerClient.Database.SettingsService(database, System.Windows.Forms.Application.StartupPath + "\\Settings.xml"));
//            database.AddService(new UniqueIDService(database));

//            return database;
//        }


//        public static void TestEngine()
//        {
//            XNAGame game;
//            game = new XNAGame();
//            game.DrawFps = true;
//            game.IsFixedTimeStep = false;
//            game.SpectaterCamera.FarClip = 10000;
            
//            TreeEngine engine = new TreeEngine(50, game);
         
//            game.InitializeEvent +=
//               delegate
//               {
//                   //engine.Intialize( 10, 200000, Vector3.Zero, new Vector3( 30000, 0, 30000 ) );
//                   //engine.Intialize(1,5, Vector3.Zero, new Vector3(10, 0, 10));
//                   engine.Intialize( 10, 12000, Vector3.Zero, new Vector3( 1500, 0, 1500 ) );
//                   //engine.Intialize( 10, 10, Vector3.Zero, new Vector3( 30, 0, 30 ) );
//                   //engine.Intialize( 10, 10000, Vector3.Zero, new Vector3( 2000, 0, 2000 ) );
//                   //engine.Intialize( 10, 1, Vector3.Zero, new Vector3( 1, 0, 1 ) );
//                   //engine.Intialize(10,60000,Vector3.Zero,new Vector3(3000,0,3000));
//                   //engine.Intialize( 10, 2000, Vector3.Zero, new Vector3( 600, 0, 600 ) );
//               };
//            game.UpdateEvent +=
//                delegate
//                {
//                    engine.Update();
                    
//                };
//            game.DrawEvent +=
//                delegate
//                {
                    
//                    engine.Render();
//                };
//            game.Run();
//        }
//        public static void TestEnginePlusGrass()
//        {
//            XNAGame game;
//            game = new XNAGame();
//            game.DrawFps = true;
//            game.IsFixedTimeStep = false;
//            game.SpectaterCamera.FarClip = 10000;
            

//            TreeEngine engine = new TreeEngine(50, game);
//            Grass.GrassMesh grass = new TreeGenerator.Grass.GrassMesh();

//            game.InitializeEvent +=
//               delegate
//               {
//                   //engine.Intialize( 10, 10, Vector3.Zero, new Vector3( 30, 0, 30 ) );
//                   engine.Intialize(10, 2000, Vector3.Zero, new Vector3(600, 0, 600));
//                   //engine.Intialize( 10, 12000, Vector3.Zero, new Vector3( 1500, 0, 1500 ) );
//                   //engine.Intialize( 10, 10, Vector3.Zero, new Vector3( 30, 0, 30 ) );
//                   //engine.Intialize( 10, 10000, Vector3.Zero, new Vector3( 2000, 0, 2000 ) );
//                   //engine.Intialize( 10, 1, Vector3.Zero, new Vector3( 1, 0, 1 ) );

//                   grass.Initialize(game,false,false);
//                   grass.CreateGrass(Vector3.Zero, new Vector3(200, 0, 200), new Vector2(1, 0.8f),new Vector2(1,1),new Vector2(1,1));
//                   grass.SetRenderData();
                   
//               };
//            game.UpdateEvent +=
//                delegate
//                {
//                    engine.Update();
                    
//                    //if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.A))
//                    //{
//                    //    EngineTreeType tree=new EngineTreeType();
//                    //    tree.seed = (int)(game.GameTime.ElapsedRealTime.Milliseconds);
//                    //    engine.seeder = new Seeder((int)(game.GameTime.ElapsedRealTime.Milliseconds));
//                    //    tree.Position = engine.seeder.NextVector3(Vector3.Zero, new Vector3(20, 0,20));
//                    //    engine.Trees.Add(tree);
//                    //    engine.gen=new EngineTreeRenderDataGenerater(20);
//                    //    engine.gen.GetRenderData(tree,game);
//                    //    engine.RenderData.Add(engine.gen.TreeRenderData);
//                    //    engine.RenderData[engine.RenderData.Count-1].Initialize();
//                    //    engine.ImpEngine.AddRenderObject(engine.RenderData[engine.RenderData.Count-1].draw, engine.RenderData[engine.RenderData.Count-1].boundingBox);
//                    //}
//                    //if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.T))
//                    //{
//                    //    engine.TreeImpEngine.textureOne.Save(game.EngineFiles.RootDirectory + "texturetest" + ".Bmp", Microsoft.Xna.Framework.Graphics.ImageFileFormat.Bmp);
//                    //}
//                };
//            game.DrawEvent +=
//                delegate
//                {

//                    engine.Render();
//                    grass.Render();
//                };
//            game.Run();
//        }
//    }
//}
