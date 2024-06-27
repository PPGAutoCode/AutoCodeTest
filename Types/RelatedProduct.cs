
using System;

namespace ProjectName.Types
{
    public class RelatedProduct
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid RelatedProductId { get; set; }
    }
}
