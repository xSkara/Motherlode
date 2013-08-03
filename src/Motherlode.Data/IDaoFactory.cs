namespace Motherlode.Data
{
    public interface IDaoFactory
    {
        #region Public Properties

        IStatelessUnitOfWork StatelessUnitOfWork { get; }
        IUnitOfWork UnitOfWork { get; }

        #endregion

        #region Public Methods and Operators

        IDao<T> CreateDao<T>() where T : class;

        IStatelessDao<T> CreateStatelessDao<T>() where T : class;

        #endregion
    }
}
