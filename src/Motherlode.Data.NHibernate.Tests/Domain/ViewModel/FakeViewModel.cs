namespace Motherlode.Data.NHibernate.Tests.Domain.ViewModel
{
    public class FakeViewModel
    {
        #region Constructors and Destructors

        public FakeViewModel(IDaoFactory daoFactory)
        {
            this.DaoFactory = daoFactory;
        }

        #endregion

        #region Public Properties

        public IDaoFactory DaoFactory { get; private set; }

        #endregion
    }
}
