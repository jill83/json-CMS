using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using JsonCMS;
using JsonCMS.Functionality;
using JsonCMS.Models;
using JsonCMS.ServiceLayer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories;
using Repositories.DataContext;
using Repositories.Repositories.Interfaces;
using Ionic.Zip;

namespace JsonCMS.Controllers
{
    public class JsonFileController : ControllerBase
    {
        internal readonly IJsonFileRepository jsonRepo;
        internal readonly IJsonFileService jsonService;

        public JsonFileController(IJsonFileRepository jsonRepo, IJsonFileService jsonService)
        {
            this.jsonRepo = jsonRepo;
            this.jsonService = jsonService;
        }

        // GET: JsonFiles
        public ActionResult Index(string searchTerm = null)
        {
            var jsonFiles = jsonRepo.DisplayFilesIncSearchTerm(searchTerm);
            return View(new JsonFileListViewModel(jsonFiles));
        }

        // GET: JsonFiles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var jsonFile = jsonRepo.GetById((int) id);
            if (jsonFile == null)
            {
                return HttpNotFound();
            }
           return View(new JsonFileViewModel(jsonFile));
        }

        // GET: JsonFiles/Create
        public ActionResult Create()
        {
            return View(new JsonFileViewModel());
        }

        // POST: JsonFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JsonFileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var application = model.ToDalEntity();
                    jsonRepo.InsertAndSubmit(application);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    base.SetErrorMessage("Whoops! Couldn't create the new application. The error was [{0}]", ex.Message);
                }

            }
            return View(model);
        }

        // GET: JsonFiles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JsonFile jsonFile = jsonRepo.GetById((int) id);
            if (jsonFile == null)
            {
                return HttpNotFound();
            }
            return View(new JsonFileViewModel(jsonFile));
        }

        // POST: JsonFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FileName,JsonString,InUse,Deleted")] JsonFileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var jsonfile = jsonRepo.GetById(model.Id);
                if (jsonfile == null) { throw new ArgumentException(string.Format("Application with Id [{0}] does not exist", model.Id)); }

                try
                {
                    model.ToDalEntity(jsonfile);
                    jsonRepo.UpdateAndSubmit(jsonfile);

                    base.SetSuccessMessage("The application [{0}] has been updated.", model.FileName);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    base.SetErrorMessage("Whoops! Couldn't update the application. The error was [{0}]", ex.Message);
                }
            }

            return RedirectToAction("Index");
        }

        // GET: JsonFiles/Delete/5
        public ActionResult Delete(int id)
        {
            var application = jsonRepo.GetById(id);

            if (application == null)
            {
                base.SetErrorMessage("Application with Id [{0}] does not exist", id.ToString());
                return RedirectToAction("Index");
            }

            return View(new JsonFileViewModel(application));
        }


        // POST: JsonFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(JsonFileViewModel model)
        {
            var application = jsonRepo.GetById(model.Id);
            if (application == null) { throw new ArgumentException(string.Format("Application with Id [{0}] does not exist", model.Id)); }

            try
            {
                jsonRepo.SoftDeleteAndSubmit(application);

                base.SetSuccessMessage("The Json File has been (soft) deleted.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                base.SetErrorMessage("Whoops! Couldn't delete the Json File. The error was [{0}]", ex.Message);
            }

            return View(model);
        }

        public  ActionResult Download(int? id)
        {
            if (id == null)
            {
                List<JsonFile> activeFiles = jsonRepo.AllActiveFiles().ToList();

                return jsonService.DownloadMultipleJsonFilesAsZip(activeFiles);
            }
            JsonFile file =  jsonRepo.GetById((int)id);

            return jsonService.DownloadSingleJsonFile(file);
        }


        [ChildActionOnly]
       

        public ActionResult Upload()
        {
            return View();
        }
         [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                var jsonFile = jsonService.SetJsonFilePropertiesAndValidateJsonString(file);


                if (ModelState.IsValid)
                {
                    jsonRepo.InsertAndSubmit(jsonFile);            
                }
            }
             return RedirectToAction("Index");
        }


    }
}
