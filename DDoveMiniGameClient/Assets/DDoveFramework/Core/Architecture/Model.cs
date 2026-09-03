namespace DDoveFramework.Core
{
    public abstract class AbstractModel : IModel
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

        void IModel.Init()
        {
            OnInit();
        }

        void IModel.Deinit()
        {
            OnDeinit();
        }

        protected abstract void OnInit();

        protected virtual void OnDeinit()
        {
        }
    }
}
