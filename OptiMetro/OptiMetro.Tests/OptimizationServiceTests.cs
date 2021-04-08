using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using OptiMetro.Model;
using OptiMetro.Optimization;
using OptiMetro.Optimization.Interfaces;
using OptiMetro.Configuration.Interfaces;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace OptiMetro.Tests
{
    public class OptimizationServiceTests
    {
        #region Mocking
        private readonly IStationService _stationServiceMock = Substitute.For<IStationService>();

        private static List<string> _colors = new List<string> {
            string.Empty,
            "Red",
            "Green" };
        private static Station _stationA = new Station() { Name = "A", Color = string.Empty };
        private static Station _stationB = new Station() { Name = "B", Color = string.Empty };
        private static Station _stationC = new Station() { Name = "C", Color = string.Empty };
        private static Station _stationD = new Station() { Name = "D", Color = string.Empty };
        private static Station _stationE = new Station() { Name = "E", Color = string.Empty };
        private static Station _stationF = new Station() { Name = "F", Color = string.Empty };
        private static Station _stationG = new Station() { Name = "G", Color = "Green" };
        private static Station _stationH = new Station() { Name = "H", Color = "Red" };
        private static Station _stationI = new Station() { Name = "I", Color = "Green" };
        private static Dictionary<string, Station> _stationsRepository = new Dictionary<string, Station> {
            {"A",_stationA},
            {"B",_stationB},
            {"C",_stationC},
            {"D",_stationD},
            {"E",_stationE},
            {"F",_stationF},
            {"G",_stationG},
            {"H",_stationH},
            {"I",_stationI}};
        private static Dictionary<string, List<StationLink>> _linksRepository = new Dictionary<string, List<StationLink>> {
            {string.Empty, new List<StationLink>(){
                new StationLink{ Origin = _stationA, Destination = _stationB},
                new StationLink{ Origin = _stationB, Destination = _stationA},
                new StationLink{ Origin = _stationB, Destination = _stationC},
                new StationLink{ Origin = _stationC, Destination = _stationB},
                new StationLink{ Origin = _stationC, Destination = _stationD},
                new StationLink{ Origin = _stationC, Destination = _stationG},
                new StationLink{ Origin = _stationD, Destination = _stationC},
                new StationLink{ Origin = _stationD, Destination = _stationE},
                new StationLink{ Origin = _stationE, Destination = _stationD},
                new StationLink{ Origin = _stationE, Destination = _stationF},
                new StationLink{ Origin = _stationF, Destination = _stationE},
                new StationLink{ Origin = _stationF, Destination = _stationI},
                new StationLink{ Origin = _stationG, Destination = _stationC},
                new StationLink{ Origin = _stationG, Destination = _stationH},
                new StationLink{ Origin = _stationH, Destination = _stationG},
                new StationLink{ Origin = _stationH, Destination = _stationI},
                new StationLink{ Origin = _stationI, Destination = _stationH},
                new StationLink{ Origin = _stationI, Destination = _stationF},
            } },
            {"Green",  new List<StationLink>(){
                new StationLink{ Origin = _stationA, Destination = _stationB},
                new StationLink{ Origin = _stationB, Destination = _stationA},
                new StationLink{ Origin = _stationB, Destination = _stationC},
                new StationLink{ Origin = _stationC, Destination = _stationB},
                new StationLink{ Origin = _stationC, Destination = _stationD},
                new StationLink{ Origin = _stationC, Destination = _stationG},
                new StationLink{ Origin = _stationD, Destination = _stationC},
                new StationLink{ Origin = _stationD, Destination = _stationE},
                new StationLink{ Origin = _stationE, Destination = _stationD},
                new StationLink{ Origin = _stationE, Destination = _stationF},
                new StationLink{ Origin = _stationF, Destination = _stationE},
                new StationLink{ Origin = _stationF, Destination = _stationI},
                new StationLink{ Origin = _stationG, Destination = _stationC},
                new StationLink{ Origin = _stationG, Destination = _stationI},
                new StationLink{ Origin = _stationI, Destination = _stationG},
                new StationLink{ Origin = _stationI, Destination = _stationF}
        } },
            {"Red", new List<StationLink>(){
                new StationLink{ Origin = _stationA, Destination = _stationB},
                new StationLink{ Origin = _stationB, Destination = _stationA},
                new StationLink{ Origin = _stationB, Destination = _stationC},
                new StationLink{ Origin = _stationC, Destination = _stationB},
                new StationLink{ Origin = _stationC, Destination = _stationD},
                new StationLink{ Origin = _stationC, Destination = _stationH},
                new StationLink{ Origin = _stationD, Destination = _stationC},
                new StationLink{ Origin = _stationD, Destination = _stationE},
                new StationLink{ Origin = _stationE, Destination = _stationD},
                new StationLink{ Origin = _stationE, Destination = _stationF},
                new StationLink{ Origin = _stationF, Destination = _stationE},
                new StationLink{ Origin = _stationF, Destination = _stationH},
                new StationLink{ Origin = _stationH, Destination = _stationC},
                new StationLink{ Origin = _stationH, Destination = _stationF},
            } },
            {"Splitted", new List<StationLink>(){
                new StationLink{ Origin = _stationA, Destination = _stationB},
                new StationLink{ Origin = _stationB, Destination = _stationA},
                new StationLink{ Origin = _stationB, Destination = _stationC},
                new StationLink{ Origin = _stationC, Destination = _stationB},
                new StationLink{ Origin = _stationD, Destination = _stationE},
                new StationLink{ Origin = _stationE, Destination = _stationD},
                new StationLink{ Origin = _stationE, Destination = _stationF},
                new StationLink{ Origin = _stationF, Destination = _stationE},
                new StationLink{ Origin = _stationF, Destination = _stationI},
                new StationLink{ Origin = _stationG, Destination = _stationH},
                new StationLink{ Origin = _stationH, Destination = _stationG},
                new StationLink{ Origin = _stationH, Destination = _stationI},
                new StationLink{ Origin = _stationI, Destination = _stationH},
                new StationLink{ Origin = _stationI, Destination = _stationF},
            } },
            {"Unknown", new List<StationLink>(){
                new StationLink{ Origin = _stationA, Destination = _stationB},
                new StationLink{ Origin = _stationB, Destination = _stationA},
                new StationLink{ Origin = _stationB, Destination = _stationC},
                new StationLink{ Origin = _stationC, Destination = _stationB},
                new StationLink{ Origin = _stationC, Destination = _stationD},
                new StationLink{ Origin = _stationC, Destination = _stationF},
                new StationLink{ Origin = _stationD, Destination = _stationC},
                new StationLink{ Origin = _stationD, Destination = _stationE},
                new StationLink{ Origin = _stationE, Destination = _stationD},
                new StationLink{ Origin = _stationE, Destination = _stationF},
                new StationLink{ Origin = _stationF, Destination = _stationE},
                new StationLink{ Origin = _stationF, Destination = _stationC}
            } }
        };
        
        public static IEnumerable<object[]> GetValidScenario()
        {
            foreach (string color in _colors)
                foreach (Station origin in _stationsRepository.Values.Where(s => s.IsValidStation(color)))
                    foreach (Station destination in _stationsRepository.Values.Where(s => s.IsValidStation(color)))
                        yield return new object[] { color, origin.Name, destination.Name };
        }
        public static IEnumerable<object[]> GetInvalidOriginScenario()
        {
            foreach (string color in _colors.Where(c => c != string.Empty))
                foreach (Station origin in _stationsRepository.Values.Where(s => !s.IsValidStation(color)))
                    foreach (Station destination in _stationsRepository.Values.Where(s => s.IsValidStation(color)))
                        yield return new object[] { color, origin.Name, destination.Name };
        }
        public static IEnumerable<object[]> GetInvalidDestinationScenario()
        {
            foreach (string color in _colors.Where(c => c != string.Empty))
                foreach (Station origin in _stationsRepository.Values.Where(s => s.IsValidStation(color)))
                    foreach (Station destination in _stationsRepository.Values.Where(s => !s.IsValidStation(color)))
                        yield return new object[] { color, origin.Name, destination.Name };
        }
        public static IEnumerable<object[]> GetSplittedValidScenario()
        {
            var leftStations = new string[] { "A", "B", "C" };
            var rightStations = new string[] { "D", "E", "F", "G", "H", "I" };
            foreach (string color in _colors)
                foreach (Station origin in _stationsRepository.Values.Where(s => s.IsValidStation(color) && leftStations.Contains(s.Name)))
                    foreach (Station destination in _stationsRepository.Values.Where(s => s.IsValidStation(color) && rightStations.Contains(s.Name)))
                        yield return new object[] { color, origin.Name, destination.Name };
        }
        public static IEnumerable<object[]> GetUnknownColorScenario()
        {
            var unknownColors = new string[] { "Cyan", "Magenta" };
            foreach (string color in unknownColors)
                foreach (Station origin in _stationsRepository.Values.Where(s => s.IsValidStation(color)))
                    foreach (Station destination in _stationsRepository.Values.Where(s => s.IsValidStation(color)))
                        yield return new object[] { color, origin.Name, destination.Name };
        }
        public static IEnumerable<object[]> GetUnknownOriginScenario()
        {
            var unknownStations = new string[] { "X", "Y", "Z" };
            foreach (string color in _colors)
                foreach (string origin in unknownStations)
                    foreach (Station destination in _stationsRepository.Values.Where(s => s.IsValidStation(color)))
                        yield return new object[] { color, origin, destination.Name };
        }
        public static IEnumerable<object[]> GetUnknownDestinationScenario()
        {
            var unknownStations = new string[] { "X", "Y", "Z" };
            foreach (string color in _colors)
                foreach (Station origin in _stationsRepository.Values.Where(s => s.IsValidStation(color)))
                    foreach (string destination in unknownStations)
                        yield return new object[] { color, origin.Name, destination };
        }
        public static IEnumerable<object[]> GetNullColorScenario()
        {
            var unknownColors = new string[] { null };
            foreach (string color in unknownColors)
                foreach (Station origin in _stationsRepository.Values.Where(s => s.IsValidStation(color)))
                    foreach (Station destination in _stationsRepository.Values.Where(s => s.IsValidStation(color)))
                        yield return new object[] { color, origin.Name, destination.Name };
        }
        public static IEnumerable<object[]> GetNullOriginScenario()
        {
            var unknownStations = new string[] { null };
            foreach (string color in _colors)
                foreach (string origin in unknownStations)
                    foreach (Station destination in _stationsRepository.Values.Where(s => s.IsValidStation(color)))
                        yield return new object[] { color, origin, destination.Name };
        }
        public static IEnumerable<object[]> GetNullDestinationScenario()
        {
            var unknownStations = new string[] { null };
            foreach (string color in _colors)
                foreach (Station origin in _stationsRepository.Values.Where(s => s.IsValidStation(color)))
                    foreach (string destination in unknownStations)
                        yield return new object[] { color, origin.Name, destination };
        }
        #endregion

        [Theory]
        [MemberData(nameof(GetValidScenario))]
        public void OptimizeRoute_IsStart_ShouldBeFirst(string trainColor, string stationStart, string stationEnd)
        {
            //Arrange
            _stationServiceMock.GetStationByName(stationStart).Returns(_stationsRepository[stationStart]);
            _stationServiceMock.GetStationByName(stationEnd).Returns(_stationsRepository[stationEnd]);
            _stationServiceMock.GetValidStationLinks(trainColor).Returns(_linksRepository[trainColor]);
            IOptimizationService optimizer = new OptimizationService(_stationServiceMock);
            //Act
            List<Station> path = optimizer.OptimizeRoute(trainColor, stationStart, stationEnd);
            //Assert
            Station start = path.FirstOrDefault();
            Assert.NotNull(start);
            Assert.Equal(stationStart, start.Name);
        }

        [Theory]
        [MemberData(nameof(GetValidScenario))]
        public void OptimizeRoute_IsEnd_ShouldBeLast(string trainColor, string stationStart, string stationEnd)
        {
            //Arrange
            _stationServiceMock.GetStationByName(stationStart).Returns(_stationsRepository[stationStart]);
            _stationServiceMock.GetStationByName(stationEnd).Returns(_stationsRepository[stationEnd]);
            _stationServiceMock.GetValidStationLinks(trainColor).Returns(_linksRepository[trainColor]);
            IOptimizationService optimizer = new OptimizationService(_stationServiceMock);
            //Act
            List<Station> path = optimizer.OptimizeRoute(trainColor, stationStart, stationEnd);
            //Assert
            Station end = path.LastOrDefault();
            Assert.NotNull(end);
            Assert.Equal(stationEnd, end.Name);
        }

        [Theory]
        [MemberData(nameof(GetValidScenario))]
        public void OptimizeRoute_IsValidStations_ShouldBeValidStations(string trainColor, string stationStart, string stationEnd)
        {
            //Arrange
            _stationServiceMock.GetStationByName(stationStart).Returns(_stationsRepository[stationStart]);
            _stationServiceMock.GetStationByName(stationEnd).Returns(_stationsRepository[stationEnd]);
            _stationServiceMock.GetValidStationLinks(trainColor).Returns(_linksRepository[trainColor]);
            IOptimizationService optimizer = new OptimizationService(_stationServiceMock);
            //Act
            List<Station> path = optimizer.OptimizeRoute(trainColor, stationStart, stationEnd);
            //Assert
            foreach (var station in path)
            {
                Assert.True(station.IsValidStation(trainColor), "station is valid");
            }
        }

        [Theory]
        [MemberData(nameof(GetValidScenario))]
        public void OptimizeRoute_IsValidStations_ShouldBeValidRoute(string trainColor, string stationStart, string stationEnd)
        {
            //Arrange
            _stationServiceMock.GetStationByName(stationStart).Returns(_stationsRepository[stationStart]);
            _stationServiceMock.GetStationByName(stationEnd).Returns(_stationsRepository[stationEnd]);
            _stationServiceMock.GetValidStationLinks(trainColor).Returns(_linksRepository[trainColor]);
            IOptimizationService optimizer = new OptimizationService(_stationServiceMock);
            //Act
            List<Station> path = optimizer.OptimizeRoute(trainColor, stationStart, stationEnd);
            //Assert
            Station currentStation = path.First();
            foreach (Station nextStation in path.Skip(1))
            {
                bool isLinkExisting = _linksRepository[trainColor].Any(sl => sl.Origin == currentStation && sl.Destination == nextStation);
                Assert.True(isLinkExisting, "Link exist between current and next");
                currentStation = nextStation;
            }
        }

        [Theory]
        [MemberData(nameof(GetInvalidOriginScenario))]
        public void OptimizeRoute_IsInvalidOrigin_ShouldBeUnsolvable(string trainColor, string stationStart, string stationEnd)
        {
            //Arrange
            _stationServiceMock.GetStationByName(stationStart).Returns(_stationsRepository[stationStart]);
            _stationServiceMock.GetStationByName(stationEnd).Returns(_stationsRepository[stationEnd]);
            _stationServiceMock.GetValidStationLinks(trainColor).Returns(_linksRepository[trainColor]);
            IOptimizationService optimizer = new OptimizationService(_stationServiceMock);
            //Act
            List<Station> path = optimizer.OptimizeRoute(trainColor, stationStart, stationEnd);
            //Assert            
            Assert.Null(path);
        }

        [Theory]
        [MemberData(nameof(GetInvalidDestinationScenario))]
        public void OptimizeRoute_IsInvalidDestination_ShouldBeUnsolvable(string trainColor, string stationStart, string stationEnd)
        {
            //Arrange
            _stationServiceMock.GetStationByName(stationStart).Returns(_stationsRepository[stationStart]);
            _stationServiceMock.GetStationByName(stationEnd).Returns(_stationsRepository[stationEnd]);
            _stationServiceMock.GetValidStationLinks(trainColor).Returns(_linksRepository[trainColor]);
            IOptimizationService optimizer = new OptimizationService(_stationServiceMock);
            //Act
            List<Station> path = optimizer.OptimizeRoute(trainColor, stationStart, stationEnd);
            //Assert            
            Assert.Null(path);
        }

        [Theory]
        [MemberData(nameof(GetSplittedValidScenario))]
        public void OptimizeRoute_IsUnreachableDestinations_ShouldBeUnsolvable(string trainColor, string stationStart, string stationEnd)
        {
            //Arrange
            _stationServiceMock.GetStationByName(stationStart).Returns(_stationsRepository[stationStart]);
            _stationServiceMock.GetStationByName(stationEnd).Returns(_stationsRepository[stationEnd]);
            _stationServiceMock.GetValidStationLinks(trainColor).Returns(_linksRepository["Splitted"]);
            IOptimizationService optimizer = new OptimizationService(_stationServiceMock);
            //Act
            List<Station> path = optimizer.OptimizeRoute(trainColor, stationStart, stationEnd);
            //Assert            
            Assert.Null(path);
        }

        [Theory]
        [MemberData(nameof(GetNullColorScenario))]
        public void OptimizeRoute_IsNullColor_ShouldBeSolvable(string trainColor, string stationStart, string stationEnd)
        {
            //Arrange
            _stationServiceMock.GetStationByName(stationStart).Returns(_stationsRepository[stationStart]);
            _stationServiceMock.GetStationByName(stationEnd).Returns(_stationsRepository[stationEnd]);
            _stationServiceMock.GetValidStationLinks(trainColor).Returns(_linksRepository[string.Empty]);
            IOptimizationService optimizer = new OptimizationService(_stationServiceMock);
            //Act
            List<Station> path = optimizer.OptimizeRoute(trainColor, stationStart, stationEnd);
            //Assert
            Assert.NotNull(path);
            Assert.True(path.Count > 0);
        }
        [Theory]
        [MemberData(nameof(GetNullOriginScenario))]
        public void OptimizeRoute_IsNullOrigin_ShouldBeUnsolvable(string trainColor, string stationStart, string stationEnd)
        {
            //Arrange
            _stationServiceMock.GetStationByName(stationStart).ReturnsNull();
            _stationServiceMock.GetStationByName(stationEnd).Returns(_stationsRepository[stationEnd]);
            _stationServiceMock.GetValidStationLinks(trainColor).Returns(_linksRepository["Unknown"]);
            IOptimizationService optimizer = new OptimizationService(_stationServiceMock);
            //Act
            List<Station> path = optimizer.OptimizeRoute(trainColor, stationStart, stationEnd);
            //Assert            
            Assert.Null(path);
        }

        [Theory]
        [MemberData(nameof(GetNullDestinationScenario))]
        public void OptimizeRoute_IsNullDestination_ShouldBeUnsolvable(string trainColor, string stationStart, string stationEnd)
        {
            //Arrange
            _stationServiceMock.GetStationByName(stationStart).Returns(_stationsRepository[stationStart]);
            _stationServiceMock.GetStationByName(stationEnd).ReturnsNull();
            _stationServiceMock.GetValidStationLinks(trainColor).Returns(_linksRepository["Unknown"]);
            IOptimizationService optimizer = new OptimizationService(_stationServiceMock);
            //Act
            List<Station> path = optimizer.OptimizeRoute(trainColor, stationStart, stationEnd);
            //Assert            
            Assert.Null(path);
        }

        [Theory]
        [MemberData(nameof(GetUnknownColorScenario))]
        public void OptimizeRoute_IsUnknownColor_ShouldBeSolvable(string trainColor, string stationStart, string stationEnd)
        {
            //Arrange
            _stationServiceMock.GetStationByName(stationStart).Returns(_stationsRepository[stationStart]);
            _stationServiceMock.GetStationByName(stationEnd).Returns(_stationsRepository[stationEnd]);
            _stationServiceMock.GetValidStationLinks(trainColor).Returns(_linksRepository["Unknown"]);
            IOptimizationService optimizer = new OptimizationService(_stationServiceMock);
            //Act
            List<Station> path = optimizer.OptimizeRoute(trainColor, stationStart, stationEnd);
            //Assert
            Assert.NotNull(path);
            Assert.True(path.Count > 0);
        }
        [Theory]
        [MemberData(nameof(GetUnknownOriginScenario))]
        public void OptimizeRoute_IsUnknownOrigin_ShouldBeUnsolvable(string trainColor, string stationStart, string stationEnd)
        {
            //Arrange
            _stationServiceMock.GetStationByName(stationStart).ReturnsNull();
            _stationServiceMock.GetStationByName(stationEnd).Returns(_stationsRepository[stationEnd]);
            _stationServiceMock.GetValidStationLinks(trainColor).Returns(_linksRepository["Unknown"]);
            IOptimizationService optimizer = new OptimizationService(_stationServiceMock);
            //Act
            List<Station> path = optimizer.OptimizeRoute(trainColor, stationStart, stationEnd);
            //Assert            
            Assert.Null(path);
        }

        [Theory]
        [MemberData(nameof(GetUnknownDestinationScenario))]
        public void OptimizeRoute_IsUnknownDestination_ShouldBeUnsolvable(string trainColor, string stationStart, string stationEnd)
        {
            //Arrange
            _stationServiceMock.GetStationByName(stationStart).Returns(_stationsRepository[stationStart]);
            _stationServiceMock.GetStationByName(stationEnd).ReturnsNull();
            _stationServiceMock.GetValidStationLinks(trainColor).Returns(_linksRepository["Unknown"]);
            IOptimizationService optimizer = new OptimizationService(_stationServiceMock);
            //Act
            List<Station> path = optimizer.OptimizeRoute(trainColor, stationStart, stationEnd);
            //Assert            
            Assert.Null(path);
        }
    }
}
