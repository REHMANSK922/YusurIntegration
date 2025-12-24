using YusurIntegration.Models.Enums;

namespace YusurIntegration.Models
{
    public static class OrderStatusMapper
    {
        public static OrderStatus FromYusur(string status)
        {
            return status switch
            {
                "ACCEPTED_BY_PROVIDER" => OrderStatus.AcceptedByProvider,
                "REJECTED_BY_PROVIDER" => OrderStatus.RejectedByProvider,
                "CANCELED_BY_PROVIDER" => OrderStatus.CanceledByProvider,

                "WAITING_ERX_HUB_APPROVAL" => OrderStatus.WaitingErxHubApproval,
                "ERX_HUB_AUTH_SUBMIT_TIMED_OUT" => OrderStatus.ErxHubAuthSubmitTimedOut,
                "ERX_HUB_TIMED_OUT" => OrderStatus.ErxHubTimedOut,

                "WAITING_ERX_HUB_CLAIM_APPROVAL" => OrderStatus.WaitingErxHubClaimApproval,
                "ERX_HUB_CLAIM_SUBMIT_TIMED_OUT" => OrderStatus.ErxHubClaimSubmitTimedOut,
                "ERX_HUB_CLAIM_FAILED" => OrderStatus.ErxHubClaimFailed,

                "WAITING_PATIENT_CONFIRMATION" => OrderStatus.WaitingPatientConfirmation,

                "READY_FOR_CUSTOMER_PICKUP" => OrderStatus.ReadyForCustomerPickup,
                "OUT_FOR_DELIVERY" => OrderStatus.OutForDelivery,
                "DELIVERED" => OrderStatus.Delivered,
                "RETURNED" => OrderStatus.Returned,

                "DISPENSED" => OrderStatus.Dispensed,
                "FAILED" => OrderStatus.Failed,
                "INACTIVE" => OrderStatus.Inactive,

                _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown Yusur status")
            };
        }
    }

}
