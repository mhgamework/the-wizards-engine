using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Entity
{
    public class SceneXMLSerializer
    {
        /// <summary>
        /// This can save ANY IScene implementation to file
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="p"></param>
        public void Serialize(IScene scene, string p)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This loads a RAMScene and dependend RAM objects from a file.
        /// </summary>
        /// <param name="testsEntityTestsceneXML"></param>
        /// <returns></returns>
        public IScene Deserialize(string testsEntityTestsceneXML)
        {
            throw new NotImplementedException();
        }
    }
}
