using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using Godot;

namespace EGFramework
{
    public class EGByteSave : EGModule//, IEGSave
    {
        
        public void SaveToFile(string content)
        {
            using var file = FileAccess.Open("user://save_game.dat", FileAccess.ModeFlags.Write);
            Variant hp = 10;
            file.StoreVar(hp);
            Variant pos = new Vector2(100,100);
            file.StoreVar(pos);
        }

        public Variant LoadFromFile()
        {
            using var file = FileAccess.Open("user://save_game.dat", FileAccess.ModeFlags.Read);
            Variant content = file.GetVar();
            Variant pos = file.GetVar();
            return pos;
        }
        public override void Init()
        {

        }

        // public TData GetDataByFile<TData>() where TData : class, new()
        // {
        //     throw new NotImplementedException();
        // }
        // public void InitSaveData(string fileName)
        // {
        //     throw new NotImplementedException();
        // }
        public void SetDataToFile<TData>(TData data)
        {

        }
    }
}