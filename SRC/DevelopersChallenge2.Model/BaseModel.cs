using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopersChallenge2.Model
{
    public class BaseModel
    {
        [Key]
        public long Id { get; set; }
        public DateTime DataReg { get; set; }
        [MaxLength(1)]
        public string EstReg { get; set; }
    }
}
