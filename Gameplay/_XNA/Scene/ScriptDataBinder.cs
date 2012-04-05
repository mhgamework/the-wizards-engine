using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Scripting.API;

namespace MHGameWork.TheWizards.Scene
{
    public class ScriptDataBinder
    {
        public void LoadData(IScript script, EntityData entityData)
        {
            var type = script.GetType();
            foreach (var fieldInfo in type.GetFields())
            {
                if (!fieldInfo.IsPublic) return;



                if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(IDataElement<>))
                {
                    // We need to bind a DataElement to the script, just plug it in

                    var el = entityData.GetDataElement(fieldInfo.Name, fieldInfo.FieldType.GetGenericArguments()[0]);

                    fieldInfo.SetValue(script, el);

                }
                else
                {
                    // We need to bind the actual data

                    var el = entityData.GetDataElement(fieldInfo.Name, fieldInfo.FieldType);

                    var dataElementInterface =
                         el.GetType().GetInterfaces().SingleOrDefault(
                             x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDataElement<>));

                    var data = dataElementInterface.GetMethod("Get").Invoke(el, null);

                    fieldInfo.SetValue(script, data);

                    // Add locking of the loaded data here
                }

            }
        }

        public void SaveData(IScript script, EntityData entityData)
        {
            var type = script.GetType();
            foreach (var fieldInfo in type.GetFields())
            {
                if (!fieldInfo.IsPublic) return;



                if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(IDataElement<>))
                {
                    // A DataElement was bound, no need to save this, because this is a direct link
                    continue;
                }

                // We have bound the actual data, save it to the entityData

                var el = entityData.GetDataElement(fieldInfo.Name, fieldInfo.FieldType);

                var dataElementInterface =
                     el.GetType().GetInterfaces().SingleOrDefault(
                         x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDataElement<>));

                dataElementInterface.GetMethod("Set").Invoke(el, new[] { fieldInfo.GetValue(script) });

                // Add unlocking of the loaded data here

            }


        }
    }
}
