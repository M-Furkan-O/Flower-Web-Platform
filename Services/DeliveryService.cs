using FlowerShop.API.Data;
using FlowerShop.API.DTOs;
using FlowerShop.API.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace FlowerShop.API.Services;

public class DeliveryService
{
    private readonly AppDbContext _context;
    
    // Shop center coordinates (Istanbul - example location)
    // This should be moved to configuration in production
    private readonly Point _shopLocation = new(41.0082, 28.9784) { SRID = 4326 };
    private const decimal BaseDeliveryFee = 15.00m;
    private const decimal PerKmRate = 2.50m;
    
    public DeliveryService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<DeliveryInfoDto> CalculateDeliveryFeeAsync(int districtId)
    {
        var district = await _context.Districts
            .FirstOrDefaultAsync(d => d.Id == districtId);
            
        if (district == null || district.Location == null)
        {
            throw new ArgumentException("District not found or location not set");
        }
        
        // Calculate distance using PostGIS ST_Distance
        // The distance will be in degrees, we need to convert to kilometers
        // Approximate conversion: 1 degree ≈ 111 km
        var distanceInDegrees = _shopLocation.Distance(district.Location);
        var distanceKm = distanceInDegrees * 111.32;
        
        // Calculate total delivery fee
        var totalDeliveryFee = BaseDeliveryFee + ((decimal)distanceKm * PerKmRate);
        
        return new DeliveryInfoDto
        {
            BaseFee = BaseDeliveryFee,
            DistanceKm = Math.Round(distanceKm, 2),
            PerKmRate = PerKmRate,
            TotalDeliveryFee = Math.Round(totalDeliveryFee, 2)
        };
    }
    
    public async Task<Dictionary<int, DeliveryInfoDto>> CalculateDeliveryFeesForDistrictsAsync(List<int> districtIds)
    {
        var result = new Dictionary<int, DeliveryInfoDto>();
        
        foreach (var districtId in districtIds)
        {
            result[districtId] = await CalculateDeliveryFeeAsync(districtId);
        }
        
        return result;
    }
    
    public async Task<DeliveryInfoDto> CalculateDeliveryFeeByCoordinatesAsync(double latitude, double longitude)
    {
        var customerLocation = new Point(latitude, longitude) { SRID = 4326 };
        
        // Calculate distance using PostGIS
        var distanceInDegrees = _shopLocation.Distance(customerLocation);
        var distanceKm = distanceInDegrees * 111.32;
        
        var totalDeliveryFee = BaseDeliveryFee + ((decimal)distanceKm * PerKmRate);
        
        return new DeliveryInfoDto
        {
            BaseFee = BaseDeliveryFee,
            DistanceKm = Math.Round(distanceKm, 2),
            PerKmRate = PerKmRate,
            TotalDeliveryFee = Math.Round(totalDeliveryFee, 2)
        };
    }
}
