using System;

namespace EGFramework{
    public static class EGColorConvertExtension
    {
        public static string ConvertByRGB(float r,float g,float b)
        {
            byte R = (byte)MathF.Floor(r*255);
            byte G = (byte)MathF.Floor(r*255);
            byte B = (byte)MathF.Floor(r*255);
            string result = "#"+R.ToString("X2")+G.ToString("X2")+B.ToString("X2");
            return result;
        }
        public static string ConvertByRGB255(byte r,byte g,byte b)
        {
            string result = "#"+r.ToString("X2")+g.ToString("X2")+b.ToString("X2");
            return result;
        }
    }
}