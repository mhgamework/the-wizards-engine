using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MHGameWork.TheWizards.Wpf;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Forms
{
    public class ClassForm<T> : IClassForm
    {
        private Window form;




        public ClassForm()
        {
            createForm(new SimpleWindowFactory());
        }

        private void createForm(IWindowFactory factory)
        {
            
            form = factory.CreateWindow();

            rootGridElement = createTypeGrid(new DataContextAttribute(this));

            var scroll = new ScrollViewer();
            scroll.Content = rootGridElement.Grid;
            form.Content = scroll;




        }

        private GridElement createTypeGrid(IAttribute attribute)
        {

            var mainPanel = new Grid();
            var col = new ColumnDefinition();
            col.Width = GridLength.Auto;
            mainPanel.ColumnDefinitions.Add(col);
            mainPanel.ColumnDefinitions.Add(new ColumnDefinition());
            var attributes = getAllAttributes(attribute.Type);

            mainPanel.VerticalAlignment = VerticalAlignment.Top;

            

            var gridElement = new GridElement(mainPanel, attribute);

            var rowCount = 0;

            foreach (var att in attributes)
            {
                var el = createFormElement(att);
                if (el == null) continue;

                var label = new Label();
                label.Content = att.Name;
                mainPanel.Children.Add(label);
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, rowCount);

                mainPanel.Children.Add(el.UIElement);
                Grid.SetColumn(el.UIElement, 1);
                Grid.SetRow(el.UIElement, rowCount);


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

        private IFormElement createFormElement(IAttribute att)
        {
            var fieldType = att.Type;

            if (fieldType == typeof(string))
                return createTextBoxElement(att, s => s, o => o.ToString());

            if (fieldType == typeof(int))
                return createTextBoxElement(att, s => int.Parse(s), i => i.ToString());

            if (fieldType == typeof(float))
                return createTextBoxElement(att, s => float.Parse(s), f => f.ToString());

            if (fieldType == typeof(byte))
                return createTextBoxElement(att, s => byte.Parse(s), f => f.ToString());

            if (fieldType.IsPrimitive)
                return createReadonlyElement(att);

            if (fieldType == typeof(Matrix))
                return createReadonlyElement(att);
            if (fieldType == typeof(Color))
                return new ColorElement(att);

            if (fieldType.IsValueType)
            {
                var typeGridElement = createTypeGrid(att);

                return typeGridElement;

            }

            return createReadonlyElement(att);

        }

        private IFormElement createReadonlyElement(IAttribute att)
        {
            return new ReadonlyElement(att);
        }

        private IFormElement createTextBoxElement<TU>(IAttribute att, Func<string, TU> fromString, Func<TU, string> toString)
        {
            IFormElement el;

            var box = new TextBox();

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
    }

    public interface IClassForm
    {
        void WriteDataContext();
        void ReadDataContext();
    }
}
