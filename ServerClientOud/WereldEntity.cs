using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient
{
    public class WereldEntity : IUnique
    {
        private TWWereld wereld;

        public TWWereld Wereld
        {
            get { return wereld; }
            set { wereld = value; }
        }


        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private Microsoft.Xna.Framework.BoundingBox boundingBox;

        public Microsoft.Xna.Framework.BoundingBox BoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }


        private List<WereldModel> models;

        public List<WereldModel> Models
        {
            get { return models; }
            set { models = value; }
        }


        public WereldEntity( TWWereld nWereld )
        {
            wereld = nWereld;
            id = -1;
            models = new List<WereldModel>();
        }

        public WereldEntity Clone( TWWereld nWereld )
        {
            WereldEntity entity = new WereldEntity( nWereld );

            for ( int i = 0; i < models.Count; i++ )
            {
                entity.AddNewModel( models[ i ].Clone() );
                entity.boundingBox = boundingBox;
            }

            return entity;
        }

        public void Render()
        {
            for ( int i = 0; i < models.Count; i++ )
            {
                models[ i ].Mesh.DrawPrimitives();
            }
        }

        public void AddNewModel( WereldModel model )
        {
            models.Add( model );
            wereld.Models.Add( model );
        }

        public void Move( Vector3 displacement )
        {
            Matrix m = Matrix.CreateTranslation( displacement );
            boundingBox.Min = Vector3.Transform( boundingBox.Min, m );
            boundingBox.Max = Vector3.Transform( boundingBox.Max, m );
            for ( int i = 0; i < models.Count; i++ )
            {
                models[ i ].WorldMatrix = models[ i ].WorldMatrix * m;
            }
        }

        public void SaveToXml()
        {
        }

    }
}
