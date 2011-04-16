using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Editor.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Editor.World;
using MHGameWork.TheWizards.Editor.Transform;
using MHGameWork.TheWizards.ServerClient.Editor;
using MHGameWork.TheWizards.Graphics;



namespace MHGameWork.TheWizards.Editor.Scene
{
    public class SceneEditor : WorldEditor
    {
        public SceneEditor(EditorScene scene)
            : base(scene)
        {

        }
    }

}

namespace MHGameWork.TheWizards.ServerClient.Editor
{

    [Obsolete("This class cannot be used anymore. Use the SceneEditor instead")]
    public class WorldEditor : IXNAObject
    {

        public EditorScene EditorScene;

        private IXNAGame game;

        public IXNAGame Game
        {
            get { return game; }
        }

        #region Properties

        [Obsolete("This reference to the Application Logic is to be removed")]
        private WizardsEditor editor;

        [Obsolete("This reference to the Application Logic is to be removed")]
        public WizardsEditor Editor
        {
            get { return editor; }
            set { editor = value; }
        }
        public WorldEditorForm Form
        {
            get { return form; }
            set { form = value; }
        }
        private WorldEditorForm form;
        #endregion





        private EditorUndoManager undoManager;

        private EditorGridOld editorGrid;


        private List<IEditorTool> tools = new List<IEditorTool>();
        private IEditorTool activeTool;
        private List<IWorldEditorRenderMode> renderModes = new List<IWorldEditorRenderMode>();
        private List<IWorldEditorRenderMode> activeRenderModes = new List<IWorldEditorRenderMode>();

        private List<IWorldEditorSelectable> selectables = new List<IWorldEditorSelectable>();
        /// <summary>
        /// TODO: maybe move this to the transform tool
        /// </summary>
        public List<IWorldEditorSelectable> Selectables
        {
            get { return selectables; }
            set { selectables = value; }
        }

        public void AddTool(IEditorTool tool)
        {
            tools.Add(tool);
        }
        public void ActivateTool(IEditorTool tool)
        {
            if (tool == null) throw new ArgumentNullException("Tool can not be null");
            if (activeTool == tool) return;
            DeactivateTool();
            tool.Activate();
            activeTool = tool;

        }
        public void DeactivateTool()
        {
            if (activeTool != null)
            {
                activeTool.Deactivate();
            }
            activeTool = null;
        }

        public T FindTool<T>() where T : class, IEditorTool
        {
            return Utilities.TypeSearcher.FindItem<T, IEditorTool>(tools);
        }

        public void AddRenderMode(IWorldEditorRenderMode renderMode)
        {
            renderModes.Add(renderMode);
        }
        public void ActivateRenderMode(IWorldEditorRenderMode renderMode)
        {
            if (activeRenderModes.Contains(renderMode)) return;
            activeRenderModes.Add(renderMode);
            renderMode.Activate();
        }
        public void DeactivateRenderMode(IWorldEditorRenderMode renderMode)
        {
            if (!activeRenderModes.Contains(renderMode)) return;
            activeRenderModes.Remove(renderMode);
            renderMode.Deactivate();
        }

        public void DeactivateAllRenderModes()
        {
            for (int i = 0; i < activeRenderModes.Count; i++)
            {
                activeRenderModes[i].Deactivate();
            }
            activeRenderModes.Clear();
        }

        public T FindRenderMode<T>() where T : class, IWorldEditorRenderMode
        {
            return Utilities.TypeSearcher.FindItem<T, IWorldEditorRenderMode>(renderModes);
        }

        public void AddSelectable(IWorldEditorSelectable selectable)
        {
            selectables.Add(selectable);
        }


        private EditorCameraOld editorCamera;


        // The dockcontaineritem representing the tab for the model viewer (on barMain)
        private DockTab dockTab;
        private XNAGameControl xnaGameControl;
        [Obsolete("Use of this property is deprecated.")]
        public XNAGameControl XNAGameControl
        {
            get
            {
                if (!editor.Form.Visible) return form.XNAGameControl;
                return xnaGameControl;
            }
            //set { XNAGameControl = value; }
        }

        private TransformControl transformControl;
        public TransformControl TransformControl
        {
            get { return transformControl; }
            set { transformControl = value; }
        }

        private TransformTool transformTool;

        public TransformTool TransformTool
        {
            get { return transformTool; }
            set { transformTool = value; }
        }


