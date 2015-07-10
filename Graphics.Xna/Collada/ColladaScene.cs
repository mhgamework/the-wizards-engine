using System;
using System.Collections.Generic;
using System.Xml;
using MHGameWork.TheWizards.Graphics.Xna.Graphics.TODO;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Graphics.Xna.Collada
{
    public class ColladaScene : ColladaSceneNodeBase
    {
        private readonly ColladaModel model;

        public ColladaModel Model
        {
            get { return model; }
        }

        public ColladaScene( ColladaModel nModel )
        {
            model = nModel;
            Type = NodeType.Scene;
            Matrix = Matrix.Identity;
            scene = this;
            parent = null;
            Instance_Geometry = null;
        }

        public void LoadScene( XmlNode visualSceneNode )
        {
            ID = XmlHelper.GetXmlAttribute( visualSceneNode, "id" );
            Name = XmlHelper.GetXmlAttribute( visualSceneNode, "name" );

            foreach ( XmlNode nodeNode in visualSceneNode )
            {
                if ( nodeNode.Name == "node" )
                {
                    ColladaSceneNode node = new ColladaSceneNode( this );
                    Nodes.Add( node );
                    node.LoadNode( nodeNode );


                }

            }
        }

        public override string ToString()
        {
            return "ColladaScene - " + ID;
        }
    }

    public abstract class ColladaSceneNodeBase
    {
        public enum NodeType
        {
            Node = 1,
            Joint,
            Scene

        }

        public string ID;
        public string Name;
        protected ColladaScene scene;
        protected ColladaSceneNodeBase parent;
        public NodeType Type;
        public Matrix Matrix;
        public ColladaMesh Instance_Geometry;

        private List<ColladaSceneNodeBase> nodes;

        public List<ColladaSceneNodeBase> Nodes
        {
            get { return nodes; }
        }

        /// <summary>
        /// Returns the total matrix built from this nodes parents.
        /// </summary>
        /// <returns></returns>
        public Matrix GetFullMatrix()
        {
            if ( parent == null ) return Matrix;
            return Matrix * parent.GetFullMatrix();
        }

        protected ColladaSceneNodeBase()
        {
            nodes = new List<ColladaSceneNodeBase>();
        }

    }

    public class ColladaSceneNode : ColladaSceneNodeBase
    {

        public ColladaSceneNode( ColladaScene nScene )
        {
            scene = nScene;
            parent = null;
        }
        private ColladaSceneNode( ColladaSceneNode nParent )
            : this( nParent.scene )
        {
            parent = nParent;
        }

        public void LoadNode( XmlNode nodeNode )
        {


            ID = XmlHelper.GetXmlAttribute( nodeNode, "id" );
            Name = XmlHelper.GetXmlAttribute( nodeNode, "name" );
            SetNodeType( XmlHelper.GetXmlAttribute( nodeNode, "type" ) );

            XmlNode matrixNode = XmlHelper.GetChildNode( nodeNode, "matrix" );
            if ( matrixNode != null ) Matrix = ColladaModel.LoadColladaMatrix( matrixNode );

            foreach ( XmlNode instance_geometryNode in nodeNode )
            {
                if ( instance_geometryNode.Name == "instance_geometry" )
                {
                    if ( Instance_Geometry != null )
                        throw new Exception(
                            "More then one Instance_Geometry node found, which is not currently implemented!" );

                    string url = XmlHelper.GetXmlAttribute( instance_geometryNode, "url" ).Substring( 1 );
                    Instance_Geometry = scene.Model.GetMesh( url );
                }
            }

            foreach ( XmlNode childNode in nodeNode )
            {
                if ( childNode.Name == "node" )
                {
                    ColladaSceneNode node = new ColladaSceneNode( this );
                    Nodes.Add( node );
                    node.LoadNode( childNode );


                }

            }
        }

        public void SetNodeType( string typeString )
        {
            switch ( typeString.ToLower() )
            {
                case "node":
                    Type = NodeType.Node;
                    break;
                case "joint":
                    Type = NodeType.Joint;
                    break;
            }
        }




        public override string ToString()
        {
            return "ColladaScene.Node - " + ID;
        }
    }
}