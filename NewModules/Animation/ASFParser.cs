using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Animation
{
    public class ASFParser
    {
        private StreamReader reader;

        private List<ParseCommandDelegate> parserStack = new List<ParseCommandDelegate>();
        private ParserLine line;
        private ASFJoint currentJoint;

        public List<ASFJoint> Joints = new List<ASFJoint>();
        public ASFJoint RootJoint;

        private struct ParserLine
        {
            public string Line;
            public List<string> Parts;

            public void SetLine(string _line)
            {
                Line = _line.Trim();

                if (Parts == null) Parts = new List<string>();
                Parts.Clear();

                int start = 0;

                int i;
                for (i = 0; i < Line.Length; i++)
                {
                    if (Line[i] != ' ')
                        continue;

                    Parts.Add(Line.Substring(start, i - start));
                    start = i + 1;
                }
                Parts.Add(Line.Substring(start, i - start));
            }

            public string GetString(int part)
            {
                return Parts[part];
            }
            public int GetInt(int part)
            {
                return int.Parse(Parts[part]);
            }
            public float GetFloat(int part)
            {
                return float.Parse(Parts[part], (new CultureInfo("en-us")).NumberFormat);
            }
            public Vector3 GetVector3(int part)
            {
                return new Vector3(GetFloat(part), GetFloat(part + 1), GetFloat(part + 2));
            }

            public static bool operator ==(string x, ParserLine y)
            {
                return x == y.Line;
            }
            public static bool operator ==(ParserLine y, string x)
            {
                return x == y.Line;
            }

            public static bool operator !=(ParserLine y, string x)
            {
                return !(y == x);
            }


            public static bool operator !=(string x, ParserLine y)
            {
                return !(x == y);
            }
        }


        /// <summary>
        /// Returns true when line was parsed, false when nothing happened.
        /// Return false when line needs to be passed on to the lower levels.
        /// </summary>
        /// <returns></returns>
        private delegate bool ParseCommandDelegate();

        public void ImportASF(Stream strm)
        {
            reader = new StreamReader(strm);

            try
            {
                Joints.Clear();

                pushCommand(parseCommand);
                pushCommand(delegate { return true; }); // placeholder

                //parse sections
                while (!reader.EndOfStream)
                {
                    readLine();

                    for (int i = 0; i < parserStack.Count; i++)
                    {
                        if (parserStack[i]()) break;
                    }

                }
            }
            finally
            {
                reader.Close(); // is this necessary?
            }
        }

        private bool parseCommand()
        {
            if (!line.Line.StartsWith(":")) return false;

            ParseCommandDelegate newCmd = null;

            if (line == ":bonedata")
            {
                newCmd = parseCommandBonedata;
            }
            if (line == ":hierarchy")
            {
                newCmd = parseCommandHierarchy;
            }
            if (line == ":root")
            {
                var j = new ASFJoint();
                j.name = "root";
                Joints.Add(j);
                RootJoint = j;
            }

            if (newCmd != null)
            {
                popCommand();
                pushCommand(newCmd);
            }

            return true;

        }

        private bool parseCommandBonedata()
        {
            if (line == "begin")
            {
                currentJoint = new ASFJoint();
                pushCommand(parseCommandJoint);
                return true;
            }
            if (line == "end")
            {
                Joints.Add(currentJoint);
                popCommand();
                return true;
            }
            return false;
        }
        private bool parseCommandJoint()
        {
            switch (line.GetString(0))
            {
                case "id":
                    currentJoint.id = line.GetInt(1);
                    break;
                case "name":
                    currentJoint.name = line.GetString(1);
                    break;
                case "direction":
                    currentJoint.direction = line.GetVector3(1);
                    break;
                case "length":
                    currentJoint.length = line.GetFloat(1);
                    break;
                case "axis":
                    currentJoint.axis = line.GetVector3(1);
                    currentJoint.axisOrder = line.GetString(4);
                    break;
                case "dof":
                    currentJoint.dof = line.Parts.Skip(1).ToArray();
                    break;
            }
            return true;
        }

        private bool parseCommandHierarchy()
        {
            if (line == "begin")
            {
                pushCommand(parseCommandHierarchyJoint);
                return true;
            }

            if (line == "end")
            {
                popCommand();
                return true;
            }

            return false;
        }
        private bool parseCommandHierarchyJoint()
        {
            var parent = Joints.Find(o => o.name == line.GetString(0));
            for (int i = 1; i < line.Parts.Count; i++)
            {
                var j = Joints.Find(o => o.name == line.GetString(i));
                j.parent = parent;

                parent.children.Add(j);
            }

            return true;
        }

        private void popCommand()
        {
            parserStack.RemoveAt(parserStack.Count - 1);
        }
        private void pushCommand(ParseCommandDelegate cmd)
        {
            parserStack.Add(cmd);
        }


        private void readLine()
        {
            line.SetLine(reader.ReadLine());
        }

        public Skeleton ImportSkeleton()
        {
            var skeleton = new Skeleton();
            Joint rootJoint = new Joint();
            rootJoint.Name = "root";
            skeleton.Joints.Add(rootJoint);
            importSkeletonJoints(skeleton, RootJoint, rootJoint);

            return skeleton;
        }

        private void importSkeletonJoints(Skeleton skeleton, ASFJoint asfJoint, Joint joint)
        {
            joint.CalculateAbsoluteMatrix();
            for (int i = 0; i < asfJoint.children.Count; i++)
            {
                var asfChild = asfJoint.children[i];
                var child = new Joint();
                child.Parent = joint;
                child.Name = asfChild.name;
                child.Length = asfChild.length;

                //get rotation component inherited from the parent rotation
                child.CalculateInitialRelativeMatrix(Matrix.Identity);
                child.CalculateAbsoluteMatrix();
                var trans = Vector3.TransformCoordinate(Vector3.Zero, child.AbsoluteMatrix);
                var rotationMat = child.AbsoluteMatrix * Matrix.Translation(-trans);

                child.CalculateInitialRelativeMatrix(XnaMathExtensions.CreateRotationMatrixMapDirection(Vector3.UnitX.xna(), asfChild.direction.xna()).dx() * Matrix.Invert(rotationMat));
                //Undo rotation of parent
                //child.RelativeMatrix = XnaMathExtensions.CreateRotationMatrixMapDirection(Vector3.UnitX, asfChild.direction) * Matrix.CreateTranslation(trans) * Matrix.Invert(child.AbsoluteMatrix);
                //child.RelativeMatrix = Matrix.Invert(rotationMat) * XnaMathExtensions.CreateRotationMatrixMapDirection(Vector3.UnitX, asfChild.direction);


                skeleton.Joints.Add(child);
                importSkeletonJoints(skeleton, asfChild, child);


            }
        }

    }
}
