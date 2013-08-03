using System.Collections.Generic;
using Motherlode.Data.NHibernate.Tests.Cfg.Providers;
using Motherlode.Data.NHibernate.Tests.Domain.Model;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Metadata;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace Motherlode.Data.NHibernate.Tests.Domain.Tests.Schema
{
    [TestFixture]
    public class SchemaTestFixture
    {
        #region Constants and Fields

        private Configuration _cfg;

        #endregion

        #region Public Methods and Operators

        [Test]
        public void AllNHibernateMappingAreOkay()
        {
            ISessionFactory sessionFactory = this._cfg.BuildSessionFactory();


            using (ISession session = sessionFactory.OpenSession())
            {
                IDictionary<string, IClassMetadata> allClassMetadata = session.SessionFactory.GetAllClassMetadata();

                foreach (var entry in allClassMetadata)
                {
                    session.CreateCriteria(entry.Key)
                           .SetMaxResults(0).List();
                }
            }
        }

        [SetUp]
        public void SetUp()
        {
            this._cfg = new IdentityConfigurationProvider(typeof(IncorrectGenre)).Create();
        }

        [Test]
        public void ValidateSchema()
        {
            var schemaValidator = new SchemaValidator(this._cfg);
            schemaValidator.Validate();
        }

        #endregion
    }
}
