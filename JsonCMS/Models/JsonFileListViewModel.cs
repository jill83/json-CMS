using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using Repositories;

namespace JsonCMS.Models
{
    public class JsonFileListViewModel
    {

        public JsonFileListViewModel()
        {
            this.JsonFiles = new List<JsonFileViewModel>();
        }

        public JsonFileListViewModel(IEnumerable<JsonFile> jsonfiles)
        {
            this.JsonFiles = new List<JsonFileViewModel>();

            foreach (var jsonfile in jsonfiles)
            {
                this.JsonFiles.Add(new JsonFileViewModel(jsonfile));
            }
        

    }
        public List<JsonFileViewModel> JsonFiles { get; set; }
    }
}