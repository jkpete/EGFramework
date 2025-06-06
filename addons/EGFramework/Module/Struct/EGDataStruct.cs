using System.Collections.Generic;

namespace EGFramework{
    public struct EGSelectParam{
        public int SelectID { set; get; }

        public Dictionary<int,string> SelectList { set; get; }
        public EGSelectParam(int selectID, Dictionary<int,string> selectList)
        {
            SelectID = selectID;
            SelectList = selectList;
        }
    }
    public struct EGRangeParam{
        public double Min { set; get; }
        public double Max { set; get; }
        public double Step { set; get; }
        public double Value { set; get; }
        public EGRangeParam(double min, double max, double step,double value)
        {
            Min = min;
            Max = max;
            Step = step;
            Value = value;
        }
    }
}