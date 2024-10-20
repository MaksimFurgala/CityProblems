using CityProblems.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace CityProblems
{
    /// <summary>
    /// контекст данных для БД
    /// </summary>
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection")
        {

        }

        /// <summary>
        /// список пробем из БД
        /// </summary>
        public DbSet<Problem> Problems { get; set; }
    }
}
