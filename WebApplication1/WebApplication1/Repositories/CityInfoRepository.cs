using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Repositories
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly AppDbContext _appDbContext;

        public CityInfoRepository(AppDbContext context)
        {
            _appDbContext = context;
        }

        public void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCity(cityId, false);
            city.PointsOfInterest.Add(pointOfInterest);
        }

        public bool CityExists(int cityId)
        {
            return _appDbContext.Cities.Any(c => c.Id == cityId);
        }

        public IEnumerable<City> GetCities()
        {
            return _appDbContext.Cities.OrderBy(c => c.Name).ToList();
        }

        public City GetCity(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return _appDbContext.Cities.Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == cityId).FirstOrDefault();
            }

            return _appDbContext.Cities.Where(c => c.Id == cityId).FirstOrDefault();
        }

        public PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId)
        {
            return _appDbContext.PointsOfInterest
               .Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefault();
        }

        public IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId)
        {
            return _appDbContext.PointsOfInterest
                           .Where(p => p.CityId == cityId).ToList();
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _appDbContext.PointsOfInterest.Remove(pointOfInterest);
        }

        public bool Save()
        {
            return (_appDbContext.SaveChanges() >= 0);
        }
    }
}
