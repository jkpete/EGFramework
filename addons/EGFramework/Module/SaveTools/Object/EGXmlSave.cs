using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Linq;
using System;
using System.Xml.Serialization;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EGFramework
{
    public class EGXmlSave : IEGSave, IEGSaveReadOnly, IEGSaveObject
    {
        public bool IsReadOnly { get; set; }
        public Encoding StringEncoding { set; get; } = Encoding.UTF8;
        private string DefaultPath { set; get; }

        private XmlDocument _SaveObject;
        private XmlDocument SaveObject{ 
            get {
                if(_SaveObject == null){
                    InitSave(DefaultPath);
                }
                return _SaveObject;
            }
        }

        /// <summary>
        /// Init a new save data file or load an other file with json suffix, if you want to load other save data, please use this function to reload;
        /// </summary>
        public void InitSave(string path)
        {
            DefaultPath = path;
            if(!File.Exists(path)){
                if (!Directory.Exists(Path.GetDirectoryName(DefaultPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(DefaultPath));
                    _SaveObject = new XmlDocument();
                    XmlElement root = _SaveObject.CreateElement("Root");
                    _SaveObject.AppendChild(root);
                    _SaveObject.Save(path);

                }else if (!File.Exists(DefaultPath))
                {
                    _SaveObject = new XmlDocument();
                    XmlElement root = _SaveObject.CreateElement("Root");
                    _SaveObject.AppendChild(root);
                    _SaveObject.Save(path);
                }
            }
            else
            {
                _SaveObject = new XmlDocument();
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                XmlReader reader = XmlReader.Create(path, settings);
                _SaveObject.Load(reader);
                reader.Close();
            }
        }

        public void AddObject<TObject>(string objectKey, TObject obj)
        {
            Dictionary<string,object> _data = obj.EGenerateDictiontaryByObject();
            XmlElement element = _SaveObject.CreateElement(objectKey);
            foreach(KeyValuePair<string,object> _param in _data)
            {
                if(_param.Value == null)
                {
                    element.SetAttribute(_param.Key,"");
                }
                else
                {
                    element.SetAttribute(_param.Key,_param.Value.ToString());
                }
            }
            _SaveObject.DocumentElement.AppendChild(element);
            _SaveObject.Save(DefaultPath);
        }

        public bool ContainsKey(string objectKey)
        {
            return this.GetKeys().Contains(objectKey);
        }

        public IEnumerable<string> GetKeys()
        {
            List<string> resultKeys = new List<string>();
            foreach(XmlNode node in _SaveObject.DocumentElement.ChildNodes)
            {
                resultKeys.Add(node.Name);
            }
            return resultKeys;
        }

        public TObject GetObject<TObject>(string objectKey)
        {
            if(!this.ContainsKey(objectKey)){
                throw new Exception("Key not found!");
            }
            XmlSerializer serializer = new XmlSerializer(typeof(TObject));
            XmlNode xml = _SaveObject.DocumentElement.GetElementsByTagName(objectKey)[0];
            string json = JsonConvert.SerializeXmlNode(xml);
            var jObject = JObject.Parse(json);
            ConvertJTokenPropertyNames(jObject[objectKey]);
            json = jObject[objectKey].ToString();
            return JsonConvert.DeserializeObject<TObject>(json);
        }

        public IEnumerable<TObject> GetObjects<TObject>(string objectKey)
        {
            if(!this.ContainsKey(objectKey)){
                throw new Exception("Key not found!");
            }
            XmlSerializer serializer = new XmlSerializer(typeof(TObject));
            XmlNodeList xmls = _SaveObject.DocumentElement.GetElementsByTagName(objectKey);
            List<TObject> objects = new List<TObject>();
            foreach (XmlNode xml in xmls)
            {
                string json = JsonConvert.SerializeXmlNode(xml);
                var jObject = JObject.Parse(json);
                ConvertJTokenPropertyNames(jObject[objectKey]);
                json = jObject[objectKey].ToString();
                objects.Add(JsonConvert.DeserializeObject<TObject>(json));
            }
            return objects;
        }

        public void InitReadOnly(string data)
        {
            throw new System.NotImplementedException();
        }

        public void InitReadOnly(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveObject<TObject>(string objectKey)
        {
            throw new System.NotImplementedException();
        }

        public void SetObject<TObject>(string objectKey, TObject obj)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateObject<TObject>(string objectKey, TObject obj)
        {
            throw new System.NotImplementedException();
        }

        private static void ConvertJTokenPropertyNames(JToken token)
        {
            if (token is JObject jObject)
            {
                // 收集要处理的属性
                var propertiesToProcess = new List<(JProperty Property, bool IsAttribute)>();

                // 先分类属性
                foreach (var property in jObject.Properties().ToList())
                {
                    bool isAttribute = property.Name.StartsWith("@");
                    propertiesToProcess.Add((property, isAttribute));
                }

                // 处理每个属性
                foreach (var (property, isAttribute) in propertiesToProcess)
                {
                    if (isAttribute)
                    {
                        // 转换带 @ 的属性：移除 @ 前缀
                        string newName = property.Name.Substring(1);
                        var value = property.Value;
                        
                        // 移除旧属性
                        property.Remove();
                        
                        // 添加新属性
                        jObject[newName] = value;
                        
                        // 递归处理子对象
                        ConvertJTokenPropertyNames(value);
                    }
                    else
                    {
                        // 删除普通属性（不带 @ 的）
                        property.Remove();
                    }
                }
            }
            else if (token is JArray jArray)
            {
                foreach (var item in jArray)
                {
                    ConvertJTokenPropertyNames(item);
                }
            }
        }
    
    
    }
}