using System;
using System.Drawing;
using System.IO;
using MHGameWork.TheWizards.Engine;
using SlimDX.Direct3D11;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// Allows recording of the backbuffer to image files
    /// </summary>
    public class RecordingSimulator : ISimulator
    {
        public bool Recording { get; private set; }
        public DirectoryInfo ActiveRecordingDirectory { get; private set; }
        public int ActiveRecordingFrameCount { get; private set; }
        public DirectoryInfo RootRecordingDirectory { get; set; }

        public Key RecordKey { get; set; }

        private DX11FontWrapper fontWrapper;


        public RecordingSimulator()
        {
            fontWrapper = new DX11FontWrapper(TW.Graphics.Device, "Verdana");

            RootRecordingDirectory = TWDir.Test.CreateSubdirectory("Recordings");
        }

        public void StartRecording()
        {
            ActiveRecordingFrameCount = 0;
            ActiveRecordingDirectory = RootRecordingDirectory.CreateSubdirectory(string.Format("{0:ddMMyy-HHmmss}",DateTime.Now));

            Recording = true;
        }
        public void StopRecording()
        {
            Recording = false;
        }


        public void Simulate()
        {
            RecordKey = Key.P;
            if (TW.Graphics.Keyboard.IsKeyPressed(RecordKey))
                if (Recording) StopRecording(); else StartRecording();

            if (Recording)
                saveBackbuffer();
        }

        private void saveBackbuffer()
        {
            Texture2D.SaveTextureToFile(TW.Graphics.Device.ImmediateContext,
                                        TW.Graphics.Form.BackBuffer, ImageFileFormat.Bmp,
                                        ActiveRecordingDirectory.FullName + "/img" +
                                        ActiveRecordingFrameCount + ".bmp");
            ActiveRecordingFrameCount++;



            TW.Graphics.Device.ImmediateContext.OutputMerger.BlendState = TW.Graphics.HelperStates.AlphaBlend;


            var pxSize = 20;

            var txt = "RECORDING " + ActiveRecordingFrameCount;

            //TW.Graphics.TextureRenderer.DrawColor(Color.Red.dx(), a.Position, a.Size);

            fontWrapper.Draw(txt, pxSize, TW.Graphics.Form.BackBuffer.Description.Width - 200, TW.Graphics.Form.BackBuffer.Description.Height - 30, Color.Red.dx());


        }
    }
}