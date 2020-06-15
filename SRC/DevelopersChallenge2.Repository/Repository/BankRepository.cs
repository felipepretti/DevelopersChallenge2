using DevelopersChallenge2.Model;
using DevelopersChallenge2.Repository.Interfaces;

namespace DevelopersChallenge2.Repository.Repository
{
    public class BankRepository : GenericoRepository<Bank>
    {
        public BankRepository(IRepositoryFactory repositoryFactory)
            : base(repositoryFactory)
        { }
    }
}
