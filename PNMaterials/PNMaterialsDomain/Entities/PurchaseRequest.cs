using System;
using System.Collections.Generic;
using System.Text;

namespace PNMaterialsDomain.Entities
{
    public class PurchaseRequest
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime DeliveryDate { get; set; }
        public RequestStatus Status { get; set; }

        public ICollection<PurchaseRequestItem> Items { get; set; } = new List<PurchaseRequestItem>();
    }
}
