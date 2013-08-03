namespace Motherlode.Data.NHibernate.Tests.Domain.Model
{
    public class MediaType : Entity
    {
        #region Public Properties

        [DomainSignature]
        public virtual string Name { get; set; }

        #endregion
    }
}
