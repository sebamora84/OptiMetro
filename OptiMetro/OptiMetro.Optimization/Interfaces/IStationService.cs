using OptiMetro.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptiMetro.Optimization.Interfaces
{
    public interface IStationService
    {
        Station GetStationByName(string startStationName);
        List<StationLink> GetValidStationLinks(string trainColor);
    }
}
