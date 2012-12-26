using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using NUnit.Framework;

namespace MHGameWork.TheWizards.TestRunner
{
    [Serializable]
    public class NUnitTestRunner : ITestRunner
    {
        public static bool IsRunningAutomated { get; private set; }

        /// <summary>
        /// This method runs given test method and stores returns the result of the test
        /// </summary>
        /// <param name="test"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public Exception RunAutomated(object test, MethodInfo method)
        {
            Exception throwedException = null;
            try
            {
                enableAutomatedTesting();
                RunNormal(test, method);
            }
            catch (Exception ex)
            {
                throwedException = ex;
                Console.WriteLine(ex);

                var attrs = method.GetCustomAttributes(typeof(ExpectedExceptionAttribute), false);
                for (int i = 0; i < attrs.Length; i++)
                {
                    var attr = (ExpectedExceptionAttribute)attrs[i];
                    if (ex.GetType() == attr.ExpectedException)
                    {
                        if (attr.ExpectedMessage == ex.Message || attr.ExpectedMessage == null)
                        {
                            throwedException = null;
                            //Console.WriteLine("This Exception is a valid result for this test");
                            break;
                        }

                    }
                }

            }
            catch
            {
                Console.WriteLine("An Unmanaged exception has been thrown!!");
                throwedException = new Exception("An Unmanaged exception has been thrown!!");
            }
            disableAutomatedTesting();
            return throwedException;
        }

        public void Run(ITest test)
        {
            var unit = (NUnitTest) test;
            var instance = Activator.CreateInstance(unit.TestClass);

            var thread = new Thread(delegate()
            {
                try
                {
                    RunNormal(instance, unit.TestMethod);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "TestThread";
            thread.Start();

            thread.Join();

        }

        /// <summary>
        /// This method runs given test method and stores returns the result of the test
        /// </summary>
        /// <param name="test"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public Exception RunAutomated(string assemblyLocation, string className, string methodName)
        {
            var ass = Assembly.LoadFrom(assemblyLocation);
            var type = ass.GetType(className);

            var test = Activator.CreateInstance(type);

            var method = type.GetMethod(methodName);


            return RunAutomated(test, method);
        }


        private void enableAutomatedTesting()
        {
            /*XNAGame.AutoShutdown = 3;
            XNAGame.DefaultInputDisabled = true;
            IsRunningAutomated = true;*/
        }
        private void disableAutomatedTesting()
        {
            /*XNAGame.AutoShutdown = -1;
            XNAGame.DefaultInputDisabled = false;
            IsRunningAutomated = false;*/
        }

        /// <summary>
        /// This simply runs the correct pieces of code for given test method.
        /// </summary>
        /// <param name="test"></param>
        /// <param name="method"></param>
        public void RunNormal(object test, MethodInfo method)
        {
            var setupMethod = FindSingleMethodWithAttribute(test.GetType(),
                                                            typeof(NUnit.Framework.SetUpAttribute));
            if (setupMethod != null)
            {
                var setupDeleg = (TestDelegate)Delegate.CreateDelegate(typeof(TestDelegate), test, setupMethod);
                setupDeleg();
            }



            var testDeleg = (TestDelegate)Delegate.CreateDelegate(typeof(TestDelegate), test, method);
            testDeleg();



            var tearDownMethod = FindSingleMethodWithAttribute(test.GetType(),
                                            typeof(NUnit.Framework.TearDownAttribute));
            if (tearDownMethod != null)
            {
                var tearDownDeleg = (TestDelegate)Delegate.CreateDelegate(typeof(TestDelegate), test, tearDownMethod);
                tearDownDeleg();
            }


        }




        public static MethodInfo FindSingleMethodWithAttribute(Type type, Type attributeType)
        {
            var methods =
                    type.GetMethods().Where(
                        m => m.GetCustomAttributes(attributeType, false).Length > 0).ToArray();

            if (methods.Length == 0) return null;
            if (methods.Length > 1) throw new Exception("More than one method with given attributeType found!");

            return methods[0];
        }

    }
}
