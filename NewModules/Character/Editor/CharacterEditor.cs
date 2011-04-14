using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.ServerClient.Editor;
using MHGameWork.TheWizards.Editor.Transform;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;

namespace MHGameWork.TheWizards.Character.Editor
{
    public class CharacterEditor
    {
        private CharacterEditorForm form;
        public CharacterEditorForm Form
        {
            get { return form; }
            //set { form = value; }
        }

        private WizardsEditor wizardsEditor;
        public WizardsEditor WizardsEditor
        {
            get { return wizardsEditor; }
            //set { wizardsEditor = value; }
        }

        public XNAGameControl XNAGameControl
        {
            get { return form.xnaGameControl1; }
        }

        private TransformControl transformControl;
        private Character character;

        private bool viewSkin;
        private bool viewBones;
        private bool viewWeights;

        private bool play;

        private SpectaterCamera specCam;

        private float animationSpeed;

        private CharacterBone selectedBone;
        private Matrix selectedBoneStartCurrentMatrix;

        public CharacterEditor( WizardsEditor _wizardsEditor )
        {
            wizardsEditor = _wizardsEditor;
            form = new CharacterEditorForm();
            WizardsEditor.AddMDIForm( form );

            form.btnViewBones.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler( btnViewBones_ItemClick );
            form.btnViewSkin.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler( btnViewSkin_ItemClick );
            form.btnPlay.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler( btnPlay_ItemClick );
            form.btnSpectater.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler( btnSpectater_ItemClick );
            form.trackSpeed.EditValueChanged += new EventHandler( trackBarSpeed_EditValueChanged );
            form.btnViewWeights.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler( btnViewWeights_ItemClick );
            form.btnBonesFirstChild.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler( btnBonesFirstChild_ItemClick );
            form.btnBonesNextSibling.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler( btnBonesNextSibling_ItemClick );
            form.btnBonesParent.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler( btnBonesParent_ItemClick );
            form.treeBones.NodeMouseClick += new TreeNodeMouseClickEventHandler( treeBones_NodeMouseClick );

            XNAGameControl.InitializeEvent += new EventHandler( XNAGameControl_InitializeEvent );
            XNAGameControl.UpdateEvent += new XNAGameControl.GameTimeEventHandler( XNAGameControl_UpdateEvent );
            XNAGameControl.DrawEvent += new XNAGameControl.GameTimeEventHandler( XNAGameControl_DrawEvent );

            specCam = new SpectaterCamera( XNAGameControl, 0.1f, 1000 );

            transformControl = new TransformControl( XNAGameControl, form, form.pageGeneral );
            transformControl.Initialize();
            transformControl.Enabled = true;

            // Call initialize, since the XNAGameControl will prob allready be initilaized
            XNAGameControl_InitializeEvent( null, null );

            //ToggleViewSkin();
            TogglePlay();
            ToggleViewBones();
            SetAnimationSpeed( 1 );
            DeselectBone();
        }

        void treeBones_NodeMouseClick( object sender, TreeNodeMouseClickEventArgs e )
        {
            if ( e.Node.Tag != null )
            {
                SelectBone( e.Node.Tag as CharacterBone );
            }
        }

        private void fillBonesTree()
        {
            form.treeBones.Nodes.Clear();

            TreeNode node = new TreeNode();
            form.treeBones.Nodes.Add( node );
            fillBonesTreeNode( node, character.Bones[ 0 ] );

        }
        private void fillBonesTreeNode( TreeNode node, CharacterBone bone )
        {
            node.Tag = bone;
            node.Text = bone.num.ToString( "00" ) + ": " + bone.Name;
            for ( int i = 0; i < bone.children.Count; i++ )
            {
                TreeNode childNode = new TreeNode();
                node.Nodes.Add( childNode );
                fillBonesTreeNode( childNode, bone.children[ i ] );
            }
        }

        void btnBonesParent_ItemClick( object sender, DevExpress.XtraBars.ItemClickEventArgs e )
        {
            SelectParentBone();
        }

        void btnBonesNextSibling_ItemClick( object sender, DevExpress.XtraBars.ItemClickEventArgs e )
        {
            SelectNextSiblingBone();
        }

        void btnBonesFirstChild_ItemClick( object sender, DevExpress.XtraBars.ItemClickEventArgs e )
        {
            SelectFirstChildBone();
        }

        public void SelectParentBone()
        {
            if ( selectedBone == null )
            {
                SelectBone( character.Bones[ 0 ] );
            }
            //TODO:
        }
        public void SelectNextSiblingBone()
        {
            //TODO:
        }
        public void SelectFirstChildBone()
        {
            //TODO:
        }

        void btnViewWeights_ItemClick( object sender, DevExpress.XtraBars.ItemClickEventArgs e )
        {
            ToggleViewWeights();
        }

