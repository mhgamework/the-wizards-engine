using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// This is a to-be abstract factory that creates ITexture's. It might just be a helper that allows easy texture sharing.
    /// Currently RAM implementation.
    /// TODO: write test
    /// TODO: Implement ITextureFactory
    /// </summary>
    public class RAMTextureFactory : ITextureFactory
    {
        private List<ITexture> textures = new List<ITexture>();
      

        public object GetAsset(Type type, Guid guid)
        {
            if (type != typeof(ITexture)) return null;

            return GetTexture(guid);
        }

        public ITexture GetTexture(Guid guid)
        {
            return textures.Find(t => t.Guid.Equals(guid));
        }

        public ITexture FindTexture(Predicate<ITexture> predicate)
        {
            return textures.Find(predicate);
        }

        public void AddTexture(ITexture texture)
        {
            textures.Add(texture);
        }
    }
}

