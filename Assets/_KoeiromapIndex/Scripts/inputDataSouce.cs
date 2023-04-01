using System;

namespace _KoeiromapIndex
{
    public class inputDataSouce
    {
        [Serializable]
        public class InputData
        {
            public string inputTextData;
            public float xValue;
            public bool isLockXValue;
            public float yValue;
            public bool isLockYValue;
            public int seed;
            public bool isLockSeed;
        }
    }
}