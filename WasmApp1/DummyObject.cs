using System;
using System.Collections.Generic;
using System.Text;

namespace WasmApp1
{
    public class DummyObject
    {
        private bool dummyBool;
        private string dummyString;
        private Object[] dummyObjectArray;

        public DummyObject()
        {
            this.dummyBool = false;
            this.dummyString = "Hello wasm!";
            this.dummyObjectArray = new object[4];

            for (int i=0; i<4; i++)
            {
                this.dummyObjectArray[i] = i;
            }
        }

        public string getDummyMessage()
        {
            return dummyString;
        }

        public Object getDummyObjectAt(int i)
        {
            if (i < 0 || i >= 4)
            {
                throw new ArgumentException("Dummy object index invalid, must be between 0 and 3");
            }

            return this.dummyObjectArray[i];
        }
    }
}
