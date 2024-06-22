using System;
namespace EGFramework{
    public static class EGDateTimeExtension
    {
        public static string GetFullDateMsg(this IEGFramework self)
        {
            return DateTime.Now.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToString("HH:mm:ss");
        }
        public static string GetDayDateMsg(this IEGFramework self)
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
        public static long GetTimeStamp(this IEGFramework self)
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0, 0);
            return System.Convert.ToInt64(ts.TotalSeconds);
        }
    }
}
    
