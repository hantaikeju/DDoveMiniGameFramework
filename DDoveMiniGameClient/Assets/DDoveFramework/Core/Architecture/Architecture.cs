using System;
using System.Collections.Generic;

namespace DDoveFramework.Core
{
    public abstract class Architecture<T> : IArchitecture where T : Architecture<T>, new()
    {
        public static Action<T> OnRegisterPatch = architecture => { };

        private static T mArchitecture;

        private bool mInited;
        private readonly HashSet<ISystem> mSystems = new HashSet<ISystem>();
        private readonly HashSet<IModel> mModels = new HashSet<IModel>();
        private readonly IOCContainer mContainer = new IOCContainer();
        private readonly TypeEventSystem mTypeEventSystem = new TypeEventSystem();

        public static IArchitecture Interface
        {
            get
            {
                if (mArchitecture == null)
                {
                    MakeSureArchitecture();
                }

                return mArchitecture;
            }
        }

        /// <summary>
        /// 反初始化并丢掉单例。下次访问 Interface 会重新 Init。
        /// </summary>
        public static void Reset()
        {
            if (mArchitecture == null)
            {
                return;
            }

            mArchitecture.Deinitialize();
            mArchitecture = null;
        }

        private static void MakeSureArchitecture()
        {
            if (mArchitecture != null)
            {
                return;
            }

            mArchitecture = new T();
            mArchitecture.Init();
            OnRegisterPatch?.Invoke(mArchitecture);

            foreach (var model in mArchitecture.mModels)
            {
                model.Init();
            }

            foreach (var system in mArchitecture.mSystems)
            {
                system.Init();
            }

            mArchitecture.mInited = true;
        }

        protected abstract void Init();

        private void Deinitialize()
        {
            foreach (var system in mSystems)
            {
                system.Deinit();
            }

            foreach (var model in mModels)
            {
                model.Deinit();
            }

            mSystems.Clear();
            mModels.Clear();
            mContainer.Clear();
            mTypeEventSystem.Clear();
            mInited = false;
        }

        public void RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem
        {
            system.SetArchitecture(this);
            mContainer.Register<TSystem>(system);
            mSystems.Add(system);

            if (mInited)
            {
                system.Init();
            }
        }

        public void RegisterModel<TModel>(TModel model) where TModel : IModel
        {
            model.SetArchitecture(this);
            mContainer.Register<TModel>(model);
            mModels.Add(model);

            if (mInited)
            {
                model.Init();
            }
        }

        public void RegisterUtility<TUtility>(TUtility utility) where TUtility : IUtility
        {
            mContainer.Register<TUtility>(utility);
        }

        public TSystem GetSystem<TSystem>() where TSystem : class, ISystem
        {
            return mContainer.Get<TSystem>();
        }

        public bool TryGetSystem<TSystem>(out TSystem system) where TSystem : class, ISystem
        {
            return mContainer.TryGet(out system);
        }

        public TModel GetModel<TModel>() where TModel : class, IModel
        {
            return mContainer.Get<TModel>();
        }

        public bool TryGetModel<TModel>(out TModel model) where TModel : class, IModel
        {
            return mContainer.TryGet(out model);
        }

        public TUtility GetUtility<TUtility>() where TUtility : class, IUtility
        {
            return mContainer.Get<TUtility>();
        }

        public bool TryGetUtility<TUtility>(out TUtility utility) where TUtility : class, IUtility
        {
            return mContainer.TryGet(out utility);
        }

        public void SendCommand<TCommand>() where TCommand : ICommand, new()
        {
            ExecuteCommand(new TCommand());
        }

        public void SendCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            ExecuteCommand(command);
        }

        protected virtual void ExecuteCommand(ICommand command)
        {
            command.SetArchitecture(this);
            command.Execute();
        }

        public TResult SendQuery<TResult>(IQuery<TResult> query)
        {
            return DoQuery(query);
        }

        protected virtual TResult DoQuery<TResult>(IQuery<TResult> query)
        {
            query.SetArchitecture(this);
            return query.Do();
        }

        public void SendEvent<TEvent>() where TEvent : new()
        {
            mTypeEventSystem.Send<TEvent>();
        }

        public void SendEvent<TEvent>(TEvent e)
        {
            mTypeEventSystem.Send<TEvent>(e);
        }

        public IUnRegister RegisterEvent<TEvent>(Action<TEvent> onEvent)
        {
            return mTypeEventSystem.Register<TEvent>(onEvent);
        }

        public void UnRegisterEvent<TEvent>(Action<TEvent> onEvent)
        {
            mTypeEventSystem.UnRegister<TEvent>(onEvent);
        }
    }
}
