﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivesta.Model;

namespace Trivesta.Data.Interface
{
    public interface INotification : IGeneric<Notification>
    {
        Task<int> CheckForNotification(Guid userID);
        Task<IEnumerable<Notification>> GetByUserID(Guid userID);
    }
}
