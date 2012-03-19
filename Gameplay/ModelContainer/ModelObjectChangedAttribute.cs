using System;
using PostSharp;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Reflection;

namespace MHGameWork.TheWizards.ModelContainer
{
    [Serializable]
    [MulticastAttributeUsage(MulticastTargets.Property, PersistMetaData = true)]
    public sealed class ModelObjectChangedAttribute : LocationInterceptionAspect
    {
        public override bool CompileTimeValidate(LocationInfo locationInfo)
        {

            if (!typeof(IModelObject).IsAssignableFrom(locationInfo.DeclaringType))
            {
                Message.Write(MessageLocation.Of(locationInfo.PropertyInfo), SeverityType.Error, "ModelErrorCode", "ModelObjectChangedAttribute was applied to a class not implementing IModelObject: " +
                    locationInfo.DeclaringType.FullName);
            }
            if (locationInfo.LocationKind != LocationKind.Property)
                return false;

            if (!locationInfo.PropertyInfo.CanRead || !locationInfo.PropertyInfo.CanWrite)
                return false;
            if (locationInfo.LocationKind == LocationKind.Property && locationInfo.PropertyInfo.PropertyType.Equals(typeof(ModelContainer))) // dont sync modelcontainers!
                return false;
            return base.CompileTimeValidate(locationInfo);
        }

        public override void OnSetValue(LocationInterceptionArgs args)
        {
            if (args.Value != args.GetCurrentValue())
            {
                var obj = (IModelObject)args.Instance;
                if (obj.Container != null)
                    obj.Container.NotifyObjectModified(obj);
            }
            args.ProceedSetValue();
            base.OnSetValue(args);
        }
    }

}
