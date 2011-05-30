using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MHGameWork.TheWizards.Forms
{
    public class ClassForm<T>
    {
        private Window form;

        private List<IAttribute> attributes = new List<IAttribute>();



        public ClassForm()
        {
            createForm();
        }

        private void createForm()
        {
            form = new Window();
            var mainPanel = new Grid();
            mainPanel.ColumnDefinitions.Add(new ColumnDefinition());
            mainPanel.ColumnDefinitions.Add(new ColumnDefinition());
            form.Content = mainPanel;


            int rowCount = 0;
            foreach (var fi in typeof(T).GetFields())
            {
                if (!fi.IsPublic) return;


                var fieldType = fi.FieldType;
                var content = fi.Name;

                IFormElement el;


                el = createFormElement(mainPanel, fieldType, content, rowCount);
                if (el == null) continue;
                attributes.Add(new FieldAttribute(fi, el));



                mainPanel.RowDefinitions.Add(new RowDefinition());
                rowCount++;
            }
            foreach (var fi in typeof(T).GetProperties())
            {
                if (!fi.CanRead || !fi.CanWrite || !fi.GetGetMethod().IsPublic || !fi.GetSetMethod().IsPublic) continue;

                mainPanel.RowDefinitions.Add(new RowDefinition());

                var fieldType = fi.PropertyType;
                var content = fi.Name;

                IFormElement el;


                el = createFormElement(mainPanel, fieldType, content, rowCount);
                if (el == null) continue;

                attributes.Add(new PropertyAttribute(fi, el));

                rowCount++;
            }



        }

        private IFormElement createFormElement(Grid mainPanel, Type fieldType, string content, int rowCount)
        {
            IFormElement el = null;
            if (fieldType == typeof(string) || fieldType == typeof(int) || fieldType == typeof(float))
            {
                var label = new Label();
                label.Content = content;
                mainPanel.Children.Add(label);
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, rowCount);

                var box = new TextBox();
                mainPanel.Children.Add(box);
                Grid.SetColumn(box, 1);
                Grid.SetRow(box, rowCount);

                el = new TextBoxElement(box);

            }
            return el;
        }


        private T dataContext;
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
            for (int i = 0; i < attributes.Count; i++)
            {
                var att = attributes[i];
                if (!att.Element.Changed) continue;

                att.Element.Changed = false;


                object value = null;
                bool problem = true;
                try
                {
                    value = writeElement(att.Type, att.Element);
                    problem = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Attribute write problem!");
                }
                if (!problem)
                    att.SetData(DataContext, value);
            }


        }
        public void ReadDataContext()
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                var att = attributes[i];
                readElement(att.Type, att.Element, att.GetData(DataContext));

            }
        }

        private void readElement(Type fieldType, IFormElement el, object value)
        {
            if (fieldType == typeof(string) || fieldType == typeof(int) || fieldType == typeof(float))
            {
                el.SetData(value.ToString());


            }
        }
        private object writeElement(Type fieldType, IFormElement el)
        {
            if (fieldType == typeof(string))
                return el.GetData().ToString();
            if (fieldType == typeof(int))
                return int.Parse(el.GetData().ToString());
            if (fieldType == typeof(float))
                return float.Parse(el.GetData().ToString());

            return null;
        }

        private interface IAttribute
        {
            Type Type { get; }
            object GetData(T obj);
            void SetData(T obj, object value);
            IFormElement Element { get; }
        }

        private class FieldAttribute : IAttribute
        {
            public IFormElement Element { get; private set; }
            public Type Type { get { return fi.FieldType; } }
            private readonly FieldInfo fi;

            public FieldAttribute(FieldInfo fi, IFormElement element)
            {
                Element = element;
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
            public IFormElement Element { get; private set; }

            public PropertyAttribute(PropertyInfo fi, IFormElement element)
            {
                this.fi = fi;
                Element = element;
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
            void SetData(object o);
            object GetData();

            bool Changed { get; set; }

        }

        public class TextBoxElement : IFormElement
        {
            private readonly TextBox box;
            public bool Changed { get; set; }


            public TextBoxElement(TextBox box)
            {
                this.box = box;
                box.TextChanged += delegate { Changed = true; };
            }

            public void SetData(object o)
            {
                if (box.IsFocused) return;
                box.Text = (string)o;
            }

            public object GetData()
            {
                return box.Text;
            }
        }



    }
}
