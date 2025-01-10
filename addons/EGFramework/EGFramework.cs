using System;
using System.Collections.Generic;

namespace EGFramework
{
	#region Architecture & Module
	public class EGArchitecture<T> : IArchitecture where T : EGArchitecture<T>, new()
	{
		private static T Architecture;
		public static IArchitecture Interface
		{
			get
			{
				if (Architecture == null)
				{
					MakeSureArchitecture();
				}
				return Architecture;
			}
		}

		private static void MakeSureArchitecture()
		{
			if (Architecture == null)
			{
				Architecture = new T();
				Architecture.Init();
			}
		}

		protected virtual void Init()
		{
			
		}

		private IOCContainer ModuleContainer = new IOCContainer();

		public void RegisterModule<TModule>(TModule module) where TModule : IModule
		{
			ModuleContainer.Register<TModule>(module);
			module.Init();
		}
		public TModule GetModule<TModule>() where TModule : class, IModule,new()
		{
			if (!ModuleContainer.self.ContainsKey(typeof(TModule)))
			{
				this.RegisterModule(new TModule());
			}
			return ModuleContainer.Get<TModule>();
		}
		public bool IsInitModule<TModule>() where TModule : class, IModule,new()
		{
			if (!ModuleContainer.self.ContainsKey(typeof(TModule)))
			{
				return true;
			}else{
				return false;
			}
		}
	}

	public abstract class EGModule:IModule{
		IArchitecture IBelongToArchitecture.GetArchitecture()
		{
			return EGArchitectureImplement.Interface;
		}
		void IModule.Init()
		{
			this.Init();
		}
		public abstract void Init();
	}
	#endregion

	#region Interface
	public interface IArchitecture
	{
		void RegisterModule<T>(T model) where T : IModule;
		T GetModule<T>() where T : class, IModule,new();
		bool IsInitModule<T>() where T : class, IModule,new();
	}
	public interface IModule : IBelongToArchitecture
	{
		void Init();
	}
	public interface IBelongToArchitecture
	{
		IArchitecture GetArchitecture();
	}
	#endregion

	#region IOC
	public class IOCContainer
	{
		private Dictionary<Type, object> Instances = new Dictionary<Type, object>();
		public void Register<T>(T instance)
		{
			var key = typeof(T);
			if (Instances.ContainsKey(key))
			{
				Instances[key] = instance;
			}
			else
			{
				Instances.Add(key, instance);
			}
		}
		public T Get<T>() where T : class
		{
			var key = typeof(T);
			if (Instances.TryGetValue(key, out var retInstance))
			{
				return retInstance as T;
			}
			return null;
		}
		public Dictionary<Type, object> self => Instances;
	}
	#endregion

	#region Event
    public interface IEasyEvent { 

    }
	public interface IUnRegister
	{
		void UnRegister();
	}

	public class EasyEvent<T> : IEasyEvent
	{
		private Action<T> OnEvent = e => { };
		public IUnRegister Register(Action<T> onEvent)
		{
			OnEvent += onEvent;
			return new CustomUnRegister(() => { UnRegister(onEvent); });
		}
		public void UnRegister(Action<T> onEvent)
		{
			OnEvent -= onEvent;
		}
		public void Invoke(T t)
		{
			OnEvent?.Invoke(t);
		}
	}
	
	public class EasyEvent : IEasyEvent
	{
		private Action OnEvent = () => { };
		public IUnRegister Register(Action onEvent)
		{
			OnEvent += onEvent;
			return new CustomUnRegister(() => { UnRegister(onEvent); });
		}
		public void UnRegister(Action onEvent)
		{
			OnEvent -= onEvent;
		}
		public void Invoke()
		{
			OnEvent?.Invoke();
		}
	}
	public struct CustomUnRegister : IUnRegister
	{
		/// <summary>
		/// delegate object 
		/// </summary>
		private Action OnUnRegister { get; set; }

		public CustomUnRegister(Action onUnRegister)
		{
			OnUnRegister = onUnRegister;
		}
		/// <summary>
		/// release by parent;
		/// </summary>
		public void UnRegister()
		{
			OnUnRegister.Invoke();
			OnUnRegister = null;
		}
	}
	#endregion

	#region FrameworkExtension
	public interface IEGFramework{}

	public class EGArchitectureImplement:EGArchitecture<EGArchitectureImplement>{
        protected override void Init()
        {
            //base.Init();
        }
	}

	public static class EGArchitectureImplementExtension{
		public static T GetModule<T>(this IEGFramework self) where T : class, IModule,new()
		{
			return EGArchitectureImplement.Interface.GetModule<T>();
		}
		public static void RegisterModule<T>(this IEGFramework self,T model) where T : class, IModule,new()
		{
			EGArchitectureImplement.Interface.RegisterModule(model);
		}
	}
	#endregion
}
