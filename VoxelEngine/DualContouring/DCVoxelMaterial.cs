using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.DualContouring
{
    public class DCVoxelMaterial
    {
        public ITexture Texture;

        protected bool Equals( DCVoxelMaterial other )
        {
            return Equals( Texture, other.Texture );
        }

        public override bool Equals( object obj )
        {
            if ( ReferenceEquals( null, obj ) ) return false;
            if ( ReferenceEquals( this, obj ) ) return true;
            if ( obj.GetType() != this.GetType() ) return false;
            return Equals( (DCVoxelMaterial) obj );
        }

        public override int GetHashCode()
        {
            return ( Texture != null ? Texture.GetHashCode() : 0 );
        }
    }
}