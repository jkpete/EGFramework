using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace EGFramework.Example.SystemExamples.PlayerStorage
{
    public interface IItem
    {
        public int GetItemId();
        public string GetItemName();
        public Texture GetIcon();
        public string GetInfo();   
    }
    public interface IBackPackItem : IItem{
        public int GetCount();
    }

    public interface ICostItem: IItem {
        public void OnCost();
    }
}