using System.Collections.Generic;
using System.Xml.Serialization;
using MHGameWork.TheWizards.WorldDatabase;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Rendering
{
    public class MeshCoreData : IDataElement
    {
        public List<Part> Parts = new List<Part>();

        public class Part
        {
            public IMeshPart MeshPart;
            public Matrix ObjectMatrix;
            public Material MeshMaterial;
        }
        public class Material
        {
            public Color DiffuseColor;
            public ITexture DiffuseMap;
            public ITexture NormalMap;
            public ITexture SpecularMap;
            public ITexture DisplacementMap;
            public bool ColoredMaterial = false;
            public string Name;

            protected bool Equals( Material other )
            {
                return DiffuseColor.Equals( other.DiffuseColor ) && Equals( DiffuseMap, other.DiffuseMap ) && Equals( NormalMap, other.NormalMap ) && Equals( SpecularMap, other.SpecularMap ) && Equals( DisplacementMap, other.DisplacementMap );
            	//TODO: add color equals check
			}

            public override bool Equals( object obj )
            {
                if ( ReferenceEquals( null, obj ) ) return false;
                if ( ReferenceEquals( this, obj ) ) return true;
                if ( obj.GetType() != this.GetType() ) return false;
                return Equals( (Material) obj );
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = DiffuseColor.GetHashCode();
                    hashCode = ( hashCode*397 ) ^ ( DiffuseMap != null ? DiffuseMap.GetHashCode() : 0 );
                    hashCode = ( hashCode*397 ) ^ ( NormalMap != null ? NormalMap.GetHashCode() : 0 );
                    hashCode = ( hashCode*397 ) ^ ( SpecularMap != null ? SpecularMap.GetHashCode() : 0 );
                    hashCode = ( hashCode*397 ) ^ ( DisplacementMap != null ? DisplacementMap.GetHashCode() : 0 );
                    return hashCode;
                }
            }

            public Material Copy()
            {
                return new MeshCoreData.Material { DiffuseMap = DiffuseMap, DiffuseColor = DiffuseColor, Name = Name, ColoredMaterial = ColoredMaterial };

            }
        }
    }
}
