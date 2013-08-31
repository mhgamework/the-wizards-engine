
using DirectX11;
using MHGameWork.TheWizards.ServerClient;
using SlimDX;
namespace MHGameWork.TheWizards.Rendering.Vegetation.Trees
{
    public struct Directions
    {
        public Vector3 Heading;
        public Vector3 Right;
        public Directions(Vector3 heading, Vector3 right)
        {
            Heading = heading;
            Right = right;
        }
        public static Directions DirectionsFromAngles(Directions _dirs, float _dropAngle, float _axialSplit)
        {
            Directions d;
            Vector3 right;
            Vector3 heading;
            right = Vector3.TransformNormal(_dirs.Right, Matrix.RotationAxis(_dirs.Heading, _axialSplit));// soùething id wrong poistion????
            heading = Vector3.TransformNormal(_dirs.Heading, Matrix.RotationAxis(right, _dropAngle));
            d.Heading = heading;
            d.Right = right;

            return d;

        }

        public static Directions DirectionsFromAngles(Directions _dirs, float _dropAngle, float _axialSplit, float _wobbelaxialsplit, float _wobbelDropAngle)
        {
            return DirectionsFromAngles(_dirs, _dropAngle + _wobbelDropAngle, _axialSplit + _wobbelaxialsplit);
        }

        public static Directions DirectionsFromAngleDown(Directions _dirs, float _dropAngle)
        {
            Directions d;
            Vector3 vec1=_dirs.Heading;
            d.Right = _dirs.Right;
            if (vec1 == Vector3.UnitY)
            {
                d.Heading = vec1;
                d=DirectionsFromAngles(d, _dropAngle, 0);
            }
            else
            {
                vec1.Y = 0;
                vec1.Normalize();
                Vector3 vec2 = Vector3.Cross(vec1, -Vector3.UnitY);
                d.Heading = Vector3.TransformCoordinate(_dirs.Heading, Matrix.RotationAxis(vec2, _dropAngle));
                
            }

            return d;
        }
       
    }
}
