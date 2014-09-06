using System;
using System.Diagnostics;
using MHGameWork.TheWizards.Profiling;
using PostSharp;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Reflection;

namespace MHGameWork.TheWizards.Data
{
    /// <summary>
    /// Applying this attribute to a method will create a profiling point around the entire method.
    /// </summary>
    [MulticastAttributeUsage(MulticastTargets.Method)]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    [Serializable]
    public sealed class TWProfileAttribute : OnMethodBoundaryAspect
    {
        private readonly NameType nameType;
        private string name;

        private ProfilingPoint el;

        public TWProfileAttribute()
            : this(NameType.Class | NameType.Method)
        {

        }
        public TWProfileAttribute(NameType nameType)
        {
            this.nameType = nameType;
            name = "";
        }

        public TWProfileAttribute(string name)
        {
            this.name = name;
        }

        [Flags]
        public enum NameType
        {
            None,
            Custom = 1,
            Class = 2,
            Method = 4
        }
        public override void RuntimeInitialize(System.Reflection.MethodBase method)
        {
            if ((nameType & NameType.Class) != NameType.None)
                name = method.DeclaringType.Name;
            if ((nameType & NameType.Method) != NameType.None)
            {
                if (name != "")
                    name += ".";
                name += method.Name;
            }

            el = Profiler.CreateElement(name);
            base.RuntimeInitialize(method);
        }

        public override bool CompileTimeValidate(System.Reflection.MethodBase method)
        {
            // type.Namespace crashes the compiler!!??
            //Message.Write(SeverityType.Warning, "Ignoring", method.DeclaringType.Name);

            // This class uses following classes to work, so if the attribute is added to this classes we get some sort of ordering problem
            //  More specific: im guessing CreateElement can't have a profilerattribute=> this is impossible
            if (method.DeclaringType.Name == "Profiler"
                || method.DeclaringType.Name == "TWProfileAttribute" // This happens automatically but anyways add it :P
                || method.DeclaringType.Name == "ProfilingPoint")
            {
                Message.Write(SeverityType.Warning, "Ignoring", method.Name);
                return false; // Dont profile this :P

            }

            //Message.Write(SeverityType.Warning, "code", "Biebabeloeba");

            //if (!typeof(IModelObject).IsAssignableFrom(locationInfo.DeclaringType))
            //{
            //    Message.Write(MessageLocation.Of(locationInfo.PropertyInfo), SeverityType.Error, "ModelErrorCode", "ModelObjectChangedAttribute was applied to a class not implementing IModelObject: " +
            //        locationInfo.DeclaringType.FullName);
            //}
            //if (locationInfo.LocationKind != LocationKind.Property)
            //    return false;

            //if (!locationInfo.PropertyInfo.CanRead || !locationInfo.PropertyInfo.CanWrite)
            //    return false;
            //if (locationInfo.LocationKind == LocationKind.Property && locationInfo.PropertyInfo.PropertyType.Equals(typeof(ModelContainer))) // dont sync modelcontainers!
            //    return false;

            //Console.WriteLine("Yello!");
            //Message.Write(MessageLocation.Of(locationInfo.PropertyInfo), SeverityType.Info, "ModelErrorCode", "Added ModelContainer changelogging to:: " +
            //        locationInfo.DeclaringType.FullName);

            return base.CompileTimeValidate(method);
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            el.Begin();

            base.OnEntry(args);
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            base.OnExit(args);
            el.End();

        }
    }

}
