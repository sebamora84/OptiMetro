
using System;
using System.Collections.Generic;
using System.Linq;
using OptiMetro.Model;
namespace OptiMetro.Optimization.Interfaces
{
    public interface IOptimizationService
    {
       List<Station> OptimizeRoute(string trainColor, string stationStart, string stationEnd);
    }
}
