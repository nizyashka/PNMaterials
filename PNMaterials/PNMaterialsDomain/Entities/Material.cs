using System;
using System.Collections.Generic;
using System.Text;

namespace PNMaterialsDomain.Entities
{
    public class Material
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public int UnitId { get; set; }
        public Unit Unit { get; set; } = null!;

        public int GroupId { get; set; }
        public MaterialGroup Group { get; set; } = null!;
    }
}
