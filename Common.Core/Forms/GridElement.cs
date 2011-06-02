using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MHGameWork.TheWizards.Forms
{
    public class GridElement : IFormElement
    {
        private readonly Grid grid;
        private readonly IAttribute attribute;
        private List<IFormElement> elements = new List<IFormElement>();
        public UIElement UIElement { get { return grid; } }


        public GridElement(Grid grid, IAttribute attribute)
        {
            this.grid = grid;
            this.attribute = attribute;
        }

        public Grid Grid
        {
            get { return grid; }
        }

        public void AddElement(IFormElement el)
        {
            elements.Add(el);
        }

        public void WriteDataContext(object context)
        {
            object value = attribute.GetData(context);

            for (int i = 0; i < elements.Count; i++)
            {
                try
                {
                    elements[i].WriteDataContext(value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Write Context problem!");
                }
            }

            if (attribute.Type.IsValueType)
                attribute.SetData(context, value);

        }

        public void ReadDataContext(object context)
        {
            object value = attribute.GetData(context);

            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].ReadDataContext(value);

            }
        }
    }
}