using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EGFramework
{
    public struct EGSelectParam
    {
        public int SelectID { set; get; }

        public Dictionary<int, string> SelectList { set; get; }
        public EGSelectParam(int selectID, Dictionary<int, string> selectList)
        {
            SelectID = selectID;
            SelectList = selectList;
        }
    }
    public struct EGRangeParam
    {
        public double Min { set; get; }
        public double Max { set; get; }
        public double Step { set; get; }
        public double Value { set; get; }
        public EGRangeParam(double min, double max, double step, double value)
        {
            Min = min;
            Max = max;
            Step = step;
            Value = value;
        }
    }

    public struct EGPathSelect
    {
        public string Path { set; get; }
        public bool IsDir { set; get; }
        public override string ToString()
        {
            return Path;
        }
    }

    public interface IEGReadOnlyString
    {
        public string GetString();
    }
    public struct EGReadOnlyString : IEGReadOnlyString
    {
        public string Value { get; private set; }
        public EGReadOnlyString(string value)
        {
            Value = value;
        }

        public string GetString()
        {
            return Value;
        }
    }

    public interface IEGTree
    {
        public string Name { set; get; }
        public IEGTree GetParent();
        public void SetParent(IEGTree tree);
        public IEnumerable<IEGTree> GetChilds();
        public void AppendTree(IEGTree tree);
        
    }

    public class EGTree : IEGTree
    {
        public string Name { set; get; }

        public string StrValue { set; get; }
        public long IntegerValue { set; get; }
        public byte[] ByteValue { set; get; }
        public float FloatValue { set; get; }
        public bool BoolValue { set; get; }

        public IEGTree Parent { set; get; }
        public List<IEGTree> Childs { set; get; } = new List<IEGTree>();

        public EGTree()
        {
        }

        public EGTree(string name)
        {
            this.Name = name;
        }

        public void AppendTree(IEGTree tree)
        {
            tree.SetParent(this);
            Childs.Add(tree);
        }

        public virtual IEnumerable<IEGTree> GetChilds()
        {
            return Childs;
        }

        public virtual IEGTree GetParent()
        {
            return Parent;
        }

        public void SetParent(IEGTree tree)
        {
            this.Parent = tree;
        }
    }

    public static class EGTreeFactory
    {
        public static EGTree CreateTreeByJson(string json)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(json));
            Stack<IEGTree> TreeStack = new Stack<IEGTree>();
            EGTree resultTree = new EGTree();
            EGTree lastTree = null;
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    if (lastTree != null)
                    {
                        TreeStack.Push(lastTree);
                    }
                }
                if (reader.TokenType == JsonToken.EndObject)
                {
                    if(TreeStack.Count>0)
                        TreeStack.Pop();
                }

                if (reader.TokenType == JsonToken.PropertyName)
                {
                    EGTree newTree = new EGTree();
                    if (reader.Value != null)
                    {
                        newTree.Name = reader.Value.ToString();
                    }
                    if (TreeStack.Count > 0)
                    {
                        TreeStack.Peek().AppendTree(newTree);
                    }
                    else
                    {
                        resultTree.AppendTree(newTree);
                    }
                    lastTree = newTree;
                }

                if (lastTree != null && reader.TokenType == JsonToken.Integer)
                {
                    lastTree.IntegerValue = (long)reader.Value;
                }
                if (lastTree != null && reader.TokenType == JsonToken.Boolean)
                {
                    lastTree.BoolValue = (bool)reader.Value;
                }
                if (lastTree != null && reader.TokenType == JsonToken.String)
                {
                    lastTree.StrValue = (string)reader.Value;
                }
                if (lastTree != null && reader.TokenType == JsonToken.Float)
                {
                    lastTree.FloatValue = (float)reader.Value;
                }
                if (lastTree!=null && reader.TokenType == JsonToken.Bytes)
                {
                    lastTree.ByteValue = (byte[])reader.Value;
                }
            }
            return resultTree;
        }
    }

}