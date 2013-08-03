namespace Motherlode.Data
{
    /// <summary>
    ///     This serves as a base interface for <see cref="EntityWithTypedId{TId}" /> and
    ///     <see cref="DomainObjectBase" />. Also provides a simple means to develop your own base entity.
    /// </summary>
    public interface IEntityWithTypedId<TId>
    {
        #region Public Properties

        TId Id { get; }

        #endregion

        #region Public Methods and Operators

        bool IsTransient();

        #endregion
    }
}
