using FirebirdSql.Data.FirebirdClient;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using YusurIntegration.Data;
using YusurIntegration.Models;
using YusurIntegration.Repositories.Interfaces;

namespace YusurIntegration.Repositories
{
    public class ApprovedDrugRepository : IApprovedDrugRepository
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ApprovedDrugRepository> _logger;
        public ApprovedDrugRepository(AppDbContext db, ILogger<ApprovedDrugRepository> logger)
        {
            _db = db;
            _logger = logger;
        }
      
        public async Task<ApprovedDrug?> GetActiveAsync(string itemNo, DateTime orderDate)
        {
            return await _db.ApprovedDrugs
                .Where(x =>
                    x.ItemNo == itemNo &&
                    x.IsActive &&
                    orderDate >= x.FromDate &&
                    orderDate <= x.ToDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ApprovedDrug?>> GetByItemNoAsync(string itemNo)
        {
            return await _db.ApprovedDrugs
                .Where(x => x.ItemNo == itemNo)
                .OrderByDescending(x => x.FromDate)
                .ToListAsync();
        }

        public async Task AddAsync(ApprovedDrug drug)
        {
            _db.ApprovedDrugs.Add(drug);
            await _db.SaveChangesAsync();
        }

        public async Task DeactivateAllAsync(string itemNo)
        {
            var actives = await _db.ApprovedDrugs
                .Where(x => x.ItemNo == itemNo && x.IsActive)
                .ToListAsync();

            foreach (var d in actives)
                d.IsActive = false;
        }
        
        //public async Task<ApprovedDrug?> GetValidAsync(string sfdacode, DateTime orderDate)
        //{
        //    return await _db.ApprovedDrugs.FirstOrDefaultAsync(a =>
        //        a.Sfdacode == sfdacode &&
        //        a.IsActive &&
        //        orderDate >= a.FromDate &&
        //        orderDate <= a.ToDate);
        //}
      
        public async Task<ApprovedDrug?> GetActiveAsyncById(string itemNo)
        {
            return await _db.ApprovedDrugs.FirstOrDefaultAsync(a =>
                a.ItemNo == itemNo && a.IsActive);
        }

        public async Task<ApprovedDrug?> GetValidAsyncBySfda(string sfdacode, DateTime orderDate)
        {

            return await _db.ApprovedDrugs
                    .Where(x =>
                        x.Sfdacode == sfdacode &&
                        x.IsActive &&
                        orderDate >= x.FromDate &&
                        orderDate <= x.ToDate)
                    .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteItemnoById(string itemno, DateTime frmdt, DateTime todt)
        {
            var product = await _db.ApprovedDrugs
                .FirstOrDefaultAsync(x => x.ItemNo == itemno &&
                x.FromDate == frmdt &&
                x.ToDate == todt);
            if (product == null) return false;
            _db.ApprovedDrugs.Remove(product);
            await _db.SaveChangesAsync();
            return true;
        }






        //public async Task<ApprovedDrug?> GetByGenericCodeAsync(string genericCode)
        //{
        //    return await _db.ApprovedItems
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(a =>
        //            a.GenericCode == genericCode &&
        //            a.IsActive);
        //}
        //public async Task<bool> IsItemApprovedAsync(string genericCode)
        //{
        //    return await _db.ApprovedItems
        //        .AnyAsync(a =>
        //            a.GenericCode == genericCode &&
        //            a.IsActive);
        //}
        //public async Task<List<ApprovedDrug>> GetAllActiveAsync()
        //{
        //    return await _db.ApprovedItems
        //        .AsNoTracking()
        //        .Where(a => a.IsActive)
        //        .OrderBy(a => a.ItemNo)
        //        .ToListAsync();
        //}
        //public async Task<int> GetApprovedItemsCountAsync()
        //{
        //    return await _db.ApprovedItems.CountAsync(a => a.IsActive);
        //}


        //public async Task<List<ApprovedDrug>> SearchByItemNoAsync(string searchTerm)
        //{
        //    return await _db.ApprovedItems
        //        .AsNoTracking()
        //        .Where(a => a.IsActive && a.ItemNo.Contains(searchTerm))
        //        .ToListAsync();
        //}

     /*
        public async Task<ApprovedDrug?> GetBestApprovedDrugAsync(
            string genericCode,
            List<string> tradeDrugCodes,
            DateTime orderDate,
            string branchLicense,
            int requiredQty)
        {
            return await
                (from w in _db.WasfatyDrugs
                 join a in _db.ApprovedDrugs
                     on w.Barcode equals a.Sfdacode
                 join s in _db.StockTable
                     on new { a.ItemNo, branchLicense }
                     equals new { s.ItemNo, branchLicense = s.BranchLicense }
                 where
                    w.GenericCode == genericCode &&
                    tradeDrugCodes.Contains(w.DrugCode) &&
                    a.IsActive &&
                    a.FromDate <= orderDate &&
                    a.ToDate >= orderDate &&
                    s.AvailableQuantity >= requiredQty
                 orderby
                    a.PriorityOrder
                 select a)
                .FirstOrDefaultAsync();
        }
     */

    }

}
