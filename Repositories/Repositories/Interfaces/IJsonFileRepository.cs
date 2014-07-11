using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Repositories.Repositories.Interfaces
{
    public interface IJsonFileRepository : IRepositoryBase<JsonFile>
    {
        IEnumerable<JsonFile> DisplayFilesIncSearchTerm(string searchTerm);

        IEnumerable<JsonFile> AllActiveFiles();

    }
}