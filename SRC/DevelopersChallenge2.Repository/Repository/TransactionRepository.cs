using DevelopersChallenge2.Model;
using DevelopersChallenge2.Repository.Interfaces;

namespace DevelopersChallenge2.Repository.Repository
{
    public class TransactionRepository : GenericoRepository<Transaction>
    {
        public TransactionRepository(IRepositoryFactory repositoryFactory)
            : base(repositoryFactory)
        { }
    }
}
