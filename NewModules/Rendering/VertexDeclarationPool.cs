using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Rendering
{
    public class VertexDeclarationPool : IXNAObject
    {
        private Dictionary<Type, Item> dict = new Dictionary<Type, Item>();
        private IXNAGame game;

        public VertexDeclaration GetVertexDeclaration<T>()
        {
            var item = getItem<T>();
            if (item.VertexElements == null)
                throw new InvalidOperationException("No VertexElements have been given for given Type (T)!");

            if (item.VertexDeclaration == null)
            {
                item.VertexDeclaration = new VertexDeclaration(game.GraphicsDevice, item.VertexElements);
            }

            return item.VertexDeclaration;
        }

        private Item getItem<T>()
        {
            Item ret;
            if (dict.TryGetValue(typeof(T), out ret))
                return ret;

            ret = new Item();
            dict[typeof(T)] = ret;

            return ret;
        }


        public void SetVertexElements<T>(VertexElement[] elements)
        {
            var item = getItem<T>();
            item.VertexElements = elements;
        }

        private class Item
        {
            public VertexDeclaration VertexDeclaration;
            public VertexElement[] VertexElements;
        }

        public void Initialize(IXNAGame _game)
        {
            game = _game;
        }

        public void Render(IXNAGame _game)
        {
        }

        public void Update(IXNAGame _game)
        {
        }
    }
}
