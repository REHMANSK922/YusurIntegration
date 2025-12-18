namespace YusurIntegration.DTOs
{
    public class ActivityValidationResultDto
    {
        public bool IsValid { get; private set; }
        public string? ItemNo { get; private set; }
        public string? DrugCode { get; private set; }
        public int Quantity { get; private set; }
        public string? Reason { get; private set; }

        public static ActivityValidationResultDto Success(
            string itemNo, string drugCode, int qty)
            => new() { IsValid = true, ItemNo = itemNo, DrugCode = drugCode, Quantity = qty };

        public static ActivityValidationResultDto Fail(string reason)
            => new() { IsValid = false, Reason = reason };
    }

}
