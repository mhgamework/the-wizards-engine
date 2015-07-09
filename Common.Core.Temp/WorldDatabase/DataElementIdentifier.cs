using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.WorldDatabase
{
    public struct DataElementIdentifier : IEquatable<DataElementIdentifier>
    {
       private string uniqueName;

        public DataElementIdentifier(string uniqueName)
        {
            if (uniqueName == null) throw new InvalidOperationException();
            this.uniqueName = uniqueName;
        }

        public string UniqueName
        {
            get { return uniqueName; }
        }

        public bool Equals(DataElementIdentifier other)
        {
            return Equals(other.uniqueName, uniqueName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (DataElementIdentifier)) return false;
            return Equals((DataElementIdentifier) obj);
        }

        public override int GetHashCode()
        {
            return (uniqueName != null ? uniqueName.GetHashCode() : 0);
        }
    }
}
