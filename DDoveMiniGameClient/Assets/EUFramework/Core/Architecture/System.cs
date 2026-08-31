namespace EUFramework.Core
{
    public abstract class AbstractSystem : ISystem
    {
        private IArchitecture mArchitecture;

        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return mArchitecture;
        }

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            mArchitecture = architecture;
        }

        void ISystem.Init()
        {
            OnInit();
        }

        void ISystem.Deinit()
        {
            OnDeinit();
        }

        protected abstract void OnInit();

        protected virtual void OnDeinit()
        {
        }
    }
}
