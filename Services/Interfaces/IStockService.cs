using YusurIntegration.DTOs;


namespace YusurIntegration.Services.Interfaces
{
    public interface IStockService
    {
        Task<StockCheckDto> CheckAvailabilityAsync(
                 string branchLicense,
                 string drugCode,
                 List<TradeDrugDto> tradeDrugs);

        // Check multiple activities (entire order)
        //Task<OrderStockCheckResult> CheckOrderStockAsync(
        //    string branchLicense,
        //    string orderId,
        //    List<ActivityDto> activities);

        // Reserve stock for accepted order
        Task<bool> ReduceStockForSaleAsync(
            string branchLicense,
            string drugCode,
            int quantitySold);


      
        Task<bool> SetStockAsync(
           string branchLicense,
           string drugCode,
           int newQuantity);
        


    }
}
