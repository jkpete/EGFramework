using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EGFramework{
    public static class EGenerateVariant{
        public static Dictionary<string,object> EGenerateDictiontaryByObject<T>(this T self){
            PropertyInfo[] propertyNames = typeof(T).GetProperties();
            FieldInfo[] fieldNames = typeof(T).GetFields();
            // object[] s = propertyNames.Select(p => p.GetValue(self)).ToArray();
            // object[] a = fieldNames.Select(p => p.GetValue(self)).ToArray();
            Dictionary<string,object> result = new Dictionary<string, object>();
            foreach(PropertyInfo pName in propertyNames){
                object p = pName.GetValue(self);
                result.Add(pName.Name,p);
            }
            foreach(FieldInfo fName in fieldNames){
                object p = fName.GetValue(self);
                result.Add(fName.Name,p);
            }
            return result;
        }

        public static List<Dictionary<string,object>> EGenerateDictionaryByGroup<T>(this IEnumerable<T> self){
            List<Dictionary<string,object>> result = new List<Dictionary<string, object>>();
            PropertyInfo[] propertyNames = typeof(T).GetProperties();
            FieldInfo[] fieldNames = typeof(T).GetFields();
            foreach(T member in self){
                Dictionary<string,object> mResult = new Dictionary<string, object>();
                foreach(PropertyInfo pName in propertyNames){
                    object p = pName.GetValue(self);
                    mResult.Add(pName.Name,p);
                }
                foreach(FieldInfo fName in fieldNames){
                    object p = fName.GetValue(self);
                    mResult.Add(fName.Name,p);
                }
                result.Add(mResult);
            }
            return result;
        }
    }
}