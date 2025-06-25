using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EGFramework{
    public static class EGenerateVariant
    {
        public static Dictionary<string, object> EGenerateDictiontaryByType(this Type self)
        {
            PropertyInfo[] propertyNames = self.GetProperties();
            FieldInfo[] fieldNames = self.GetFields();
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (PropertyInfo pName in propertyNames)
            {
                result.Add(pName.Name, pName.Name);
            }
            foreach (FieldInfo fName in fieldNames)
            {
                result.Add(fName.Name, fName.Name);
            }
            return result;
        }
        public static Dictionary<string, object> EGenerateEmptyDictiontaryByType(this Type self)
        {
            PropertyInfo[] propertyNames = self.GetProperties();
            FieldInfo[] fieldNames = self.GetFields();
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (PropertyInfo pName in propertyNames)
            {
                result.Add(pName.Name, "");
            }
            foreach (FieldInfo fName in fieldNames)
            {
                result.Add(fName.Name, "");
            }
            return result;
        }

        public static Dictionary<string, object> EGenerateDictiontaryByObject<T>(this T self)
        {
            PropertyInfo[] propertyNames = typeof(T).GetProperties();
            FieldInfo[] fieldNames = typeof(T).GetFields();
            // object[] s = propertyNames.Select(p => p.GetValue(self)).ToArray();
            // object[] a = fieldNames.Select(p => p.GetValue(self)).ToArray();
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (PropertyInfo pName in propertyNames)
            {
                object p = pName.GetValue(self);
                result.Add(pName.Name, p);
            }
            foreach (FieldInfo fName in fieldNames)
            {
                object p = fName.GetValue(self);
                result.Add(fName.Name, p);
            }
            return result;
        }

        public static List<Dictionary<string, object>> EGenerateDictionaryByGroup<T>(this IEnumerable<T> self)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            PropertyInfo[] propertyNames = typeof(T).GetProperties();
            FieldInfo[] fieldNames = typeof(T).GetFields();
            foreach (T member in self)
            {
                Dictionary<string, object> mResult = new Dictionary<string, object>();
                foreach (PropertyInfo pName in propertyNames)
                {
                    object p = pName.GetValue(member);
                    mResult.Add(pName.Name, p);
                }
                foreach (FieldInfo fName in fieldNames)
                {
                    object p = fName.GetValue(member);
                    mResult.Add(fName.Name, p);
                }
                result.Add(mResult);
            }
            return result;
        }

        //Default primary key is id,Id,ID.
        public static string EGetDefaultPrimaryKey(this Dictionary<string, object> self)
        {
            foreach (KeyValuePair<string, object> param in self)
            {
                if (param.Key == "ID" || param.Key == "Id" || param.Key == "id")
                {
                    return param.Key;
                }
            }
            return "";
        }
    }
}