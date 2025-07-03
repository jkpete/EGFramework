using System;
using System.Collections.Generic;
using System.Reflection;

namespace EGFramework
{
    public static class EGSqlExtension
    {
        public static string ToCreateTableSQL(this PropertyInfo property)
        {
            string sqlCommand;
            if (property.Name == "ID" || property.Name == "id" || property.Name == "Id")
            {
                return "";
            }
            if (property.PropertyType == typeof(int) || property.PropertyType.IsEnum)
            {
                sqlCommand = "`" + property.Name + "`   INTEGER" + "     NOT NULL,";
            }
            else if (property.PropertyType == typeof(double) || property.PropertyType == typeof(float))
            {
                sqlCommand = "`" + property.Name + "`   REAL" + "     NOT NULL,";
            }
            else if (property.PropertyType == typeof(bool))
            {
                sqlCommand = "`" + property.Name + "`   REAL" + "     NOT NULL,";
            }
            else if (property.PropertyType == typeof(long))
            {
                sqlCommand = "`" + property.Name + "`   BIGINT(20)" + "     NOT NULL,";
            }
            else if (property.PropertyType == typeof(string))
            {
                sqlCommand = "`" + property.Name + "`   VARCHAR(255)" + "     NOT NULL,";
            }
            else
            {
                sqlCommand = "`" + property.Name + "`   VARCHAR(255)" + "     NOT NULL,";
            }
            return sqlCommand;
        }

        public static string ToCreateTableSQL(this FieldInfo field)
        {
            string sqlCommand;
            if (field.Name == "ID" || field.Name == "id" || field.Name == "Id")
            {
                return "";
            }
            if (field.FieldType == typeof(int) || field.FieldType.IsEnum)
            {
                sqlCommand = "`" + field.Name + "`   INTEGER" + "     NOT NULL,";
            }
            else if (field.FieldType == typeof(double) || field.FieldType == typeof(float))
            {
                sqlCommand = "`" + field.Name + "`   REAL" + "     NOT NULL,";
            }
            else if (field.FieldType == typeof(bool))
            {
                sqlCommand = "`" + field.Name + "`   REAL" + "     NOT NULL,";
            }
            else if (field.FieldType == typeof(long))
            {
                sqlCommand = "`" + field.Name + "`   BIGINT(20)" + "     NOT NULL,";
            }
            else if (field.FieldType == typeof(string))
            {
                sqlCommand = "`" + field.Name + "`   VARCHAR(255)" + "     NOT NULL,";
            }
            else
            {
                sqlCommand = "`" + field.Name + "`   VARCHAR(255)" + "     NOT NULL,";
            }
            return sqlCommand;
        }

        public static string ToCreateTableSQL(this KeyValuePair<string, Type> param)
        {
            string sqlCommand;
            if (param.Key == "ID" || param.Key == "id" || param.Key == "Id")
            {
                return "";
            }
            if (param.Value == typeof(int) || param.Value.IsEnum)
            {
                sqlCommand = "`" + param.Key + "`   INTEGER" + "     NOT NULL,";
            }
            else if (param.Value == typeof(double) || param.Value == typeof(float))
            {
                sqlCommand = "`" + param.Key + "`   REAL" + "     NOT NULL,";
            }
            else if (param.Value == typeof(bool))
            {
                sqlCommand = "`" + param.Key + "`   REAL" + "     NOT NULL,";
            }
            else if (param.Value == typeof(long))
            {
                sqlCommand = "`" + param.Key + "`   BIGINT(20)" + "     NOT NULL,";
            }
            else if (param.Value == typeof(string))
            {
                sqlCommand = "`" + param.Key + "`   VARCHAR(255)" + "     NOT NULL,";
            }
            else
            {
                sqlCommand = "`" + param.Key + "`   VARCHAR(255)" + "     NOT NULL,";
            }
            return sqlCommand;
        }

        public static string ToCreateTableSQL(this Type type, string tableName)
        {
            var properties = type.GetProperties();
            FieldInfo[] fields = type.GetFields();
            string keySet = "";
            foreach (PropertyInfo key in properties)
            {
                keySet += key.ToCreateTableSQL();
            }
            foreach (FieldInfo key in fields)
            {
                keySet += key.ToCreateTableSQL();
            }
            keySet = keySet.TrimEnd(',');
            string createSql = @"CREATE TABLE " + tableName + " (" +
            "`ID` INTEGER PRIMARY KEY AUTOINCREMENT, " + keySet
            + ");";
            return createSql;
        }
        
        public static string ToCreateTableSQL(this Dictionary<string,Type> tableParam,string tableName)
        {

            string keySet = "";
            foreach(KeyValuePair<string,Type> key in tableParam){
                keySet += key.ToCreateTableSQL();
            }
            keySet = keySet.TrimEnd(',');
            string createSql = @"CREATE TABLE "+tableName+" ("+
            "`ID` INTEGER PRIMARY KEY AUTOINCREMENT, "+keySet
            +");";
            return createSql;
        }

    }
}