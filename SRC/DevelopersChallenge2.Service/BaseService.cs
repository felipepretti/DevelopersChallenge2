using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopersChallenge2.Service
{
    public class BaseService : IDisposable
    {
        private bool disposed = false;

       public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    
                }
                
                disposed = true;
            }
        }

        ~BaseService()
        {
            Dispose(false);
        }
    }
}
