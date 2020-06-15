using DevelopersChallenge2.Model;
using DevelopersChallenge2.Model.Enums;
using System;
using System.Collections.Generic;

namespace DevelopersChallenge2.Models
{
    public class TransactionViewModel
    {
        public TransactionType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Bank { get; set; }
    }
}