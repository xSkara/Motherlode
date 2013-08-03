using System;
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
using Motherlode.Data.NHibernate.Wpf.Interceptor;
using Motherlode.Data.NHibernate.Wpf.Interceptor.ObjectFactory;
using Motherlode.Data.NHibernate.Wpf.ProxyFactoryFactory;
using NHibernate.Cfg;
using NHibernate.Context;

namespace Motherlode.Data.NHibernate.Tests.Cfg.Providers
{
    public class HiLoWpfConfigurationProvider : IConfigurationProvider
    {
        #region Constants and Fields

        private const int AdoNetBatchSize = 10;
        private readonly bool _editableNotifiableProxyFactory;

        private readonly bool _lazyLoading;
        private readonly bool _notifiableProxyFactory;
        private readonly bool _useAdoBatch;

        #endregion

        #region Constructors and Destructors

        public HiLoWpfConfigurationProvider(
            bool useAdoBatch = false,
            bool lazyLoading = true,
            bool notifiableProxyFactory = false,
            bool editableNotifiableProxyFactory = false)
        {
            this._editableNotifiableProxyFactory = editableNotifiableProxyFactory;
            this._lazyLoading = lazyLoading;
            this._useAdoBatch = useAdoBatch;
            this._notifiableProxyFactory = notifiableProxyFactory;

            this.ObjectsFactoryInterceptorGenerator = () => new ObjectFactoryInterceptor(new NotifiableObjectsFactory());
        }

        #endregion

        #region Public Properties

        public Func<ObjectFactoryInterceptor> ObjectsFactoryInterceptorGenerator { get; set; }

        #endregion

        #region Public Methods and Operators

        public Configuration Create()
        {
            DirectoryInfo di = Directory.CreateDirectory(Path.Combine(Environment.TargetDirectory, "AutoMappings"));

            FluentConfiguration cfg = Fluently.Configure()
                                              .Database(this.createDatabaseConfiguration())
                                              .Mappings(
                                                  m => m.AutoMappings.Add(this.createAutoMapModel()).ExportTo(di.FullName))
                                              .CollectionTypeFactory<WpfCollectionTypeFactory>()
                                              .ExposeConfiguration(
                                                  config => config.SetInterceptor(this.ObjectsFactoryInterceptorGenerator()))
                                              .CurrentSessionContext<ThreadStaticSessionContext>()
                // ExposeConfiguration does not work for the CollectionTypeFactory property.
                // Possibly because of property application order in the FluentConfiguration.BuildConfiguration method.
                /*.ExposeConfiguration(
                    cfg =>
                    cfg.SetProperty(
                        NHibernate.Cfg.Environment.CollectionTypeFactoryClass,
                        typeof(WpfCollectionTypeFactory).AssemblyQualifiedName))*/;

            if (this._notifiableProxyFactory)
            {
                cfg.ProxyFactoryFactory<NotifableProxyFactoryFactory>();
            }

            if (this._editableNotifiableProxyFactory)
            {
                cfg.ProxyFactoryFactory<EditableNotifableProxyFactoryFactory>();
            }

            return cfg.BuildConfiguration();
        }

        #endregion

        #region Methods

        private AutoPersistenceModel createAutoMapModel()
        {
            AutoPersistenceModel model = AutoMap
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
                            builder.AddParam("Where", string.Format("EntityName = '{0}'", instance.EntityType.Name)))))
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

            if (this._lazyLoading)
            {
                model.Conventions.Add(DefaultLazy.Always());
            }
            else
            {
                model.Conventions.Add(DefaultLazy.Never());
            }

            return model;
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
