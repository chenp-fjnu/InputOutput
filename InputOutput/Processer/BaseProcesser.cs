using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputOutput.Processer
{
    public abstract class BaseProcesser : IProcesser
    {
        public abstract string Process(string user, string input);
    }
}
