﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyApp
{
    internal interface IClientRepository
    {
        Client GetById(int id);
    }
}
