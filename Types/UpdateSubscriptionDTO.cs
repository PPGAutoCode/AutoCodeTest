
namespace ProjectName.Types
{
    public class UpdateSubscriptionDTO
    {
        public Guid Id { get; set; }
        public Guid ProductsId { get; set; }
        public Guid ApplicationsId { get; set; }
        public DateTime Changed { get; set; }
    }
}
