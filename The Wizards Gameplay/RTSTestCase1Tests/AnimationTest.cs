using System;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.RTSTestCase1.Animation;
using NUnit.Framework;

namespace MHGameWork.TheWizards.RTSTestCase1.Tests
{
    /// <summary>
    /// Test for the Animator, AnimationDescription etc
    /// </summary>
    [EngineTest]
    [TestFixture]
    public class AnimationTest
    {
        [Test]
        public void TestKeyframed()
        {
            var a = new Animator();
            var desc = a.CreateDescription();

            float value = 0;

            var prop = desc.CreateProperty(() => value, v => value = v);

            prop.AddKey(1, 1f);
            prop.AddKey(2, 2f);
            prop.AddKey(4, 3f);


            var runAndCheck = (Action)delegate()
                {
                    a.Run(desc);

                    a.Update(0.5f); // 0.5
                    Assert.AreEqual(1, value, 0.00001f);
                    a.Update(1);  // 1.5
                    Assert.AreEqual(1.5f, value, 0.00001f);
                    a.Update(1); // 2.5
                    Assert.AreEqual(2.25f, value, 0.00001f);
                    a.Update(1); // 3.5
                    Assert.AreEqual(2.75f, value, 0.00001f);
                    a.Update(1); // 4.5
                    Assert.AreEqual(3, value, 0.00001f);
                    a.Update(1); // 5.5
                    Assert.AreEqual(3, value, 0.00001f);
                };

            runAndCheck();

            a.Update(0.5f);

            runAndCheck();


        }

        [Test]
        public void TestAction()
        {
            

            var a = new Animator();
            var desc = a.CreateDescription();

            float value = 0;


            desc.AddAction(1, () => value++);

            a.Run(desc);

            a.Update(0.5f); // 0.5
            Assert.AreEqual(0, value, 0.00001f);
            a.Update(1);  // 1.5
            Assert.AreEqual(1, value, 0.00001f);
            a.Update(1); // 2.5
            Assert.AreEqual(1, value, 0.00001f);
            a.Update(1); // 3.5
            Assert.AreEqual(1, value, 0.00001f);
        }

    }
}