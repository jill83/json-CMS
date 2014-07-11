using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Ionic.Zip;
using JsonCMS.Functionality;
using JsonCMS.ServiceLayer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories;

namespace JsonCMS.Controllers
{
    public class JsonFileService: IJsonFileService
    {
        public MemoryStream ZipFilesToStream(List<JsonFile> activeFiles)
        {
            MemoryStream workStream = new MemoryStream();
            ZipFile zip = new ZipFile();

            foreach (var file in activeFiles)
            {
                //verify if json is object or an array of objects
                var token = JToken.Parse(file.JsonString);

                if (token is JArray)
                {
                    var jsonFormatted = JArray.Parse(file.JsonString).ToString(Formatting.Indented);
                    zip.AddEntry(file.FileName, jsonFormatted);
                }
                else if (token is JObject)
                {
                    var jsonFormatted = JObject.Parse(file.JsonString).ToString(Formatting.Indented);
                    zip.AddEntry(file.FileName, jsonFormatted);
                }
            }
            zip.Save(workStream);
            workStream.Position = 0;
            return workStream;
        }

        public JsonFile SetJsonFilePropertiesAndValidateJsonString(HttpPostedFileBase file)
        {
            JsonFile jsonFile = new JsonFile(); //new line
            jsonFile.FileName = file.FileName;
            jsonFile.JsonString = new StreamReader(file.InputStream).ReadToEnd();
            Regex.Replace(jsonFile.JsonString, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
            jsonFile.InUse = true;
            return jsonFile;
        }

        public FileStringResult DownloadSingleJsonFile(JsonFile file)
        {
            var token = JToken.Parse(file.JsonString);
            try
            {
                if (token is JArray)
                {
                    var json = JArray.Parse(file.JsonString);
                    return new FileStringResult(json.ToString(Formatting.Indented), "application/text")
                    {
                        FileDownloadName = file.FileName
                    };
                }
                if (token is JObject)
                {
                    var json = JObject.Parse(file.JsonString);
                    return new FileStringResult(json.ToString(Formatting.Indented), "application/text")
                    {
                        FileDownloadName = file.FileName
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public FileResult DownloadMultipleJsonFilesAsZip(List<JsonFile> activeFiles)
        {
            try
            {
                var workStream = ZipFilesToStream(activeFiles);
                FileStreamResult fileResult = new FileStreamResult(workStream,
                    System.Net.Mime.MediaTypeNames.Application.Zip);
                fileResult.FileDownloadName = "MultipleJsonDownload.zip";

                return fileResult;
            }
            catch (Exception ex)
            {
                return null;
            }
          
        }
    }
}