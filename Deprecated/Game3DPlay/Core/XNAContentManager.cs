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
	class XNAContentManager : ContentManager
	{
		// Fields
		private const string contentExtension = ".xnb";
		private List<IDisposable> disposableAssets;
		private Dictionary<string, object> loadedAssets;
		private Dictionary<object, string> loadedAssetsInverse;
		private string rootDirectory;
		private IServiceProvider serviceProvider;

		// Methods
		public XNAContentManager(IServiceProvider serviceProvider)
			: this(serviceProvider, string.Empty)
		{
		}

		public XNAContentManager(IServiceProvider serviceProvider, string rootDirectory)
			: base(serviceProvider, rootDirectory)
		{
			this.loadedAssets = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			this.loadedAssetsInverse = new Dictionary<object, string>();
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
				if (disposing && (this.loadedAssets != null))
				{
					this.Unload();
				}
			}
			finally
			{
				this.loadedAssets = null;
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
			object obj2;
			if (this.loadedAssets == null)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			if (string.IsNullOrEmpty(assetName))
			{
				throw new ArgumentNullException("assetName");
			}
			assetName = GetCleanPath(assetName);
			if (this.loadedAssets.TryGetValue(assetName, out obj2))
			{
				if (!(obj2 is T))
				{
					throw new Exception("throw new ContentLoadException(string.Format(CultureInfo.CurrentCulture, Resources.BadXnbWrongType, new object[] { assetName, obj2.GetType(), typeof(T) }));");
				}
				return (T)obj2;
			}
			T local = this.ReadAsset<T>(assetName, RecordDisposableObject);
			this.loadedAssets.Add(assetName, local);
			loadedAssetsInverse.Add(local, assetName);
			return local;
		}

		protected override Stream OpenStream(string assetName)
		{
			Stream stream;
			try
			{
				stream = new FileStream(GetCleanPath(Path.Combine(this.rootDirectory, assetName + ".xnb")), FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch (Exception exception)
			{
				if ((exception is FileNotFoundException) || (exception is DirectoryNotFoundException))
				{
					throw new Exception("throw new ContentLoadException(string.Format(CultureInfo.CurrentCulture, Resources.OpenStreamNotFound, new object[] { assetName }), exception);");
				}
				if (((exception is ArgumentException) || (exception is NotSupportedException)) || ((exception is IOException) || (exception is UnauthorizedAccessException)))
				{
					throw new Exception("throw new ContentLoadException(string.Format(CultureInfo.CurrentCulture, Resources.OpenStreamError, new object[] { assetName }), exception);");
				}
				throw;
			}
			return stream;
		}

		void RecordDisposableObject(IDisposable disposable)
		{
			this.disposableAssets.Add(disposable);
		}

		public override void Unload()
		{
			if (this.loadedAssets == null)
			{
				throw new ObjectDisposedException(this.ToString());
			}
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
				this.loadedAssets.Clear();
				this.loadedAssetsInverse.Clear();
				this.disposableAssets.Clear();
			}
		}

		public void Unload(IDisposable asset)
		{
			string key = loadedAssetsInverse[asset];
			loadedAssetsInverse.Remove(asset);
			loadedAssets.Remove(key);
			disposableAssets.Remove(asset);

			asset.Dispose();
		}

	}
}
