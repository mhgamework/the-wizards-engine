using System;
using PostSharp;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Reflection;

namespace MHGameWork.TheWizards.Model
{
    [Serializable]
    [MulticastAttributeUsage(MulticastTargets.Property)]
    public sealed class ModelObjectChangedAttribute : LocationInterceptionAspect
    {
        public override bool CompileTimeValidate(LocationInfo locationInfo)
        {

            if (!typeof(IModelObject).IsAssignableFrom(locationInfo.DeclaringType))
            {
                Message.Write(MessageLocation.Of(locationInfo.PropertyInfo), SeverityType.Error, "ModelErrorCode", "ModelObjectChangedAttribute was applied to a class not implementing IModelObject: " +
                    locationInfo.DeclaringType.FullName);
            }
            Message.Write(SeverityType.Warning, "mqlsdkfj", locationInfo.PropertyInfo.Name);
            if (locationInfo.LocationKind == LocationKind.Property && locationInfo.PropertyInfo.PropertyType.Equals(typeof(ModelContainer))) //TODO: maybe cheat
                return false;
            return base.CompileTimeValidate(locationInfo);
        }

        public override void CompileTimeInitialize(LocationInfo targetLocation, AspectInfo aspectInfo)
        {
            base.CompileTimeInitialize(targetLocation, aspectInfo);
        }

        public override void RuntimeInitialize(LocationInfo locationInfo)
        {

            base.RuntimeInitialize(locationInfo);
        }


        public override void OnSetValue(LocationInterceptionArgs args)
        {

            if (args.Value != args.GetCurrentValue())
            {
                Console.WriteLine("Changed!");
            }
            args.ProceedSetValue();
            base.OnSetValue(args);
        }
    }

}
