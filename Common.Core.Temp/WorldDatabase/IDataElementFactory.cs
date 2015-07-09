using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.WorldDatabase
{
    /// <summary>
    /// INTERNAL USE ONLY. Do NOT USE THIS CLASS. Use the generic implementation instead. 
    /// </summary>
    public interface IDataElementFactory
    {
        string GetUniqueName();
        IDataElement ReadFromDisk(DataItemIdentifier item, DataRevisionIdentifier revision);
        void WriteToDisk(DataItemIdentifier item, DataRevisionIdentifier revision, IDataElement dataElement);
    }
    public interface IDataElementFactory<T> : IDataElementFactory where T : IDataElement
    {
        new T ReadFromDisk(DataItemIdentifier item, DataRevisionIdentifier revision);
        void WriteToDisk(DataItemIdentifier item, DataRevisionIdentifier revision, T dataElement);
    }
}
