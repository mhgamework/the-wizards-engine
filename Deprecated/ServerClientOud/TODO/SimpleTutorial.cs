//By Jason Zelsnack

using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using NovodexWrapper;



//This is not a D3D tutorial, so just ignore that stuff and the GUI code.
//The main sections of interest are startPhysics(), tickPhysics(), createPhysicsStuff()

//Novodex is unitless. This sample is using 1 meter per unit which just means gravity is -9.8
// Using one meter per unit is a good scale to achieve numerical precision
// When choosing a unit size you need to adjust the skinWidth distance accordingly or else
//  objects may never go to sleep. In this demo the skinWidth is 1 centimeter

//Any class starting with "Novodex" (ex: NovodexUtil, NovodexDebugRenderer, NovodexMemoryStream....)
// is my own custom class and has nothing to with the native SDK

//I added constructors and static utility functions in the object classes to simplify the
// creation of those objects. Natively Novodex heavily uses descriptor classes where you specify
// every parameter before creating the object. You can still do that with this wrapper, but you can
// also use the constructors and utility functions to create your objects. After you have
// your object you can manually set the parameters by calling methods from that object.

//Note: For some reason NxUserOutputStream and the ListBox don't like eachother. When Novodex throws
// an error message it will crash the program if you try to add the message to the ListBox immediately.
// If you call Debug.WriteLine() that will work fine. To get around the problem I created a print
// cache which I purge after each frame.
//Sometimes cooked meshes contain different numbers of triangles and or vertices.
// This probably has something to with Novodex optimizing the meshes for collision detection.
//Gravity hasn't been implemented for the controllers yet. Controllers have to be explicity moved to where they belong so they are not effected by the scene's gravity.

//Trouble:
//All memory isn't properly freed when the scene is reset.
// TriangleMeshes, ConvexMeshes, Cloths leak native memory. HeightFields don't seem to leak, though.
//The lines aren't z-buffered. Since the cloths are rendered last they always look like they are in front of other objects.
//The way cloths are rendered are really slow and dumb. A better method of getting at the triangles will be added eventually.
//Tearable cloths can cause it to crash. Cloths of medium dimensions (~250 tris) are worst. High density cloths are more crash tolerant.


namespace SimpleTutorial
{
    public enum DriveObjectEnum { Camera, Unicycle, CapsuleController, BoxController, Car }


    public class SimpleTutorial : System.Windows.Forms.Form
    {
        static private SimpleTutorial staticSimpleTutorial = null;		//This is for the static printLine() method. It's static so other classes can easily call it.

        #region GUI components
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.Panel viewport_panel;
        private System.Windows.Forms.CheckBox pause_checkBox;
        private System.Windows.Forms.CheckBox gravity_checkBox;
        private System.Windows.Forms.Button resetScene_button;
        private System.Windows.Forms.Button clear_button;
        private System.Windows.Forms.Button printScene_button;
        private System.Windows.Forms.Button printInfo_button;
        private System.Windows.Forms.Button garbageCollect_button;
        private System.Windows.Forms.TrackBar wind_trackBar;
        private System.Windows.Forms.Label wind_label;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton unicycle_radioButton;
        private System.Windows.Forms.RadioButton capsuleController_radioButton;
        private System.Windows.Forms.RadioButton boxController_radioButton;
        private System.Windows.Forms.RadioButton car_radioButton;
        #endregion

        #region Renderer Stuff (Non physics related)
        Matrix cameraMatrix;
        bool[] keyStates = new bool[ 256 ];
        float nearClipDistance = 0.01f;
        float farClipDistance = 2000.0f;
        float verticalFOV = 60.0f;
        bool pauseFlag = false;
        bool startedFlag = false;
        int randomActorCount = 0;
        #endregion

        NxPhysicsSDK physicsSDK = null;
        NxScene physicsScene = null;
        //This is built into the wrapper to provide a D3D debug renderer for the physics

        //Actor axes, joint axes, normals.... are scaled by this value

        //There's an array which is used to smooth out the framerate for the physics timestep

        float[] timeStepArray = null;




