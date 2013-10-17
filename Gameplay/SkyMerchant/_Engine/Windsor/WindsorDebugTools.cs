using System;
using System.Linq;
using System.Text;
using Castle.Core;
using Castle.Windsor;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.Windsor
{
    /// <summary>
    /// Some debugging facilities for windsor. Currently logging to a string is supported
    /// Currently tweaked for the skymerchant specifically :)
    /// </summary>
    public class WindsorDebugTools
    {
        private StringBuilder builder = new StringBuilder();
        /// <summary>
        /// Generates a string of all components, their lifestyle and their services
        /// </summary>
        /// <returns></returns>
        public string GenerateDependenciesString(IWindsorContainer container)
        {
            builder.Clear();
            foreach (var node in container.Kernel.GraphNodes.Cast<ComponentModel>())
            {
                write(node.Name + " : " + node.LifestyleType);

                foreach (var s in node.Services) write(" -- " + s);
            }
            return builder.ToString();
        }

        private void write(string line)
        {
            builder.AppendLine(line
                .Replace("MHGameWork.TheWizards.SkyMerchant", "SKY")
                .Replace("MHGameWork.TheWizards", "TW"));
        }

        /// <summary>
        /// Logs every created component into the console
        /// </summary>
        /// <param name="container"></param>
        public void EnableCreationLogging(IWindsorContainer container)
        {
            container.Kernel.ComponentCreated +=delegate(ComponentModel model, object instance)
                {
                    Console.WriteLine("WINDSOR Created {0}", model.Name);
                };
        }
    }
}