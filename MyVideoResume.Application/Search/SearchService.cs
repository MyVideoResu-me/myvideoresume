using MyVideoResume.Abstractions.Core;
using MyVideoResume.Data.Models.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Application.Search;

public class SearchService<T> where T : GISData
{
    // Function to calculate distance between two coordinates using Haversine formula
    public static double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 3958.8; // Radius of the Earth in miles

        double lat1Rad = DegreesToRadians(lat1);
        double lon1Rad = DegreesToRadians(lon1);
        double lat2Rad = DegreesToRadians(lat2);
        double lon2Rad = DegreesToRadians(lon2);

        double deltaLat = lat2Rad - lat1Rad;
        double deltaLon = lon2Rad - lon1Rad;

        double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c; // Distance in miles
    }

    // Function to convert degrees to radians
    public static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    // Function to get cities within a specific radius
    public static List<T> GetItemsWithinRadius(List<T> items, double originLat, double originLong, double radius)
    {
        var result = new List<T>();

        foreach (var item in items)
        {
            if (item.Latitude.HasValue && item.Longitude.HasValue)
            {
                double distance = GetDistance(originLat, originLong, item.Latitude.Value, item.Longitude.Value);

                if (distance <= radius)
                {
                    result.Add(item);
                }
            }
        }

        return result;
    }
}
