using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.DataContext
{
    public class DataContext : DbContext, IDataContext
    {
        public DataContext()
            : base("database")
        {
        }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

       
        public IDbSet<JsonFile> JsonFiles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public void ExecuteCommand(string command, params object[] parameters)
        {
            base.Database.ExecuteSqlCommand(command, parameters);
        }
    }
}
