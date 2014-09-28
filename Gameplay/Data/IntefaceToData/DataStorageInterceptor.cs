using System;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using Castle.DynamicProxy;
using MHGameWork.TheWizards.Reflection;

namespace MHGameWork.TheWizards.GodGame._Engine.IntefaceToData
{
    /// <summary>
    /// This interceptor allows redirecting all property reads and writes to an interface to a custom location
    /// The interface should only contain read-write properties. 
    /// TODO: WARNING: this data structure does not have default values, and depends on the storage to ensure that a initial read returns a value
    /// IDEA: on construction of the interceptor, set a default value on each property in the intercepted interface?
    /// </summary>
    public class DataStorageInterceptor<T> : Castle.DynamicProxy.IInterceptor
    {
        private readonly IObjectStorage storage;

        private Subject<string> obs = new Subject<string>();
        public IObservable<string> Observable { get { return obs; } }

        private DataStorageInterceptor(IObjectStorage storage)
        {
            this.storage = storage;
        }

        public static T ImplementInterface(IObjectStorage storage)
        {
            var generator = new ProxyGenerator();
            var interceptor = new DataStorageInterceptor<T>(storage);
            var proxy = (T)generator.CreateInterfaceProxyWithoutTarget(typeof(T), interceptor);
            return proxy;
        }

        public void Intercept(Castle.DynamicProxy.IInvocation invocation)
        {
            if (!ReflectionHelper.IsMethodFromProperty(invocation.Method))
                invocation.Proceed(); //DataStorageInterceptor only implements properties, not methods

            var property = ReflectionHelper.GetPropertyForMethod(invocation.Method);

            if (invocation.Method == property.GetSetMethod())
            {
                storage.Set(property.Name, invocation.Arguments[0]);
                obs.OnNext(property.Name);
            }
            else if (invocation.Method == property.GetGetMethod())
            {
                invocation.ReturnValue = storage.Get(property.Name);
            }
            else
                throw new InvalidOperationException("Unforseen problem?");

        }


    }
}