        void trackBarSpeed_ValueChanged( object sender, EventArgs e )
        {
            //form.trackBarSpeed.value
            //animationSpeed = (int)form.trackSpeed.EditValue / 100f;
        }
        private void SetAnimationSpeed( float value )
        {
            animationSpeed = value;
            form.trackSpeed.EditValue = (int)( value * 100 );
        }

        void trackBarSpeed_EditValueChanged( object sender, EventArgs e )
        {
            animationSpeed = (int)form.trackSpeed.EditValue / 100f;
        }

        void btnSpectater_ItemClick( object sender, DevExpress.XtraBars.ItemClickEventArgs e )
        {
            ToggleSpectater();
        }


        void btnPlay_ItemClick( object sender, DevExpress.XtraBars.ItemClickEventArgs e )
        {
            TogglePlay();
        }

        void btnViewSkin_ItemClick( object sender, DevExpress.XtraBars.ItemClickEventArgs e )
        {
            ToggleViewSkin();
        }

        void btnViewBones_ItemClick( object sender, DevExpress.XtraBars.ItemClickEventArgs e )
        {
            ToggleViewBones();
        }
        void XNAGameControl_InitializeEvent( object sender, EventArgs e )
        {
            character = CharacterBuilder.LoadCharacter( XNAGameControl, ColladaModel.LoadSimpleCharacterAnim001() );
            //character = CharacterBuilder.LoadCharacter( XNAGameControl, ColladaModel.LoadTriangleBones001() );
            character.WorldMatrix = Matrix.CreateRotationX( -MathHelper.PiOver2 );
            fillBonesTree();
            ToggleViewBones(); ToggleViewBones();
            ToggleViewSkin(); ToggleViewSkin();
            ToggleViewWeights(); ToggleViewWeights();
        }

        void XNAGameControl_UpdateEvent( Microsoft.Xna.Framework.GameTime ntime )
        {
            if ( XNAGameControl.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.R ) )
            {
                XNAGameControl_InitializeEvent( null, null );

            }
            transformControl.Update();
            if ( play )
            {
                character.UpdateAnimation( XNAGameControl.Elapsed * animationSpeed );
            }
            for ( int i = 0; i < character.Bones.Count; i++ )
            {
                //character.Bones[i].BoxMesh
            }
            if ( XNAGameControl.Mouse.LeftMouseJustPressed )
            {
                if ( transformControl.IsGizmoTargeted() == false ) checkSelectBone();
            }

            if ( XNAGameControl.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.LeftAlt ) && XNAGameControl.Camera == specCam )
            {
                specCam.Enabled = !specCam.Enabled;
                XNAGameControl.Mouse.CursorEnabled = !specCam.Enabled;
            }

