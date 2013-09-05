using System;

namespace MHGameWork.TheWizards.Engine.Testing
{
    /// <summary>
    /// Responsible for holding information on how to construct the current test scene, and providing ways to construct it
    /// (NOTE: this is called by the MultiTypeTestRunner)
    /// 
    /// This checks the TW.Data to see if the scene is already initially build, if it isnt, builds it
    /// </summary>
    public class TestSceneBuilder
    {
        private EngineTestState testState;

        public TestSceneBuilder(EngineTestState testState)
        {
            this.testState = testState;
        }

        public void EnsureTestSceneLoaded()
        {
            if (Setup == null) return;

            var testName = testState.GetTestName();

            

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