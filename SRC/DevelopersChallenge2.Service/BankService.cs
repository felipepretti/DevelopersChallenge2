using DevelopersChallenge2.DAL.Context;
using DevelopersChallenge2.Model;
using DevelopersChallenge2.Model.Enums;
using DevelopersChallenge2.Repository.Interfaces;
using DevelopersChallenge2.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace DevelopersChallenge2.Service
{
    public class BankService : GenericoService<BankRepository, SqlDbContext>
    {
        public BankService() : this(null) { }
        public BankService(IRepositoryFactory repositoryFactory) : base(repositoryFactory) { }

        public IQueryable<Bank> GetAllActives()
        {
            return Repository.GetAllActives();
        }

        public Bank GetByCode(string code)
        {
            return Repository.GetAllActives()
                .Where(x => x.Code == code)
                .FirstOrDefault();
        }
    }
}
