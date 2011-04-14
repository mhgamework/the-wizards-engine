using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public class EntityOud : IDisposable
    {
        DatabaseOud wereld;
        int id = -1;

        List<IEntityElement> elements = new List<IEntityElement>();

        public int ID
        {
            get { return id; }
        }
                public DatabaseOud Wereld
        {
            get { return wereld; }
        }

        /// <summary>
        /// DO NOT USE CONSTRUCTOR! This constructor is used by the world only. Use 'Wereld.CreateNewEntity()' instead.
        /// </summary>
        /// <param name="nWereld"></param>
        public EntityOud( DatabaseOud nWereld, int nID )
        {
            wereld = nWereld;
            id = nID;
        }



        public void Dispose()
        {
            for ( int i = 0; i < elements.Count; i++ )
            {
                elements[ i ].Dispose();
            }

        }


        public void Save( TWByteWriter bw )
        {
        }














        //// Methods and functions that im not sure of that they should be in this place or even exist.
        ////
        ////
        ////
        ////

        ///// <summary>
        ///// Creates a new WereldModel, adds it to this entity and to the world.
        ///// </summary>
        ///// <returns></returns>
        //public DatabaseModel CreateNewModel()
        //{
        //    DatabaseModel model = new DatabaseModel();

        //    wereld.AddModel( model );

        //    return model;

        //}

        ///// <summary>
        ///// Adds a WereldModel to this entity and to the world
        ///// </summary>
        ///// <param name="model"></param>
        //public void AddModel( DatabaseModel model )
        //{
        //    //elements.Add( model );
        //    wereld.AddModel( model );

        //}

        //public static void TestReadWriteXML()
        //{
        //    throw new Exception( "Not implemented yet!" );
        //}


    }
}
