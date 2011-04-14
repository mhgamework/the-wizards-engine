using System;
using MHGameWork.TheWizards.WorldDatabase;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// TODO: document
    /// </summary>
    public class MeshAdditionalData : IDataElement
    {
        public string ImportedFilename;
        public DateTime ImportedDate;
        public string ImporterDescription;

        /// <summary>
        /// User that imported this file
        /// </summary>
        public string ImportedBy;


    }
}
