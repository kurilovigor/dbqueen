using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbqueen.sys
{
    public interface IProgram
    {
        void Execute(ISystem sys, IScope scope);
    }
}
