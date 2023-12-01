using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivesta.Data.Interface;
using Trivesta.Model;

namespace Trivesta.Data.Repository
{
    public class MediaRepository : GenericRepository<Media>, IMedia
    {
        public MediaRepository(TrivestaContext db) : base(db) { }
    }
}
