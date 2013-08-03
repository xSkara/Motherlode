namespace Motherlode.Data.NHibernate.Tests.Domain.Model
{
    public class IncorrectGenre : Entity
    {
        #region Public Properties

        [DependsOn("Foo")]
        public virtual int Bar
        {
            get
            {
                return 42;
            }
        }

        [DomainSignature]
        public virtual string Name { get; set; }

        #endregion
    }
}