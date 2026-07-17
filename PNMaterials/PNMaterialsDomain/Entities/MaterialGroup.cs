using System;
using System.Collections.Generic;
using System.Text;

namespace PNMaterialsDomain.Entities
{
    public class MaterialGroup
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
