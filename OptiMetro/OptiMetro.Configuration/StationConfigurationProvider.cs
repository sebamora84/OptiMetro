using OptiMetro.Configuration.Interfaces;
using OptiMetro.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OptiMetro.Configuration
{
    public class StationConfigurationProvider : IStationConfigurationProvider
    {
        private string _configurationFile;
        private List<StationLink> _stationLinks = new List<StationLink>();
        private Dictionary<string, Station> _stations = new Dictionary<string, Station>();
        public StationConfigurationProvider(string configurationFile)
        {
            _configurationFile = configurationFile;
            var lines = LoadFile();
            GenerateStationLinks(lines);
        }

        public List<StationLink> GetStationLinks()
        {
            return _stationLinks;
        }

        public List<Station> GetStations()
        {
            return _stations.Values.ToList();
        }
        private void GenerateStationLinks(List<string> lines)
        {
            CreateStations(lines);
            CreateStationLinks(lines);
        }

        private void CreateStationLinks(List<string> lines)
        {
            var destinationStationNames = lines[0].Split(";");
            foreach (var line in lines.Skip(1))
            {
                var fields = line.Split(";");
                var stationOriginName = fields[1] ?? string.Empty;
                var stationOrigin = _stations[stationOriginName];
                for(int i=2; i<fields.Count(); i++)
                {
                    if (fields[i] != "1")
                    {
                        continue;
                    }
                    var stationDestination = _stations[destinationStationNames[i]];
                    AddNewLink(stationOrigin, stationDestination);
                    AddNewLink(stationDestination, stationOrigin);
                }
            }
        }

        private void AddNewLink(Station stationOrigin, Station stationDestination)
        {
            var newLink = new StationLink { Origin = stationOrigin, Destination = stationDestination };
            if (_stationLinks.Any(sl => sl.IsLinkRepeated(newLink)))
            {
                return;
            }
            _stationLinks.Add(newLink);
        }

        private void CreateStations(List<string> lines)
        {
            foreach (var line in lines.Skip(1))
            {
                var fields = line.Split(";");
                var stationColor = fields[0] ?? string.Empty;
                var stationName = fields[1] ?? string.Empty;

                if (_stations.ContainsKey(stationName))
                {
                    _stations[stationName].Color = stationColor;
                    continue;
                }
                
                _stations.Add(stationName, new Station { Name = stationName, Color = stationColor });                
            }

            var destinationStationNames = lines[0].Split(";").Skip(2);
            foreach (var stationName in destinationStationNames)
            {
                if (_stations.ContainsKey(stationName))
                {
                    continue;
                }
                _stations.Add(stationName, new Station { Name = stationName, Color = string.Empty });
            }
        }

        private List<string> LoadFile()
        {
            if(!File.Exists(_configurationFile)){
                Console.WriteLine($"station configuration file does not exist {_configurationFile}");
                return new List<string>();
            }

            var lines = new List<string>();
            using (var reader = new StreamReader(_configurationFile))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    lines.Add(line);
                }
            }
            return lines;
        }
    }
}
