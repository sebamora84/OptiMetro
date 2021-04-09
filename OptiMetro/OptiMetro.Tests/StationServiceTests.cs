using NSubstitute;
using OptiMetro.Configuration.Interfaces;
using OptiMetro.Model;
using OptiMetro.Optimization;
using OptiMetro.Optimization.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace OptiMetro.Tests
{
    public class StationServiceTests
    {

        #region Mocking
        IStationConfigurationProvider _stationConfigurationProviderMock = Substitute.For<IStationConfigurationProvider>();

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
            } }
        };
        public static IEnumerable<object[]> GetValidStationScenario()
        {
            foreach (Station station in _stationsRepository.Values)
                yield return new object[] { station.Name };
        }
        public static IEnumerable<object[]> GetUnknownStationScenario()
        {
            foreach (var station in new object[] { "X", "Y", "Z" })
                yield return new object[] { station };
        }
        public static IEnumerable<object[]> GetNullStationScenario()
        {
            yield return new object[] { null };
        }

        public static IEnumerable<object[]> GetValidColorScenario()
        {
            foreach(var color in new object[] { "Green", "Red" })
                yield return new object[] { color } ;
        }

        public static IEnumerable<object[]> GetInvalidColorScenario()
        {
            foreach (var color in new object[] { "Cyan", "Magenta" })
                yield return new object[] { color };
        }
        public static IEnumerable<object[]> GetEmptyColorScenario()
        {
            yield return new object[] { string.Empty };
        }

        public static IEnumerable<object[]> GetNullColorScenario()
        {
            yield return new object[] { null };
        }
        #endregion


        [Theory]
        [MemberData(nameof(GetValidStationScenario))]
        public void GetStationByName_IsValidStation_ShouldBeValidStation(string stationName)
        {
            //Arrange
            _stationConfigurationProviderMock.GetStations().Returns(_stationsRepository.Values.ToList());            
            IStationService stationService = new StationService(_stationConfigurationProviderMock);
            //Act
            Station station = stationService.GetStationByName(stationName);
            //Assert
            Assert.NotNull(station);
            Assert.True(station.Name == stationName);
        }
        [Theory]
        [MemberData(nameof(GetUnknownStationScenario))]
        public void GetStationByName_IsUnknownStation_ShouldBeNullStation(string stationName)
        {
            //Arrange
            _stationConfigurationProviderMock.GetStations().Returns(_stationsRepository.Values.ToList());
            IStationService stationService = new StationService(_stationConfigurationProviderMock);
            //Act
            Station station = stationService.GetStationByName(stationName);
            //Assert
            Assert.Null(station);
        }
        [Theory]
        [MemberData(nameof(GetNullStationScenario))]
        public void GetStationByName_IsNullStation_ShouldBeNullStation(string stationName)
        {
            //Arrange
            _stationConfigurationProviderMock.GetStations().Returns(_stationsRepository.Values.ToList());
            IStationService stationService = new StationService(_stationConfigurationProviderMock);
            //Act
            Station station = stationService.GetStationByName(stationName);
            //Assert
            Assert.Null(station);
        }

        [Theory]
        [MemberData(nameof(GetEmptyColorScenario))]
        public void GetValidStationLinks_IsEmptyColor_ShouldBeAllLinks(string trainColor)
        {
            //Arrange
            _stationConfigurationProviderMock.GetStations().Returns(_stationsRepository.Values.ToList());
            _stationConfigurationProviderMock.GetStationLinks().Returns(_linksRepository[string.Empty]);
            IStationService stationService = new StationService(_stationConfigurationProviderMock);
            //Act
            List<StationLink> links = stationService.GetValidStationLinks(trainColor);
            //Assert
            Assert.Equal(_linksRepository[string.Empty].Count, links.Count);
        }
        [Theory]
        [MemberData(nameof(GetNullColorScenario))]
        public void GetValidStationLinks_IsNullColor_ShouldBeAllLinks(string trainColor)
        {
            //Arrange
            _stationConfigurationProviderMock.GetStations().Returns(_stationsRepository.Values.ToList());
            _stationConfigurationProviderMock.GetStationLinks().Returns(_linksRepository[string.Empty]);
            IStationService stationService = new StationService(_stationConfigurationProviderMock);
            //Act
            List<StationLink> links = stationService.GetValidStationLinks(trainColor);
            //Assert
            Assert.Equal(_linksRepository[string.Empty].Count, links.Count);
        }
        [Theory]
        [MemberData(nameof(GetValidColorScenario))]
        public void GetValidStationLinks_IsValidColor_ShouldBeValidLinks(string trainColor)
        {
            //Arrange
            _stationConfigurationProviderMock.GetStations().Returns(_stationsRepository.Values.ToList());
            _stationConfigurationProviderMock.GetStationLinks().Returns(_linksRepository[string.Empty]);
            IStationService stationService = new StationService(_stationConfigurationProviderMock);
            //Act
            List<StationLink> links = stationService.GetValidStationLinks(trainColor);
            //Assert
            int validCount = _linksRepository[string.Empty].Where(sl => sl.IsValidLink(trainColor)).Count();
            Assert.Equal(validCount, links.Count);
        }
        [Theory]
        [MemberData(nameof(GetInvalidColorScenario))]
        public void GetValidStationLinks_IsInvalidColor_ShouldBeValidLinks(string trainColor)
        {
            //Arrange
            _stationConfigurationProviderMock.GetStations().Returns(_stationsRepository.Values.ToList());
            _stationConfigurationProviderMock.GetStationLinks().Returns(_linksRepository[string.Empty]);
            IStationService stationService = new StationService(_stationConfigurationProviderMock);
            //Act
            List<StationLink> links = stationService.GetValidStationLinks(trainColor);
            //Assert
            int validCount = _linksRepository[string.Empty].Where(sl => sl.IsValidLink(trainColor)).Count();
            Assert.Equal(validCount, links.Count);
        }
    }
}
