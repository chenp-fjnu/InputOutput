﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputOutput.Processer
{
    public interface IProcesser
    {
        string Process(string user, string input);
    }
}
