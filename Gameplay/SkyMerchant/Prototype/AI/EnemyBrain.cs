using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.AI
{
    /// <summary>
    /// This is the belief base for the enemy ai agent (and knowledge?)
    /// DO NOT FORGET TO SET THE PERCEPTS!
    /// </summary>
    public class EnemyBrain
    {

        #region Knowledge
        
        public int LookDistance { get; set; }


        public int ShootDistance { get; set; }
        public float ShootInterval { get; set; }
        public float GunDamage { get; set; }

        #endregion

        #region Percepts
        public Vector3 Position { get; set; }
        #endregion

        #region Beliefs

        public RobotPlayerPart TargetPlayer { get; set; }

        
        public Vector3 Destination { get; set; }

        public ItemPart TargetItem { get; set; }

        #endregion


        /// <summary>
        /// TODO: use DI and access this data autonomously using an injected interface
        /// </summary>
        /// <param name="position"></param>
        public void UpdatePercepts(Vector3 position)
        {
            Position = position;
        }
        
        
    }
}