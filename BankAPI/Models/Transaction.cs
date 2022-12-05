using BankAPI.Models.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAPI.Models
{
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public string TransactionUniqueRefrence { get; set; }
        public decimal TransactionAmount { get; set; }
        public TranStatue TransactionStatue { get; set; }
        public bool IsSuccessful => TransactionStatue.Equals(TranStatue.Success);
        public string TransactionSourceAccount { get; set; }
        public string TransactionDestinationAccount { get; set; }
        public string TransactionParticulars { get; set; }
        public TransType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }

        public Transaction()
        {
            TransactionUniqueRefrence = $"{Guid.NewGuid().ToString().Replace("-", "").Substring(1, 27)}";
        }


    }
}
