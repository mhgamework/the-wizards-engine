using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;


namespace MHGameWork.Game3DPlay.Core
{
	public class DynamicContentManager : Microsoft.Xna.Framework.Content.ContentManager
	{
		class WrapAsset
		{
			public WrapAsset(object nAsset, string nAssetName)
			{
				Asset = nAsset;
				AssetName = nAssetName;
			}

			private object _asset;
			public object Asset
			{
				get { return _asset; }
				set { _asset = value; }
			}

			private int _refCount;
			public int ReferenceCount
			{
				get { return _refCount; }
				set { _refCount = value; }
			}

			private string _assetName;

			public string AssetName
			{
				get { return _assetName; }
				set { _assetName = value; }
			}
	


		}
		// Fields
		private const string contentExtension = ".xnb";
		private List<IDisposable> disposableAssets;
		//private Dictionary<string, object> loadedAssets;
		//private Dictionary<object, string> loadedAssetsInverse;
		private string rootDirectory;
		private IServiceProvider serviceProvider;

		private System.Collections.Generic.Dictionary<string, WrapAsset> names_wrapAssets;
		private System.Collections.Generic.Dictionary<object, WrapAsset> assets_wrapAssets;

		// Methods
		public DynamicContentManager(IServiceProvider serviceProvider)
			: this(serviceProvider, string.Empty)
		{
		}

		public DynamicContentManager(IServiceProvider serviceProvider, string rootDirectory)
			: base(serviceProvider, rootDirectory)
		{
			//this.loadedAssets = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			//this.loadedAssetsInverse = new Dictionary<object, string>();
			names_wrapAssets = new Dictionary<string, WrapAsset>(StringComparer.OrdinalIgnoreCase);
			assets_wrapAssets = new Dictionary<object, WrapAsset>();

			this.disposableAssets = new List<IDisposable>();
			if (serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}
			if (rootDirectory == null)
			{
				throw new ArgumentNullException("rootDirectory");
			}
			rootDirectory = Path.GetFullPath(Path.Combine(StorageContainer.TitleLocation, rootDirectory));
			this.serviceProvider = serviceProvider;
			this.rootDirectory = rootDirectory;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && (this.names_wrapAssets != null))
				{
					this.Unload();
				}
			}
			finally
			{
				//this.loadedAssets = null;
				names_wrapAssets = null;
				assets_wrapAssets = null;
				this.disposableAssets = null;
			}
		}

		static string GetCleanPath(string path)
		{
			int num2;
			path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			for (int i = 1; i < path.Length; i = Math.Max(num2 - 1, 1))
			{
				i = path.IndexOf(@"\..\", i);
				if (i < 0)
				{
					return path;
				}
				num2 = path.LastIndexOf(Path.DirectorySeparatorChar, i - 1) + 1;
				path = path.Remove(num2, (i - num2) + @"\..\".Length);
			}
			return path;
		}

		public override T Load<T>(string assetName)
		{
			WrapAsset wrapAsset;
			if (string.IsNullOrEmpty(assetName))
			{
				throw new ArgumentNullException("assetName");
			}
			assetName = GetCleanPath(assetName);
			if (names_wrapAssets.ContainsKey(assetName))
			{
				wrapAsset = names_wrapAssets[assetName];
				if (!(wrapAsset.Asset is T))
				{
					throw new Exception("throw new ContentLoadException(string.Format(CultureInfo.CurrentCulture, Resources.BadXnbWrongType, new object[] { assetName, obj2.GetType(), typeof(T) }));");
				}
				wrapAsset.ReferenceCount += 1;
				return (T)wrapAsset.Asset;
			}
			T local = this.ReadAsset<T>(assetName, RecordDisposableObject);

			wrapAsset = new WrapAsset(local,assetName);
			wrapAsset.ReferenceCount += 1;

			names_wrapAssets.Add(assetName, wrapAsset);
			assets_wrapAssets.Add(wrapAsset.Asset, wrapAsset);

			/*this.loadedAssets.Add(assetName, local);
			loadedAssetsInverse.Add(local, assetName);*/
			return local;
		}

		public virtual T LoadNew<T>(string assetName)
		{
			WrapAsset wrapAsset;
			if ( string.IsNullOrEmpty( assetName ) )
			{
				throw new ArgumentNullException( "assetName" );
			}
			assetName = GetCleanPath( assetName );


			//Note: check DontRecordDisposableObject
			T local = this.ReadAsset<T>( assetName, DontRecordDisposableObject );

			return local;
		}

		void RecordDisposableObject(IDisposable disposable)
		{
			this.disposableAssets.Add(disposable);
		}

		void DontRecordDisposableObject(IDisposable disposable)
		{

		}

		public override void Unload()
		{
			try
			{
				List<IDisposable>.Enumerator enumerator = this.disposableAssets.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						enumerator.Current.Dispose();
					}
				}
				finally
				{
					enumerator.Dispose();
				}
			}
			finally
			{
				this.names_wrapAssets.Clear();
				this.assets_wrapAssets.Clear();
				this.disposableAssets.Clear();
			}
		}

		public void Unload(IDisposable asset)
		{
			WrapAsset wrapAsset = assets_wrapAssets[asset];
			wrapAsset.ReferenceCount -= 1;
			if (wrapAsset.ReferenceCount == 0)
			{
				names_wrapAssets.Remove(wrapAsset.AssetName);
				assets_wrapAssets.Remove(asset);
				disposableAssets.Remove(asset);
			}
			asset.Dispose();
		}

	}
}
