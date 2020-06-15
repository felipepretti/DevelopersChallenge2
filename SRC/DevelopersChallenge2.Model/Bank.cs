using DevelopersChallenge2.Model.Enums;
using DevelopersChallenge2.Model.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevelopersChallenge2.Model
{
    public class Bank : BaseModel, IBaseModel, ISoftDeletable
    {
        public Bank()
        {
            Transactions = new HashSet<Transaction>();
        }

        public string Name { get; set; }
        public string Code { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
