using System.Xml.Serialization;

namespace MHGameWork.TheWizards.Collada.COLLADA140
{
    public enum InputSemantic
    {
        Unknown = 0,
        [XmlEnum("BINORMAL")]
        Binormal,
        [XmlEnum( "COLOR" )]
        Color,
        [XmlEnum( "CONTINUITY" )]
        Continuity,
        [XmlEnum( "IMAGE" )]
        Image,
        [XmlEnum( "INPUT" )]
        Input,
        [XmlEnum( "IN_TANGENT" )]
        In_tangent,
        [XmlEnum( "INTERPOLATION" )]
        Interpolation,
        [XmlEnum( "INV_BIND_MATRIX" )]
        Inv_Bind_Matrix,
        [XmlEnum( "JOINT" )]
        Joint,
        [XmlEnum( "LINEAR_STEPS" )]
        Linear_Steps,
        [XmlEnum( "MORPH_TARGET" )]
        Morph_Target,
        [XmlEnum( "MORPH_WEIGHT" )]
        Morph_Weight,
        [XmlEnum( "NORMAL" )]
        Normal,
        [XmlEnum( "OUTPUT" )]
        Output,
        [XmlEnum( "OUT_TANGENT" )]
        Out_Tangent,
        [XmlEnum( "POSITION" )]
        Position,
        [XmlEnum( "TANGENT" )]
        Tangent,
        [XmlEnum( "TEXBINORMAL" )]
        Texbinormal,
        [XmlEnum( "TEXCOORD" )]
        Texcoord,
        [XmlEnum( "TEXTANGENT" )]
        Textangent,
        [XmlEnum( "UV" )]
        UV,
        [XmlEnum( "VERTEX" )]
        Vertex,
        [XmlEnum( "WEIGHT" )]
        Weight
    }
}