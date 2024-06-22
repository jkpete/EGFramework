using System;
using System.Collections.Generic;

namespace EGFramework
{
    public interface IEGObject
    {
        void RegisterObject<T>(T object_);
        T GetObject<T>() where T : class,new();
		
    }
    public class EGObject : EGModule,IEGObject
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
            return EGArchitectureImplement.Interface.GetModule<EGObject>().GetObject<T>();
        }
    }
    public static class CanRegisterObjectExtension
    {
        public static void EGRegisterObject<T>(this IArchitecture self,T object_) where T : class,new()
        {
            self.GetModule<EGObject>().RegisterObject(object_);
        }
        public static void EGRegisterObject<T>(this IEGFramework self,T object_) where T : class,new()
        {
            EGArchitectureImplement.Interface.GetModule<EGObject>().RegisterObject(object_);
        }
    }

    public static class CanContainsObjectExtension{
        public static bool EGContainsObject<T>(this IEGFramework self)
        {
            return EGArchitectureImplement.Interface.GetModule<EGObject>().ContainsObject<T>();
        }
    }

}
