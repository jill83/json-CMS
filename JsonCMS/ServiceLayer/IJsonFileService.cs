using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using JsonCMS.Functionality;
using Repositories;

namespace JsonCMS.ServiceLayer
{
   public interface IJsonFileService
    {
        FileResult DownloadMultipleJsonFilesAsZip(List<JsonFile> activeFiles);

       FileStringResult DownloadSingleJsonFile(JsonFile file);

      JsonFile SetJsonFilePropertiesAndValidateJsonString(HttpPostedFileBase file);

        MemoryStream ZipFilesToStream(List<JsonFile> activeFiles);

    }
}
