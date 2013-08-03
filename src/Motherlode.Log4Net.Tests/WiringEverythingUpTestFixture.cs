using Motherlode.Common;
using Motherlode.Log4Net.Tests.Domain.ViewModel;
using Ninject;
using NUnit.Framework;
using SharpTestsEx;

namespace Motherlode.Log4Net.Tests
{
    [TestFixture]
    public class WiringEverythingUpTestFixture
    {
        #region Public Methods and Operators

        [Test]
        public void ninject_log4net_extension_is_correctly_configured_and_providing_loggers()
        {
            Logger.ConfigureWithAppConfig();
            var appender = (DelegateAppender)Logger.GetAppender("DelegateAppender");
            int count = 0;
            appender.OnAppend += (sender, args) =>
            {
                args.AppendedData.Should().StartWith("[Motherlode.Log4Net.Tests.Domain.ViewModel.FakeViewModel]");
                count++;
            };

            var kernel = new StandardKernel(new MotherlodeCommonModule());

            var vm1 = kernel.Get<FakeViewModel>();
            var vm2 = kernel.Get<FakeViewModel>();

            vm1.Logger.Debug("test for vm1");
            vm2.Logger.Info("test for vm2");

            count.Should().Be.EqualTo(2);
        }

        #endregion
    }
}
