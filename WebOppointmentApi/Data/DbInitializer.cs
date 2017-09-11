using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebOppointmentApi.Models;
using WebOppointmentApi.Common;

namespace WebOppointmentApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApiContext context)
        {
            //context.Database.MigrateAsync();

        }
    }
}
