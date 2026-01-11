using HigzTrade.Application.DTOs.Products;
using HigzTrade.Application.Interfaces;
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
        public async Task<List<PoductQueryDto.Response>> SearchByKeywordAsync(string keyword, CancellationToken ct)
        {
            var query = _db.Products.AsNoTracking(); // prevent chang tracking

            // 1. กรองข้อมูลตาม Keyword (ถ้ามี)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p => p.Name.Contains(keyword) || p.Sku.Contains(keyword));
            }

            // 2. ใช้ ProjectToType เพื่อทำ SQL Join และเลือกเฉพาะฟิลด์ที่ต้องการ
            // Mapster จะฉลาดพอที่จะดึง Category.Name และคำนวณ Stock (ถ้าเจ้านายตั้งชื่อฟิลด์ใน Response ให้ตรง)
            return await ProjectToProductResponse(query).ToListAsync(ct);
        }
        public async Task<PoductQueryDto.Response> SearchByIdAsync(int productId, CancellationToken ct)
        {
            var query = _db.Products.AsNoTracking(); // prevent chang tracking

            query = query.Where(p => p.ProductId == productId);

            var result = await ProjectToProductResponse(query).SingleOrDefaultAsync(ct);
            return result ?? throw new BusinessException("Not found product");
        }

        private IQueryable<PoductQueryDto.Response> ProjectToProductResponse(IQueryable<Product> query)
        {
            return query.Select(p => new PoductQueryDto.Response(
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
