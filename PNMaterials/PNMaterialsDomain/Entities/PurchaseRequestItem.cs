using System;
using System.Collections.Generic;
using System.Text;

namespace PNMaterialsDomain.Entities
{
    public class PurchaseRequestItem
    {
        public int Id { get; set; }

        public int RequestId { get; set; }
        public PurchaseRequest Request { get; set; } = null!;

        public int MaterialId { get; set; }
        public Material Material { get; set; } = null!;

        public decimal Quantity { get; set; }
        public string? PositionText { get; set; }
    }
}
