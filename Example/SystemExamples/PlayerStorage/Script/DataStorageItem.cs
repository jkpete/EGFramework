using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace EGFramework.Example.SystemExamples.PlayerStorage
{
    public partial class DataStorageItem : Resource, IItem
    {
        [Export] public int Id { set; get; }
        [Export] public string Name { set; get; }
        [Export(PropertyHint.MultilineText)] public string Info { set; get; }
        [Export] public Texture Icon { set; get; }

        public Texture GetIcon()
        {
            throw new NotImplementedException();
        }

        public string GetInfo()
        {
            throw new NotImplementedException();
        }

        public int GetItemId()
        {
            throw new NotImplementedException();
        }

        public string GetItemName()
        {
            throw new NotImplementedException();
        }
    }
}