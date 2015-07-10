using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Common.Core;

using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// This currently uses a very simple caching algorithm. This class could therefore be made an abstract factory.
    /// </summary>
    public class TexturePool : IXNAObject
    {
        private Dictionary<ITexture, Texture2D> cache = new Dictionary<ITexture,Texture2D>();
        
        private IXNAGame game;

        public void Initialize(IXNAGame _game)
        {
            game = _game;
        }

        public void Render(IXNAGame _game)
        {
        }

        public void Update(IXNAGame _game)
        {
        }

        public Texture2D LoadTexture(ITexture tex)
        {

            Texture2D ret;
            if (!cache.TryGetValue(tex, out ret))
            {
                var data = tex.GetCoreData();
                var p =TextureCreationParameters.Default;
                
                switch (data.StorageType)
                {
                    case TextureCoreData.TextureStorageType.Disk:
                        ret = Texture2D.FromFile(game.GraphicsDevice, data.DiskFilePath,p);
                        break;
                    case TextureCoreData.TextureStorageType.Assembly:
                        ret = Texture2D.FromFile(game.GraphicsDevice,
                                                 EmbeddedFile.GetStream(data.Assembly, data.AssemblyResourceName, null),p);
                        break;

                }
                cache[tex] = ret;
            }
            return ret;
        }


    }
}
