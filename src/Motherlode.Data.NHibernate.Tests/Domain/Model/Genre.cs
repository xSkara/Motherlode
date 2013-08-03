namespace Motherlode.Data.NHibernate.Tests.Domain.Model
{
    public class Genre : Entity
    {
        #region Public Properties

        [DomainSignature]
        public virtual string Name { get; set; }

        #endregion
    }
}
