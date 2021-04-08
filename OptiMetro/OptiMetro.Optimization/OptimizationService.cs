using System;
using System.Collections.Generic;
using System.Linq;
using OptiMetro.Configuration.Interfaces;
using OptiMetro.Model;
using OptiMetro.Optimization.Interfaces;
namespace OptiMetro.Optimization
{
    public class OptimizationService : IOptimizationService
    {
        private readonly IStationService _stationService;

        public OptimizationService(IStationService stationService)
        {
            _stationService = stationService;
        }

        public List<Station> OptimizeRoute(string trainColor, string stationStart, string stationEnd)
        {
            Station start = _stationService.GetStationByName(stationStart);
            if (start == null)
            {
                return null;
            }
            if (!start.IsValidStation(trainColor))
            {
                return null;
            }

            Station end = _stationService.GetStationByName(stationEnd);
            if (end == null)
            {
                return null;
            }
            if (!end.IsValidStation(trainColor))
            {
                return null;
            }

            List<StationLink> links = _stationService.GetValidStationLinks(trainColor);
            return GetDijkstraOptimizedPath(links, start, end);
        }

        private List<Station> GetDijkstraOptimizedPath(List<StationLink> links, Station start, Station end)
        {
            List<Station> unvisitedStations = GetStationsFromLinks(links);
            if (!unvisitedStations.Contains(start))
            {
                return null;
            }
            if (!unvisitedStations.Contains(end))
            {
                return null;
            }

            List<Station> visitedStations = new List<Station>();
            Dictionary<Station, StationDistance> stationDistances = GenerateStationDistances(unvisitedStations);
            stationDistances[start].DistanceFromStart = 0;

            while (unvisitedStations.Count > 0)
            {
                Station currentStation = GetNextStation(unvisitedStations, stationDistances);
                var currentStationLinks = links.Where(l => l.Origin == currentStation && !visitedStations.Contains(l.Destination));
                foreach (var link in currentStationLinks)
                {
                    var currentStationDistance = stationDistances[currentStation];
                    var distanceToLinkDestination = currentStationDistance.DistanceFromStart + 1;

                    var linkDestinationDistance = stationDistances[link.Destination];

                    if (distanceToLinkDestination < linkDestinationDistance.DistanceFromStart)
                    {
                        linkDestinationDistance.DistanceFromStart = distanceToLinkDestination;
                        linkDestinationDistance.PreviousStation = currentStation;
                    }
                }
                visitedStations.Add(currentStation);
                unvisitedStations.Remove(currentStation);
            }
            return GetRouteToEnd(stationDistances, start, end);
        }

        private static Dictionary<Station, StationDistance> GenerateStationDistances(List<Station> unvisitedStations)
        {
            return unvisitedStations.ToDictionary(s => s, s => new StationDistance());
        }

        private static List<Station> GetStationsFromLinks(List<StationLink> links)
        {
            return links.Select(l => l.Origin).Union(links.Select(l => l.Destination)).Distinct().ToList();
        }

        private static List<Station> GetRouteToEnd(Dictionary<Station, StationDistance> stationDistances, Station start,  Station end)
        {
            var current = end;
            var routeToEnd = new List<Station>() ;
            while (current != null)
            {
                routeToEnd.Add(current);
                current = stationDistances[current].PreviousStation;
            }

            if (routeToEnd.LastOrDefault() != start)
            {
                return null;
            }

            routeToEnd.Reverse();            
            return routeToEnd;
        }

        private static Station GetNextStation(List<Station> unvisitedStations, Dictionary<Station, StationDistance> stationDistances)
        {
            var unvisitedStationDistances = stationDistances.Where(sd => unvisitedStations.Contains(sd.Key));
            var minDistanceValue = unvisitedStationDistances.Min(sd => sd.Value.DistanceFromStart);
            var minDistance = unvisitedStationDistances.FirstOrDefault(sd => sd.Value.DistanceFromStart == minDistanceValue);
            return minDistance.Key;
        }
        

        private class StationDistance
        {
            public int DistanceFromStart { get; set; } = int.MaxValue;
            public Station PreviousStation { get; set; }
        }
    }
}