        public SimpleTutorial()
        {
            InitializeComponent();
            Directory.SetCurrentDirectory( Application.StartupPath );

            timeStepArray = new float[ 32 ];
            for ( int i = 0; i < timeStepArray.Length; i++ )	//Initialize the array to a reasonable target frame rate of 60 FPS.
            { timeStepArray[ i ] = 1 / 60.0f; }
        }

        //This is outside the constructor because physicsDebugRenderer relies upon the
        // renderDevice to be initialized before it is created in startPhysics()
        public void appStarted()
        {
            staticSimpleTutorial = this;	//This is for the static printLine() method
            //setProperFocus();			//set focus to get key events using a wacky technique

            if ( startPhysics() )
            {
                resetScene();
                //printInfo();
            }
        }

        //////////////////////////////PHYSICS CODE BELOW////////////////////////////////
        public bool startPhysics()
        {
            //If you don't have your own NxUserOutputStream just use NxPhysicsSDK.Create() instead
            physicsSDK = NxPhysicsSDK.Create();
            if ( physicsSDK == null )
            {
                MessageBox.Show( "Failed to start physics", "Error", MessageBoxButtons.OK );
                this.Close();
                return false;
            }

            //There are several createScene() methods. In this case the simplest is used.
            physicsScene = physicsSDK.createScene( new Vector3( 0, 1, 0 ) );
            if ( physicsScene == null )
            {
                MessageBox.Show( "Failed to create scene", "Error", MessageBoxButtons.OK );
                this.Close();
                return false;
            }

            return true;
        }



        public void killPhysics()
        {
            if ( physicsSDK != null )
            {
                if ( physicsScene != null )
                { physicsSDK.releaseScene( physicsScene ); }
                physicsSDK.release();
                physicsSDK = null;
                physicsScene = null;
            }
        }




        //This will delete all the actors in the scene and then rebuild the start scene and reset the camera
        public void resetScene()
        {

            createPhysicsStuff();


        }


        public void createPhysicsStuff()
        {

            int seed2 = 18812;
            createHeightField( new Vector3( 20, -3, -30 ), 16, 16, 50, 50, 10, seed2, 3, 1.5f );


        }


        //Note: The heightField's origin is at the corner. Also, Row=X and Column=Z in Novodex.
        public NxActor createHeightField( Vector3 pos, int gridXsubdivisions, int gridZsubdivisions, float gridWidth, float gridDepth, float maxHeight, int seed, int numSmooths, float smoothWeight )
        {
            float[] heightArray = createHeightFieldData( gridXsubdivisions, gridZsubdivisions, gridWidth, gridDepth, maxHeight, seed, numSmooths, smoothWeight );

            int numRows = gridXsubdivisions + 1;
            int numColumns = gridZsubdivisions + 1;
            float verticalExtent = -maxHeight * 2; //The area under the heightField which will force an object on top of it
            NxHeightFieldDesc heightFieldDesc = new NxHeightFieldDesc( numRows, numColumns, verticalExtent, 0, 0 );
            for ( int row = 0; row < numRows; row++ )
            {
                for ( int column = 0; column < numColumns; column++ )
                {
                    //The tessFlag determines which way the triangles in a quad are tessellated. This code sets the terrain to be tessellated in a diamond manner, instead of all the quads being subdivided the same direction.
                    bool tessFlag = false;
                    if ( ( row + column ) % 2 == 1 )
                    { tessFlag = true; }

                    //The height data range is between 0 and maxHeight. Divide the height in the array by maxHeight to get it in a range of 0 and 1 and then convert to a short value by multiplying by short.MaxValue
                    short height = (short)( ( heightArray[ ( row * numColumns ) + column ] / maxHeight ) * short.MaxValue );
                    heightFieldDesc.setSample( row, column, new NxHeightFieldSample( height, 1, tessFlag, 1 ) );
                }
            }

            NxHeightField heightField = physicsSDK.createHeightField( heightFieldDesc );
            float rowScale = gridWidth / numRows;
            float columnScale = gridDepth / numColumns;
            float heightScale = maxHeight / short.MaxValue;
            NxHeightFieldShapeDesc shapeDesc = new NxHeightFieldShapeDesc( heightField, heightScale, rowScale, columnScale, 0, 0, 0 );
            NxActorDesc actorDesc = new NxActorDesc( shapeDesc, null, 1, Matrix.CreateTranslation( pos ) );
            actorDesc.name = "HeightField";
            return physicsScene.createActor( actorDesc );
        }

