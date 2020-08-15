using System;
using System.ComponentModel.DataAnnotations;

namespace bankaccounts.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId {get;set;}

        [Required]
        [Display(Name = "Deposit/Withdraw $ ")]
        public decimal Amount {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;

        [Required]
        public int UserId {get;set;}

        public User Owner {get;set;}
    }
}