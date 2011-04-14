using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Entity.Editor;
using MHGameWork.TheWizards.ServerClient.Editor;
using MHGameWork.TheWizards.Editor.World;

namespace MHGameWork.TheWizards.Entities.Editor
{
    public class EditorEntityRenderMode : IWorldEditorRenderMode, IDisposable
    {
        private Dictionary<EditorObject, EditorObjectRenderData> editorObjectsRenderData;

        public Dictionary<EditorObject, EditorObjectRenderData> EditorObjectsRenderData
        {
            get { return editorObjectsRenderData; }
            set { editorObjectsRenderData = value; }
        }

        private List<EditorEntityRenderData> entitiesRenderData;

        public List<EditorEntityRenderData> EntitiesRenderData
        {
            get { return entitiesRenderData; }
            set { entitiesRenderData = value; }
        }

        private Dictionary<EditorEntity, EditorEntityRenderData> entitiesRenderDataDict;

        public Dictionary<EditorEntity, EditorEntityRenderData> EntitiesRenderDataDict
        {
            get { return entitiesRenderDataDict; }
            set { entitiesRenderDataDict = value; }
        }

        private WorldEditor worldEditor;
        private EntityManagerService ems;

        public EditorEntityRenderMode( WorldEditor _worldEditor )
        {
            worldEditor = _worldEditor;
            ems = worldEditor.Editor.Database.FindService<EntityManagerService>();


            entitiesRenderData = new List<EditorEntityRenderData>();
            editorObjectsRenderData = new Dictionary<EditorObject, EditorObjectRenderData>();
            entitiesRenderDataDict = new Dictionary<EditorEntity, EditorEntityRenderData>();
        }


        public void InitializeEntityRenderData( EditorEntity ent )
        {
            EditorObjectRenderData objRenderData;
            if ( editorObjectsRenderData.ContainsKey( ent.EditorObject ) )
            {
                objRenderData = editorObjectsRenderData[ ent.EditorObject ];
            }
            else
            {
                objRenderData = new EditorObjectRenderData();
                objRenderData.Initialize( worldEditor.XNAGameControl, ent.FullData.ObjectFullData );

                editorObjectsRenderData.Add( ent.EditorObject, objRenderData );
            }

            EditorEntityRenderData renderData = ent.TaggedEntity.GetTag<EditorEntityRenderData>();
            renderData.Initialize( worldEditor.XNAGameControl, objRenderData, ent );

            entitiesRenderData.Add( renderData );
            entitiesRenderDataDict.Add( ent, renderData );


        }
        private void UnloadEntityRenderData( EditorEntity ent )
        {
            EditorEntityRenderData renderData = entitiesRenderDataDict[ ent ];
            renderData.Dispose();
            entitiesRenderData.Remove( renderData );
            entitiesRenderDataDict.Remove( ent );
        }

        public void UpdateEntityRenderDataTransform( EditorEntity ent )
        {
            entitiesRenderDataDict[ ent ].WorldMatrix = ent.FullData.Transform.CreateMatrix();

        }




        [Obsolete( "Not used anymore?" )]
        public void Dispose()
        {
            if ( editorObjectsRenderData != null )
            {
                foreach ( EditorObjectRenderData objRenderData in editorObjectsRenderData.Values )
                {
                    objRenderData.Dispose();
                }
                editorObjectsRenderData.Clear();
            }
            editorObjectsRenderData = null;

            if ( entitiesRenderData != null )
            {
                for ( int i = 0; i < entitiesRenderData.Count; i++ )
                {
                    EditorEntityRenderData data = entitiesRenderData[ i ];
                    data.Dispose();
                }
                entitiesRenderData.Clear();
                entitiesRenderData = null;

                entitiesRenderDataDict.Clear();
                entitiesRenderDataDict = null;
            }
        }

        public void Render()
        {
            //TODO:
            for ( int i = 0; i < entitiesRenderData.Count; i++ )
            {
                EditorEntityRenderData renderData = entitiesRenderData[ i ];
                renderData.Render( worldEditor.XNAGameControl );
            }
        }




        public void Activate()
        {
            for ( int i = 0; i < ems.Entities.Count; i++ )
            {
                InitializeEntityRenderData( ems.Entities[ i ].GetTag<EditorEntity>() );
            }
        }

        public void Deactivate()
        {
            for ( int i = 0; i < entitiesRenderData.Count; i++ )
            {
                entitiesRenderData[ i ].Dispose();
            }
            entitiesRenderData.Clear();
            entitiesRenderDataDict.Clear();
            foreach ( KeyValuePair<EditorObject, EditorObjectRenderData> item in EditorObjectsRenderData )
            {
                item.Value.Dispose();
            }
            editorObjectsRenderData.Clear();
        }

        public void Update()
        {
        }
    }
}
