using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MHGameWork.TheWizards.WorldDatabase
{
    public class DataElementXMLFactory<T> : IDataElementFactory<T> where T : IDataElement
    {
        public string GetUniqueName()
        {
            return "DataElementXMLFactory001";
        }

        public T ReadFromDisk( DataItemIdentifier item, DataRevisionIdentifier revision )
        {
            throw new NotImplementedException();
        }

        public void WriteToDisk( DataItemIdentifier item, DataRevisionIdentifier revision, T dataElement )
        {
            //XmlSerializer serializer = new XmlSerializer(typeof (T),);
            throw new NotImplementedException();
            
            
        }

        IDataElement IDataElementFactory.ReadFromDisk( DataItemIdentifier item, DataRevisionIdentifier revision )
        {
            throw new NotImplementedException();
            //return ReadFromDisk( item, revision );
        }

        public void WriteToDisk( DataItemIdentifier item, DataRevisionIdentifier revision, IDataElement dataElement )
        {
            throw new NotImplementedException();
            //WriteToDisk( item, revision, (T)dataElement );
        }
    }
}
