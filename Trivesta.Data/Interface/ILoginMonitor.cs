using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivesta.Model;

namespace Trivesta.Data.Interface
{
    public interface ILoginMonitor : IGeneric<LoginMonitor>
    {
        Task<LoginMonitor> GetMonitorByUserID(Guid userID, Guid appID);
        Task<LoginMonitor> GetMonitorByUserIDOnly(Guid userID);
    }
}
