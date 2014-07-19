using System;
using System.Collections.Concurrent;
using System.Reflection;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Reflection;

namespace NotifyPropertyChangedSample
{
    [Serializable]
    public class NotifyPropertyChanged : LocationInterceptionAspect
    {
        private readonly ConcurrentDictionary<Type, MethodInfo> _raisePropertyChangedMethods = new ConcurrentDictionary<Type, MethodInfo>();

        public override bool CompileTimeValidate(LocationInfo locationInfo)
        {
            if (GetRaisePropertyChangedMethod(locationInfo.DeclaringType) != null) return true;

            Message.Write(SeverityType.Error, "NotifyPropertyChangedSample01", "{0} does not contain a method named RaisePropertyChanged.", locationInfo.DeclaringType);
            return false;
        }

        public override void OnSetValue(LocationInterceptionArgs args)
        {
            var propertyInfo = args.Location.PropertyInfo;
            if (propertyInfo == null) return;

            var oldValue = args.GetCurrentValue();
            var newValue = args.Value;
            if (Equals(oldValue, newValue)) return;

            args.ProceedSetValue();

            var raisePropertyChangedMethod = _raisePropertyChangedMethods.GetOrAdd(args.Location.DeclaringType, GetRaisePropertyChangedMethod);
            raisePropertyChangedMethod.Invoke(args.Instance, new[] {propertyInfo.Name});
        }

        private static MethodInfo GetRaisePropertyChangedMethod(Type declaringType)
        {
            var raisePropertyChangedMethod = declaringType.GetMethod("RaisePropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return raisePropertyChangedMethod;
        }
    }
}