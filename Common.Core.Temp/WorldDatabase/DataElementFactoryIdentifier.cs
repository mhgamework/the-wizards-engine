using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.WorldDatabase
{
    public struct DataElementFactoryIdentifier : IEquatable<DataElementFactoryIdentifier>
    {
       private string uniqueName;

        public DataElementFactoryIdentifier(string uniqueName)
        {
            if (uniqueName == null) throw new InvalidOperationException();
            this.uniqueName = uniqueName;
        }

        public string UniqueName
        {
            get { return uniqueName; }
        }

        public bool Equals(DataElementFactoryIdentifier other)
        {
            return Equals(other.uniqueName, uniqueName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(DataElementFactoryIdentifier)) return false;
            return Equals((DataElementFactoryIdentifier)obj);
        }

        public override int GetHashCode()
        {
            return (uniqueName != null ? uniqueName.GetHashCode() : 0);
        }
    }
}
