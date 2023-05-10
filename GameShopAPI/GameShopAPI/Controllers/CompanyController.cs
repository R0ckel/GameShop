using GameShopAPI.DTOs.Company;
using GameShopAPI.Models.Base;
using GameShopAPI.Services.CompanyService;
using Microsoft.AspNetCore.Mvc;

namespace GameShopAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CompanyController : ControllerBase
{
    private ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    // GET: api/v1/Company + ?params
    [HttpGet]
    public async Task<BaseResponse<CompanyResponse>> Get([FromQuery] CompanyFilter? filter,
                                                         int page = 1,
                                                         int pageSize = 10)
        => await _companyService.ReadPageAsync(page, pageSize, filter);

    // GET api/v1/Company/5
    [HttpGet("{id}")]
    public async Task<BaseResponse<CompanyResponse>> Get(Guid id)
        => await _companyService.ReadAsync(id);

    // POST api/v1/Company
    [HttpPost]
    public async Task<BaseResponse<CompanyResponse>> Post([FromBody] CreateCompanyRequest request)
        => await _companyService.CreateAsync(request);

    // PUT api/v1/Company/5
    [HttpPut("{id}")]
    public async Task<BaseResponse<CompanyResponse>> Put(Guid id,
                                                         [FromBody] UpdateCompanyRequest request)
        => await _companyService.UpdateAsync(id, request);

    // DELETE api/v1/Company/5
    [HttpDelete("{id}")]
    public async Task<BaseResponse<CompanyResponse>> Delete(Guid id)
        => await _companyService.DeleteAsync(id);
}
