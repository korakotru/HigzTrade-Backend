using HigzTrade.Application.DTOs.Products;
using HigzTrade.Application.Interfaces.Repositories;
using HigzTrade.Domain.Entities;
using HigzTrade.Domain.Exceptions;
using HigzTrade.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace HigzTrade.Infrastructure.Persistence.Repositories
{
    public class ProductQuery : IProductQuery
    {
        private readonly HigzTradeDbContext _db;
        public ProductQuery(HigzTradeDbContext db) => _db = db;

        public Task<bool> IsSkuExists(string sku, CancellationToken ct)
        {
            return _db.Products.AnyAsync(ent => ent.Sku.ToLower() == sku.ToLower(), ct);
        }
        public async Task<ProductQueryDto.PagedResponse<ProductQueryDto.Response>> SearchByKeywordAsync(
      ProductQueryDto.Request request,
      CancellationToken ct)
        {
            var query = _db.Products.AsNoTracking();

            // 1. กรองข้อมูลตาม Keyword (ที่ระดับ Entity)
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                query = query.Where(p => p.Name.Contains(request.Keyword) || p.Sku.Contains(request.Keyword) || (request.Keyword ?? "") == "");
            }

            // 2. นับจำนวนทั้งหมด
            var totalCount = await query.CountAsync(ct);

            // 3. ทำ Order และ Paging ที่ "query" (ซึ่งยังเป็น DbSet<Product>)
            // การเรียงลำดับต้องทำก่อน Skip/Take เสมอครับพี่
            var pagedQuery = query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize);

            // 4. ค่อยส่งข้อมูลที่ตัดแบ่งหน้าแล้วไปเข้า Projector เพื่อ Join ตารางอื่น
            var items = await ProjectToProductResponse(pagedQuery)
                .ToListAsync(ct);

            // 5. ส่งกลับเป็น PagedResponse
            return new ProductQueryDto.PagedResponse<ProductQueryDto.Response>
            {
                Items = items,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                CurrentPage = request.PageIndex
            };
        }
        public async Task<ProductQueryDto.Response> SearchByIdAsync(int productId, CancellationToken ct)
        {
            var query = _db.Products.AsNoTracking(); // prevent change tracking

            query = query.Where(p => p.ProductId == productId);

            var result = await ProjectToProductResponse(query).SingleOrDefaultAsync(ct);
            return result ?? throw new BusinessException("Not found product");
        }

        private IQueryable<ProductQueryDto.Response> ProjectToProductResponse(IQueryable<Product> query)
        {
            return query.Select(p => new ProductQueryDto.Response(
                p.ProductId,
                p.Name,
                p.Sku,
                p.Price,
                p.Status,
                p.CategoryId,
                p.Category.Name,
                p.CreatedBy,
                p.CreatedAt,
                p.UpdatedBy ?? "",
                p.UpdatedAt,
                p.Stock == null ? 0 : p.Stock.AvailableQty
            ));
        }
    }
}
