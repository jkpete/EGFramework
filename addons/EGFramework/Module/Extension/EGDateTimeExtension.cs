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
        public static long GetDateTime(this object self)
        {
            DateTime dt = DateTime.Now;
            return dt.Ticks;
        }
        public static string GetFullDateMsg(this long ticks){
            DateTime dateTime = new DateTime(ticks);
            return dateTime.ToString("yyyy-MM-dd") + " " + dateTime.ToString("HH:mm:ss");
        }
        public static string GetDayDateMsg(this long ticks){
            DateTime dateTime = new DateTime(ticks);
            return dateTime.ToString("HH:mm:ss");
        }
        public static string GetDateMsg(this long ticks){
            DateTime dateTime = new DateTime(ticks);
            return dateTime.ToString("yyyy-MM-dd");
        }
    }
}