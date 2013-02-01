using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    public class PathfinderVisualizer<T> where T : class
    {
        private readonly DX11Game game;
        private readonly Action<LineManager3DLines, T,Color4> renderFunction;
        private LineManager3DLines lines;

        private HashSet<T> selected = new HashSet<T>();
        private HashSet<T> enqueued = new HashSet<T>();

        public PathfinderVisualizer(DX11Game game, PathFinder2D<T> finder, Action<LineManager3DLines, T,Color4> renderFunction)
        {
            this.game = game;
            this.renderFunction = renderFunction;
            finder.Begin += finder_Begin;
            finder.SelectNode += finder_SelectNode;
            finder.EnqueueNode += finder_EnqueueNode;

            lines = new LineManager3DLines(game.Device);
            lines.SetMaxLines(1024 * 64);
        }

        void finder_EnqueueNode(T obj)
        {
            enqueued.Add(obj);
            //renderFunction(lines, obj, new Color4(0, 1, 1));

        }

        void finder_SelectNode(T obj)
        {
            enqueued.Remove(obj);
            selected.Add(obj);
            //renderFunction(lines, obj, new Color4(0,0,1));
        }

        void finder_Begin()
        {
            selected.Clear();
            enqueued.Clear();
            lines.ClearAllLines();
        }

        public void Render()
        {
            //TODO: cache? 
            foreach (var e in enqueued) renderFunction(lines, e, new Color4(0, 1, 1));

            foreach (var e in selected)
            {
                renderFunction(lines, e, new Color4(0, 0, 1));
            }
            game.LineManager3D.Render(lines, game.Camera);
        }
    }
}
