using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopersChallenge2.Model.Interfaces
{
    public interface IBaseModel
    {
        DateTime DataReg { get; set; }
        string EstReg { get; set; }
    }
}
