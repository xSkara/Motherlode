using Ninject.Modules;

namespace Motherlode.Common
{
    /// <summary>NInject module binding the Security and SystemInfo classes in singleton scope.</summary>
    public class MotherlodeCommonModule : NinjectModule
    {
        #region Public Methods and Operators

        public override void Load()
        {
            this.Bind<Security>().ToSelf().InSingletonScope();
            this.Bind<SystemInfo>().ToSelf().InSingletonScope();
        }

        #endregion
    }
}
