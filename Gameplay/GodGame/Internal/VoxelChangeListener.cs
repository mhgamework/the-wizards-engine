using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using MHGameWork.TheWizards.GodGame.Internal.Model;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    /// <summary>
    /// Responsible for providing listeners to certain types of voxel change events
    ///     - block updates (surrounding block changes data)
    ///     - any change in voxel data
    /// </summary>
    public class VoxelChangeListener
    {
        private Dictionary<GameVoxel, Action<GameVoxel>> changeActions = new Dictionary<GameVoxel, Action<GameVoxel>>();
        private Subject<GameVoxel> changedVoxels;

        public VoxelChangeListener()
        {
            changedVoxels = new Subject<GameVoxel>();
        }

        public void RegisterAdjacentChange(GameVoxel target, Action<GameVoxel> func)
        {
            if (changeActions.ContainsKey(target))
                throw new InvalidOperationException("Can only register one handler");
            changeActions.Add(target, func);
        }

        public void UnRegisterAdjacentChange(GameVoxel target)
        {
            changeActions.Remove(target);
        }


        public IObservable<GameVoxel> ChangedVoxels
        {
            get { return changedVoxels; }
        }

        public void ProcessChanges(Internal.Model.World world)
        {
            world.ChangedVoxels.ForEach(v =>
                {
                    changedVoxels.OnNext(v);

                    world.Get8Connected(v.Coord).ForEach(neighbour =>
                        {
                            if (changeActions.ContainsKey(neighbour))
                                changeActions[neighbour](v);
                        });

                });
        }
    }
}