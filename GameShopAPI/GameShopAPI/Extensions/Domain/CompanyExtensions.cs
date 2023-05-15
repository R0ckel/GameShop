using GameShopAPI.DTOs.Company;
using GameShopAPI.DTOs.GameGenre;
using GameShopAPI.Models.Domain;

namespace GameShopAPI.Extensions.Domain;

public static class CompanyExtensions
{
    public static Company ToModel(this CreateCompanyRequest request)
    {
        return new Company
        {
            Name = request.Name,
            Country = request.Country
        };
    }

    public static Company ToModel(this UpdateCompanyRequest request)
    {
        return new Company
        {
            Id = request.Id,
            Name = request.Name,
            Country = request.Country
        };
    }

    public static CompanyResponse ToResponse(this Company company)
    {
        return new CompanyResponse
        {
            Id = company.Id,
            Name = company.Name,
            Country = company.Country
        };
    }

    public static CompanyCard ToCard(this Company company)
    {
        return new CompanyCard
        {
            Id = company.Id,
            Name = company.Name ?? string.Empty
        };
    }
}
