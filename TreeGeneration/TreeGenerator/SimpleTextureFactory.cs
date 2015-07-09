using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;

namespace TreeGenerator
{
    public class SimpleTextureFactory:ITextureFactory
    {
        private Dictionary<Guid, ITexture> map = new Dictionary<Guid, ITexture>();
        private List<ITexture> textures = new List<ITexture>();
        public ITexture GetTexture(Guid guid)
        {
            return map[guid];
        }

        public ITexture FindTexture(Predicate<ITexture> predicate)
        {
            return textures.Find(predicate);
        }

        public void AddTexture(ITexture texture)
        {
            AddTexture(texture.Guid, texture);
        }


        public void AddTexture(Guid guid, ITexture texture)
        {
            map.Add(guid, texture);
            textures.Add(texture);
        }

        public object GetAsset(Type type, Guid guid)
        {
            if (type != typeof(ITexture)) return null;
            return GetTexture(guid);
        }


    }
}