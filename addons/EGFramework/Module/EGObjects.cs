using System;
using System.Collections.Generic;

namespace EGFramework
{
    public interface IEGObjects
    {
        void RegisterObject<T>(T object_);
        T GetObject<T>() where T : class,new();
		
    }
    public class EGObjects : EGModule,IEGObjects
    {
        private IOCContainer ObjectContainer = new IOCContainer();
        public override void Init()
        {
            
        }

        public TObject GetObject<TObject>() where TObject : class,new()
        {
            if (!ObjectContainer.self.ContainsKey(typeof(TObject)))
            {
                this.RegisterObject(new TObject());
            }
            return ObjectContainer.Get<TObject>();
        }

        public void RegisterObject<TObject>(TObject object_)
        {
            ObjectContainer.Register(object_);
        }

        public bool ContainsObject<TObject>(){
            return ObjectContainer.self.ContainsKey(typeof(TObject));
        }
    }
    
    public static class CanGetObjectExtension
    {
        public static T EGGetObject<T>(this IEGFramework self) where T : class,new()
        {
            return EGArchitectureImplement.Interface.GetModule<EGObjects>().GetObject<T>();
        }
    }
    public static class CanRegisterObjectExtension
    {
        public static void EGRegisterObject<T>(this IArchitecture self,T object_) where T : class,new()
        {
            self.GetModule<EGObjects>().RegisterObject(object_);
        }
        public static void EGRegisterObject<T>(this IEGFramework self,T object_) where T : class,new()
        {
            EGArchitectureImplement.Interface.GetModule<EGObjects>().RegisterObject(object_);
        }
    }

    public static class CanContainsObjectExtension{
        public static bool EGContainsObject<T>(this IEGFramework self)
        {
            return EGArchitectureImplement.Interface.GetModule<EGObjects>().ContainsObject<T>();
        }
    }

}
