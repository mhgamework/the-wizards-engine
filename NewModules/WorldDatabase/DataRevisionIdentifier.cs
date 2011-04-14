using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.WorldDatabase
{
    public struct DataRevisionIdentifier : IEquatable<DataRevisionIdentifier>
    {
        private readonly int revisionNumber;

        public int RevisionNumber
        {
            get { return revisionNumber; }
        }


        public DataRevisionIdentifier(int _revisionNumber)
        {
            revisionNumber = _revisionNumber;
        }

        public bool IsWorkingCopy
        {
            get { return revisionNumber == -1; }
        }

        public static DataRevisionIdentifier WorkingCopy
        {
            get { return new DataRevisionIdentifier(-1); }
        }

        public bool Equals(DataRevisionIdentifier other)
        {
            return other.revisionNumber == revisionNumber;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (DataRevisionIdentifier)) return false;
            return Equals((DataRevisionIdentifier) obj);
        }

        public override int GetHashCode()
        {
            return revisionNumber;
        }

        public static bool operator ==(DataRevisionIdentifier left, DataRevisionIdentifier right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DataRevisionIdentifier left, DataRevisionIdentifier right)
        {
            return !left.Equals(right);
        }
    }
}
