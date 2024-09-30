using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using Godot;

namespace EGFramework
{
    public class EGByteSave : EGModule, IEGSave,IEGSaveObject
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

        public void InitSaveFile(string path)
        {
            //throw new NotImplementedException();
        }

        public void SetObject<TObject>(string objectKey , TObject obj)
        {
            throw new NotImplementedException();
        }

        public TObject GetObject<TObject>(string objectKey) where TObject : new()
        {
            throw new NotImplementedException();
        }
    }
}