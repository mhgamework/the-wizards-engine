using System;
using System.Collections.Generic;
using System.Text;

namespace SamplePlugin
{
    class Program
    {
        public static  void Main()
        {
            Test.RibbonForm1 ribbonForm = new Test.RibbonForm1();
            Test.PluginForm pluginForm = new global::SamplePlugin.Test.PluginForm();
            ribbonForm.ribbonPageMain.Groups.Add( pluginForm.ribbonPageGroupTest );

            ribbonForm.ShowDialog();
            
        }
    }
}
