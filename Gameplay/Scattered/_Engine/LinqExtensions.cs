using System;

namespace MHGameWork.TheWizards.Scattered._Engine
{
    public static class LinqExtensions
    {
        public static T Alter<T>(this T obj, Action<T> act)
        {
            act(obj);
            return obj;
        }
    }
}