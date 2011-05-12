using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;

namespace TreeGenerator
{
    public class SimpleTextureFactory:ITextureFactory
    {
        private Dictionary<Guid, ITexture> textures = new Dictionary<Guid, ITexture>();
        public ITexture GetTexture(Guid guid)
        {
            return textures[guid];
        }


        public void AddTexture(Guid guid, ITexture texture)
        {
            textures.Add(guid, texture);
        }

        public object GetAsset(Type type, Guid guid)
        {
            throw new NotImplementedException();
        }
    }
}
