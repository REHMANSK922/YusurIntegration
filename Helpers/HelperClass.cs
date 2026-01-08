using YusurIntegration.Models;
using static YusurIntegration.DTOs.YusurPayloads;

namespace YusurIntegration.Helpers
{
    public static class HelperClass
    {
        public static Order MapDtoToOrder(NewOrderDto dto)
        {
            var order = new Order
            {
                OrderId = dto.orderId,
                VendorId = dto.vendorId,
                BranchLicense = dto.branchLicense,
                ErxReference = dto.erxReference,
                IsPickup = dto.isPickup,
                Status = "RECEIVED",
                // Map Patient
                Patient = dto.patient != null ? new Patient
                {
                    firstName = dto.patient.firstName,
                    nationalId = dto.patient.nationalId,
                    memberId = dto.patient.memberId,
                    lastName = dto.patient.lastName,
                    gender = dto.patient.gender,
                    bloodGroup = dto.patient.bloodGroup
                } : null,
                // Map Address
                ShippingAddress = dto.shippingAddress != null ? new ShippingAddress
                {
                    addressLine1 = dto.shippingAddress.addressLine1,
                    addressLine2 = dto.shippingAddress.addressLine2,
                    area = dto.shippingAddress.area,
                    city = dto.shippingAddress.city,
                    Coordinates = new Coordinates
                    {
                        latitude = dto.shippingAddress.coordinates?.latitude ?? 0,
                        longitude = dto.shippingAddress.coordinates?.longitude ?? 0
                    }
                } : null,
                Activities = new List<Activity>()
            };

            // Map Activities and TradeDrugs
            if (dto.activities != null)
            {
                foreach (var act in dto.activities)
                {
                    var activity = new Activity
                    {
                        ActivityId = act.id,
                        GenericCode = act.genericCode,
                        Instructions = act.instructions,
                        ArabicInstructions = act.arabicInstructions,
                        Duration = act.duration,
                        Refills = act.refills,
                        TradeDrugs = act.tradeDrugs?.Select(td => new TradeDrug
                        {
                            Code = td.code,
                            Name = td.name,
                            Quantity = td.quantity,
                            ActivityId = act.id
                        }).ToList() ?? new List<TradeDrug>()
                    };
                    order.Activities.Add(activity);
                }
            }

            return order;
        }
    }
}
