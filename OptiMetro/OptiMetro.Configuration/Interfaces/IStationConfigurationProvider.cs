using OptiMetro.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptiMetro.Configuration.Interfaces
{
    public interface IStationConfigurationProvider
    {
        List<StationLink> GetStationLinks();
        List<Station> GetStations();
    }
}
