﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Animation
{
    public class AMCParser
    {
        public List<AMCSample> Samples = new List<AMCSample>();


        public void ImportAMC(Stream strm)
        {
            using (StreamReader reader = new StreamReader(strm))
            {
                var currentSampleIndex = -1;
                Samples.Clear();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();


                    if (line.StartsWith("#") || line.StartsWith(":")) continue;

                    int sampleNumber;

                    if (int.TryParse(line, out sampleNumber))
                    {
                        //This is dangerous
                        while (Samples.Count <= sampleNumber || Samples[sampleNumber] == null)
                        {
                            Samples.Add(new AMCSample());
                        }
                        currentSampleIndex = sampleNumber - 1;
                        continue;
                    }

                    string[] parts = line.Split(' ');

                    var seg = new AMCSegment();
                    seg.JointName = parts[0];

                    var cultureInfo = new CultureInfo("en-gb");
                    seg.Data = parts.Skip(1).Select(str => float.Parse(str, cultureInfo.NumberFormat)).ToArray();

                    Samples[currentSampleIndex].Segments.Add(seg);
                }
            }
        }

        public void ImportAnimation()
        {

        }

        public Microsoft.Xna.Framework.Matrix CalculateRelativeMatrix(AMCSegment seg, ASFJoint asfJoint)
        {

            if (asfJoint.name == "root")
            {

                //use root orientation here? prob not
                Matrix c = Matrix.Identity;
                c =
                    Matrix.CreateRotationX(MathHelper.ToRadians(seg.Data[3])) *
                    Matrix.CreateRotationY(MathHelper.ToRadians(seg.Data[4])) * Matrix.CreateRotationZ(MathHelper.ToRadians(seg.Data[5]));

                //use root position here? prob not
                Matrix b = Matrix.CreateTranslation(seg.Data[0], seg.Data[1], seg.Data[2]);
                Matrix m = Matrix.Identity;

                m = Matrix.Identity;

                Matrix l = Matrix.Invert(c) * m * c * b;

                return l;
            }
            else
            {

                //WARNING: not using the given order asfJoint.axisOrder
                Matrix c = Matrix.CreateRotationX(MathHelper.ToRadians( asfJoint.axis.X))
                           * Matrix.CreateRotationY(MathHelper.ToRadians( asfJoint.axis.Y))
                           * Matrix.CreateRotationZ(MathHelper.ToRadians( asfJoint.axis.Z));

                Matrix b = Matrix.CreateTranslation(asfJoint.parent.direction * asfJoint.parent.length);
                Matrix m = Matrix.Identity;

                for (int i = 0; i < asfJoint.dof.Length; i++)
                {
                    switch (asfJoint.dof[i])
                    {
                        case "rx":
                            m = m * Matrix.CreateRotationX(MathHelper.ToRadians(seg.Data[i]));
                            break;
                        case "ry":
                            m = m * Matrix.CreateRotationY(MathHelper.ToRadians(seg.Data[i]));
                            break;
                        case "rz":
                            m = m * Matrix.CreateRotationZ(MathHelper.ToRadians(seg.Data[i]));
                            break;
                    }
                }

                Matrix l = Matrix.Invert(c) * m * c * b;
                return l;
            }
        }
    }
}
