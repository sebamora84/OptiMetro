using System;
using System.Collections.Generic;
using System.Text;

namespace OptiMetro.Model
{
    public static class StationExtensions
    {
        public static bool IsValidStation(this Station station, string trainColor)
        {
            return station.Color == trainColor || string.IsNullOrEmpty(station.Color) || string.IsNullOrEmpty(trainColor);
        }
        public static bool IsValidOrigin(this StationLink link, string trainColor)
        {
            return link.Origin.IsValidStation(trainColor) ;
        }
        public static bool IsValidDestination(this StationLink link, string trainColor)
        {
            return link.Destination.IsValidStation(trainColor);
        }
        public static bool IsValidLink(this StationLink link, string trainColor)
        {
            return link.IsValidOrigin(trainColor) && link.IsValidDestination(trainColor);
        }
        public static bool IsLinkRepeated(this StationLink stationLink,  StationLink stationLinkRepeated)
        {
            return stationLink.Origin == stationLinkRepeated.Origin && stationLink.Destination == stationLinkRepeated.Destination;
        }
    }
}
