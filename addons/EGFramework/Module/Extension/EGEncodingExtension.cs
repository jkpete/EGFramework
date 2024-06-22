using System.Text;

namespace EGFramework{
    //use this extension,you should add System.Text.Encoding.CodePages package from Nuget
    public static class EGEncodingExtension
    {
        public static bool IsInit{ set; get; }
        
        /// <summary>
        /// get encoding from encoding params(string).
        /// </summary>
        /// <param name="self"></param>
        /// <param name="encodingTxt"></param>
        /// <returns></returns>
        public static Encoding GetEncoding(this IEGFramework self,string encodingTxt){
            if(!IsInit){
                IsInit = true;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            }
            return Encoding.GetEncoding(encodingTxt);
        }

        /// <summary>
        /// Make a string to bytes with encoding params(string).
        /// </summary>
        /// <param name="self"></param>
        /// <param name="encodingTxt"></param>
        /// <returns></returns>
        public static byte[] ToBytesByEncoding(this string self,string encodingTxt){
            if(!IsInit){
                IsInit = true;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            }
            return Encoding.GetEncoding(encodingTxt).GetBytes(self);
        }
        /// <summary>
        /// Make a string to bytes with encoding.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ToBytesByEncoding(this string self,Encoding encoding){
            if(!IsInit){
                IsInit = true;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            }
            return encoding.GetBytes(self);
        }

        /// <summary>
        /// Make a bytes to string with encoding params(string).
        /// </summary>
        /// <param name="self"></param>
        /// <param name="encodingTxt"></param>
        /// <returns></returns>
        public static string ToStringByEncoding(this byte[] self,string encodingTxt){
            if(!IsInit){
                IsInit = true;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            }
            return Encoding.GetEncoding(encodingTxt).GetString(self);
        }
        /// <summary>
        /// Make a bytes to string with encoding.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="encodingTxt"></param>
        /// <returns></returns>
        public static string ToStringByEncoding(this byte[] self,Encoding encoding){
            if(!IsInit){
                IsInit = true;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            }
            return encoding.GetString(self);
        }
    }
}

