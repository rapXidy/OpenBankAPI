using BankAPI.Models;
using System;

namespace BankAPI.DTOs
{
    public class TransactionRequestDTO
    {
        public decimal TransactionAmount { get; set; }
        public string TransactionSourceAccount { get; set; }
        public string TransactionDestinationAccount { get; set; }
        public TranType TransactionType { get; set; }// this is another enum
        public DateTime TransactionDate { get; set; }

    }
}