        [Obsolete("This reference to the Application Logic is to be removed")]
        public WorldEditor(WizardsEditor nEditor)
        {
            editor = nEditor;
            form = new WorldEditorForm();
            nEditor.AddMDIForm(form);


            form.btnTransformTool.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnSelectTool_ItemClick);


            form.XNAGameControl.InitializeEvent += XNAGameControl_InitializeEvent;
            form.XNAGameControl.DrawEvent += XNAGameControl_DrawEvent;
            form.XNAGameControl.UpdateEvent += XNAGameControl_UpdateEvent;




            transformTool = new TransformTool(this);
            AddTool(TransformTool);

            undoManager = new EditorUndoManager(50);


            form.FormClosed += new FormClosedEventHandler(form_FormClosed);






            CreateControls();


            transformControl = new TransformControl(XNAGameControl, form, form.pageGeneral);
            transformControl.Initialize();




        }

        public WorldEditor(EditorScene scene)
        {
            EditorScene = scene;
            transformTool = new TransformTool(this);
            AddTool(TransformTool);

            undoManager = new EditorUndoManager(50);
        }

        public void ActivateSelectTool()
        {
            btnSelectTool_ItemClick(null, null);
        }
        public void ActivateEditorCamera()
        {
            game.SetCamera(editorCamera);
            editorCamera.Enabled = true;
        }

