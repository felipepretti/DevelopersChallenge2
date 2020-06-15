using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopersChallenge2.Repository
{
    public static class Extensions
    {
        public static IEnumerable<Type> GetTypesImplementsInterface(this Type[] types, Type @interface)
        {
            return types.Where(o =>
                @interface.IsAssignableFrom(o)
                 && !o.IsInterface
                 && !o.IsAbstract
            );
        }
    }
}
