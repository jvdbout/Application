﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinEchek.Persistance
{
    interface ILoader
    {
        Game Load(string path);
        string Filter();
    }
}
