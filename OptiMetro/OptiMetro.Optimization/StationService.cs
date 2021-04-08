using OptiMetro.Configuration.Interfaces;
using OptiMetro.Model;
using OptiMetro.Optimization.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptiMetro.Optimization
{
    public class StationService : IStationService
    {
        private IStationConfigurationProvider _stationConfigurationProvider;

        public StationService(IStationConfigurationProvider stationConfigurationProvider)
        {
            _stationConfigurationProvider = stationConfigurationProvider;
        }

        public Station GetStationByName(string stationName)
        {
            List<Station> stations = _stationConfigurationProvider.GetStations();
            return stations.FirstOrDefault(s => s.Name == stationName);
        }

        public List<StationLink> GetValidStationLinks(string trainColor)
        {
            List<StationLink> stationLinks = _stationConfigurationProvider.GetStationLinks();
            List<Station> invalidStations = _stationConfigurationProvider.GetStations().Where(s => !s.IsValidStation(trainColor)).ToList();

            while (stationLinks.Any(sl=> !sl.IsValidLink(trainColor)))
            {
                foreach (Station invalidStation in invalidStations)
                {
                    ReplaceInvalidStationLinks(stationLinks, invalidStation);
                }
            }
            return stationLinks;
        }

        private void ReplaceInvalidStationLinks(List<StationLink> stationLinks, Station invalidStation)
        {
            List<StationLink> invalidDestinationLinks = stationLinks.Where(sl => sl.Destination == invalidStation).ToList();
            List<StationLink> invalidOriginLinks = stationLinks.Where(sl => sl.Origin == invalidStation).ToList();
            var virtualLinks = GenerateVirtualLinks(invalidDestinationLinks, invalidOriginLinks);

            virtualLinks.RemoveAll(vl => stationLinks.Any(sl=> sl.IsLinkRepeated(vl)));
            stationLinks.RemoveAll(sl => invalidDestinationLinks.Contains(sl) || invalidOriginLinks.Contains(sl));
            stationLinks.AddRange(virtualLinks);
        }

        private List<StationLink> GenerateVirtualLinks(List<StationLink> invalidDestinationLinks, List<StationLink> invalidOriginLinks)
        {
            List<StationLink> virtualLinks = new List<StationLink>();
            foreach (StationLink invalidDestinationLink in invalidDestinationLinks)
            {
                Station origin = invalidDestinationLink.Origin;
                foreach (StationLink invalidOriginLink in invalidOriginLinks)
                {
                    Station destination = invalidOriginLink.Destination;
                    StationLink virtualLink = new StationLink { Origin = origin, Destination = destination };
                    virtualLinks.Add(virtualLink);
                }
            }
            return virtualLinks;
        }
        
       
    }
}
