using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Common.Core;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using MHGameWork.TheWizards.Rendering;
using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred
{
    /// <summary>
    /// This currently uses a very simple caching algorithm. This class could therefore be made an abstract factory.
    /// </summary>
    public class TexturePool
    {
        private readonly DX11Game game;
        private Dictionary<ITexture, TextureInfo> cache = new Dictionary<ITexture, TextureInfo>();

        public TexturePool(DX11Game game)
        {
            this.game = game;
        }


        public ShaderResourceView LoadTexture(ITexture tex)
        {
            TextureInfo ret;
            if (!cache.TryGetValue(tex, out ret))
            {
                var data = tex.GetCoreData();
                //var p =TextureCreationParameters.Default;

                Texture2D tx;

                switch (data.StorageType)
                {
                    case TextureCoreData.TextureStorageType.Disk:
                        //ret = Texture2D.FromFile(game.GraphicsDevice, data.DiskFilePath,p);
                        tx = Texture2D.FromFile(game.Device, data.DiskFilePath);
                        break;
                    case TextureCoreData.TextureStorageType.Assembly:
                        //ret = Texture2D.FromFile(game.GraphicsDevice,
                        //                         EmbeddedFile.GetStream(data.Assembly, data.AssemblyResourceName, null),p);
                        using (var strm = EmbeddedFile.GetStream(data.Assembly, data.AssemblyResourceName, null))
                            tx = Texture2D.FromStream(game.Device, strm, (int)strm.Length);

                        break;
                    default:
                        throw new InvalidOperationException();

                }

                ret = new TextureInfo
                          {
                              Texture2D = tx,
                              ResourceView = new ShaderResourceView(game.Device, tx)
                          };

                cache[tex] = ret;
            }
            return ret.ResourceView;
        }

        private class TextureInfo
        {
            public Texture2D Texture2D;
            public ShaderResourceView ResourceView;
        }


    }
}
