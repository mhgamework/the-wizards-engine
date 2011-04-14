using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace MHGameWork.TheWizards.ServerClient.Graphics
{
    /// <summary>
    /// Momentarily only for creating a vertexdeclaration in a cool way!
    /// Taken from http://www.gamedev.net/reference/programming/features/xnaVertexElement/
    /// </summary>
    public class AttributeSystem
    {
        static Dictionary<Type, VertexElement[]> cachedData = new Dictionary<Type, VertexElement[]>();

        // For C# 3.0
        //public static VertexDeclaration CreateVertexDeclaration(this GraphicsDevice device, Type vertexType)
        #region Properties
        public static VertexDeclaration CreateVertexDeclaration( GraphicsDevice device, Type vertexType )
        {
            if ( cachedData.ContainsKey( vertexType ) )
                return new VertexDeclaration( device, cachedData[ vertexType ] );

            if ( !vertexType.IsValueType )
                throw new InvalidOperationException( "Vertex types must be value types." );

            List<VertexElementAttribute> objectAttributes = new List<VertexElementAttribute>();
            FieldInfo[] fields = vertexType.GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );
            foreach ( FieldInfo field in fields )
            {
                VertexElementAttribute[] attributes = (VertexElementAttribute[])field.GetCustomAttributes( typeof( VertexElementAttribute ), false );

                if ( field.Name.Contains( "<" ) && field.Name.Contains( ">" ) )
                {
                    int index1 = field.Name.IndexOf( '<' );
                    int index2 = field.Name.IndexOf( '>' );

                    string propertyName = field.Name.Substring( index1 + 1, index2 - index1 - 1 );
                    PropertyInfo property = vertexType.GetProperty( propertyName, field.FieldType );
                    if ( property != null )
                        attributes = (VertexElementAttribute[])property.GetCustomAttributes( typeof( VertexElementAttribute ), false );
                }

                if ( attributes.Length == 1 )
                {
                    if ( attributes[ 0 ].Format == VertexElementFormat.Unused )
                    {
                        if ( field.FieldType == typeof( Vector2 ) )
                            attributes[ 0 ].Format = VertexElementFormat.Vector2;
                        else if ( field.FieldType == typeof( Vector3 ) )
                            attributes[ 0 ].Format = VertexElementFormat.Vector3;
                        else if ( field.FieldType == typeof( Vector4 ) )
                            attributes[ 0 ].Format = VertexElementFormat.Vector4;
                        else if ( field.FieldType == typeof( Color ) )
                            attributes[ 0 ].Format = VertexElementFormat.Color;
                    }

                    attributes[ 0 ].Offset = Marshal.OffsetOf( vertexType, field.Name ).ToInt32();
                    objectAttributes.Add( attributes[ 0 ] );
                }
            }

            if ( objectAttributes.Count < 1 )
                throw new InvalidOperationException( "The vertex type must have at least one field or property marked with the VertexElement attribute." );

            List<VertexElement> elements = new List<VertexElement>();
            Dictionary<VertexElementUsage, int> usages = new Dictionary<VertexElementUsage, int>();
            foreach ( VertexElementAttribute attribute in objectAttributes )
            {
                if ( !usages.ContainsKey( attribute.Usage ) )
                    usages.Add( attribute.Usage, 0 );

                int index = usages[ attribute.Usage ];
                usages[ attribute.Usage ]++;

                elements.Add( new VertexElement( (short)attribute.Stream, (short)attribute.Offset, attribute.Format,
                    attribute.Method, attribute.Usage, (byte)index ) );
            }

            VertexElement[] elementArray = elements.ToArray();
            cachedData.Add( vertexType, elementArray );
            return new VertexDeclaration( device, elementArray );
        }
        #endregion
    }
}
