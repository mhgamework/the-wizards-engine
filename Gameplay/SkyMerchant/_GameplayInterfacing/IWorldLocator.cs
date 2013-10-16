using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing
{
    public interface IWorldLocator
    {
        IEnumerable<IPositionComponent> AtPosition(Vector3 point, float radius);
        IEnumerable<IPositionComponent> Raycast(Ray ray);
        IEnumerable<T> AtObject<T>(IPositionComponent ph, float radius) where T : IGameObjectComponent;
    }

}