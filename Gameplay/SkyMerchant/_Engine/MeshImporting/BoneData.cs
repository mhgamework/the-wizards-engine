﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.MeshImporting
{
    /// <summary>
    /// Helper class
    /// </summary>
    public class BoneData
    {
        public String Name;
        public String ParentName;
        public float Length;
        public Vector3 ZeroTranslation; //todo: set in importer!!
        public Quaternion ZeroRotation; //todo: set in importer!!
        public Vector3 ZeroScale; //todo: set in importer!!
    }
}