        //The data range is between 0 and maxHeight
        public float[] createHeightFieldData( int gridXsubdivisions, int gridZsubdivisions, float gridWidth, float gridDepth, float maxHeight, int seed, int numSmooths, float smoothWeight )
        {
            int numXverts = gridXsubdivisions + 1;
            int numZverts = gridZsubdivisions + 1;

            NovodexUtil.seedRandom( seed );

            //Create a random array of heights
            float[] heightArray = new float[ numXverts * numZverts ];
            for ( int z = 0; z < numZverts; z++ )
            {
                for ( int x = 0; x < numXverts; x++ )
                { heightArray[ x + ( z * numXverts ) ] = NovodexUtil.randomFloat( 0, maxHeight ); }
            }

            //Smooth the random values so the terrain isn't so jagged
            for ( int i = 0; i < numSmooths; i++ )
            {
                for ( int z = 0; z < numZverts; z++ )
                {
                    int z0 = z;
                    int z1 = NovodexUtil.clampInt( z + 1, 0, numZverts - 1 );

                    for ( int x = 0; x < numXverts; x++ )
                    {
                        int x0 = x;
                        int x1 = NovodexUtil.clampInt( x + 1, 0, numXverts - 1 );

                        float a = heightArray[ x0 + ( z0 * numXverts ) ];
                        float b = heightArray[ x1 + ( z0 * numXverts ) ];
                        float c = heightArray[ x1 + ( z1 * numXverts ) ];
                        float d = heightArray[ x0 + ( z1 * numXverts ) ];

                        heightArray[ x + ( z * numXverts ) ] = ( ( a * smoothWeight ) + b + c + d ) / ( smoothWeight + 3 );
                    }
                }
            }

            return heightArray;
        }



        #region Direct3D and Windows Code
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.viewport_panel = new System.Windows.Forms.Panel();
            this.listBox = new System.Windows.Forms.ListBox();
            this.pause_checkBox = new System.Windows.Forms.CheckBox();
            this.gravity_checkBox = new System.Windows.Forms.CheckBox();
            this.resetScene_button = new System.Windows.Forms.Button();
            this.clear_button = new System.Windows.Forms.Button();
            this.printScene_button = new System.Windows.Forms.Button();
            this.garbageCollect_button = new System.Windows.Forms.Button();
            this.wind_trackBar = new System.Windows.Forms.TrackBar();
            this.wind_label = new System.Windows.Forms.Label();
            this.printInfo_button = new System.Windows.Forms.Button();
            this.unicycle_radioButton = new System.Windows.Forms.RadioButton();
            this.capsuleController_radioButton = new System.Windows.Forms.RadioButton();
            this.boxController_radioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.car_radioButton = new System.Windows.Forms.RadioButton();
            ( (System.ComponentModel.ISupportInitialize)( this.wind_trackBar ) ).BeginInit();
            this.SuspendLayout();
            // 
            // viewport_panel
            // 
            this.viewport_panel.BackColor = System.Drawing.Color.FromArgb( ( (System.Byte)( 128 ) ), ( (System.Byte)( 255 ) ), ( (System.Byte)( 128 ) ) );
            this.viewport_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewport_panel.Location = new System.Drawing.Point( 8, 8 );
            this.viewport_panel.Name = "viewport_panel";
            this.viewport_panel.Size = new System.Drawing.Size( 512, 524 );
            this.viewport_panel.TabIndex = 0;
            // 
            // listBox
            // 
            this.listBox.HorizontalScrollbar = true;
            this.listBox.Location = new System.Drawing.Point( 528, 88 );
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size( 256, 446 );
            this.listBox.TabIndex = 1;
            this.listBox.TabStop = false;

