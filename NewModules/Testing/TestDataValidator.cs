using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Reflection;

namespace MHGameWork.TheWizards.Testing
{
    public class TestDataValidator
    {
        public void Validate(IEnumerable array)
        {
            foreach (var el in array)
            {
                if (el == null)
                {
                    write("[[[NULL]]]");
                    continue;
                }
                write(el.ToString());
            }
        }
        public void Validate(Object obj)
        {
            write(obj.ToString());
        }

        public void ValidateFields(Object obj)
        {
            write(obj.GetType().Name);
            foreach (var fieldInfo in obj.GetType().GetFields())
            {
                write(fieldInfo.Name);
                Validate((dynamic)fieldInfo.GetValue(obj));
            }
        }

        private void write(String str)
        {
            Console.WriteLine(str);
        }
    }
}
