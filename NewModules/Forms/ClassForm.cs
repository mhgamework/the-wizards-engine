using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MHGameWork.TheWizards.Forms
{
    public class ClassForm<T>
    {
        private Window form;




        public ClassForm()
        {
            createForm();
        }

        private void createForm()
        {
            form = new Window();

            rootGridElement = createTypeGrid(new DataContextAttribute(this));
            form.Content = rootGridElement.Grid;




        }

        private GridElement createTypeGrid(IAttribute attribute)
        {

            var mainPanel = new Grid();
            mainPanel.ColumnDefinitions.Add(new ColumnDefinition());
            mainPanel.ColumnDefinitions.Add(new ColumnDefinition());
            var attributes = getAllAttributes(attribute.Type);


            var gridElement = new GridElement(mainPanel, attribute);

            var rowCount = 0;

            foreach (var att in attributes)
            {
                var el = createFormElement(att, mainPanel, rowCount);
                if (el == null) continue;

                gridElement.AddElement(el);

                mainPanel.RowDefinitions.Add(new RowDefinition());
                rowCount++;
            }

            return gridElement;
        }

        private List<IAttribute> getAllAttributes(Type type)
        {
            var ret = new List<IAttribute>();
            foreach (var fi in type.GetFields())
            {
                if (!fi.IsPublic) continue;

                ret.Add(new FieldAttribute(fi));
            }
            foreach (var fi in type.GetProperties())
            {
                if (!fi.CanRead || !fi.CanWrite || !fi.GetGetMethod().IsPublic || !fi.GetSetMethod().IsPublic) continue;

                ret.Add(new PropertyAttribute(fi));
            }
            return ret;
        }

        private IFormElement createFormElement(IAttribute att, Grid grid, int rowCount)
        {
            var fieldType = att.Type;

            if (fieldType == typeof(string))
                return createTextBoxElement(att, s => s, o => o.ToString(), grid, rowCount);

            if (fieldType == typeof(int))
                return createTextBoxElement(att, s => int.Parse(s), i => i.ToString(), grid, rowCount);

            if (fieldType == typeof(float))
                return createTextBoxElement(att, s => float.Parse(s), f => f.ToString(), grid, rowCount);

            if (fieldType == typeof(byte))
                return createTextBoxElement(att, s => byte.Parse(s), f => f.ToString(), grid, rowCount);

            if (fieldType.IsPrimitive)
                return null;

            if (fieldType.IsValueType)
            {
                var label = new Label();
                label.Content = att.Name;
                grid.Children.Add(label);
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, rowCount);

                var typeGridElement = createTypeGrid(att);
                grid.Children.Add(typeGridElement.Grid);
                Grid.SetColumn(typeGridElement.Grid, 1);
                Grid.SetRow(typeGridElement.Grid, rowCount);
                return typeGridElement;

            }

            return null;
        }

        private IFormElement createTextBoxElement<TU>(IAttribute att, Func<string, TU> fromString, Func<TU, string> toString, Grid grid, int rowCount)
        {
            IFormElement el;
            var label = new Label();
            label.Content = att.Name;
            grid.Children.Add(label);
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, rowCount);

            var box = new TextBox();
            grid.Children.Add(box);
            Grid.SetColumn(box, 1);
            Grid.SetRow(box, rowCount);


            el = new TextBoxElement<TU>(box, att, toString, fromString);
            return el;
        }


        private T dataContext;
        private GridElement rootGridElement;

        public T DataContext
        {
            get { return dataContext; }
            set
            {
                dataContext = value;
                ReadDataContext();
            }
        }


        public void Show()
        {
            form.Show();
        }

        public void WriteDataContext()
        {
            // This locks the GUI and main thread
            form.Dispatcher.Invoke(writeDataContextInternal);
        }
        public void ReadDataContext()
        {
            // This locks the GUI and main thread
            form.Dispatcher.Invoke(readDataContextInternal);
        }

        private void writeDataContextInternal()
        {
            rootGridElement.WriteDataContext(DataContext);
        }



        private void readDataContextInternal()
        {
            rootGridElement.ReadDataContext(DataContext);

        }


        private interface IAttribute
        {
            string Name { get; }
            Type Type { get; }
            object GetData(object obj);
            void SetData(object obj, object value);
        }

        private class FieldAttribute : IAttribute
        {
            public Type Type { get { return fi.FieldType; } }
            public string Name { get { return fi.Name; } }
            private readonly FieldInfo fi;

            public FieldAttribute(FieldInfo fi)
            {
                this.fi = fi;
            }


            public object GetData(object obj)
            {
                return fi.GetValue(obj);
            }

            public void SetData(object obj, object value)
            {
                fi.SetValue(obj, value);
            }
        }
        private class PropertyAttribute : IAttribute
        {
            private readonly PropertyInfo fi;
            public Type Type { get { return fi.PropertyType; } }
            public string Name { get { return fi.Name; } }

            public PropertyAttribute(PropertyInfo fi)
            {
                this.fi = fi;
            }


            public object GetData(object obj)
            {
                return fi.GetGetMethod().Invoke(obj, null);
            }

            public void SetData(object obj, object value)
            {
                fi.GetSetMethod().Invoke(obj, new[] { value });
            }

        }
        /// <summary>
        /// This simply forms a way to access the DataContext
        /// </summary>
        private class DataContextAttribute : IAttribute
        {
            private readonly ClassForm<T> form;

            public DataContextAttribute(ClassForm<T> form)
            {
                this.form = form;
            }

            public string Name
            {
                get { return "_DataContext"; }
            }

            public Type Type
            {
                get { return typeof(T); }
            }

            public object GetData(object obj)
            {
                return form.DataContext;
            }

            public void SetData(object obj, object value)
            {
                throw new InvalidOperationException();
            }
        }


        private interface IFormElement
        {
            void WriteDataContext(object context);
            void ReadDataContext(object context);


        }

        private class TextBoxElement<TU> : IFormElement
        {
            private readonly TextBox box;
            private readonly IAttribute attribute;
            private readonly Func<TU, string> toString;
            private readonly Func<string, TU> fromString;
            private bool Changed { get; set; }


            public TextBoxElement(TextBox box, IAttribute attribute, Func<TU, string> toString, Func<string, TU> fromString)
            {
                this.box = box;
                this.attribute = attribute;
                this.toString = toString;
                this.fromString = fromString;
                box.TextChanged += delegate { Changed = true; };

            }

            public void WriteDataContext(object context)
            {
                if (!Changed) return;
                Changed = false;

                attribute.SetData(context, fromString(box.Text));
            }

            public void ReadDataContext(object context)
            {
                if (box.IsFocused) return;
                box.Text = toString((TU)attribute.GetData(context));
            }
        }

        private class GridElement : IFormElement
        {
            private readonly Grid grid;
            private readonly IAttribute attribute;
            private List<IFormElement> elements = new List<IFormElement>();


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
}
