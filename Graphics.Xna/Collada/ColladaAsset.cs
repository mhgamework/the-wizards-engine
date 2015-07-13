using MHGameWork.TheWizards.Graphics.Xna.XML;

namespace MHGameWork.TheWizards.Graphics.Xna.Collada
{
    public class ColladaAsset
    {
        public enum UpAxisType
        {
            None = 0,
            Z_UP
        }

        public UpAxisType UpAxis;
        public string UnitName;
        public float UnitInMeters;

        public void Load( TWXmlNode assetNode )
        {
            UpAxis = UpAxisType.None;
            UnitName = "Unknown";
            UnitInMeters = 1;

            string up_axis = assetNode.ReadChildNodeValue( "up_axis", "" );
            if ( up_axis == "Z_UP" ) UpAxis = UpAxisType.Z_UP;

            TWXmlNode unitNode = assetNode.FindChildNode( "unit" );
            UnitName = unitNode.GetAttribute( "inch" );
            UnitInMeters = float.Parse( unitNode.GetAttribute( "meter" ) );

        }
    }
}
