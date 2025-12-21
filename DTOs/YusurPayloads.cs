namespace YusurIntegration.DTOs
{
    public class YusurPayloads
    {
        public record NewOrderDto(
        string orderId,
        string branchLicense,
        string vendorId,
        string erxReference,
        PatientDto patient,
        List<ActivityDto> activities,
        ShippingAddressDto shippingAddress,
        bool isPickup,
        string deliveryTimeSlotId,
        string deliveryTimeSlotStartTime,
        string deliveryTimeSlotEndTime,
        string deliveryDate
    );

        public record PatientDto(string nationalId, string memberId, string firstName, string lastName, string dateOfBirth, string gender, string bloodGroup);

        public record ActivityDto(string id, string genericCode, string instructions, string arabicInstructions, string duration, int refills, List<TradeDrugDto> tradeDrugs);
        public record TradeDrugDto(string code, string name, int quantity);
        public record ShippingAddressDto(string addressLine1, string addressLine2, string area, string city, CoordinatesDto coordinates);
        public record CoordinatesDto(double latitude, double longitude);

        public record OrderAllocationDto(string orderId, string branchLicense, string vendorId, List<AllocationActivityDto> activities);
        public record AllocationActivityDto(string id, string tradeCode);

        public record AuthorizationResponseDto(string vendorId, string branchLicense, string orderId, string failureReason, string status, List<AuthActivityDto> activities);
        public record AuthActivityDto(string id, string tradeCode, string authStatus, string rejectionReason);

        public record StatusUpdateDto(string vendorId, string branchLicense, string orderId, bool isParentOrder, string failureReason, string status);
    }
}
