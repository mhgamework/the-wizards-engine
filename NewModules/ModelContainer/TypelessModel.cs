using System;
using System.Collections.Generic;
using System.Reflection;
using MHGameWork.TheWizards.Reflection;
using System.Linq;

namespace MHGameWork.TheWizards.ModelContainer
{
    public class TypelessModel
    {
        private List<TypelessObject> objects = new List<TypelessObject>();

        public void UpdateFromModel(ModelContainer model)
        {
            var array = new List<IModelObject>();
            foreach (var el in model.Objects)
                array.Add((IModelObject)el);

            foreach (var obj in array)
            {
                if (obj is TypelessObject)
                {
                    objects.Add((TypelessObject)obj);
                    continue;
                }
                var t = new TypelessObject();
                t.Type = obj.GetType().FullName;
                foreach (var att in getAllPersistentAttributes(obj.GetType()))
                {
                    t.SetData(att.Name, att.GetData(obj));
                }

                objects.Add(t);

            }
        }

        public void AddToModel(ModelContainer model, Assembly assembly)
        {
            foreach (var t in objects)
            {
                IModelObject obj;
                try
                {
                    obj = (IModelObject)assembly.CreateInstance(t.Type);
                    foreach (var att in getAllPersistentAttributes(obj.GetType()))
                    {
                        if (t.HasData(att.Name))
                            att.SetData(obj, t.GetData(att.Name));
                    }



                }
                catch (Exception ex)
                {
                    model.AddObject(t);
                }

            }
        }

        private IEnumerable<IAttribute> getAllPersistentAttributes(Type t)
        {
            return ReflectionHelper.GetAllAttributes(t);
        }

        private class TypelessObject : BaseModelObject
        {
            public string Type;

            private Dictionary<string, object> map = new Dictionary<string, object>();
            public object GetData(string name)
            {
                return map[name];
            }
            public void SetData(string name, object obj)
            {
                map[name] = obj;
            }

            public bool HasData(string name)
            {
                return map.ContainsKey(name);
            }
        }
    }
}