using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MHGameWork.TheWizards.ServerClient.Engine
{
	public class ModelData : IDisposable
	{
		private string assetName;
		private int referenceCount;
		private bool loaded;

		private XnaModel xnaModel;
		private Matrix[] transforms;
		private Matrix objectMatrix;

		public ModelData(string nAssetName)
		{
			assetName = nAssetName;
			loaded = false;
		}

		~ModelData()
		{
			Dispose();
		}

		public void Dispose()
		{
			Unload();
		}

		public int AddReference(Model nModel)
		{
			referenceCount++;
			return referenceCount;
		}

		public int RemoveReference(Model nModel)
		{
			referenceCount--;
			return referenceCount;
		}

		public void Load(Game3DPlay.Core.DynamicContentManager content)
		{
			Unload( content );

			xnaModel = content.LoadNew <XnaModel>( assetName );
			// Get matrices for each mesh part
			transforms = new Matrix[ xnaModel.Bones.Count ];
			xnaModel.CopyAbsoluteBoneTransformsTo( transforms );

			float scaling;

			// Calculate scaling for this object, used for rendering.

			scaling = xnaModel.Meshes[ 0 ].BoundingSphere.Radius *
				transforms[ 0 ].Right.Length();
			if ( scaling == 0 )
				scaling = 0.0001f;

			objectMatrix = Matrix.Identity;

			objectMatrix *= Matrix.CreateTranslation( -xnaModel.Meshes[ 0 ].BoundingSphere.Center );
			// Apply scaling to objectMatrix to rescale object to size of 1.0
			objectMatrix *= Matrix.CreateScale( 1.0f / scaling );


			loaded = true;
		}

		public void Unload(ContentManager content)
		{
			Unload();
		}

		public void Unload()
		{
			if ( xnaModel != null )
			{
				//TODO: this disposing mechanism should be checked against de normal xna contentmanager disposing mechanism
				for ( int i = 0; i < xnaModel.Meshes.Count; i++ )
				{

					for ( int e = 0; e < xnaModel.Meshes[ i ].Effects.Count; e++ )
					{
						xnaModel.Meshes[ i ].Effects[ e ].Dispose();
					}
					xnaModel.Meshes[ i ].IndexBuffer.Dispose();
					xnaModel.Meshes[ i ].VertexBuffer.Dispose();

				}
			}
			xnaModel = null;
			loaded = false;
		}





		public string AssetName { get { return assetName; } }
		public bool Loaded { get { return loaded; } }
		public XnaModel XNAModel { get { return xnaModel; } }
		public Matrix ObjectMatrix { get { return objectMatrix; } }
		public Matrix[] Transforms { get { return transforms; } }
		public int ReferenceCount { get { return referenceCount; } }


	}
}
