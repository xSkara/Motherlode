using Ninject.Extensions.Logging;

namespace Motherlode.Log4Net.Tests.Domain.ViewModel
{
    public class FakeViewModel
    {
        #region Constructors and Destructors

        public FakeViewModel(ILogger logger)
        {
            this.Logger = logger;
        }

        #endregion

        #region Public Properties

        public ILogger Logger { get; private set; }

        #endregion
    }
}
