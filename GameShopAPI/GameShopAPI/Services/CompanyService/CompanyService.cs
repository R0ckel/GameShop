using GameShopAPI.Data;
using GameShopAPI.DTOs.Company;
using GameShopAPI.DTOs.GameGenre;
using GameShopAPI.Extensions.Domain;
using GameShopAPI.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace GameShopAPI.Services.CompanyService;

public class CompanyService : ICompanyService
{
    private readonly GameShopContext _context;

    public CompanyService(GameShopContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<CompanyResponse>> ReadPageAsync(int page, int pageSize, CompanyFilter? filter)
    {
        var response = new BaseResponse<CompanyResponse>();

        try
        {
            var query = _context.Companies.AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Name))
                    query = query.Where(c => c.Name != null && c.Name.Contains(filter.Name));

                if (!string.IsNullOrWhiteSpace(filter.Country))
                    query = query.Where(c => c.Country != null && c.Country.Contains(filter.Country));
            }

            var companies = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.ToResponse())
                .ToListAsync();

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.AddRange(companies);
            response.ValueCount = companies.Count;
            response.PageNumber = page;
            response.PageSize = pageSize;
            response.PageCount = (int)Math.Ceiling(await query.CountAsync() / (double)pageSize);
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<CompanyCard>> ReadCardsAsync()
    {
        var response = new BaseResponse<CompanyCard>();

        try
        {
            var companies = await _context.Companies
                .Select(x => x.ToCard())
                .ToListAsync();

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.AddRange(companies);
            response.ValueCount = companies.Count;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<CompanyResponse>> ReadAsync(Guid id)
    {
        var response = new BaseResponse<CompanyResponse>();

        try
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                response.Message = "Company not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            response.Success = true;
            response.Message = "Success";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.Add(company.ToResponse());
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<CompanyResponse>> CreateAsync(CreateCompanyRequest request)
    {
        var response = new BaseResponse<CompanyResponse>();

        try
        {
            if (await _context.Companies.AnyAsync(c => c.Name == request.Name))
            {
                response.Message = "There is a company with this name already";
                return response;
            }

            var entry = await _context.Companies.AddAsync(request.ToModel());
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Company created successfully";
            response.StatusCode = StatusCodes.Status201Created;
            response.Values.Add(entry.Entity.ToResponse());
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<CompanyResponse>> UpdateAsync(Guid id, UpdateCompanyRequest request)
    {
        var response = new BaseResponse<CompanyResponse>();

        try
        {
            if (id != request.Id)
            {
                response.Message = "Ids don`t match";
                return response;
            }

            if (!await _context.Companies.AnyAsync(c => c.Id == request.Id))
            {
                response.Message = "Company not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            if (await _context.Companies.AnyAsync(c => c.Name == request.Name && c.Id != request.Id))
            {
                response.Message = "There is a company with this name already";
                return response;
            }

            var company = request.ToModel();
            _context.Entry(company).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Company updated successfully";
            response.StatusCode = StatusCodes.Status200OK;
            response.Values.Add(company.ToResponse());
            response.ValueCount = 1;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }

    public async Task<BaseResponse<CompanyResponse>> DeleteAsync(Guid id)
    {
        var response = new BaseResponse<CompanyResponse>();

        try
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                response.Message = "Company not found";
                response.StatusCode = StatusCodes.Status404NotFound;
                return response;
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Company deleted successfully";
            response.StatusCode = StatusCodes.Status204NoContent;
        }
        catch (Exception ex)
        {
            response.Message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }

        return response;
    }
}
