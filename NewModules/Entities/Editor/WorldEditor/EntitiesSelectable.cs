using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Editor;
using MHGameWork.TheWizards.Editor.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Entities.Editor
{
    public class EntitiesSelectable : TheWizards.Editor.World.IWorldEditorSelectable
    {
        private EditorEntity selectedEntity;
        public EditorEntity SelectedEntity
        {
            get { return selectedEntity; }
            set { selectedEntity = value; }
        }

        private WorldEditor worldEditor;
        private EntityManagerService ems;

        private Raycast.GeometryRaycastResult<EditorEntity> lastRaycast;

        public EntitiesSelectable( WorldEditor _worldEditor )
        {
            worldEditor = _worldEditor;
            ems = worldEditor.Editor.Database.FindService<EntityManagerService>();

        }


        private void SelectEntity( EditorEntity ent )
        {
            selectedEntity = ent;
            if ( selectedEntity != null )
            {
                worldEditor.TransformControl.SetTransformation( selectedEntity.FullData.Transform );

            }
        }

        public void SelectLastRaycasted()
        {
            if ( lastRaycast == null ) throw new InvalidOperationException( "No raycast hit yet!" );
            SelectEntity( lastRaycast.Item );
        }

        public void Deselect()
        {
            SelectEntity( null );
        }

        public void Render()
        {
            if ( selectedEntity == null ) return;
            worldEditor.XNAGameControl.LineManager3D.AddAABB(
                selectedEntity.EditorObject.FullData.BoundingBox,
                selectedEntity.FullData.Transform.CreateMatrix(),
                Color.Black );

        }

        public void Update()
        {
            if ( selectedEntity == null ) return;
            selectedEntity.FullData.Transform = worldEditor.TransformControl.GetTransformation();
            // Update renderdata
            //TODO: speedup, maybe move to editorentity
            selectedEntity.TaggedEntity.GetTag<EditorEntityRenderData>().WorldMatrix = selectedEntity.FullData.Transform.CreateMatrix();


            // If gizmo's are not being targeted
            //if ( !IsGizmosTargeted() )
            //{
            //    UpdatePickWorldOud( PickType.All );

            //    if ( XNAGameControl.Mouse.LeftMouseJustPressed )
            //    {
            //        // When don't click on a model but on an empty place in de window, deselect
            //        // Otherwise, don't
            //        if ( XNAGameControl.IsCursorOnControl() )
            //        {
            //            if ( pickedType == PickType.Entities )
            //            {
            //                SelectEntity( pickedEntity );
            //            }
            //            else
            //            {
            //                SelectEntity( null );
            //            }

            //        }
            //    }

            //}

            //if ( selectedEntity != null && !IsGizmosMoving() )
            //{
            //    if ( XNAGameControl.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.Delete ) )
            //    {
            //        // Delete selected entity
            //        UnloadEntityRenderData( selectedEntity );
            //        editor.DeleteEditorEntity( selectedEntity );
            //        SelectEntity( null );
            //    }
            //}
        }



        public Raycast.RaycastResult<IWorldEditorSelectable> Raycast( Ray ray )
        {
            //TODO: This is not ok ! need speedup
            //Get all editor entities
            List<EditorEntity> ents = new List<EditorEntity>();
            for ( int i = 0; i < ems.Entities.Count; i++ )
            {
                ents.Add( ems.Entities[ i ].GetTag<EditorEntity>() );
            }

            TheWizards.Raycast.GeometryRaycastResult<EditorEntity> result
                = TheWizards.Raycast.RaycastHelper.MultipleRayscast<EditorEntity, TheWizards.Raycast.GeometryRaycastResult<EditorEntity>>( ents, ray );

            if ( result == null ) return new Raycast.RaycastResult<IWorldEditorSelectable>();
            TheWizards.Raycast.RaycastResult<IWorldEditorSelectable> ret;
            ret = new MHGameWork.TheWizards.Raycast.RaycastResult<IWorldEditorSelectable>( result.Distance, this );

            lastRaycast = result;

            return ret;

            //return TheWizards.Raycast.RaycastHelper.MultipleRayscast<EditorEntity, TheWizards.Raycast.GeometryRaycastResult<EditorEntity>>( ents, ray );

            //for ( int i = 0; i < editor.EditorEntities.Count; i++ )
            //{
            //    EditorEntity ent = editor.EditorEntities[ i ];
            //    Vector3 v1, v2, v3;
            //    float? dist = ent.RaycastEntity( ray, out v1, out v2, out v3 );

            //    if ( !dist.HasValue ) continue;

            //    if ( result.IsCloser( dist.Value ) )
            //    {
            //        result.Type = PickType.Entities;
            //        result.Point = ray.Position + ray.Direction * dist.Value;
            //        result.Distance = dist.Value;
            //        result.Entity = ent;
            //        result.V1 = v1;
            //        result.V2 = v2;
            //        result.V3 = v3;

            //    }
            //}
        }




        public Microsoft.Xna.Framework.Vector3 GetSelectionCenter()
        {
            if ( selectedEntity == null ) return Vector3.Zero;// throw new InvalidOperationException( "Nothing selected!" );
            return selectedEntity.FullData.Transform.Translation;
        }

    }
}
