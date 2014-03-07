﻿using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Engine.Raycasting;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Selecting
{
    /// <summary>
    /// 
    /// </summary>
    public class BoundingBoxSelectableProvider : IWorldSelectableProvider
    {
        private Func<object, BoundingBox> getBoundingBox;
        private IEnumerable<object> items;

        public bool Enabled { get; set; }

        public object Targeted
        {
            get { return targeted; }
        }

        private object targeted;
        private Action<object> onClick;

        

        public bool IsTargeted(object obj)
        {
            return obj == Targeted;
        }
        public void SetTargeted(Selectable selectable)
        {
            if (selectable == null)
            {
                targeted = null;
                return;
            }
            targeted = selectable.Object;
        }

        public object GetTargeted()
        {
            return targeted;
        }

        public void Select(Selectable selectable)
        {
            //TODO: not sure this is correct, it should be selecting? instead of clicking
            onClick(selectable.Object);
        }

        public void Render()
        {
            if (!Enabled) return;
            foreach (var item in items)
            {
                var bb = getBoundingBox(item);

                var c = new Color4(0, 1, 0);

                if (IsTargeted(item))
                    c = new Color4(1, 0, 0);

                TW.Graphics.LineManager3D.AddBox(bb, c);
            }
        }

        private BoundingBoxSelectableProvider()
        {
            Enabled = true;
        }

        public IEnumerable<Selectable> GetSelectables()
        {
            if (!Enabled) yield break;

            foreach (var i in items)
            {
                object i1 = i;
                yield return new Selectable(ray => checkIntersects(i1, ray), i);
            }
        }

        private RaycastResult checkIntersects(object o, Ray ray)
        {
            var bb = getBoundingBox(o);
            var dist = ray.xna().Intersects(bb.xna());

            var res = new RaycastResult();
            res.Set(dist, o);

            return res;
        }

        public static BoundingBoxSelectableProvider Create<T>(IEnumerable<T> items, Func<T, BoundingBox> getBoundingBox, Action<T> onClick) where T : class
        {
            var ret = new BoundingBoxSelectableProvider();
            ret.getBoundingBox = o => getBoundingBox((T)o);
            ret.items = items;
            ret.onClick = o => onClick((T)o);

            return ret;
        }


    }
}