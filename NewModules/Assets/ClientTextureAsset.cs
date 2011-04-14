using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.Assets
{
    public class ClientTextureAsset : ITexture
    {
        private const int TextureFileIndex = 0;

        public ClientAsset ClientAsset { get; private set; }


        public ClientTextureAsset(ClientAsset clientAsset)
        {
            ClientAsset = clientAsset;
        }

        public Guid Guid
        {
            get { return ClientAsset.GUID; }
        }

        public TextureCoreData GetCoreData()
        {
            if (!ClientAsset.IsAvailable) throw new InvalidOperationException();

            var file = ClientAsset.GetFileComponent(TextureFileIndex);

            var coreData = new TextureCoreData();
            coreData.DiskFilePath = file.GetFullPath();
            coreData.StorageType = TextureCoreData.TextureStorageType.Disk;

            return coreData;
        }
    }
}
