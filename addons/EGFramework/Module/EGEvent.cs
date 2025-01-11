using System;
using System.Collections.Generic;

namespace EGFramework
{
    public interface IEGEvent{
        void SendEvent<T>() where T : new();
		void SendEvent<T>(T e);
		IUnRegister RegisterEvent<T>(Action<T> onEvent);
		void UnRegisterEvent<T>(Action<T> onEvent);
    }
    public class EGEvent : EGModule,IEGEvent
    {
        public override void Init()
        {
            
        }
        private readonly EasyEvents Events = new EasyEvents();
		public void SendEvent<TEvent>() where TEvent : new()
		{
			Events.GetEvent<EasyEvent<TEvent>>()?.Invoke(new TEvent());
		}

		public void SendEvent<TEvent>(TEvent e)
		{
			Events.GetEvent<EasyEvent<TEvent>>()?.Invoke(e);
		}

		public IUnRegister RegisterEvent<TEvent>(Action<TEvent> onEvent)
		{
			var e = Events.GetOrAddEvent<EasyEvent<TEvent>>();
			return e.Register(onEvent);
		}

		public void UnRegisterEvent<TEvent>(Action<TEvent> onEvent)
		{
			var e = Events.GetEvent<EasyEvent<TEvent>>();
			if (e != null)
			{
				e.UnRegister(onEvent);
			}
		}
    }
	
	public class EasyEvents
	{
		private static EasyEvents GlobalEvents = new EasyEvents();
		public static T Get<T>() where T : IEasyEvent
		{
			return GlobalEvents.GetEvent<T>();
		}
		public static void Register<T>() where T : IEasyEvent, new()
		{
			GlobalEvents.AddEvent<T>();
		}
		private Dictionary<Type, IEasyEvent> TypeEvents = new Dictionary<Type, IEasyEvent>();
		public void AddEvent<T>() where T : IEasyEvent, new()
		{
			TypeEvents.Add(typeof(T), new T());
		}
		public T GetEvent<T>() where T : IEasyEvent
		{
			IEasyEvent e;
			if (TypeEvents.TryGetValue(typeof(T), out e))
			{
				return (T)e;
			}
			return default;
		}
		public T GetOrAddEvent<T>() where T : IEasyEvent, new()
		{
			var eType = typeof(T);
			if (TypeEvents.TryGetValue(eType, out var e))
			{
				return (T)e;
			}
			var t = new T();
			TypeEvents.Add(eType, t);
			return t;
		}
	}

	/// <summary>
	/// This EasyEvent will release all registered function while invoked.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class EasyEventOnce<T> : IEasyEvent
	{
		private Action<T> OnEvent = e => { };
		private List<CustomUnRegister> AutoUnRegister = new List<CustomUnRegister>();
		public IUnRegister Register(Action<T> onEvent)
		{
			OnEvent += onEvent;
			CustomUnRegister unRegister = new CustomUnRegister(() => { UnRegister(onEvent); });
			AutoUnRegister.Add(unRegister);
			return unRegister;
		}
		public void UnRegister(Action<T> onEvent)
		{
			OnEvent -= onEvent;
		}
		public void Invoke(T t)
		{
			if(AutoUnRegister.Count>0){
				OnEvent?.Invoke(t);
				foreach(CustomUnRegister unRegister in AutoUnRegister){
					unRegister.UnRegister();
				}
				AutoUnRegister.Clear();
			}
		}
	}

	/// <summary>
	/// This EasyEvent will release all registered function while invoked.
	/// </summary>
	public class EasyEventOnce : IEasyEvent{
		private Action OnEvent = () => { };
		private List<CustomUnRegister> AutoUnRegister = new List<CustomUnRegister>();
		public IUnRegister Register(Action onEvent)
		{
			OnEvent += onEvent;
			CustomUnRegister unRegister = new CustomUnRegister(() => { UnRegister(onEvent); });
			AutoUnRegister.Add(unRegister);
			return unRegister;
		}
		public void UnRegister(Action onEvent)
		{
			OnEvent -= onEvent;
		}
		public void Invoke()
		{
			if(AutoUnRegister.Count>0){
				OnEvent?.Invoke();
				foreach(CustomUnRegister unRegister in AutoUnRegister){
					unRegister.UnRegister();
				}
				AutoUnRegister.Clear();
			}
		}
	}


	public static class CanRegisterEventExtension
	{
		public static IUnRegister EGRegisterEvent<T>(this IEGFramework self, Action<T> onEvent)
		{
			return EGArchitectureImplement.Interface.GetModule<EGEvent>().RegisterEvent<T>(onEvent);
		}
		public static void EGUnRegisterEvent<T>(this IEGFramework self, Action<T> onEvent)
		{
			EGArchitectureImplement.Interface.GetModule<EGEvent>().UnRegisterEvent<T>(onEvent);
		}
	}

	public static class CanSendEventExtension
	{
		public static void EGSendEvent<T>(this IEGFramework self) where T : new()
		{
			EGArchitectureImplement.Interface.GetModule<EGEvent>().SendEvent<T>();
		}
		public static void EGSendEvent<T>(this IEGFramework self, T e)
		{
			EGArchitectureImplement.Interface.GetModule<EGEvent>().SendEvent<T>(e);
		}
	}
}