using System.Reflection;

namespace MHGameWork.TheWizards.GodGame._Engine
{
    public static class InternalReflectionExtensions
    {
        public static T GetInternalField<T>(this object obj, string name)
        {
            return (T)obj.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(obj);
        }

        public static T CallInternalMethod<T>(this object obj, string method, object[] parameters)
        {
            return (T)obj.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Invoke(obj, parameters);
        }
        public static T CallInternalMethod<T>(this object obj, string method)
        {
            return obj.CallInternalMethod<T>(method, new object[] { });
        }
        public static void CallInternalMethodVoid(this object obj, string method, object[] parameters)
        {
            obj.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Invoke(obj, parameters);
        }
        public static void CallInternalMethodVoid(this object obj, string method)
        {
            obj.CallInternalMethodVoid(method, new object[] { });
        }
    }
}