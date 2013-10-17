using System;
using MHGameWork.TheWizards.Data;
using PostSharp;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Reflection;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.ChangeNotification
{
    /// <summary>
    /// Applying this attribute to an IModelObject implementing class will make it notify the ModelContainer it holds of property changes.
    /// NOTE: only PROPERTY changes
    /// </summary>
    [Serializable]
    [MulticastAttributeUsage(MulticastTargets.Property | MulticastTargets.Field, PersistMetaData = true)]
    public sealed class TrackChangesAttribute : LocationInterceptionAspect
    {
        private string name;

        public override bool CompileTimeValidate(LocationInfo locationInfo)
        {

            if (!locationInfo.PropertyInfo.CanRead || !locationInfo.PropertyInfo.CanWrite)
                return false;
            Message.Write(MessageLocation.Of(locationInfo.PropertyInfo), SeverityType.Info, "ModelErrorCode", "Added Change tracking  changelogging to: " +
                    locationInfo.DeclaringType.FullName);

            return base.CompileTimeValidate(locationInfo);
        }
        public override void CompileTimeInitialize(LocationInfo targetLocation, AspectInfo aspectInfo)
        {
            name = targetLocation.Name;
            base.CompileTimeInitialize(targetLocation, aspectInfo);
        }

        public override void OnSetValue(LocationInterceptionArgs args)
        {
            if (args.Value != args.GetCurrentValue())
            {
                var obj = args.Instance;
                ChangesSink.Current.NotifyChange(obj, name);
            }
            base.OnSetValue(args);
        }
    }

}
