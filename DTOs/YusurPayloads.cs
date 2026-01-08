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
        string? deliveryTimeSlotId,
        string? deliveryTimeSlotStartTime,
        string? deliveryTimeSlotEndTime,
        string? deliveryDate
    );

        public record PatientDto(string nationalId, string memberId, string firstName, string? lastName, string? dateOfBirth, string? gender, string? bloodGroup);

        public record ActivityDto(string id, string genericCode, string instructions, string arabicInstructions, string duration, int? refills, List<TradeDrugDto> tradeDrugs);
        public record TradeDrugDto(string code, string name, int quantity);
        public record ShippingAddressDto(string? addressLine1, string? addressLine2, string? area, string? city, CoordinatesDto? coordinates);
        public record CoordinatesDto(double latitude, double longitude);

        public record OrderAllocationDto(string orderId, string branchLicense, string vendorId, List<AllocationActivityDto> activities);
        public record AllocationActivityDto(string id, string tradeCode);

        public record AuthorizationResponseDto(string vendorId, string branchLicense, string orderId, string failureReason, string status, List<AuthActivityDto> activities);
        public record AuthActivityDto(string id, string tradeCode, string authStatus, string rejectionReason);


        public record StatusUpdateDto1(string vendorId, string branchLicense, string orderId, bool isParentOrder, string failureReason, string status);


        public record StatusUpdateDto(string orderId, string status, string branchLicense, string failureReason);



        public record OrderAcceptRequestDto(string orderId, List<AcceptActivityDto> activities);
        public record AcceptActivityDto(string id, string tradeCode, int Quantity);

        public record OrderRejectRequestDto(string orderId, string rejectionReason);

        public record OrderCancelRequestDto(string orderId,string cancellationReason);

        public record LoginRequestDto(string userName,string password);
        public record LoginResponseDto(string? accessToken,  List<ErrorDto>? errors);

        public record ApiErrorResponseDto(List<ErrorDto>? errors);

        public record ErrorDto(string message, bool field);

        public record SubmitAuthorizationRequestDto(string orderId);
        public record ResubmitAuthorizationRequestDto( string orderId, List<AcceptActivityDto> activities);


        public record ResponseRejectReason(string[] rejectionResaons, List<ErrorDto>? errors);

        public record DispenseSuccessResponse(string orderId, string status, string message);

        public record DispenseFailedResponse(string orderId, List<ErrorDto>? errors);

        public record DispenseResponse(string? orderId, List<ErrorDto>? errors);


    }
}
