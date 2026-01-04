//using HigzTrade.Application.Interfaces;
using HigzTrade.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HigzTrade.Infrastructure.Persistence.UnitOfWork
{
    public sealed class EfUnitOfWork //: IAppUnitOfWork
    {
        private readonly HigzTradeDbContext _db;

        public EfUnitOfWork(HigzTradeDbContext db) => _db = db;

        public async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken ct)
        {
            // ตรวจสอบว่ามี Transaction เปิดอยู่แล้วหรือไม่ (ป้องกันการเปิดซ้อน)
            if (_db.Database.CurrentTransaction != null)
            {
                await action(ct);
                await _db.SaveChangesAsync(ct);
                return;
            }

            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                /*Test lock table and prove the CancellationToken is work*/
                //await _db.Database.ExecuteSqlRawAsync("SELECT TOP 1 1 FROM products WITH (TABLOCKX, HOLDLOCK)", ct);
                //await Task.Delay(15000, ct);
                /*********************************************************/

                await action(ct);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw; // ส่ง Exception ต่อไปให้ Global Exception Handler จัดการ
            }
        }

        // เพิ่ม Overload สำหรับกรณีที่ต้องการค่าคืนกลับมา (เช่น สร้าง Order แล้วได้ OrderId)
        public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct)
        {
            if (_db.Database.CurrentTransaction != null)
            {
                var result = await action(ct);
                await _db.SaveChangesAsync(ct);
                return result;
            }

            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                var result = await action(ct);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return result;
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }
    }
}
