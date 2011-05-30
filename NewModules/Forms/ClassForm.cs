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

        private List<IFormElement> elements = new List<IFormElement>();



        public ClassForm()
        {
            createForm();
        }

        private void createForm()
        {
            form = new Window();
            mainPanel = new Grid();
            mainPanel.ColumnDefinitions.Add(new ColumnDefinition());
            mainPanel.ColumnDefinitions.Add(new ColumnDefinition());
            form.Content = mainPanel;

            var attributes = getAllAttributes();

            rowCount = 0;
            foreach (var att in attributes)
            {
                var el = createFormElement(att);
                if (el == null) continue;
                elements.Add(el);

                mainPanel.RowDefinitions.Add(new RowDefinition());
                rowCount++;
            }


        }

        private List<IAttribute> getAllAttributes()
        {
            var ret = new List<IAttribute>();
            foreach (var fi in typeof(T).GetFields())
            {
                if (!fi.IsPublic) continue;

                ret.Add(new FieldAttribute(fi));
            }
            foreach (var fi in typeof(T).GetProperties())
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

            return null;
        }

        private IFormElement createTextBoxElement<TU>(IAttribute att, Func<string, TU> fromString, Func<TU, string> toString)
        {
            IFormElement el;
            var label = new Label();
            label.Content = att.Name;
            mainPanel.Children.Add(label);
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, rowCount);

            var box = new TextBox();
            mainPanel.Children.Add(box);
            Grid.SetColumn(box, 1);
            Grid.SetRow(box, rowCount);


            el = new TextBoxElement<TU>(box, att, toString, fromString);
            return el;
        }


        private T dataContext;
        private int rowCount;
        private Grid mainPanel;

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
            for (int i = 0; i < elements.Count; i++)
            {
                try
                {
                    elements[i].WriteDataContext(DataContext);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Write Context problem!");
                }
            }
        }

      

        private void readDataContextInternal()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].ReadDataContext(DataContext);

            }
        }


        private interface IAttribute
        {
            string Name { get; }
            Type Type { get; }
            object GetData(T obj);
            void SetData(T obj, object value);
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


            public object GetData(T obj)
            {
                return fi.GetValue(obj);
            }

            public void SetData(T obj, object value)
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


            public object GetData(T obj)
            {
                return fi.GetGetMethod().Invoke(obj, null);
            }

            public void SetData(T obj, object value)
            {
                fi.GetSetMethod().Invoke(obj, new[] { value });
            }

        }

        private interface IFormElement
        {
            void WriteDataContext(T context);
            void ReadDataContext(T context);


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

            public void WriteDataContext(T context)
            {
                if (!Changed) return;
                Changed = false;

                attribute.SetData(context, fromString(box.Text));
            }

            public void ReadDataContext(T context)
            {
                if (box.IsFocused) return;
                box.Text = toString((TU)attribute.GetData(context));
            }
        }



    }
}
