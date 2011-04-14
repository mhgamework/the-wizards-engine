using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Assets
{
    /// <summary>
    /// This is currently a Client interface + it is not used by code, only for a generic interface for clarity's sake
    /// </summary>
    public class ClientAsset
    {
        public ClientAssetSyncer Syncer { get; set; }
        public Guid GUID { get; private set; }

        private List<ClientAssetFile> files = new List<ClientAssetFile>();

        public ClientAsset(ClientAssetSyncer syncer, Guid guid)
        {
            Syncer = syncer;
            GUID = guid;
        }

        private bool available;
        /// <summary>
        /// Returns whether this asset is available on the client. All data is up to date.
        /// </summary>
        /// <returns></returns>
        public bool IsAvailable
        {
            get { return available; }
        }

        internal void setAvailable(bool value)
        {
            available = value;
        }

        internal bool IsComponentsUpToDate;

        /// <summary>
        /// Blocks current thread and forces synchronization of this asset
        /// </summary>
        public void Synchronize()
        {
            Syncer.ScheduleForSync(this);
            Syncer.WaitForSynchronized(this);

        }
        /// <summary>
        /// Changes the synchronization priority
        /// </summary>
        public void ChangePriority(AssetSynchronizationPriority priority)
        {
            Syncer.ScheduleForSync(this);

        }

        public ClientAssetFile GetFileComponent(int index)
        {
            return files[index];

        }

        public int FileComponentCount { get { return files.Count; } }

        internal void applyAssetContentPacket(AssetContentPacket p)
        {
            files.Clear();
            for (int i = 0; i < p.Files.Length; i++)
            {
                var file = new ClientAssetFile(this, p.Files[i].Path, p.Files[i].Mode);
                file.ServerHash = p.Files[i].Hash;

                files.Add(file);
            }

            IsComponentsUpToDate = true;

        }

    }
}
