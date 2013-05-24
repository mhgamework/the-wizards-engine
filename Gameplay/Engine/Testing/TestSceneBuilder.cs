using System;

namespace MHGameWork.TheWizards.Engine.Testing
{
    /// <summary>
    /// Responsible for holding information on how to construct the current test scene, and providing ways to construct it
    /// (NOTE: this is called by the EngineTestRunner)
    /// 
    /// This checks the TW.Data to see if the scene is already initially build, if it isnt, builds it
    /// </summary>
    public class TestSceneBuilder
    {
        public void EnsureTestSceneLoaded()
        {
            if (Setup == null) return;

            var testData = TW.Data.Get<TestingData>();

            var testName = testData.ActiveTestClass + "." + testData.ActiveTestMethod;

            if (testName == TW.Data.Get<Data>().LoadedTestScene) return;

            Setup();

            TW.Data.Get<Data>().LoadedTestScene = testName;
        }

        public Action Setup { get; set; }

        private class Data : EngineModelObject
        {
            public string LoadedTestScene { get; set; }
        }
    }
}