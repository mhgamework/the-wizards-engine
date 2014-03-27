using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.Rendering.Text;
using MHGameWork.TheWizards.Scattered.ProcBuilder;
using NUnit.Framework;
using ProceduralBuilder.Building;
using ProceduralBuilder.Conditions.PreConditions;
using ProceduralBuilder.Helpers;
using ProceduralBuilder.IO;
using ProceduralBuilder.Rendering;
using ProceduralBuilder.RulebaseModules;
using ProceduralBuilder.RulebaseModules.GUI;
using ProceduralBuilder.RulebaseModules.RulebaseGenerators;
using ProceduralBuilder.Rules;
using ProceduralBuilder.Scattered;
using ProceduralBuilder.Shapes;
using ProceduralBuilder.Tools;
using SlimDX;
using SlimDX.DirectInput;
using PointLight = MHGameWork.TheWizards.Rendering.Deferred.PointLight;

namespace ProceduralBuilder.Test
{
    [TestFixture]
    [EngineTest]
    public class IslandTest
    {
        #region Rendering

        private DeferredRenderer deferredRenderer;
        private DX11Game game;
        private static MainRandomGenerator rnd = MainRandomGenerator.Instance;
        private PointLight cameraLight;
        private PointLight sun;
        private PointLight extraLight;

        private TextTexture seedDisplay;
        private TextTexture batchDisplay;
        private string batchText = "";
        private TextTexture infoDisplay;
        private string infoText = "";

        private ShowCamera camera;
        private bool hideAxes;

        private TWEngine engine = EngineFactory.CreateEngine();

        private void initializeTW()
        {
            game = TW.Graphics;
            deferredRenderer = TW.Graphics.AcquireRenderer();

            var boxRenderer = BoxRenderer.Instance;
            boxRenderer.SetGame(game);

            if (hideAxes)
            {
                //Hide axes, ugly hack
                PropertyInfo prop = game.GetType().GetProperty("RenderAxis", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                prop.SetValue(game, false, null);
            }

            cameraLight = deferredRenderer.CreatePointLight();
            cameraLight.LightRadius = 75;

            extraLight = deferredRenderer.CreatePointLight();
            extraLight.LightRadius = 100f;
            extraLight.ShadowsEnabled = true;
            extraLight.Color = new Vector3(1, 1, 0.85f);
            extraLight.LightPosition = new Vector3(1000, 1000, 1000);

            sun = deferredRenderer.CreatePointLight();
            sun.LightRadius = 1000;
            float intensity = 1.25f;
            sun.Color = new Vector3(0.75f * intensity, 0.75f * intensity, 0.5f * intensity);
            sun.ShadowsEnabled = true;
            sun.LightPosition = new Vector3(-5, 30, 10);

            seedDisplay = new TextTexture(game, 500, 50);
            batchDisplay = new TextTexture(game, 500, 50);
            infoDisplay = new TextTexture(game, 500, 50);
        }

        private void update()
        {
            cameraLight.LightPosition = camera != null ? camera.GetCameraPosition() : game.SpectaterCamera.CameraPosition;

            seedDisplay.Clear();
            seedDisplay.DrawText("Current seed: " + rnd.GetCurrentSeed().ToString() + ".", new Vector2(0, 0), Color.White);
            seedDisplay.UpdateTexture();
            batchDisplay.Clear();
            batchDisplay.DrawText(batchText, new Vector2(0, 0), Color.White);
            batchDisplay.UpdateTexture();
            infoDisplay.Clear();
            infoDisplay.DrawText(infoText, new Vector2(0, 0), Color.White);
            infoDisplay.UpdateTexture();
            game.Device.ImmediateContext.ClearState();
            game.SetBackbuffer();
            game.Device.ImmediateContext.OutputMerger.BlendState =
                game.HelperStates.AlphaBlend;
            //game.Device.ImmediateContext.Rasterizer.State = game.HelperStates.RasterizerShowAll;

            game.TextureRenderer.Draw(seedDisplay.GPUTexture.View, new Vector2(0, 0),
                                      new Vector2(500, 50));
            game.TextureRenderer.Draw(batchDisplay.GPUTexture.View, new Vector2(0, 40),
                                      new Vector2(500, 50));
            game.TextureRenderer.Draw(infoDisplay.GPUTexture.View, new Vector2(0, 20),
                                      new Vector2(500, 50));

            game.Device.ImmediateContext.ClearState();
            game.SetBackbuffer();

        }

        #endregion Rendering

        [Test]
        public void TestGenerateBridge()
        {
            #region init

            initializeTW();
            var renderer = new TWRenderWrapper(deferredRenderer, game);

            const string startSemId = "Start";
            const bool hideMeshes = false;

            int width = 4;
            int height = 2;
            int length = 20;

            var startShapes = new List<IBuildingElement>();
            rnd.Reset();
            var face = new Face(startSemId, Matrix.RotationX(-(float)Math.PI * 0.5f) * Matrix.RotationY((float)Math.PI), new Vector2(width, length));
            startShapes.Add(face);

            #endregion init

            var builder = new Builder(renderer);
            builder.Build(startShapes, BridgeRuleBase(startSemId, hideMeshes, height), rnd.GetCurrentSeed());

            var exporter = new EasyExporter();

            game.GameLoopEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Key.F))
                    exporter.ExportScene(renderer.GetCurrentMeshes(), "bridge");

