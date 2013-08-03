using FluentNHibernate.Mapping;
using Motherlode.Data.NHibernate.Tests.Domain.Model;

namespace Motherlode.Data.NHibernate.Tests.Domain.FluentMappings
{
    /// <summary>
    ///     A Fluent NHobernate mapper class for the Driver entity.
    /// </summary>
    public class GenreMap : ClassMap<Genre>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GenreMap" /> class.
        /// </summary>
        public GenreMap()
        {
            this.Id(x => x.Id).GeneratedBy.Identity().Column("GenreId");
            this.Map(x => x.Name);
        }

        #endregion
    }
}
