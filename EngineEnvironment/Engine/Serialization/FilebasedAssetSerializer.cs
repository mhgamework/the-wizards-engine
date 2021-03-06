﻿using System;
using System.IO;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Serialization;
using MHGameWork.TheWizards.WorldRendering;

namespace MHGameWork.TheWizards.Persistence
{
    /// <summary>
    /// Responsible for string-serializing Assets using their loaded filenames
    /// </summary>
    public class FilebasedAssetSerializer : IConditionalSerializer
    {

        public FilebasedAssetSerializer()
        {
        }

        public bool CanOperate(Type type)
        {
            return typeof(IAsset).IsAssignableFrom(type);
        }

        public string Serialize(object obj, Type type, StringSerializer stringSerializer)
        {
            if (typeof(IMesh).IsAssignableFrom(type))
            {
                var mesh = (IMesh)obj;
                var ret =  TW.Assets.GetLoadedPath(mesh);
                if (ret != null) return ret;
            }
            if (typeof (RAMTexture).IsAssignableFrom(type))
            {
                var tex = (RAMTexture) obj;
                return tex.GetCoreData().DiskFilePath.Replace(TWDir.GameData.FullName + "\\","");
            }
            return StringSerializer.Unknown;
        }

        public object Deserialize(string value, Type type, StringSerializer stringSerializer)
        {
            if (value == StringSerializer.Unknown) return null;
            if (typeof(IMesh).IsAssignableFrom(type))
            {
                return TW.Assets.LoadMesh(value);
            }
            if (typeof (ITexture).IsAssignableFrom(type))
            {
                return TW.Assets.LoadTexture(value);
            }
            return null;
        }
    }
}