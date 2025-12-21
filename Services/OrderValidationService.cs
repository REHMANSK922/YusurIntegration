using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using YusurIntegration.DTOs;
using YusurIntegration.Models;
using YusurIntegration.Repositories.Interfaces;
using YusurIntegration.Services.Interfaces;

namespace YusurIntegration.Services
{
    public class OrderValidationService : IOrderValidationService
    {
        private readonly IWasfatyDrugRepository _wasfatyRepo;
        private readonly IApprovedDrugRepository _approvedRepo;
        private readonly IStockRepository _stockRepo;

        public OrderValidationService(
            IWasfatyDrugRepository wasfatyRepo,
            IApprovedDrugRepository approvedRepo,
            IStockRepository stockRepo)
        {
            _wasfatyRepo = wasfatyRepo;
            _approvedRepo = approvedRepo;
            _stockRepo = stockRepo;
        }

        public async Task<ActivityValidationResultDto> ValidateActivityAsync(
            string branchLicense,
            DateTime orderDate,
            List<TradeDrugs> tradeDrugs)
        {
            foreach (var trade in tradeDrugs)
            {
                // 1️ Map to Wasfaty
                var wasfaty = await _wasfatyRepo.GetByDrugCodeAsync(trade.Code);

                if (wasfaty == null || string.IsNullOrEmpty(wasfaty.Barcode)) continue;

                // 2️ Approved drug check
                var approved = await _approvedRepo.GetValidAsyncBySfda(wasfaty.Barcode, orderDate);

                if (approved == null) continue;

                // 3️ Stock check
                int stock = await _stockRepo.GetStockAsync(approved.ItemNo, branchLicense);

                if (stock >= (int)trade.Quantity)
                {
                    return ActivityValidationResultDto.Success(
                        approved.ItemNo,
                        trade.Code,
                       (int)trade.Quantity);
                }
            }

            return ActivityValidationResultDto.Fail(
                "No approved drug with sufficient stock");
        }
 




        //public async Task<ApprovedDrug?> GetBestApprovedDrugAsync(
        //                string genericCode,
        //                List<string> tradeDrugCodes,
        //                DateTime orderDate,
        //                string branchLicense,
        //                int requiredQty)
        //{
        //    return await
        //        (from w in _context.WasfatyDrugs
        //         join a in _context.ApprovedDrugs
        //             on w.Barcode equals a.SfdaCode
        //         join s in _context.StockTables
        //             on new { a.ItemNo, branchLicense }
        //             equals new { s.ItemNo, branchLicense = s.BranchLicense }
        //         where
        //            w.GenericCode == genericCode &&
        //            tradeDrugCodes.Contains(w.DrugCode) &&
        //            a.IsActive &&
        //            a.FromDate <= orderDate &&
        //            a.ToDate >= orderDate &&
        //            s.Quantity >= requiredQty
        //         orderby
        //            a.PriorityOrder,
        //            a.FromDate
        //         select a)
        //        .FirstOrDefaultAsync();
        //}




    }

}
