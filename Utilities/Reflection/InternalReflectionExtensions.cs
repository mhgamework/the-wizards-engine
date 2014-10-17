using System.Reflection;

namespace MHGameWork.TheWizards.GodGame._Engine
{
    public static class InternalReflectionExtensions
    {
        public static T GetInternalField<T>(this object obj, string name)
        {
            return (T)obj.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(obj);
        }

        public static T CallInternalMethod<T>(this object obj, string method)
        {
            return (T)obj.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Invoke(obj, new object[] { });
        }
    }
}