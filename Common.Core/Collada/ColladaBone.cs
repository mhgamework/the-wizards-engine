using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient
{
    public class ColladaBone
    {
        /// <summary>
        /// Parent bone, very important to get all parent matrices when
        /// building the finalMatrix for rendering.
        /// </summary>
        public ColladaBone parent = null;

        /// <summary>
        /// Children bones, not really used anywhere except for the ShowBones
        /// helper method, but also useful for debugging.
        /// </summary>
        public List<ColladaBone> children = new List<ColladaBone>();

        /// <summary>
        /// Position, very useful to position bones to show bones in 3D, also
        /// only used for debugging and testing purposes.
        /// </summary>
        public Vector3 pos;

        /// <summary>
        /// Initial matrix we get from loading the collada model, it contains
        /// the start position and is used for the calculation to get the
        /// absolute and final matrices (see below).
        /// </summary>
        public Matrix initialMatrix;

        /// <summary>
        /// Bone number for the skinning process. This is just our internal
        /// number and children do always have higher numbers, this way going
        /// through the bones list is quicker and easier. The collada file
        /// might use a different order, see LoadAnimation for details.
        /// </summary>
        public int num;

        /// <summary>
        /// Id and name of this bone, makes debugging and testing easier, but
        /// it is also used to identify this bone later on in the loading process
        /// because our bone order might be different from the one in the file.
        /// </summary>
        public string id;

        public string sid;

        /// <summary>
        /// Animation matrices for the precalculated bone animations.
        /// These matrices must be set each frame (use time) in order
        /// for the animation to work.
        /// </summary>
        public List<Matrix> animationMatrices = new List<Matrix>();

        /// <summary>
        /// invBoneMatrix is a special helper matrix loaded directly from
        /// the collada file. It is used to transform the final matrix
        /// back to a relative format after transforming and rotating each
        /// bone with the current animation frame. This is very important
        /// because else we would always move and rotate vertices around the
        /// center, but thanks to this inverted skin matrix the correct
        /// rotation points are used.
        /// </summary>
        public Matrix invBoneSkinMatrix;

        /// <summary>
        /// Final absolute matrix, which is calculated in UpdateAnimation each
        /// frame after all the loading is done. It can directly be used to
        /// find out the current bone positions, but for rendering we have
        /// to apply the invBoneSkinMatrix first to transform all vertices into
        /// the local space.
        /// </summary>
        public Matrix finalMatrix;

        /// <summary>
        /// Create bone
        /// </summary>
        /// <param name="setMatrix">Set matrix</param>
        /// <param name="setParentBone">Set parent bone</param>
        /// <param name="setNum">Set number</param>
        /// <param name="setId">Set id name</param>
        public ColladaBone( Matrix setMatrix, ColladaBone setParentBone, int setNum,
                string setId , string nSid )
        {
            initialMatrix = setMatrix;
            pos = initialMatrix.Translation;
            parent = setParentBone;
            num = setNum;
            id = setId;
            sid = nSid;

            invBoneSkinMatrix = Matrix.Identity;
        } // Bone(setMatrix, setParentBone, setNum)

        /// <summary>
        /// Get matrix recursively
        /// </summary>
        /// <returns>Matrix</returns>
        public Matrix GetMatrixRecursively()
        {
            Matrix ret = initialMatrix;

            // If we have a parent mesh, we have to multiply the matrix with the
            // parent matrix.
            if ( parent != null )
                ret *= parent.GetMatrixRecursively();

            return ret;
        } // GetMatrixRecursively()

        /// <summary>
        /// To string, useful for debugging and testing.
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return "Bone: Id=" + id + ", Num=" + num + ", Position=" + pos;
        } // ToString()

    } // class Bone


}
