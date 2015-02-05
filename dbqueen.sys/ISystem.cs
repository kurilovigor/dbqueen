using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbqueen.sys
{
    public interface ISystem
    {
        void Run(IProgram onExecute, IProgram onFail, IProgram onSuccess);
        void Run(IProgram onExecute, IProgram onFail);
        void Run(IProgram onExecute);
    }
}
