using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Repositories;

namespace JsonCMS.Models
{
    public class JsonFileViewModel
    {
        public JsonFileViewModel()
        {
        }

        public JsonFileViewModel(JsonFile jsonFile)
        {
            this.Id = jsonFile.Id;
            this.FileName = jsonFile.FileName;
            this.JsonString = jsonFile.JsonString;
            this.InUse = jsonFile.InUse;
        }

        public JsonFile ToDalEntity()
        {
            return ToDalEntity(new JsonFile());
        }

        public JsonFile ToDalEntity(JsonFile jsonfile)
        {
            jsonfile.Id = this.Id;
            jsonfile.FileName = this.FileName;
            jsonfile.JsonString = this.JsonString;
            jsonfile.InUse = this.InUse;
            return jsonfile;
        }
        public int Id { get; set; }
        [Required(ErrorMessage = "File Name is required")]
        [MaxLength(50)]
        [Display(Name = "File Name")]
        public string FileName { get; set; }
        [Display(Name = "Json String")]
        [AllowHtml]
        public string JsonString { get; set; }
        [Display(Name = "In Use")]
        public bool InUse { get; set; }
    }
}