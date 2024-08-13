
namespace simple_pos_backend.Models
{
    public class TransactionDTO
    {
        public int TransactionId { get; set; }
        public string TransactionDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string TransactionProduct { get; set; }
    }
}
