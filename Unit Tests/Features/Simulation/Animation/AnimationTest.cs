using System.Drawing;
using DirectX11;
using MHGameWork.TheWizards.Animation;
using MHGameWork.TheWizards.Common.Core;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Graphics;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Tests.Features.Simulation.Animation
{
    [TestFixture]
    public class AnimationTest
    {

        /// <summary>
        /// This test uses bad and unfinished design and is to be corrected
        /// </summary>
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestSkeletonVisualizer()
        {
            
            var game = new DX11Game();

            var skeleton = new Skeleton();
            Joint joint;

            joint = new Joint();
            joint.Name = "Root";
            joint.Length = 2;
            joint.AbsoluteMatrix = Matrix.RotationZ(MathHelper.PiOver2) * // This makes X the forward direction
                Matrix.Translation(5, 0, 5);
            skeleton.Joints.Add(joint);

            var parent = joint;

            joint = new Joint();
            joint.Name = "Arm1";
            joint.Length = 2;
            joint.Parent = parent;
            joint.AbsoluteMatrix = Matrix.RotationZ(MathHelper.PiOver4) * Matrix.Translation(4, 0, 0)
                * joint.Parent.AbsoluteMatrix;
            skeleton.Joints.Add(joint);

            joint = new Joint();
            joint.Name = "Arm2Upper";
            joint.Length = 2;
            joint.Parent = parent;
            joint.AbsoluteMatrix = Matrix.RotationZ(-MathHelper.PiOver4) * Matrix.Translation(4, 0, 0)
                * joint.Parent.AbsoluteMatrix;
            skeleton.Joints.Add(joint);

            parent = joint;

            joint = new Joint();
            joint.Name = "Arm2Lower";
            joint.Length = 2;
            joint.Parent = parent;
            joint.AbsoluteMatrix = Matrix.RotationZ(MathHelper.PiOver4) * Matrix.Translation(2, 0, 0)
                * joint.Parent.AbsoluteMatrix;
            skeleton.Joints.Add(joint);


            var vis = new SkeletonVisualizer();



            game.GameLoopEvent += delegate
                                  {
                                      vis.VisualizeSkeleton(game, skeleton);
                                  };

            game.Run();

        }

        /// <summary>
        /// This test uses bad and unfinished design and is to be corrected
        /// </summary>
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestSkeletonUpdateAbsoluteMatrices()
        {
            var game = new DX11Game();

            var skeleton = new Skeleton();
            Joint joint;

            joint = new Joint();
            joint.Name = "Root";
            joint.Length = 4;
            joint.CalculateInitialRelativeMatrix(Matrix.Translation(5, 0, 5));
            skeleton.Joints.Add(joint);

            var parent = joint;

            joint = new Joint();
            joint.Name = "Arm1";
            joint.Length = 2;
            joint.Parent = parent;
            joint.CalculateInitialRelativeMatrix(Matrix.RotationZ(MathHelper.PiOver4));
            skeleton.Joints.Add(joint);

            joint = new Joint();
            joint.Name = "Arm2Upper";
            joint.Length = 2;
            joint.Parent = parent;
            joint.CalculateInitialRelativeMatrix(Matrix.RotationZ(-MathHelper.PiOver4));
            skeleton.Joints.Add(joint);

            parent = joint;

            joint = new Joint();
            joint.Name = "Arm2Lower";
            joint.Length = 2;
            joint.Parent = parent;
            joint.CalculateInitialRelativeMatrix(Matrix.RotationY(MathHelper.PiOver4));
            skeleton.Joints.Add(joint);


            var vis = new SkeletonVisualizer();

            skeleton.UpdateAbsoluteMatrices();

            game.GameLoopEvent += delegate
            {
                vis.VisualizeSkeleton(game, skeleton);
            };

            game.Run();

        }
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestImportASF()
        {
            var parser = new ASFParser();

            using (var strm = EmbeddedFile.GetStream("MHGameWork.TheWizards.Tests.Features.Simulation.Animation.Files.TestSkeleton01.asf"))
            {
                parser.ImportASF(strm);
            }

            var game = new DX11Game();


            game.GameLoopEvent += delegate
                                  {
                                      drawASFJoint(game, parser.RootJoint, new Vector3(4, 0, 4));
                                  };

            game.Run();

        }
        private void drawASFJoint(DX11Game game, ASFJoint joint, Vector3 pos)
        {
            for (int i = 0; i < joint.children.Count; i++)
            {
                var child = joint.children[i];
                var end = pos + child.direction * child.length;
                game.LineManager3D.AddLine(pos, end, Color.White);
                drawASFJoint(game, child, end);
            }
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestImportSkeletonFromASF()
        {
            var parser = new ASFParser();


            var root = new ASFJoint();
            parser.RootJoint = root;

            var child1 = new ASFJoint();
            child1.length = 4;
            child1.direction = MathHelper.Up;
            root.children.Add(child1);

            var child2 = new ASFJoint();
            child2.direction = MathHelper.Up;
            child1.children.Add(child2);

            var skeleton1 = parser.ImportSkeleton();

            skeleton1.UpdateAbsoluteMatrices();

            using (var strm = EmbeddedFile.GetStream("MHGameWork.TheWizards.Tests.Features.Simulation.Animation.Files.TestSkeleton01.asf"))
            {
                parser.ImportASF(strm);

            }
            var skeleton2 = parser.ImportSkeleton();
            skeleton2.UpdateAbsoluteMatrices();

            var game = new DX11Game();
            var vis = new SkeletonVisualizer();

            game.GameLoopEvent += delegate
            {
                vis.VisualizeSkeleton(game, skeleton1, new Vector3(4, 0, 4));
                vis.VisualizeSkeleton(game, skeleton2, new Vector3(11, 0, 11));
            };

            game.Run();

        }
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestImportColladaSkeleton()
        {

        }
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestImportColladaAnimation()
        {

        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestPlayAnimation()
        {
            var controller = new AnimationControllerSkeleton(null);

            Skeleton skeleton;
            TheWizards.Animation.Animation animation;
            CreateTestAnimation(out skeleton, out animation);

            controller.SetAnimation(0, animation);

            var p = new Vector3(4, 5, 4);

            var m1 = Matrix.RotationAxis(Vector3.Normalize(new Vector3(-1, 4, 3)), 4) * Matrix.Translation(p);
            var m2 = Matrix.RotationAxis(Vector3.Normalize(new Vector3(3, 2, 8)), 4) * Matrix.Translation(p);

            var game = new DX11Game();
            var visualizer = new SkeletonVisualizer();

            game.GameLoopEvent += delegate
                                    {
                                        controller.ProgressTime(game.Elapsed);
                                        controller.UpdateSkeleton();
                                        skeleton.UpdateAbsoluteMatrices();
                                        visualizer.VisualizeSkeleton(game, skeleton);
                                    };

            game.Run();
        }

        public static void CreateTestAnimation(out Skeleton skeleton, out TheWizards.Animation.Animation animation)
        {
            skeleton = new Skeleton();
            Joint joint;

            joint = new Joint();
            joint.Name = "Root";
            joint.Length = 4;
            joint.RelativeMatrix = Matrix.Translation(4, 0, 4);
            skeleton.Joints.Add(joint);

            var parent = joint;

            joint = new Joint();
            joint.Name = "Arm1";
            joint.Parent = parent;

            skeleton.Joints.Add(joint);

            parent = joint;

            joint = new Joint();
            joint.Name = "Arm2Endnode";
            joint.Parent = parent;
            joint.RelativeMatrix = Matrix.Translation(0, 2, 0);
            skeleton.Joints.Add(joint);

            animation = new TheWizards.Animation.Animation();
            animation.Length = 2;
            animation.Tracks.Add(new TheWizards.Animation.Animation.Track()
                                     {Joint = skeleton.Joints[1]});

            animation.Tracks[0].Frames.Add(new TheWizards.Animation.Animation.Keyframe()
                                               {
                                                   Time = 0,
                                                   Value = Matrix.Identity * Matrix.Translation(0, 2, 0)
                                               });
            animation.Tracks[0].Frames.Add(new TheWizards.Animation.Animation.Keyframe()
                                               {
                                                   Time = 1,
                                                   Value = Matrix.RotationZ(MathHelper.PiOver4)*Matrix.Translation(0, 2, 0)
                                               });
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestInterpolateBoneRelativeMatrices()
        {

               var controller = new AnimationControllerSkeleton(null);

            var p = new Vector3(4, 5, 4);

            var m1 = Matrix.RotationAxis(Vector3.Normalize(new Vector3(-1, 4, 3)), 4) * Matrix.Translation(p);
            var m2 = Matrix.RotationAxis(Vector3.Normalize(new Vector3(3, 2, 8)), 4) * Matrix.Translation(p);

            var game = new DX11Game();

            var factor = 0f;
            float dir = 1;
            game.GameLoopEvent += delegate
                                  {
                                      factor += game.Elapsed*dir;
                                      if (factor > 1) dir = -1;
                                      if (factor < 0) dir = 1;

                                      var m = controller.InterpolateBoneRelativeMatrices(m1, m2, factor);

                                      game.LineManager3D.DrawGroundShadows = true;

                                      game.LineManager3D.AddLine(p, Vector3.TransformCoordinate(MathHelper.Up * 5, m1),
                                                                 Color.Red);
                                      game.LineManager3D.AddLine(p, Vector3.TransformCoordinate(MathHelper.Up * 5, m2),
                                                                 Color.Green);
                                      game.LineManager3D.AddLine(p, Vector3.TransformCoordinate(MathHelper.Up * 5, m),
                                                                 Color.Yellow);
                                  };
            game.Run();
        }
        
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestBlendAnimation()
        {

        }
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestImportAMC()
        {
            AMCParser parser = new AMCParser();
            using (var strm = EmbeddedFile.GetStream("MHGameWork.TheWizards.Tests.Features.Simulation.Animation.Files.TestAnimation01.amc"))
            {
                parser.ImportAMC(strm);

            }




        }
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestImportAMCCreateRelativeMatrices()
        {
            AMCParser parser = new AMCParser();
            using (var strm = EmbeddedFile.GetStream("MHGameWork.TheWizards.Tests.Features.Simulation.Animation.Files.TestAnimation02.amc"))
            {
                parser.ImportAMC(strm);

            }

            // Import skeleton
            var asfParser = new ASFParser();
            using (var strm = EmbeddedFile.GetStream("MHGameWork.TheWizards.Tests.Features.Simulation.Animation.Files.TestSkeleton02.asf"))
            {
                asfParser.ImportASF(strm);

            }
            var skeleton = asfParser.ImportSkeleton();


            var game = new DX11Game();
            var vis = new SkeletonVisualizer();

            for (int i = 0; i < skeleton.Joints.Count; i++)
            {
                skeleton.Joints[i].RelativeMatrix = Matrix.Identity;
            }

            float time = 0;
            float speed = 1;
            game.GameLoopEvent += delegate
                                {
                                    game.LineManager3D.DrawGroundShadows = true;

                                    time += game.Elapsed;
                                    var sampleNum = (int)(time * 120 * speed) % parser.Samples.Count;

                                    for (int i = 0; i < parser.Samples[sampleNum].Segments.Count; i++)
                                    {
                                        var seg = parser.Samples[sampleNum].Segments[i];

                                        var asfJoint = asfParser.Joints.Find(j => j.name == seg.JointName);
                                        var joint = skeleton.Joints.Find(j => j.Name == seg.JointName);

                                        Matrix relativeMat = parser.CalculateRelativeMatrix(seg, asfJoint);

                                        joint.RelativeMatrix = relativeMat;

                                    }
                                    skeleton.UpdateAbsoluteMatrices();

                                    vis.VisualizeSkeleton(game, skeleton, new Vector3(4, 0, 4));

                                };

            game.Run();

        }
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestImportAnimationFromAMC()
        {

        }
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestRenderSkinned()
        {

        }


    }
}
