using System.IO;
using System.Reflection;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using Motherlode.Data.NHibernate.AutoMapping;
using Motherlode.Data.NHibernate.Dialect;
using Motherlode.Data.NHibernate.Tests.Domain.Model;
using Motherlode.Data.NHibernate.Wpf.CollectionsTypeFactory;
using NHibernate.Cfg;
using NHibernate.Context;

namespace Motherlode.Data.NHibernate.Tests.Cfg.Providers
{
    public class HiLoConfigurationProvider : IConfigurationProvider
    {
        #region Constants and Fields

        private const int AdoNetBatchSize = 10;

        private readonly bool _useAdoBatch;

        #endregion

        #region Constructors and Destructors

        public HiLoConfigurationProvider(bool useAdoBatch)
        {
            this._useAdoBatch = useAdoBatch;
        }

        public HiLoConfigurationProvider()
        {
            this._useAdoBatch = false;
        }

        #endregion

        #region Public Methods and Operators

        public Configuration Create()
        {
            DirectoryInfo di = Directory.CreateDirectory(Path.Combine(Environment.TargetDirectory, "AutoMappings"));

            return Fluently.Configure()
                           .Database(this.createDatabaseConfiguration())
                           .Mappings(m => m.AutoMappings.Add(createAutoMapModel()).ExportTo(di.FullName))
                           .CollectionTypeFactory<WpfCollectionTypeFactory>()
                           .CurrentSessionContext<ThreadStaticSessionContext>().BuildConfiguration();
        }

        #endregion

        #region Methods

        private static AutoPersistenceModel createAutoMapModel()
        {
            return AutoMap
                .Assembly(Assembly.GetExecutingAssembly(), new AutomappingConfiguration())
                .Conventions.Add(
                    PrimaryKey.Name.Is(i => i.EntityType.Name + "Id"),
                    ForeignKey.EndsWith("Id"),
                    ConventionBuilder.HasMany.Always(convention => convention.Inverse()),
                    ConventionBuilder.Property.When(
                        criteria => criteria.Expect(i => i.Type == typeof(decimal)),
                        instance =>
                        {
                            instance.Precision(2);
                            instance.Length(10);
                            instance.CustomSqlType("numeric");
                        }),
                    ConventionBuilder.Id.Always(
                        instance => instance.GeneratedBy.HiLo(
                            "HibernateHiValue",
                            "HiValue",
                            "3",
                            builder =>
                            builder.AddParam("Where", string.Format("EntityName = '{0}'", instance.EntityType.Name)))),
                    DefaultLazy.Always())
                .Conventions.Add<StringLengthAttributeConvention>()
                .Conventions.Add<RequiredAttributeConvention>()
                .Override<Playlist>(
                    mapping =>
                    mapping
                        .HasManyToMany(x => x.Tracks)
                        .Table("PlaylistTrack")
                        .ParentKeyColumn("PlaylistId")
                        .ChildKeyColumn("TrackId")
                        .AsSet())
                .Override<Customer>(
                    mapping => mapping.References(x => x.SupportRepresentant).Column("SupportRepId"))
                .Override<Employee>(
                    mapping => mapping.References(x => x.ReportsTo).Column("ReportsTo"))
                .IgnoreBase<Person>();
        }

        private MsSqlCeConfiguration createDatabaseConfiguration()
        {
            MsSqlCeConfiguration cfg = MsSqlCeConfiguration.Standard
                                                           .Dialect<FixedMsSqlCe40Dialect>()
                                                           .ConnectionString(c => c.FromConnectionStringWithKey("default"))
                                                           .ShowSql();

            if (this._useAdoBatch)
            {
                cfg = cfg.AdoNetBatchSize(AdoNetBatchSize);
            }

            return cfg;
        }

        #endregion
    }
}
