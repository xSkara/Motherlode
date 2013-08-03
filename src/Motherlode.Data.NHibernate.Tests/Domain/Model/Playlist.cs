using System.Collections.Generic;

namespace Motherlode.Data.NHibernate.Tests.Domain.Model
{
    public class Playlist : Entity
    {
        #region Constructors and Destructors

        public Playlist()
        {
            this.Tracks = new List<Track>();
        }

        #endregion

        #region Public Properties

        [DomainSignature]
        public virtual string Name { get; set; }

        [DomainSignature]
        public virtual ICollection<Track> Tracks { get; protected set; }

        #endregion

        #region Public Methods and Operators

        public virtual void AddTrack(Track track)
        {
            this.Tracks.Add(track);
        }

        #endregion
    }
}
