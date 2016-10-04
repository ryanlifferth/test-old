﻿using Ryan.AddressUtility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.AddressUtility.Interfaces
{
    public interface IDistanceCalculation
    {

        DistanceResponse CalculateDistances(string originAddress, List<string> destinationAddresses);

    }

}
