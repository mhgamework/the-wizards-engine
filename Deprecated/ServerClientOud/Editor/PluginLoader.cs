using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public static class PluginLoader
    {

        /// <summary>
        /// T should be an interface!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static List<T> GetPlugins<T>( string folder )
        {
            if ( !Directory.Exists( folder ) ) return new List<T>();
            string[] files = Directory.GetFiles( folder, "*.dll" );

            List<T> tList = new List<T>();

            Debug.Assert( typeof( T ).IsInterface );



            foreach ( string file in files )
            {

                /*try
                {*/

                Assembly assembly = Assembly.LoadFile( file );

                tList.AddRange( GetPlugins<T>( assembly ) );
                

            }

            return tList;

        }

        public static List<T> GetPlugins<T>( Assembly[] assemblies )
        {
            List<T> list = new List<T>();
            for (int i = 0; i < assemblies.Length; i++)
            {
                list.AddRange( GetPlugins<T>( assemblies[ i ] ) );
            }
            return list;
        }

        public static List<T> GetPlugins<T>( Assembly assembly )
        {
            List<T> tList = new List<T>();
            foreach ( Type type in assembly.GetTypes() )
            {

                if ( !type.IsClass || type.IsNotPublic ) continue;

                Type[] interfaces = type.GetInterfaces();

                if ( ( (IList<Type>)interfaces ).Contains( typeof( T ) ) )
                {

                    object obj = Activator.CreateInstance( type );

                    T t = (T)obj;

                    tList.Add( t );

                }

            }

            /*}

            catch ( Exception ex )
            {

                LogError( ex );

            }*/

            return tList;
        }
    }
}
