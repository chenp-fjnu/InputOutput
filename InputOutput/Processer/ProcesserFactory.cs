﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputOutput.Processer
{
    public class ProcesserFactory
    {
        public static IProcesser GetProcesser(string input)
        {
            if (!input.ToLower().StartsWith(Constant.ChangeProcesserKey))
                return null;
            IProcesser processer = null;
            var processerKey = input.Substring(Constant.ChangeProcesserKey.Length + 1);
            //TODO: user configuration and reflect to remove hardcode and support others to plug in other processer.
            switch (processerKey)
            {
                case "user_info":
                    processer = new UserInfoProcesser();
                    break;
                default:
                    processer = null;
                    break;
            }
            return processer;
        }
    }
    public partial class Constant
    {
        public const string ChangeProcesserKey = "enter";
    }
}
