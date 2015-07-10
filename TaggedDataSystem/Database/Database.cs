using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Database;

namespace MHGameWork.TheWizards.Database
{
    /// <summary>
    /// To be implemented.
    /// This class manages all data in The Wizards. You can retrieve data by specifiing an ID of an item and a tag
    /// example: an object id and the tag ObjectFullData, which gives you a class containing all of the data for that object
    /// The database manages the loading and unloading from disk/memory
    /// Edit: to be renamed Engine or smt.
    /// 
    /// Edit: Design has been expanded. The new aim is to support plugins, so a more abstract design
    ///         Later optimizations are of course possible
    /// Edit: Plugins should now also reside in this class. There should be a function RequirePlugin that loads the given plugin (once).
    ///       This class is tending towards a general TWEngine class that represents an instance of The Wizards. Then by loading
    ///         plugins all functionality is included.
    /// Edit: After some thinking the conclusion is that the services are the actual plugins that should be loaded.
    ///        So some of the current services should be plugins, some should be accessible through the plugins
    /// Conclusion: Services are classes that represent a functionality that is added to the Engine (aka Database)
    ///              'Editor', 'Entities', 'Entities.Editor', 'Terrain'
    ///             Plugins (classes) are classes that load the services in to a final release of the 
    ///              engine. These plugin classes reside in dlls in the /Plugins folder.
    /// NOTE: This is starting to look like a singleton holder class!!!? Simply to solve the simultaniously server/client problem?
    /// </summary>
    public class Database : IDisposable
    {
        private List<IGameService> services = new List<IGameService>();
        private List<IGameObject> objects = new List<IGameObject>();

        /// <summary>
        /// This checks if given service is loaded and loads it if necessary.
        /// I use 'require' here because this function includes functionality into the engine.
        /// Returns the service.
        /// NOTE: 2 possible ways to use services: or make a service load all its dependencies,
        ///        or make a service only load when all dependencies are loaded.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [Obsolete("Do not use, automatic dependancy solving is a no-go!")]
        public T RequireService<T>() where T : class, IGameService002, new()
        {
            T service = FindService<T>();
            if ( service == null )
            {
                service = new T();
                service.Load( this );
                AddService( service );


            }

            return service;

        }

        /// <summary>
        /// Edit: not obsolete anymore!
        /// </summary>
        /// <param name="service"></param>
        public void AddService( IGameService service )
        {
            services.Add( service );
        }

        public T FindService<T>() where T : class, IGameService
        {
            for ( int i = 0; i < services.Count; i++ )
            {
                IGameService service = services[ i ];
                if ( service.GetType().Equals( typeof( T ) ) )
                {
                    return service as T;
                }
            }
            return null;
        }

        public void AddObject( IGameObject obj )
        {
            objects.Add( obj );
        }


        #region IDisposable Members

        public void Dispose()
        {
            if ( services != null )
            {
                for ( int i = 0; i < services.Count; i++ )
                {
                    IGameService service = services[ i ];
                    service.Dispose();
                }
                services.Clear();
            }
            if ( objects != null )
            {
                for ( int i = 0; i < objects.Count; i++ )
                {
                    IGameObject obj = objects[ i ];
                    obj.Dispose();
                }
                objects.Clear();
            }
            objects = null;
            services = null;

        }

        #endregion
    }
}