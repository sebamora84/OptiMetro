using OptiMetro.Configuration;
using OptiMetro.Configuration.Interfaces;
using OptiMetro.Model;
using OptiMetro.Optimization;
using OptiMetro.Optimization.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace OptiMetro
{
    class Program
    {
        static void Main(string[] args)
        {
            string configurationFile = "StationMatrix.csv";
            string startStationName = "A";
            string endStationName = "F";
            string trainColor = string.Empty;
            for (int i = 0; i<args.Length; i++)
            {
                if (!args[i].StartsWith('-'))
                {
                    continue;
                }

                switch (args[i].ToLower())
                {
                    case "-p":
                    case "--path":
                            configurationFile = args[i + 1];
                        break;
                    case "-s":
                    case "--start":
                            startStationName = args[i + 1];
                        break;
                    case "-e":
                    case "--end":
                            endStationName = args[i + 1];
                        break;
                    case "-c":
                    case "--color":
                            trainColor = args[i + 1];
                        break;
                }
            }

            IStationConfigurationProvider stationConfigurationProvider = new StationConfigurationProvider(configurationFile);
            IStationService stationService = new StationService(stationConfigurationProvider);
            IOptimizationService optimizationService = new OptimizationService(stationService);

            List<Station> stations = optimizationService.OptimizeRoute(trainColor, startStationName, endStationName);
            if (stations == null)
            {
                Console.WriteLine($"No optimal route found");
            }
            else
            {
                Console.WriteLine($"Optimal route found: {string.Join('>', stations.Select(s => s.Name).ToArray())}");
            }
            
            Console.WriteLine("Press a key to finish");
            Console.ReadLine();
        }

        
    }
}
