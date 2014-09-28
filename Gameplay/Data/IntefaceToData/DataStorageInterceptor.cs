﻿using System;
using System.Collections.Generic;
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

        private Dictionary<MethodInfo, PropertyInfo> setters = new Dictionary<MethodInfo, PropertyInfo>();
        private Dictionary<MethodInfo, PropertyInfo> getters = new Dictionary<MethodInfo, PropertyInfo>();

        private DataStorageInterceptor(IObjectStorage storage)
        {
            this.storage = storage;

            setters = typeof(T).GetProperties().ToDictionary(p => p.GetSetMethod());
            getters = typeof(T).GetProperties().ToDictionary(p => p.GetGetMethod());
        }

        /// <summary>
        /// TODO:IDEA: the intercepter can be shared for improved performance!
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="generator"></param>
        /// <returns></returns>
        public static T ImplementInterface(IObjectStorage storage, ProxyGenerator generator)
        {
            var interceptor = new DataStorageInterceptor<T>(storage);
            var proxy = (T)generator.CreateInterfaceProxyWithoutTarget(typeof(T), interceptor);
            return proxy;
        }

        public void Intercept(Castle.DynamicProxy.IInvocation invocation)
        {
            if (setters.ContainsKey(invocation.Method))
            {
                var prop = setters[invocation.Method];
                storage.Set(prop.Name, invocation.Arguments[0]);
                obs.OnNext(prop.Name);
            }
            else if (getters.ContainsKey(invocation.Method))
            {
                invocation.ReturnValue = storage.Get(getters[invocation.Method].Name);
            }
            else
                invocation.Proceed(); //DataStorageInterceptor only implements properties, not methods

        }


    }
}