using System.Collections.Generic;

namespace Motherlode.Data.NHibernate.Tests.Domain.Model
{
    public class Album : Entity
    {
        #region Constructors and Destructors

        public Album()
        {
            this.Tracks = new List<Track>();
        }

        #endregion

        #region Public Properties

        [DomainSignature]
        public virtual Artist Artist { get; set; }

        [DomainSignature]
        public virtual string Title { get; set; }

        [DomainSignature]
        public virtual IList<Track> Tracks { get; protected set; }

        #endregion

        #region Public Methods and Operators

        public virtual void AddTrack(Track track)
        {
            track.Album = this;
            this.Tracks.Add(track);
        }

        #endregion
    }
}
