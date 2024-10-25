using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Universe.CpuUsage;

/*
 * Supported NUnit (and Console Runner): 3.12 ... 3.18.3
 */
namespace Universe {

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Assembly, AllowMultiple = false)]
    public class NUnitPipelineActionAttribute : Attribute, ITestAction
    {
        public ActionTargets Targets => ActionTargets.Test | ActionTargets.Suite;

        public void BeforeTest(ITest test)
        {
        }


        public void AfterTest(ITest test)
        {
        }

        static string EmptyLineBetweenTests()
        {
#if NETFRAMEWORK
            return Environment.NewLine;
#else
            return string.Empty;
#endif

        }



    }
}