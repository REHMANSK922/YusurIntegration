using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using YusurIntegration.Data;
using YusurIntegration.DTOs;
using YusurIntegration.Models;
using YusurIntegration.Repositories.Interfaces;
using YusurIntegration.Services.Interfaces;




namespace YusurIntegration.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepo;
        private readonly IApprovedDrugRepository _approvedRepo;
        private readonly ILogger<StockService> _logger;

        public StockService(
            IStockRepository stockRepo,
            IApprovedDrugRepository approvedRepo,
            ILogger<StockService> logger
            )
        {
            _stockRepo = stockRepo;
            _approvedRepo = approvedRepo;
            _logger = logger;

        }

        //public async Task<OrderStockCheckResult> CheckOrderStockAsync(
        //    string branchLicense,
        //    string orderId,
        //    List<ActivityDto> activities)
        //{
        //    _logger.LogInformation(
        //        "Checking stock for order {OrderId} with {Count} activities",
        //        orderId,
        //        activities.Count);

        //    var result = new OrderStockCheckResult
        //    {
        //        OrderId = orderId,
        //        BranchLicense = branchLicense
        //    };

        //    // Check each activity
        //    foreach (var activity in activities)
        //    {
        //        var stockCheck = await CheckActivityStockAsync(branchLicense, activity);
        //        stockCheck.ActivityId = activity.Id;
        //        stockCheck.Duration = activity.Duration;
        //        stockCheck.Instructions = activity.Instructions;
        //        result.ActivityChecks.Add(stockCheck);
        //    }

        //    _logger.LogInformation(
        //        "Order {OrderId}: {Available}/{Total} items available",
        //        orderId,
        //        result.AvailableItems,
        //        result.TotalItems);

        //    return result;
        //}










        // -------------------------------------------
        // 1. Check availability for Yusur webhook
        // -------------------------------------------
        public async Task<StockCheckDto> CheckAvailabilityAsync(
            string branchLicense,
            string drugCode,
            List<TradeDrugDto> tradeDrugs)
        {
            // Get stock from DB
            int availableQty = await _stockRepo.GetStockAsync(drugCode, branchLicense);

            if (availableQty <= 0)
            {
                return new StockCheckDto
                {
                    GenericCode = drugCode,
                    Available = false,
                    Reason = "Product not available",
                    StockLevel = 0
                };
            }

            // Find matching trade drug from Yusur payload
            var trade = tradeDrugs.FirstOrDefault(td => td.Code == drugCode);

            if (trade == null)
            {
                return new StockCheckDto
                {
                    GenericCode = drugCode,
                    Available = false,
                    Reason = "No matching trade drug found",
                    StockLevel = availableQty
                };
            }

            bool isAvailable = availableQty >= trade.Quantity;

            return new StockCheckDto
            {
                GenericCode = drugCode,
                Available = isAvailable,
                Reason = isAvailable
                    ? $"Available ({availableQty})"
                    : $"Insufficient stock. Need {trade.Quantity}, Have {availableQty}",
                SelectedTradeDrug = trade,
                StockLevel = availableQty
            };
        }

        // -------------------------------------------
        // 2. Update stock after sale (POS → API)
        // -------------------------------------------
        public async Task<bool> ReduceStockForSaleAsync(
            string branchLicense,
            string drugCode,
            int quantitySold)
        {
            // Get existing stock
            int availableQty = await _stockRepo.GetStockAsync(drugCode, branchLicense);

            if (availableQty < quantitySold)
                return false;

            var item = new StockTable
            {
                BranchLicense = branchLicense,
                ItemNo = drugCode,
                AvailableQuantity = availableQty - quantitySold
            };

            await _stockRepo.UpdateStockAsync(item);
            return true;
        }

        // -------------------------------------------
        // 3. Add or override stock for sync
        // -------------------------------------------
        public async Task<bool> SetStockAsync(
            string branchLicense,
            string drugCode,
            int newQuantity)
        {
            var item = new StockTable
            {
                BranchLicense = branchLicense,
                ItemNo = drugCode,
                AvailableQuantity = newQuantity
            };

            await _stockRepo.UpdateStockAsync(item);
            return true;
        }
       
    
    
    }
}



     
//    public class StockService
//    {
//        private readonly StockDbContext _db;
//        private readonly YusurApiClient _yusur;
//        public StockService(StockDbContext db, YusurApiClient yusur)
//        {
//            _db = db;
//            _yusur = yusur;
//        }

//        public async Task<StockCheckDto> CheckAvailabilityAsync(
//            string branchLicense,
//            string genericCode,
//            List<TradeDrugDto> tradeDrugs)
//        {
 
//            var stock = await GetByBranchAndGenericCodeAsync(branchLicense, genericCode);

//            if (stock == null)
//            {
//                return new StockCheckDto
//                {
//                    GenericCode = genericCode,
//                    Available = false,
//                    Reason = "Product not found in inventory",
//                    StockLevel = 0
//                };
//            }

//            var matchingTrade = tradeDrugs.FirstOrDefault(td => td.Code == stock.GenericCode);

//            if (matchingTrade == null)
//            {
//                return new StockCheckDto
//                {
//                    GenericCode = genericCode,
//                    Available = false,
//                    Reason = "No matching trade drug",
//                    StockLevel = stock.AvailableQuantity
//                };
//            }

//            bool isAvailable = stock.AvailableQuantity >= matchingTrade.Quantity;

//            return new StockCheckDto
//            {
//                GenericCode = genericCode,
//                Available = isAvailable,
//                Reason = isAvailable
//                    ? $"In stock ({stock.AvailableQuantity} units)"
//                    : $"Insufficient (Need: {matchingTrade.Quantity}, Have: {stock.AvailableQuantity})",
//                SelectedTradeDrug = matchingTrade,
//                StockLevel = stock.AvailableQuantity
//            };
//        }
//        public async Task<StockTable?> GetByBranchAndGenericCodeAsync(
//            string branchLicense,
//            string genericCode)
//        {
//            return await _db.StockTable
//                .FirstOrDefaultAsync(s =>
//                    s.BranchLicense == branchLicense &&
//                    s.GenericCode == genericCode);
//        }
//        public async Task<bool> AddOrUpdateStockAsync(string branchLicense, string tradeCode, int quantity)
//        {
//            var stock = await _db.StockTable
//                .FirstOrDefaultAsync(s => s.BranchLicense == branchLicense && s.GenericCode == tradeCode);

//            if (stock == null || stock.AvailableQuantity < quantity)
//                return false;

//            stock.AvailableQuantity += quantity;
//            stock.LastUpdated = DateTime.UtcNow;
//            await _db.SaveChangesAsync();
//            return true;
//        }
//    } 

//}
