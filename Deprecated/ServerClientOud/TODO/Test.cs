using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public class Test
    {
        private Object myDataLockObj = new Object();

        private Class1 myData1;
        private Class2 myData2;
        private int myData3;

        public void CodeThread1()
        {
            //Do stuff

            lock ( myDataLockObj )
            {
                //Use myData1, myData2, myData3, ...
            }

            //More stuff
        }

        public void CodeThread2()
        {
            //Do other stuff

            lock ( myDataLockObj )
            {
                //Use myData1, myData2, myData3, ... in a different way than CodeThread1()
            }

            //More other stuff
        }






        //2

        private MyStruct bufferObj;


        private List<MyStruct> objects = new List<MyStruct>();

        public void CodeThread1()
        {
            while ( true )
            {
                if ( bufferObj.IsEmpty() )
                {
                    MyStruct newObj = new MyStruct();
                    // Set vars in MyStruct

                    bufferObj = newObj;
                }
            }
        }

        public void CodeThread1()
        {
            while ( true )
            {
                if ( !bufferObj.IsEmpty() )
                {
                    objects.Add( bufferObj );

                    bufferObj = MyStruct.Empty;
                }
            }
        }

    }
}
