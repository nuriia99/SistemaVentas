﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.Entity;

namespace SistemaVenta.DAL.Interfaces
{
    public interface IRolService
    {
        Task<List<Rol>> Lista();

    }
}
