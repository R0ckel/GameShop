using GameShopAPI.DTOs.Company;
using GameShopAPI.Models.Base;

namespace GameShopAPI.Services.CompanyService;

public interface ICompanyService
{
    Task<BaseResponse<CompanyResponse>> CreateAsync(CreateCompanyRequest request);
    Task<BaseResponse<CompanyResponse>> ReadAsync(Guid id);
    Task<BaseResponse<CompanyResponse>> UpdateAsync(Guid id, UpdateCompanyRequest request);
    Task<BaseResponse<CompanyResponse>> DeleteAsync(Guid id);
    Task<BaseResponse<CompanyResponse>> ReadPageAsync(int page, int pageSize, CompanyFilter? filter);
}
