namespace YusurIntegration.Models.Enums
{
    public enum OrderStatus
    {
        AcceptedByProvider,
        RejectedByProvider,
        CanceledByProvider,

        WaitingErxHubApproval,
        ErxHubAuthSubmitTimedOut,
        ErxHubTimedOut,

        WaitingErxHubClaimApproval,
        ErxHubClaimSubmitTimedOut,
        ErxHubClaimFailed,

        WaitingPatientConfirmation,

        ReadyForCustomerPickup,
        OutForDelivery,
        Delivered,
        Returned,

        Dispensed,
        Failed,
        Inactive
    }

}
