using System;

namespace Tools
{
    /// <summary>
    /// This testtool just prints something to console
    /// </summary>
    public class Test: ITool
    {
        public void Execute()
        {
            Console.WriteLine("Yay!");  
        }
    }
}
