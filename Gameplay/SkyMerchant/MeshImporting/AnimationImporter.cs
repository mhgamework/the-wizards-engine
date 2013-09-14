using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.MeshImporting
{
    /// <summary>
    /// Responsible for parsing .twanim files
    /// </summary>
    public class AnimationImporter
    {

        private List<BoneData> bones; //TODO: export and import bone zero-transformations (currently just frame 0 values)
        private List<Frame> frames; 

        public void LoadAnimation(String path, out List<BoneData> boneStructure, out List<Frame> frameData)
        {
            reset();

            using (var reader = File.OpenText(path))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    String[] linePieces = line.Split(' ');
                    switch (linePieces[0])
                    {
                        case "b":
                            parseBoneData(linePieces[1]);
                            break;
                        case "f":
                            parseFrameData(linePieces[1]);
                            break;
                        default:
                            break;
                    }

                    line = reader.ReadLine();
                }


                //todo: properly export/import zero-transforms
                var frameZero = frames.Where(e => e.FrameID == 0).ToList().First();
                setZeroTransformations(frameZero);


                boneStructure = bones.ToList();
                frameData = frames.ToList();
            }
        }

        private void reset()
        {
            bones = new List<BoneData>();
            frames = new List<Frame>();
        }

        private void parseBoneData(String s)
        {
            var pieces = s.Split('/');
            var bone = new BoneData();
            bone.Name = pieces[0];
            bone.ParentName = pieces[1];
            bone.Length = float.Parse(pieces[2].Replace('.', ','));

            bones.Add(bone);
        }

        private void parseFrameData(String s)
        {
            var pieces = s.Split('/');
            var boneTransform = new BoneTransformation();
            boneTransform.BoneName = pieces[1];
            boneTransform.Translation = parseVector3(pieces[2]);
            boneTransform.Rotation = parseQuaternion(pieces[3]);
            boneTransform.Scale = parseVector3(pieces[4]);
            var frame = getFrame(int.Parse(pieces[0]));
            frame.AddBoneRecord(boneTransform);
        }

        private Frame getFrame(int frameID)
        {
            var frray = frames.Where(e => e.FrameID == frameID).ToArray();
            if (frray.Length> 0 && frray[0] != null)
                return frray[0];

            var ret = new Frame();
            ret.FrameID = frameID;
            frames.Add(ret);
            return ret;
        }

        private Vector3 parseVector3(String s)
        {
            var pieces = s.Split(',');
            pieces = pieces.Select(e => e.Replace('.', ',')).ToArray();
            return new Vector3(float.Parse(pieces[0]), float.Parse(pieces[1]), float.Parse(pieces[2]));
        }

        private Quaternion parseQuaternion(String s)
        {
            var pieces = s.Split(',');
            pieces = pieces.Select(e => e.Replace('.', ',')).ToArray();
            return new Quaternion(float.Parse(pieces[0]), float.Parse(pieces[1]), float.Parse(pieces[2]), float.Parse(pieces[3]));
        }
    
        /// <summary>
        /// Sets the zero-transforms of all bones to the transform they have in given frame.
        /// </summary>
        /// <param name="f"></param>
        private void setZeroTransformations(Frame f)
        {
            foreach (var b in f.getBoneData())
            {
                var bone = bones.Where(e => e.Name == b.BoneName).ToList().First();
                bone.ZeroRotation = b.Rotation;
                bone.ZeroScale = b.Scale;
                bone.ZeroTranslation = b.Translation;
            }
        }


    }
}
