using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Castle.DynamicProxy;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Persistence.POSystem;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX;
using MHGameWork.TheWizards.Scattered._Engine;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Internal.Model
{
    /// <summary>
    /// World starts at 0,0 and grows in positive direction
    /// Responsible for
    ///     - GameVoxel data structure, defining coords and neighbours
    ///     - World to voxel coords
    ///     - Storing a list of changes to gamevoxels
    /// 
    /// Rethink of responsibilities:
    ///     Responsible for lifecycle and manipulation of 'voxels'
    ///     A voxel refers is a 'unit of space', this space can contain things
    ///     The world is responsible for manipulating the state of this voxel unit of space
    /// </summary>
    [PersistedObject]
    public class World :IPOEventsReceiver
    {
        private readonly Func<World, Point2, GameVoxel> createVoxel;
        private readonly ProxyGenerator generator;
        public Vector2 VoxelSize { get; private set; }
        private Array2D<GameVoxel> voxels;
        public int WorldSize { get { return voxels.Size.X; } }



        public World(Func<World, Point2, GameVoxel> createVoxel, ProxyGenerator generator)
        {
            this.createVoxel = createVoxel;
            this.generator = generator;
        }

        public void Initialize(int size, float voxelSize)
        {
            VoxelSize = new Vector2(voxelSize);
            voxels = new Array2D<GameVoxel>(new Point2(size, size));
            voxels.ForEach((v, p) => voxels[p] = createVoxel(this, p));
        }

        public GameVoxel GetVoxelAtGroundPos(Vector3 groundPos)
        {
            var index = Vector2.Modulate(groundPos.TakeXZ(), new Vector2(1 / VoxelSize.X, 1 / VoxelSize.Y));
            return voxels[index.Floor()];
        }

        public void ForEach(Action<GameVoxel, Point2> func)
        {
            voxels.ForEach(func);
        }

        public BoundingBox GetBoundingBox(Point2 gameVoxel)
        {
            var botLeft = Vector2.Modulate(gameVoxel, VoxelSize).ToXZ(0);
            botLeft.Y += GetVoxel(gameVoxel).Data.Height;
            return new BoundingBox(botLeft, botLeft + VoxelSize.ToXZ(0.1f));
        }

        public GameVoxel GetVoxel(Point2 p)
        {
            return voxels[p];
        }

        public IEnumerable<GameVoxel> Get8Connected(Point2 coord)
        {
            return GetVoxel(coord).Get8Connected().Cast<GameVoxel>();
            //return voxels.Get8Connected(coord);
        }


        public IEnumerable<GameVoxel> GetRange(GameVoxel center, int radius)
        {
            for (int x = center.Coord.X - radius; x <= center.Coord.X + radius; x++)
                for (int y = center.Coord.Y - radius; y <= center.Coord.Y + radius; y++)
                {
                    var v = voxels[new Point2(x, y)];

                    if (v != null)
                        yield return v;
                }
        }


        private HashSet<GameVoxel> changedVoxels = new HashSet<GameVoxel>();
        /// <summary>
        /// TODO: WARNING: this currently only works for type changes
        /// TODO: remove this
        /// </summary>
        public IEnumerable<GameVoxel> ChangedVoxels
        {
            get { return changedVoxels; }
        }

        private Subject<IVoxel> voxelChanged = new Subject<IVoxel>();
        public IObservable<IVoxel> VoxelChanged { get { return voxelChanged.AsObservable(); } }
        /// <summary>
        /// Idea: replace this with a 'Wake/Sleep mechanism?'
        /// </summary>
        public void NotifyVoxelChanged(IVoxel v)
        {
            changedVoxels.Add((GameVoxel)v);
            voxelChanged.OnNext(v);
        }

        public void ClearChangedFlags()
        {
            changedVoxels.Clear();
        }

        /// <summary>
        /// This method changes the type of the given voxel, changing its contents
        /// </summary>
        public void ChangeType(IVoxel currentVoxel, IGameVoxelType type)
        {
            DestroyVoxelContents(currentVoxel);
            CreateVoxelContents(currentVoxel, type);

        }

        public void DestroyVoxelContents(IVoxel voxel)
        {
            if (voxel.Data != null && voxel.Data.Type != null)
                voxel.Data.Type.OnDestroyed(voxel);

            //Destroy contents, but height is not considered contents so keep that

            //note: put gameplay-related changes here
            var prevHeight = 0f;
            if (voxel.Data != null) prevHeight = voxel.Data.Height;
            ((GameVoxel)voxel).Data = new ObservableVoxelData(() =>
            {
                if (voxel.Data == null) return; // Ignore changes in the observablevoxeldata constructor
                NotifyVoxelChanged(voxel);
            }, generator);
            voxel.Data.Height = prevHeight;
        }

        public void CreateVoxelContents(IVoxel voxel, IGameVoxelType type)
        {
            voxel.Data.Type = type;

            type.OnCreated(voxel); // Maybe register this too as an event
            created.OnNext(voxel);

        }

        private Subject<IVoxel> created = new Subject<IVoxel>();
        /// <summary>
        /// Observable for all voxels for which contents were created
        /// </summary>
        public IObservable<IVoxel> Created { get { return created.AsObservable(); } }

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {

        }
    }
}