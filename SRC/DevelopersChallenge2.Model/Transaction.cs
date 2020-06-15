using DevelopersChallenge2.Model.Enums;
using DevelopersChallenge2.Model.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace DevelopersChallenge2.Model
{
    public class Transaction : BaseModel, IBaseModel, ISoftDeletable
    {
        [ForeignKey(nameof(Bank))]
        public long IdBank { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Value { get; set; }
        [MaxLength(96)]
        public string Description { get; set; }
        [MaxLength(64)]
        public string Hash { get; set; }
        public virtual Bank Bank { get; set; }
    }
}