                if (game.Keyboard.IsKeyPressed(Key.R))
                {
                    startShapes = new List<IBuildingElement>();

                    rnd.Reset();

                    width = rnd.GetRandInt(2, 6);
                    height = rnd.GetRandInt(2, 4);
                    length = rnd.GetRandInt(15, 35);

                    face = new Face(startSemId, Matrix.RotationX(-(float)Math.PI * 0.5f) * Matrix.RotationY((float)Math.PI), new Vector2(width, length));
                    startShapes.Add(face);

                    builder.Purge();
                    builder.Build(startShapes, BridgeRuleBase(startSemId, hideMeshes, height), rnd.GetCurrentSeed());
                }

                renderer.Update();
                update();
            };

            game.Run();
        }

        /// <summary>
        /// Takes a flat groundplane (XZ) (the complete base of the bridge).
        /// </summary>
        /// <param name="startSemId"></param>
        /// <param name="hideMeshes"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static RuleBase BridgeRuleBase(string startSemId, bool hideMeshes, float height)
        {
            var rules = new List<ComponentRule>();

            const float stairlength = 2f;
            const float stepHeight = 0.2f;
            var nbStep = (int)Math.Round(height / stepHeight);

            rules.Add(RuleHelper.SplitRule(startSemId, Axis.YAxis, new[] { stairlength, 1, stairlength }, new[] { "a", "r", "a" }, new[] { "StairBase", "SpanBase", "StairBaseToRotate" }));
            rules.Add(new AxisOrientationRule { SemanticId = "StairBaseToRotate", NbTurns = 2, NewSemanticId = "StairBase" });

            var spanRB = BridgeSpanRuleBase("SpanBase", hideMeshes, height);
            var stairRB = StairRuleBase("StairBase", hideMeshes, stairlength, height, nbStep);

            foreach (var rule in spanRB.Rules)
            {
                rules.Add(rule);
            }
            foreach (var rule in stairRB.Rules)
            {
                rules.Add(rule);
            }

            if (!hideMeshes)
            {
                #region MeshLoading

                /*var blockTrans = new Vector3(blockSize * 0.5f, blockSize * 0.5f, 0);

                var meshInfo = new MeshInfo { Folder = @"Assets\Island\", MeshFileName = "MarbleBlock001.obj", MaterialFileName = "MarbleBlock001.mtl", InitialSize = new Vector3(1, 1, 1) };
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", SelectionWeight = 2, MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", SelectionWeight = 2, MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ((float)Math.PI * 0.5f) } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", SelectionWeight = 2, MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ((float)Math.PI) } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", SelectionWeight = 2, MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ(-(float)Math.PI * 0.5f) } });

                meshInfo = new MeshInfo { Folder = @"Assets\Island\", MeshFileName = "MarbleBlock002.obj", MaterialFileName = "MarbleBlock002.mtl", InitialSize = new Vector3(1, 1, 1) };
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ((float)Math.PI * 0.5f) } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ((float)Math.PI) } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ(-(float)Math.PI * 0.5f) } });

                meshInfo = new MeshInfo { Folder = @"Assets\Island\", MeshFileName = "MarbleBlock003.obj", MaterialFileName = "MarbleBlock003.mtl", InitialSize = new Vector3(1, 1, 1) };
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ((float)Math.PI * 0.5f) } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ((float)Math.PI) } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ(-(float)Math.PI * 0.5f) } });

                meshInfo = new MeshInfo { Folder = @"Assets\Island\", MeshFileName = "MarbleBlockCorner001.obj", MaterialFileName = "MarbleBlockCorner001.mtl", InitialSize = new Vector3(1, 1, 1) };
                rules.Add(new SubstituteRule { SemanticId = "BottomLeftCorner", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ((float)Math.PI * 0.5f) } });
                rules.Add(new SubstituteRule { SemanticId = "TopLeftCorner", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans } });
                rules.Add(new SubstituteRule { SemanticId = "BottomRightCorner", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ((float)Math.PI) } });
                rules.Add(new SubstituteRule { SemanticId = "TopRightCorner", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ(-(float)Math.PI * 0.5f) } });
                */
                #endregion MeshLoading
            }

            return new RuleBase { Rules = rules };
        }

        public static RuleBase BridgeSpanRuleBase(string startSemId, bool hideMeshes, float height)
        {
            var rules = new List<ComponentRule>();

            const float blockSize = 0.5f;

            rules.Add(new ExtrudeRule { SemanticId = startSemId, Amount = height - 0.5f, Parameters = new ExtrudeRule.ExtrudeParameters { Face02 = "LowerSpanSide", Face04 = "LowerSpanSide", OppositeFace = "SpanTop" } });

            rules.Add(RuleHelper.SplitRule("SpanTop", Axis.XAxis, new[] { 0.5f, 1f, 0.5f }, new[] { "a", "r", "a" }, new[] { "RoadSide", "Road", "RoadSide01" }));
            rules.Add(new AxisOrientationRule { SemanticId = "RoadSide01", NbTurns = 2, NewSemanticId = "RoadSide" });

            //SpanSides
            rules.Add(new RepeatSplitRule { SemanticId = "LowerSpanSide", Axis = Axis.XAxis, DesiredSplitSize = 1.8f, NewSemanticId = "LowerSpanSideTile" });
            rules.Add(RuleHelper.SplitRule("LowerSpanSideTile", Axis.XAxis, new[] { 0.3f, 1 }, new[] { "a", "r" }, new[] { "SupportBeam", "ArchPart" }));
            rules.Add(RuleHelper.SplitRule("ArchPart", Axis.YAxis, new[] { 1f, 1f }, new[] { "r", "a" }, new[] { "ToDelete", "StoneArch" }));
            rules.Add(RuleHelper.SplitRule("SupportBeam", Axis.YAxis, new[] { 1f, 1f }, new[] { "a", "r" }, new[] { "SupportBeamBase", "SupportBeamRest" }));
            rules.Add(new RepeatSplitRule { SemanticId = "SupportBeamRest", Axis = Axis.YAxis, DesiredSplitSize = 1, NewSemanticId = "SupportBeamUnit" });

            //Road
            rules.Add(new RepeatSplitRule { SemanticId = "Road", Axis = Axis.YAxis, DesiredSplitSize = blockSize, NewSemanticId = "RoadRow" });
            rules.Add(new RepeatSplitRule { SemanticId = "RoadRow", Axis = Axis.XAxis, DesiredSplitSize = blockSize, NewSemanticId = "BlockTile" });

            var roadSideTileSize = 0.75f;
            rules.Add(RuleHelper.SplitRule("RoadSide", Axis.YAxis, new[] { 0.5f, 1 }, new[] { "a", "r" }, new[] { "ColumnTile", "RoadSideRest" }));
            rules.Add(new RepeatSplitRule { SemanticId = "RoadSideRest", Axis = Axis.YAxis, DesiredSplitSize = 2.5f, NewSemanticId = "RoadSideRestTile" });
            rules.Add(RuleHelper.SplitRule("RoadSideRestTile", Axis.YAxis, new[] { 1, 0.5f }, new[] { "r", "a" }, new[] { "RoadSideToTile", "ColumnTile" }));
            rules.Add(new RepeatSplitRule { SemanticId = "RoadSideToTile", Axis = Axis.YAxis, DesiredSplitSize = roadSideTileSize, NewSemanticId = "RoadSideTile" });
            rules.Add(new RenameRule { SemanticId = "RoadSideTile", NewSemanticId = "SmoothBlock" });

            rules.Add(new RenameRule { SemanticId = "ColumnTile", NewSemanticId = "HighBlock" });

            rules.Add(new DeleteRule { SemanticId = "ToDelete" });


            if (!hideMeshes)
            {
                #region MeshLoading

                var meshInfo = new MeshInfo { Folder = @"Assets\Other\", MeshFileName = "SupportBeam001Base.obj", MaterialFileName = "SupportBeam001Base.mtl", InitialSize = new Vector3(0.3f, 1, 0.3f) };
                rules.Add(new SubstituteRule { SemanticId = "SupportBeamBase", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = new Vector3(0.15f, 0, 0) } });
                meshInfo = new MeshInfo { Folder = @"Assets\Other\", MeshFileName = "SupportBeam001.obj", MaterialFileName = "SupportBeam001.mtl", InitialSize = new Vector3(0.3f, 1, 0.3f) };
                rules.Add(new SubstituteRule { SemanticId = "SupportBeamUnit", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = new Vector3(0.15f, 0, 0) } });
                meshInfo = new MeshInfo { Folder = @"Assets\Other\", MeshFileName = "Arch001.obj", MaterialFileName = "Arch001.mtl", InitialSize = new Vector3(1.5f, 1, 0.3f) };
                rules.Add(new SubstituteRule { SemanticId = "StoneArch", MeshInfo = meshInfo });

                var blockTrans = new Vector3(blockSize * 0.5f, blockSize * 0.5f, -0.3f);
                meshInfo = new MeshInfo { Folder = @"Assets\Island\", MeshFileName = "MarbleBlock001Dark.obj", MaterialFileName = "MarbleBlock001Dark.mtl", InitialSize = new Vector3(1, 1, 1) };
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", SelectionWeight = 2, MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, ForceZScaling = true } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", SelectionWeight = 2, MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ((float)Math.PI * 0.5f), ForceZScaling = true } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", SelectionWeight = 2, MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ((float)Math.PI), ForceZScaling = true } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", SelectionWeight = 2, MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ(-(float)Math.PI * 0.5f), ForceZScaling = true } });
                meshInfo = new MeshInfo { Folder = @"Assets\Island\", MeshFileName = "MarbleBlock002Dark.obj", MaterialFileName = "MarbleBlock002Dark.mtl", InitialSize = new Vector3(1, 1, 1) };
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, ForceZScaling = true } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ((float)Math.PI * 0.5f), ForceZScaling = true } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ((float)Math.PI), ForceZScaling = true } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ(-(float)Math.PI * 0.5f), ForceZScaling = true } });
                meshInfo = new MeshInfo { Folder = @"Assets\Island\", MeshFileName = "MarbleBlock003Dark.obj", MaterialFileName = "MarbleBlock003Dark.mtl", InitialSize = new Vector3(1, 1, 1) };
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, ForceZScaling = true } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ((float)Math.PI * 0.5f), ForceZScaling = true } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ((float)Math.PI), ForceZScaling = true } });
                rules.Add(new SubstituteRule { SemanticId = "BlockTile", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = blockTrans, LocalRotation = Matrix.RotationZ(-(float)Math.PI * 0.5f), ForceZScaling = true } });

                var smoothBlockTrans = new Vector3(roadSideTileSize * 0.5f - 0.2f, roadSideTileSize * 0.5f - 0.05f, 0);
                meshInfo = new MeshInfo { Folder = @"Assets\Island\", MeshFileName = "MarbleBlock001Dark.obj", MaterialFileName = "MarbleBlock001Dark.mtl", InitialSize = new Vector3(0.6f, 1, 1.2f) };
                rules.Add(new SubstituteRule { SemanticId = "SmoothBlock", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = smoothBlockTrans, ForceZScaling = true } });
                rules.Add(new SubstituteRule { SemanticId = "SmoothBlock", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = smoothBlockTrans, LocalRotation = Matrix.RotationZ((float)Math.PI), ForceZScaling = true } });
                var highBlockTrans = new Vector3(0.25f, 0.25f, -0.1f);
                meshInfo = new MeshInfo { Folder = @"Assets\Island\", MeshFileName = "HighBlock001Dark.obj", MaterialFileName = "HighBlock001Dark.mtl", InitialSize = new Vector3(0.9f, 0.9f, 0.75f) };
                rules.Add(new SubstituteRule { SemanticId = "HighBlock", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = highBlockTrans, ForceZScaling = true } });
                rules.Add(new SubstituteRule { SemanticId = "HighBlock", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = highBlockTrans, LocalRotation = Matrix.RotationZ((float)Math.PI * 0.5f), ForceZScaling = true } });
                rules.Add(new SubstituteRule { SemanticId = "HighBlock", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = highBlockTrans, LocalRotation = Matrix.RotationZ((float)Math.PI), ForceZScaling = true } });
                rules.Add(new SubstituteRule { SemanticId = "HighBlock", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = highBlockTrans, LocalRotation = Matrix.RotationZ(-(float)Math.PI * 0.5f), ForceZScaling = true } });

                #endregion MeshLoading

            }

            return new RuleBase { Rules = rules };
        }

        public static RuleBase StairRuleBase(string startSemId, bool hideMeshes, float width, float height, int nbSteps)
        {
            var rules = new List<ComponentRule>();

            var stepWidth = width / nbSteps;
            var stepHeight = height / nbSteps;

            var stairSizeCond = new SizeCondition
            {
                AxisToValidate = Axis.YAxis,
                CompareType = CompareTypes.GreaterThanOrEquals,
                Value = stepWidth * 2
            };
            var stairSizeCondInv = new SizeCondition
            {
                AxisToValidate = Axis.YAxis,
                CompareType = CompareTypes.GreaterThanOrEquals,
                Value = stepWidth * 2,
                Invert = true
            };

            rules.Add(new RenameRule { SemanticId = startSemId, NewSemanticId = "StairToExtrude" });

            rules.Add(RuleHelper.SplitRule("StairTop", Axis.YAxis, new[] { stepWidth, 1 }, new[] { "a", "r" }, new[] { "StepTop", "StairToExtrude" }, new[] { (IPreCondition)stairSizeCond }.ToList()));
            rules.Add(new ExtrudeRule { SemanticId = "StairToExtrude", Amount = stepHeight, Parameters = new ExtrudeRule.ExtrudeParameters { Face01 = "StairSide", Face02 = "StairSideToFlip", Face04 = "StairSideToFlip", OppositeFace = "StairTop" } });


            rules.Add(new RepeatSplitRule { SemanticId = "StairSide", Axis = Axis.XAxis, DesiredSplitSize = 0.5f, NewSemanticId = "StairBlock" });
            rules.Add(new FlipRule { SemanticId = "StairSideToFlip", NewSemanticId = "StairSideSide" });
            rules.Add(new RepeatSplitRule { SemanticId = "StairSideSide", Axis = Axis.XAxis, DesiredSplitSize = 0.5f, NewSemanticId = "StairSideBlock" });
            rules.Add(new DeleteRule { SemanticId = "StepTop" });
            rules.Add(new DeleteRule { SemanticId = "StairTop", PreConditionList = new[] { (IPreCondition)stairSizeCondInv }.ToList() });

            if (!hideMeshes)
            {
                #region MeshLoading

                var meshInfo = new MeshInfo { Folder = @"Assets\Island\", MeshFileName = "MarbleBlock001Dark.obj", MaterialFileName = "MarbleBlock001Dark.mtl", InitialSize = new Vector3(1, 1, 1) };
                rules.Add(new SubstituteRule { SemanticId = "StairBlock", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = new Vector3(0.25f, 0, -0.40f), ForceZScaling = true } });
                rules.Add(new SubstituteRule { SemanticId = "StairSideBlock", MeshInfo = meshInfo, SubstitutionParameters = new SubstitutionParam { LocalTranslation = new Vector3(0.25f, 0, -0.25f), ForceZScaling = true } });

                #endregion MeshLoading

            }

            return new RuleBase { Rules = rules };
        }
        
        [Test]
        public void TestIslandGenerator()
        {
            initializeTW();

            var generator = new IslandGenerator();

            int seed = 1;
            var startShapes = generator.GetIslandBase(seed);
            var islandMesh = generator.GetIslandMesh(startShapes, seed);

            deferredRenderer.CreateMeshElement(islandMesh);

            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestIslandParts()
        {
            var seed = 1;
            
            #region init
            hideAxes = true;
            initializeTW();
            var renderer = new TWRenderWrapper(deferredRenderer, game);
            renderer.DrawFaceUnitAxes = false;
            rnd.Reset(seed);
            #endregion init

            var generator = new IslandGenerator();
            var startShapes = generator.GetIslandBase(seed);
            IMesh islandMesh;
            List<IBuildingElement> navMesh;
            List<IBuildingElement> buildMesh;
            List<IBuildingElement> borderMesh;
            generator.GetIslandParts(startShapes, seed, true, out islandMesh, out navMesh, out buildMesh, out borderMesh);

            var allFaces = navMesh.Select(element => ((Face) element).ExtraTransform(Matrix.Translation(0, 0, 0.25f), "nav")).ToList();
            allFaces.AddRange(buildMesh.Select(element => ((Face) element).ExtraTransform(Matrix.Translation(0, 0, 4), "build")));
            allFaces.AddRange(borderMesh.Select(element => ((Face) element).ExtraTransform(Matrix.Translation(0, 0, 8), "border")));

            var builder = new Builder(renderer);
            builder.Build(allFaces, new RuleBase(), seed);
            deferredRenderer.CreateMeshElement(islandMesh);

            game.GameLoopEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Key.L))
                {
                    extraLight.LightPosition = camera.GetCameraPosition();
                }
                renderer.Update();
                update();
                cameraLight.LightPosition = new Vector3(10, 15, 5);
            };

            game.Run();
        }
    }
}