        void btnSelectTool_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ActivateTool(transformTool);
        }

        void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            OnTabClosed(sender, null);
            Close();
        }
        private void CreateControls()
        {
            dockTab = editor.CreateNewTab();
            dockTab.Text = "World";
            dockTab.Control = new PanelDockContainer();

            dockTab.TabActivated += new EventHandler<EventArgs>(dockTab_TabActivated);
            dockTab.TabDeactivated += new EventHandler<EventArgs>(dockTab_TabDeActivated);
            dockTab.TabClosed += new EventHandler<DockTabClosingEventArgs>(dockTab_TabClosed);

            if (dockTab.Displayed) dockTab_TabActivated(this, null);

            if (!editor.Form.Visible) return;


            xnaGameControl = new XNAGameControl();
            // The events have to be added before controls.add, because initialize is called in controls.add

            XNAGameControl.InitializeEvent += new EventHandler(XNAGameControl_InitializeEvent);
            XNAGameControl.DrawEvent += new XNAGameControl.GameTimeEventHandler(XNAGameControl_DrawEvent);
            XNAGameControl.UpdateEvent += new XNAGameControl.GameTimeEventHandler(XNAGameControl_UpdateEvent);

            XNAGameControl.EditorCamera.OrbitLookAt = false;

            dockTab.Control.Controls.Add(XNAGameControl);
            XNAGameControl.Dock = System.Windows.Forms.DockStyle.Fill;

        }



        void dockTab_TabClosed(object sender, DockTabClosingEventArgs e)
        {
            OnTabClosed(sender, e);
        }
        public event EventHandler<DockTabClosingEventArgs> TabClosed;
        public void OnTabClosed(object sender, DockTabClosingEventArgs e)
        {
            if (TabClosed != null) TabClosed(sender, e);
        }




        #region IXNAObject Members

        public void Initialize(IXNAGame _game)
        {
            game = _game;

            transformControl = new TransformControl(_game);
            transformControl.Initialize();

            if (editorCamera == null)
            {
                editorCamera = new EditorCameraOld(_game);
                editorCamera.Tag = "SceneEditorCamera";
            }


            editorGrid = new EditorGridOld(game);


        }

        public void Render(IXNAGame _game)
        {
            // Note: this can occur when we are disposing of the XNAGameControl
            if (_game == null) return;


            editorGrid.Render();
            _game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None;

            //TODO: maybe create classes for the different modes?
            //EDIT: dividing functionality in tools, allow modular structure.

            for (int i = 0; i < activeRenderModes.Count; i++)
            {
                activeRenderModes[i].Render();
            }
            if (activeTool != null)
            {
                activeTool.Render();
            }
            else if (IsPutObjectsMode)
            {
                // Put objects mode
                //TODO
            }
            else
            {
                //Gizmo mode
                if (picked)
                {
                    _game.LineManager3D.AddLine(pickPoint, pickPoint + Vector3.Up * 3, Color.Green);
                    if (pickedType == PickType.Entities)
                    {
                        _game.LineManager3D.AddTriangle(pickV1, pickV2, pickV3, Color.Yellow);
                    }
                }

                //Note: Force the linemanager to render before the gizmo for correct display
                _game.LineManager3D.Render();

                _game.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Vector4.Zero, 1, 0);
            }

            TransformControl.Render();
        }

        public void Update(IXNAGame _game)
        {
            if (_game == null) return;
            picked = false;

            editorCamera.UpdateCameraMoveModeDefaultControls();
            editorCamera.Update();
            TransformControl.Update();


            //TODO: maybe create classes for the different modes?
            if (editorCamera.ActiveMoveMode != EditorCameraOld.MoveMode.None)
            {
                // Camera active!
            }
            else if (activeTool != null)
            {
                activeTool.Update();
            }
            else if (IsPutObjectsMode)
            {
                // Put objects mode
                //TODO:

            }
            else
            {
                //Gizmo mode

                // Moved To EntitySelectionTool

            }
            for (int i = 0; i < activeRenderModes.Count; i++)
            {
                activeRenderModes[i].Update();
            }
        }

        #endregion

        void XNAGameControl_UpdateEvent(Microsoft.Xna.Framework.GameTime ntime)
        {
            Update(xnaGameControl);


        }
        void XNAGameControl_DrawEvent(Microsoft.Xna.Framework.GameTime ntime)
        {
            Render(xnaGameControl);

        }
        void XNAGameControl_InitializeEvent(object sender, EventArgs e)
        {
            Initialize(xnaGameControl);

        }


        private bool IsGizmosTargeted()
        {
            return gizmoTranslation.ActiveHoverPart != EditorGizmoTranslation.GizmoPart.None ||
                     gizmoRotation.ActiveHoverPart != EditorGizmoRotation.GizmoPart.None ||
                     gizmoScaling.ActiveHoverPart != EditorGizmoScaling.GizmoPart.None;
        }

        private bool IsGizmosMoving()
        {
            return gizmoTranslation.ActiveMoveMode != EditorGizmoTranslation.GizmoPart.None ||
                     gizmoRotation.ActiveMoveMode != EditorGizmoRotation.GizmoPart.None ||
                     gizmoScaling.ActiveMoveMode != EditorGizmoScaling.GizmoPart.None;
        }






        public void dockTab_TabActivated(object sender, EventArgs e)
        {
            //NOTE: maybe i should make an interface instead of using these events, and making the WorldEditor invoking the functions
            //  Note: in the interface instead of adding and removing event handlers all the time
            //EDIT: TODO: I'm quite sure i should.

            editor.Form.btnUndo.Click += new EventHandler(btnUndo_Click);
            editor.Form.btnRedo.Click += new EventHandler(btnRedo_Click);

            editor.EnableWorldEditorButtons();
        }

        public void dockTab_TabDeActivated(object sender, EventArgs e)
        {
            editor.Form.btnUndo.Click -= new EventHandler(btnUndo_Click);
            editor.Form.btnRedo.Click -= new EventHandler(btnRedo_Click);

            editor.DisableWorldEditorButtons();
        }

        public void Show()
        {
            form.Show();
            dockTab.Selected = true;
        }

        public void Close()
        {
            if (form.Disposing) throw new InvalidOperationException();
            //form.Close();
            form.Dispose();
            form = null;
            // Dispose objects
            //TODO:

            // Clear all event handlers to allow disposal. If we don't this object will not be released by the GC after a disposal because of
            //  the reference in the event handler


            // VERRY important! stop the xna thread!!
            if (editor.Form.Visible)
                if (XNAGameControl != null)
                {
                    XNAGameControl.Exit();
                    XNAGameControl.Dispose();
                }



            // NOTE: VERY important! bug in dockContainerItem!!!! When docktab is disposed, it is not automatically removed
            // NOTE:  from barMain, causing xna to malfunction
            // NOTE: EDIT: the docktab is removed from items automatically when the users clicks the x, but after fireing the event DockTabClosing
            // NOTE:  so we just ignore disposing it and just let it hanging in free memory space and hope it will dissapear on its own
            //editor.Form.barMain.Items.Remove( dockTab );
            //dockTab.Dispose();

            dockTab = null;
            xnaGameControl = null;
        }


        /*private void SelectModel( EditorModel model )
        {
            selectedModel = model;
            if ( selectedModel != null )
            {
                Vector3 scale, transl;
                Quaternion rot;
                selectedModel.FullData.ObjectMatrix.Decompose( out scale, out rot, out transl );
                gizmoTranslation.Position = transl;
                gizmoRotation.RotationQuat = rot;
                gizmoScaling.Scaling = scale;

            }
        }*/

        EditorGizmoTranslation gizmoTranslation;
        EditorGizmoRotation gizmoRotation;
        EditorGizmoScaling gizmoScaling;

        //void btnWorldScale_Click( object sender, EventArgs e )
        //{
        //    DisableAllModeButtons();

        //    gizmoScaling.Enabled = true;

        //    UpdateTransformButtons();
        //}

        //void btnWorldRotate_Click( object sender, EventArgs e )
        //{
        //    // little trick
        //    if ( !gizmoTranslation.Enabled ) DisableAllModeButtons();


        //    gizmoRotation.Enabled = !gizmoRotation.Enabled;
        //    UpdateTransformButtons();
        //}

        //void btnWorldMove_Click( object sender, EventArgs e )
        //{
        //    // little trick
        //    if ( !gizmoRotation.Enabled ) DisableAllModeButtons();

        //    gizmoTranslation.Enabled = !gizmoTranslation.Enabled;

        //    UpdateTransformButtons();
        //}

        //void btnWorldSelect_Click( object sender, EventArgs e )
        //{
        //    DisableAllModeButtons();
        //    UpdateTransformButtons();
        //}







        //private void DisableTransformButtons()
        //{
        //    gizmoTranslation.Enabled = false;
        //    gizmoRotation.Enabled = false;
        //    gizmoScaling.Enabled = false;
        //    UpdateTransformButtons();

        //}

        //private void UpdateTransformButtons()
        //{
        //    editor.Form.btnWorldMove.Checked = gizmoTranslation.Enabled;
        //    editor.Form.btnWorldRotate.Checked = gizmoRotation.Enabled;
        //    editor.Form.btnWorldScale.Checked = gizmoScaling.Enabled;

        //    form.btnMove.Down = gizmoTranslation.Enabled;
        //    form.btnRotate.Down = gizmoRotation.Enabled;
        //    form.btnScale.Down = gizmoScaling.Enabled;

        //    editor.Form.btnWorldSelect.Checked = true;
        //    form.btnSelect.Down = true;

        //    if ( gizmoTranslation.Enabled || gizmoRotation.Enabled || gizmoScaling.Enabled )
        //    {
        //        editor.Form.btnWorldSelect.Checked = false;
        //        form.btnSelect.Down = false;

        //    }
        //    if ( editor.Form.btnWorldPutObjects.Checked )
        //    {
        //        editor.Form.btnWorldSelect.Checked = false;
        //        form.btnSelect.Down = false;
        //    }

        //}

        //private void DisableAllModeButtons()
        //{
        //    DeactivateTool();
        //    gizmoTranslation.Enabled = false;
        //    gizmoRotation.Enabled = false;
        //    gizmoScaling.Enabled = false;
        //    editor.Form.btnWorldPutObjects.Checked = false;
        //    editor.Form.btnWorldTerrainCreate.Checked = false;
        //    editor.Form.btnWorldTerrainRaise.Checked = false;
        //    editor.Form.btnWorldTerrainPaint.Checked = false;

        //    form.btnSelect.Down = false;
        //    form.btnMove.Down = false;
        //    form.btnRotate.Down = false;
        //    form.btnSelect.Down = false;
        //}

        //void gizmoScaling_ScalingChanged( object sender, EventArgs e )
        //{
        //    if ( selectedEntity == null ) throw new Exception( "impossible?" );

        //    Transformation t = selectedEntity.FullData.Transform;
        //    t.Scaling = gizmoScaling.Scaling;
        //    selectedEntity.FullData.Transform = t;

        //    UpdateEntityRenderDataTransform( selectedEntity );
        //}

        //void gizmoRotation_RotationChanged( object sender, EventArgs e )
        //{
        //    if ( selectedEntity == null ) throw new Exception( "impossible?" );

        //    Transformation t = selectedEntity.FullData.Transform;
        //    t.Rotation = gizmoRotation.RotationQuat;
        //    selectedEntity.FullData.Transform = t;


        //    UpdateEntityRenderDataTransform( selectedEntity );

        //}

        //void gizmoTranslation_PositionChanged( object sender, EventArgs e )
        //{
        //    if ( selectedEntity == null ) throw new Exception( "impossible?" );


        //    gizmoRotation.Position = gizmoTranslation.Position;
        //    gizmoScaling.Position = gizmoTranslation.Position;

        //    Transformation t = selectedEntity.FullData.Transform;
        //    t.Translation = gizmoTranslation.Position;
        //    selectedEntity.FullData.Transform = t;

        //    UpdateEntityRenderDataTransform( selectedEntity );


        //}




        //private Transformation selectedEntityOldTransform;

        //private void OnSelectedEntityStartGizmoMove()
        //{
        //    if ( selectedEntity == null ) throw new Exception( "impossible?" );

        //    selectedEntityOldTransform = selectedEntity.FullData.Transform;
        //}

        //private void OnSelectedEntityEndGizmoMove()
        //{
        //    if ( selectedEntity == null ) throw new Exception( "impossible?" );



        //    // Make undo action
        //    UndoActionEntityTransformation action = new UndoActionEntityTransformation();
        //    action.EditorEntity = selectedEntity;
        //    action.OldTransform = selectedEntityOldTransform;
        //    action.NewTransform = selectedEntity.FullData.Transform;
        //    action.WorldEditor = this;

        //    //Quite unlikely: if ( !action.OldTransform.EqualsExactly( action.NewTransform ) )
        //    undoManager.AddIUndoAction( action );


        //    XNAGameControl.EditorCamera.OrbitPoint = gizmoTranslation.Position;

        //}

        //private void gizmoRotation_StartMoveMode( object sender, EventArgs e )
        //{
        //    OnSelectedEntityStartGizmoMove();
        //}

        //private void gizmoRotation_EndMoveMode( object sender, EventArgs e )
        //{
        //    OnSelectedEntityEndGizmoMove();

        //}

        //private void gizmoScaling_StartMoveMode( object sender, EventArgs e )
        //{
        //    OnSelectedEntityStartGizmoMove();
        //}

        //void gizmoScaling_EndMoveMode( object sender, EventArgs e )
        //{
        //    OnSelectedEntityEndGizmoMove();
        //}

        //void gizmoTranslation_EndMoveMode( object sender, EventArgs e )
        //{
        //    OnSelectedEntityEndGizmoMove();
        //}

        //void gizmoTranslation_StartMoveMode( object sender, EventArgs e )
        //{
        //    OnSelectedEntityStartGizmoMove();
        //}




        void btnRedo_Click(object sender, EventArgs e)
        {
            undoManager.Redo();
        }
        void btnUndo_Click(object sender, EventArgs e)
        {
            undoManager.Undo();
        }









        /*
         * Terrain
         * 
         * */


        //private TerrainCreateTool terrainCreateTool;
        //private Terrain.TerrainRaiseTool terrainRaiseTool;
        //private TerrainPaintTool terrainPaintTool;

        //void btnWorldTerrainCreate_Click( object sender, EventArgs e )
        //{
        //    bool flag = false;
        //    if ( !IsTerrainCreateMode ) flag = true;

        //    DisableAllModeButtons();
        //    editor.Form.btnWorldTerrainCreate.Checked = true;
        //    UpdateTransformButtons();


        //    if ( flag ) terrainCreateTool.Activate();

        //}


        //void btnWorldTerrainRaise_Click( object sender, EventArgs e )
        //{

        //    DisableAllModeButtons();
        //    editor.Form.btnWorldTerrainRaise.Checked = true;
        //    UpdateTransformButtons();

        //    ActivateTool( terrainRaiseTool );
        //}

        //void btnWorldTerrainPaint_Click( object sender, EventArgs e )
        //{
        //    bool flag = false;
        //    if ( !IsTerrainPaintMode ) flag = true;

        //    DisableAllModeButtons();
        //    editor.Form.btnWorldTerrainPaint.Checked = true;
        //    UpdateTransformButtons();

        //    if ( flag ) terrainPaintTool.Activate();
        //}




        public bool IsPutObjectsMode
        {
            get
            {
                return false; //TODO
                //return editor.Form.btnWorldPutObjects.Checked;
            }
        }
        public bool IsTerrainCreateMode
        {
            get { return editor.Form.btnWorldTerrainCreate.Checked; }
        }
        public bool IsTerrainPaintMode
        {
            get { return editor.Form.btnWorldTerrainPaint.Checked; }
        }


        [Flags]
        public enum PickType
        {
            None = 0,
            GroundPlane = 1,
            Entities = 2,
            Terrains = 4,

            All = GroundPlane | Entities | Terrains
        }

        public class PickResult
        {
            public PickType Type;
            public Vector3 Point;
            public float Distance;
            public Vector3 V1;
            public Vector3 V2;
            public Vector3 V3;
            //public EditorEntity Entity;
            public EditorTerrain Terrain;

            public PickResult()
            {
                Type = PickType.None;
                Point = Vector3.Zero;
                Distance = 0;

            }

            public bool Picked
            { get { return this.Type != PickType.None; } }

            public bool IsCloser(float dist)
            {
                if (Picked == false) return true;
                if (dist < Distance) return true;

                return false;
            }
        }

        private bool picked;
        private PickType pickedType;
        private Vector3 pickPoint;
        private float pickDistance;
        private Vector3 pickV1;
        private Vector3 pickV2;
        private Vector3 pickV3;


        private void UpdatePickWorldOud(PickType filter)
        {
            picked = false;
            pickedType = PickType.None;
            pickDistance = 0;

            PickResult result = PickWorld(filter);

            if (!result.Picked) return;
            picked = true;
            pickedType = result.Type;
            pickPoint = result.Point;
            pickDistance = result.Distance;
            pickV1 = result.V1;
            pickV2 = result.V2;
            pickV3 = result.V3;
            //pickedEntity = result.Entity;
        }

        public PickResult PickWorld(PickType filter)
        {
            if (game.IsCursorInWindow() == false) return new PickResult();
            Ray ray = Game.GetWereldViewRay(Game.Mouse.CursorPositionVector);
            return PickWorld(filter, ray);
        }

        public PickResult PickWorld(PickType filter, Ray ray)
        {

            PickResult result = new PickResult();
            if ((filter & PickType.Terrains) != PickType.None)
                UpdatePickTerrains(ray, result);

            //if ( ( filter & PickType.Entities ) != PickType.None )
            //    UpdatePickEntities( ray, result );

            if ((filter & PickType.GroundPlane) != PickType.None)
                UpdatePickGroundPlane(ray, result);


            return result;

        }

        private void UpdatePickTerrains(Ray ray, PickResult result)
        {
            for (int i = 0; i < editor.Terrains.Count; i++)
            {
                EditorTerrain terr = editor.Terrains[i];
                Vector3 v1, v2, v3;
                float? dist = terr.Raycast(ray, out v1, out v2, out v3);

                if (!dist.HasValue) continue;

                if (result.IsCloser(dist.Value))
                {
                    result.Type = PickType.Terrains;
                    result.Point = ray.Position + ray.Direction * dist.Value;
                    result.Distance = dist.Value;
                    result.Terrain = terr;
                    result.V1 = v1;
                    result.V2 = v2;
                    result.V3 = v3;

                }
            }

        }

        //private void UpdatePickEntities( Ray ray, PickResult result )
        //{
        //    for ( int i = 0; i < editor.EditorEntities.Count; i++ )
        //    {
        //        EditorEntity ent = editor.EditorEntities[ i ];
        //        Vector3 v1, v2, v3;
        //        float? dist = ent.RaycastEntity( ray, out v1, out v2, out v3 );

        //        if ( !dist.HasValue ) continue;

        //        if ( result.IsCloser( dist.Value ) )
        //        {
        //            result.Type = PickType.Entities;
        //            result.Point = ray.Position + ray.Direction * dist.Value;
        //            result.Distance = dist.Value;
        //            result.Entity = ent;
        //            result.V1 = v1;
        //            result.V2 = v2;
        //            result.V3 = v3;

        //        }
        //    }

        //}

        private void UpdatePickGroundPlane(Ray ray, PickResult result)
        {
            Plane groundPlane = new Plane(Vector3.Up, 0);

            float? dist = ray.Intersects(groundPlane);


            if (!dist.HasValue) return; // No intersection

            if (result.IsCloser(dist.Value))
            {
                result.Type = PickType.GroundPlane;
                result.Point = ray.Position + ray.Direction * dist.Value;
                result.Distance = dist.Value;
            }
        }











    }
}

