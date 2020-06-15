using DevelopersChallenge2.Model;
using System.Collections.Generic;

namespace DevelopersChallenge2.Models
{
    public class HomeViewModel
    {
        public string Message { get; set; }
        public bool? Error { get; set; }
        public IEnumerable<TransactionViewModel> DatagridData { get; set; }

        public HomeViewModel()
        {
            DatagridData = new List<TransactionViewModel>();
        }
    }
}