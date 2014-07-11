using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Repositories.DataContext;
using Repositories.Repositories.Interfaces;

namespace Repositories.Repositories
{
    public class JsonFileRepository : RepositoryBase<JsonFile>, IJsonFileRepository
    {

        public JsonFileRepository(IDataContext dataContext)
            : base(dataContext)
        { }

        public IEnumerable<JsonFile> DisplayFilesIncSearchTerm(string searchTerm)
        {

            return SearchFor(r => searchTerm == null || r.FileName.StartsWith(searchTerm));
        }

        public IEnumerable<JsonFile> AllActiveFiles()
        {

            return SearchFor(j => j.InUse == true);
        }
       
    }
}
