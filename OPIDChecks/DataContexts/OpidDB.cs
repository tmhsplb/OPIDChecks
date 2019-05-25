using OPIDEntities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace OPIDChecks.DataContexts
{
    public class OpidDB : DbContext
    {

        public OpidDB()
          // :base("OPIDEntities")  PLB: Commented out on 4/7/19
          : base(Config.ConnectionString) //  PLB: Added on 4/7/19
        {
        }

        public DbSet<RCheck> RChecks { get; set; }
    }
}