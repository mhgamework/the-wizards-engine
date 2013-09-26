using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using PostSharp.Aspects;
using PostSharp.Extensibility;

namespace MHGameWork.TheWizards.SkyMerchant._Engine
{
    /// <summary>
    /// Caches the return values of this method
    /// FROM: http://www.postsharp.net/blog/post/SOLID-Caching
    /// </summary>
    [Serializable]
    public class CacheAttribute : MethodInterceptionAspect
    {
        [NonSerialized]
        private static readonly Dictionary<string,object> _cache;
        private string _methodName;

        /// <summary>
        /// No clue why this is static
        /// </summary>
        static CacheAttribute()
        {
            if (!PostSharpEnvironment.IsPostSharpRunning)
            {
                _cache = new Dictionary<string, object>();
            }
        }

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            _methodName = method.Name;
        }

        public override void OnInvoke(MethodInterceptionArgs args)
        {
            var key = BuildCacheKey(args.Arguments);
            if (_cache.ContainsKey(key))
            {
                args.ReturnValue = _cache[key];
            }
            else
            {
                var returnVal = args.Invoke(args.Arguments);
                args.ReturnValue = returnVal;
                _cache[key] = returnVal;
            }
        }

        private string BuildCacheKey(Arguments arguments)
        {
            var sb = new StringBuilder();
            sb.Append(_methodName);
            foreach (var argument in arguments.ToArray())
            {
                sb.Append(argument == null ? "_" : argument.ToString());
            }
            return sb.ToString();
        }
    }
}