            // 
            // pause_checkBox
            // 
            this.pause_checkBox.Location = new System.Drawing.Point( 552, 8 );
            this.pause_checkBox.Name = "pause_checkBox";
            this.pause_checkBox.Size = new System.Drawing.Size( 64, 24 );
            this.pause_checkBox.TabIndex = 0;
            this.pause_checkBox.TabStop = false;
            this.pause_checkBox.Text = "Pause";
            // 
            // gravity_checkBox
            // 
            this.gravity_checkBox.Checked = true;
            this.gravity_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.gravity_checkBox.Location = new System.Drawing.Point( 552, 32 );
            this.gravity_checkBox.Name = "gravity_checkBox";
            this.gravity_checkBox.Size = new System.Drawing.Size( 64, 24 );
            this.gravity_checkBox.TabIndex = 0;
            this.gravity_checkBox.TabStop = false;
            this.gravity_checkBox.Text = "Gravity";
            // 
            // resetScene_button
            // 
            this.resetScene_button.Location = new System.Drawing.Point( 688, 8 );
            this.resetScene_button.Name = "resetScene_button";
            this.resetScene_button.Size = new System.Drawing.Size( 88, 20 );
            this.resetScene_button.TabIndex = 0;
            this.resetScene_button.TabStop = false;
            this.resetScene_button.Text = "Reset Scene";
            // 
            // clear_button
            // 
            this.clear_button.Location = new System.Drawing.Point( 736, 540 );
            this.clear_button.Name = "clear_button";
            this.clear_button.Size = new System.Drawing.Size( 48, 23 );
            this.clear_button.TabIndex = 0;
            this.clear_button.TabStop = false;
            this.clear_button.Text = "Clear";
            // 
            // printScene_button
            // 
            this.printScene_button.Location = new System.Drawing.Point( 688, 32 );
            this.printScene_button.Name = "printScene_button";
            this.printScene_button.Size = new System.Drawing.Size( 88, 20 );
            this.printScene_button.TabIndex = 0;
            this.printScene_button.TabStop = false;
            this.printScene_button.Text = "Print Scene";
            // 
            // garbageCollect_button
            // 
            this.garbageCollect_button.Location = new System.Drawing.Point( 528, 540 );
            this.garbageCollect_button.Name = "garbageCollect_button";
            this.garbageCollect_button.Size = new System.Drawing.Size( 96, 23 );
            this.garbageCollect_button.TabIndex = 2;
            this.garbageCollect_button.TabStop = false;
            this.garbageCollect_button.Text = "Garbage Collect";
            // 
            // wind_trackBar
            // 
            this.wind_trackBar.AutoSize = false;
            this.wind_trackBar.Location = new System.Drawing.Point( 588, 60 );
            this.wind_trackBar.Maximum = 100;
            this.wind_trackBar.Name = "wind_trackBar";
            this.wind_trackBar.Size = new System.Drawing.Size( 80, 20 );
            this.wind_trackBar.TabIndex = 4;
            this.wind_trackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.wind_trackBar.Value = 30;
            // 
            // wind_label
            // 
            this.wind_label.Location = new System.Drawing.Point( 548, 60 );
            this.wind_label.Name = "wind_label";
            this.wind_label.Size = new System.Drawing.Size( 32, 23 );
            this.wind_label.TabIndex = 5;
            this.wind_label.Text = "Wind";
            // 
            // printInfo_button
            // 
            this.printInfo_button.Location = new System.Drawing.Point( 688, 56 );
            this.printInfo_button.Name = "printInfo_button";
            this.printInfo_button.Size = new System.Drawing.Size( 88, 20 );
            this.printInfo_button.TabIndex = 6;
            this.printInfo_button.TabStop = false;
            this.printInfo_button.Text = "Print Info";
            // 
            // unicycle_radioButton
            // 
            this.unicycle_radioButton.Checked = true;
            this.unicycle_radioButton.Location = new System.Drawing.Point( 32, 536 );
            this.unicycle_radioButton.Name = "unicycle_radioButton";
            this.unicycle_radioButton.Size = new System.Drawing.Size( 68, 32 );
            this.unicycle_radioButton.TabIndex = 7;
            this.unicycle_radioButton.TabStop = true;
            this.unicycle_radioButton.Text = "Unicycle";
            // 
            // capsuleController_radioButton
            // 
            this.capsuleController_radioButton.Location = new System.Drawing.Point( 136, 536 );
            this.capsuleController_radioButton.Name = "capsuleController_radioButton";
            this.capsuleController_radioButton.Size = new System.Drawing.Size( 72, 32 );
            this.capsuleController_radioButton.TabIndex = 8;
            this.capsuleController_radioButton.Text = "Capsule Controller";
            this.capsuleController_radioButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // boxController_radioButton
            // 
            this.boxController_radioButton.Location = new System.Drawing.Point( 244, 536 );
            this.boxController_radioButton.Name = "boxController_radioButton";
            this.boxController_radioButton.Size = new System.Drawing.Size( 72, 32 );
            this.boxController_radioButton.TabIndex = 9;
            this.boxController_radioButton.Text = "Box Controller";
            this.boxController_radioButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point( 8, 544 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 24, 20 );
            this.label1.TabIndex = 10;
            this.label1.Text = "F1:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point( 112, 544 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 24, 20 );
            this.label2.TabIndex = 11;
            this.label2.Text = "F2:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point( 220, 544 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 24, 20 );
            this.label3.TabIndex = 12;
            this.label3.Text = "F3:";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point( 328, 544 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 24, 20 );
            this.label4.TabIndex = 14;
            this.label4.Text = "F4:";
            // 
            // car_radioButton
            // 
            this.car_radioButton.Location = new System.Drawing.Point( 352, 536 );
            this.car_radioButton.Name = "car_radioButton";
            this.car_radioButton.Size = new System.Drawing.Size( 44, 32 );
            this.car_radioButton.TabIndex = 13;
            this.car_radioButton.Text = "Car";
            // 
            // SimpleTutorial
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
            this.ClientSize = new System.Drawing.Size( 794, 568 );
            this.Controls.Add( this.label4 );
            this.Controls.Add( this.car_radioButton );
            this.Controls.Add( this.label3 );
            this.Controls.Add( this.label2 );
            this.Controls.Add( this.label1 );
            this.Controls.Add( this.boxController_radioButton );
            this.Controls.Add( this.capsuleController_radioButton );
            this.Controls.Add( this.unicycle_radioButton );
            this.Controls.Add( this.gravity_checkBox );
            this.Controls.Add( this.printInfo_button );
            this.Controls.Add( this.wind_label );
            this.Controls.Add( this.wind_trackBar );
            this.Controls.Add( this.garbageCollect_button );
            this.Controls.Add( this.printScene_button );
            this.Controls.Add( this.clear_button );
            this.Controls.Add( this.resetScene_button );
            this.Controls.Add( this.pause_checkBox );
            this.Controls.Add( this.listBox );
            this.Controls.Add( this.viewport_panel );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SimpleTutorial";
            this.Text = "Simple Tutorial";
            ( (System.ComponentModel.ISupportInitialize)( this.wind_trackBar ) ).EndInit();
            this.ResumeLayout( false );

        }
        #endregion

        [STAThread]
        static void Main()
        {
            using ( SimpleTutorial simpleTutorial = new SimpleTutorial() )
            {

                simpleTutorial.Show();
                simpleTutorial.appStarted();

                while ( simpleTutorial.Created )	//Main loop
                {
                    if ( simpleTutorial.startedFlag )
                    {
                        //simpleTutorial.processKeys();
                        //simpleTutorial.tickPhysics();
                    }
                    //simpleTutorial.render();

                    //SimpleTutorial.purgePrintList();
                    Application.DoEvents();
                }
                simpleTutorial.killPhysics();	//be nice and properly release the physics
            }
        }

        static public void printLine( string message )
        {
        }


        #endregion


    }
}