            if ( selectedBone != null )
            {
                Matrix mat = Matrix.Identity;
                if ( selectedBone.parent != null ) mat = selectedBone.parent.finalMatrix;
                transformControl.Position = ( mat * character.WorldMatrix ).Translation;
                selectedBone.CurrentMatrix = Matrix.CreateFromQuaternion( transformControl.Rotation ) * selectedBoneStartCurrentMatrix;
                character.CalculateTotalMatrices();
            }

        }

        void XNAGameControl_DrawEvent( Microsoft.Xna.Framework.GameTime ntime )
        {
            XNAGameControl.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None;

            if ( viewSkin ) renderSkin();
            if ( viewBones ) renderBones();


            transformControl.Render();
        }

        private void renderBones()
        {
            foreach ( CharacterBone bone in character.Bones )
            {
                foreach ( CharacterBone childBone in bone.children )
                {
                    float length = Vector3.Distance(
                        ( bone.finalMatrix * character.WorldMatrix ).Translation,
                        ( childBone.finalMatrix * character.WorldMatrix ).Translation );
                    childBone.BoxMesh.PivotPoint = new Vector3( 0, 0.5f, 0.5f );
                    childBone.BoxMesh.Dimensions = new Vector3( childBone.CurrentMatrix.Translation.Length(), 1, 1 );
                    childBone.BoxMesh.WorldMatrix = 
                         Matrix.CreateFromQuaternion( Quaternion.CreateFromRotationMatrix( childBone.CurrentMatrix ) )
                        *bone.finalMatrix* character.WorldMatrix;
                    childBone.BoxMesh.Color = Character.BoneColors[ bone.num % Character.BoneColors.Length ];
                    if ( childBone == selectedBone ) childBone.BoxMesh.Color = Color.Red;
                    childBone.BoxMesh.Render( XNAGameControl );
                    //XNAGameControl.LineManager3D.AddLine(
                    //    ( bone.finalMatrix * character.WorldMatrix ).Translation,
                    //    ( childBone.finalMatrix * character.WorldMatrix ).Translation,
                    //    Character.BoneColors[ bone.num % Character.BoneColors.Length ] );
                }
                bone.BoxMesh.PivotPoint = new Vector3( 0.5f, 0.5f, 0.5f );
                bone.BoxMesh.Dimensions = new Vector3( 1, 1, 1 );
                bone.BoxMesh.WorldMatrix = bone.finalMatrix * character.WorldMatrix;
                bone.BoxMesh.Color = Character.BoneColors[ bone.num % Character.BoneColors.Length ];
                if ( bone == selectedBone ) bone.BoxMesh.Color = Color.Red;
                bone.BoxMesh.Render( XNAGameControl );
                XNAGameControl.LineManager3D.AddCenteredBox( ( bone.finalMatrix * character.WorldMatrix ).Translation, 0.2f, Character.BoneColors[ bone.num % Character.BoneColors.Length ] );

            } // foreach (bone)

            //for (int i = 0; i < character.Bones.Count; i++)
            //{
            //    character.Bones[ i ].BoxMesh.WorldMatrix = character.Bones[ i ].finalMatrix * character.WorldMatrix;
            //    character.Bones[ i ].BoxMesh.Dimensions = new Vector3( 3, 1, 1 );
            //    character.Bones[ i ].BoxMesh.PivotPoint = Vector3.Zero;
            //    character.Bones[ i ].BoxMesh.Render( XNAGameControl );
            //}
            character.RenderBonesLines();
        }

        private void renderSkin()
        {
            character.Render();
        }

        private void checkSelectBone()
        {
            if ( XNAGameControl.Mouse.CursorEnabled == false ) return;
            if ( XNAGameControl.IsCursorOnControl() == false ) return;
            Raycast.RaycastResult<CharacterBone> result;
            result = Raycast.RaycastHelper.MultipleRayscast<CharacterBone>( character.Bones, GetMousePickRay() );
            if ( result.IsHit )
            {
                SelectBone( result.Item );
            }
            else
            {
                DeselectBone();
            }
        }

        public void SelectBone( CharacterBone bone )
        {
            DeselectBone();
            selectedBone = bone;
            transformControl.Enabled = true;
            transformControl.Rotation = Quaternion.Identity;
            selectedBoneStartCurrentMatrix = selectedBone.CurrentMatrix;
            for ( int i = 0; i < character.Meshes.Count; i++ )
            {
                character.Meshes[ i ].Shader.SelectedBoneIndex = bone.num;
            }
            form.Text = selectedBone.Name;
        }

        public void DeselectBone()
        {
            selectedBone = null;
            transformControl.Enabled = false;
            for ( int i = 0; i < character.Meshes.Count; i++ )
            {
                character.Meshes[ i ].Shader.SelectedBoneIndex = -1;
            }
        }

        private Ray GetMousePickRay()
        {
            return XNAGameControl.GetWereldViewRay( XNAGameControl.Mouse.CursorPositionVector );
        }

        private void ToggleSpectater()
        {
            specCam.Enabled = false;
            if ( XNAGameControl.Camera == specCam )
            {
                XNAGameControl.Mouse.CursorEnabled = true;
                XNAGameControl.Camera = XNAGameControl.EditorCamera;
                XNAGameControl.EditorCamera.Enabled = true;
                form.btnSpectater.Down = false;
            }
            else
            {
                XNAGameControl.Mouse.CursorEnabled = false;
                XNAGameControl.EditorCamera.Enabled = false;
                XNAGameControl.Camera = specCam;
                form.btnSpectater.Down = true;
            }
        }
        public void ToggleViewSkin()
        {
            viewSkin = !viewSkin;
            form.btnViewSkin.Down = viewSkin;
        }
        public void ToggleViewWeights()
        {
            viewWeights = !viewWeights;
            form.btnViewWeights.Down = viewWeights;
            if ( viewWeights )
            {
                for ( int i = 0; i < character.Meshes.Count; i++ )
                {
                    character.Meshes[ i ].Shader.Technique = Common.Core.Collada.SkinnedShader.TechniqueType.DisplayWeights;
                }
            }

            else
            {
                for ( int i = 0; i < character.Meshes.Count; i++ )
                {
                    character.Meshes[ i ].Shader.Technique = Common.Core.Collada.SkinnedShader.TechniqueType.Colored;
                }
            }
        }

        public void ToggleViewBones()
        {
            viewBones = !viewBones;
            form.btnViewBones.Down = viewBones;
        }

        public void TogglePlay()
        {
            play = !play;
            form.btnPlay.Down = play;
        }

        public static void TestCharacterEditor()
        {
            WizardsEditor editor = new WizardsEditor();
            CharacterEditorService ces = editor.Database.RequireService<CharacterEditorService>();

            ces.CharacterWizardsEditorForm.OpenCharacterEditor();

            editor.RunEditor();
        }
    }
}
