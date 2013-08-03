using System;
using System.IO;
using System.Linq;
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

namespace Motherlode.Data.NHibernate.Tests.Cfg.Providers
{
    public class IdentityConfigurationProvider : IConfigurationProvider
    {
        private readonly Type[] typesToIgnoreAutoMapping;

        #region Public Methods and Operators

        public IdentityConfigurationProvider()
        {
            this.typesToIgnoreAutoMapping = new[] { typeof(Genre), typeof(Playlist) };
        }

        public IdentityConfigurationProvider(params Type[] typesToIgnoreAutoMapping) : this()
        {
            this.typesToIgnoreAutoMapping = this.typesToIgnoreAutoMapping.Union(typesToIgnoreAutoMapping).ToArray();
        }

        public Configuration Create()
        {
            DirectoryInfo di = Directory.CreateDirectory(Path.Combine(Environment.TargetDirectory, "AutoMappings"));

            return Fluently.Configure()
                           .Database(
                               MsSqlCeConfiguration.Standard
                                                   .Dialect<FixedMsSqlCe40Dialect>()
                                                   .ConnectionString(
                                                       c => c.FromConnectionStringWithKey("default_AutoIncrementPKs"))
                                                   .ShowSql())
                           .Mappings(
                               m =>
                               {
                                   m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly());
                                   m.HbmMappings.AddFromAssembly(Assembly.GetExecutingAssembly());
                                   m.AutoMappings.Add(this.createAutoMapModel()).ExportTo(di.FullName);
                               })
                           .CollectionTypeFactory<WpfCollectionTypeFactory>()
                           .ExposeConfiguration(x => x.SetProperty("connection.release_mode", "on_close")).BuildConfiguration();
        }

        #endregion

        #region Methods

        private AutoPersistenceModel createAutoMapModel()
        {
            return AutoMap
                .Assembly(Assembly.GetExecutingAssembly(), new AutomappingConfiguration(this.typesToIgnoreAutoMapping))
                .Conventions.Add(
                    PrimaryKey.Name.Is(i => i.EntityType.Name + "Id"),
                    ForeignKey.EndsWith("Id"),
                    ConventionBuilder.Id.Always(instance => instance.GeneratedBy.Native()),
                    ConventionBuilder.HasMany.Always(convention => convention.Inverse()),
                    ConventionBuilder.Property.When(
                        criteria => criteria.Expect(i => i.Type == typeof(decimal)),
                        instance =>
                        {
                            instance.Precision(2);
                            instance.Length(10);
                            instance.CustomSqlType("numeric");
                        }),
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

        #endregion
    }
}
