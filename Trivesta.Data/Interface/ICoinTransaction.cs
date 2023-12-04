﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivesta.Model;

namespace Trivesta.Data.Interface
{
    public interface ICoinTransaction : IGeneric<CoinTransaction>
    {
        Task<IEnumerable<CoinTransaction>> GetByUserID(Guid userID);
    }
}
