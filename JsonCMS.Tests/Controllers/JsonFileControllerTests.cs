using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Web.Mvc;
using JsonCMS.Models;
using JsonCMS.Controllers;
using JsonCMS.ServiceLayer;
using JsonCMS.Tests.Builders;
using Moq;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repositories;
using Repositories.Repositories.Interfaces;

namespace JsonCMS.Tests.Controllers
{
    [TestClass]
    public class JsonFileControllerTests
    {

        private Mock<IJsonFileRepository> jsonFileRepo;
        private Mock<IJsonFileService> jsonService;

        [TestInitialize]
        public void TestInitialize()
        {
           jsonFileRepo = new Mock<IJsonFileRepository>(); 
           jsonService = new Mock<IJsonFileService>();
        }

        [TestMethod]
        public void Index_returns_correct_viewmodel()
        {
            Assert.IsNotInstanceOfType(typeof(JsonFileListViewModel), CreateInstance().Index().GetType());
        }

        [TestMethod]
        public void Index_returns_list_of_all_applications_ordered_by_name()
        {
            // setup;
            jsonFileRepo.Setup(p => p.GetAll()).Returns(new List<JsonFile>() { 
                new JsonFileBuilder().WithName("B").Build(),
                new JsonFileBuilder().WithName("A").Build()});

            // act
            var controller = CreateInstance();
            ViewResult model = (ViewResult)controller.Index();
            var result = (JsonFileListViewModel)model.Model;

            // verify
            Assert.AreEqual(2, result.JsonFiles.Count);
            Assert.AreEqual("A", result.JsonFiles[0].FileName);
            Assert.AreEqual("B", result.JsonFiles[1].FileName);
        }

        [TestMethod]
        public void Index_returns_empty_list_when_there_are_no_applications()
        {
            // setup
            var controller = CreateInstance();
            ViewResult model = (ViewResult)controller.Index();
            var result = (JsonFileListViewModel)model.Model;

            // verify
            Assert.AreEqual(0, result.JsonFiles.Count);
        }

        [TestMethod]
        public void Create_saves_new_application_on_succesful_submit_and_returns_to_index_with_success_message()
        {
            // setup
            var application = new JsonFileBuilder().WithName("New app").Build();
            var viewModel = new JsonFileViewModel(application);

            // act
            var controller = CreateInstance();
            var model = (RedirectToRouteResult)controller.Create(viewModel);

            //**********check line above

            // verify if all properties are passed to DAL for insert
            jsonFileRepo.Verify(p => p.InsertAndSubmit(It.Is<JsonFile>(x =>
                x.FileName== application.FileName
                && x.JsonString == application.JsonString
                && x.InUse == application.InUse)), Times.Once());

            // verify
            Assert.IsTrue(controller.ViewData.ModelState.IsValid);
            Assert.IsNotNull(controller.ViewBag.SuccessMessage);
            Assert.IsNull(controller.ViewBag.ErrorMessage);
            Assert.AreEqual("Index", model.RouteValues["Action"]);
        }

        [TestMethod]
        public void Create_sets_error_message_on_unhandled_DAL_exception_and_returns_same_view()
        {
            // setup
            var application = new JsonFileBuilder().Build();
            var viewModel = new JsonFileViewModel(application);
            jsonFileRepo.Setup(p => p.InsertAndSubmit(It.IsAny<JsonFile>())).Throws(new InvalidOperationException());

            // act
            var controller = CreateInstance();
            ViewResult model = (ViewResult)controller.Create(viewModel);

            // verify
            Assert.IsNotNull(controller.ViewBag.ErrorMessage);
            Assert.IsNull(controller.ViewBag.SuccessMessage);
        }

       

        [TestMethod]
        public void Edit_throws_exception_if_application_does_not_exist_after_submit()
        {
            // setup
            var application = new JsonFileBuilder().Build();
            jsonFileRepo.Setup(p => p.GetById(application.Id)).Verifiable(); // don't return anything

            // act
            var controller = CreateInstance();

            try
            {
                controller.Edit(new JsonFileViewModel(application));
                Assert.Fail("ArgumentException was expected but not thrown");
            }
            catch (ArgumentException)
            {
                // should be thrown
            }
            finally
            {
                jsonFileRepo.VerifyAll();
            }
        }

        [TestMethod]
        public void Edit_returns_to_index_with_error_message_if_application_does_not_exist()
        {
            // setup
            var application = new JsonFileBuilder().Build();
            jsonFileRepo.Setup(p => p.GetById(application.Id)).Verifiable(); // don't return anything

            // act
            var controller = CreateInstance();
            var model = (RedirectToRouteResult)controller.Edit(application.Id);

            // verify
            jsonFileRepo.VerifyAll();
            Assert.AreEqual("Index", model.RouteValues["Action"]);
            Assert.IsNull(controller.ViewBag.SuccessMessage);
            Assert.IsNotNull(controller.ViewBag.ErrorMessage);
        }

        [TestMethod]
        public void Edit_saves_application_changes_on_succesful_and_returns_to_index_with_succes_message()
        {
            // setup

            var application = new JsonFileBuilder().WithName("A").Build();
            jsonFileRepo.Setup(p => p.GetById(application.Id)).Returns(application);

            // set up application change for filename
            application.FileName = "New name";
            var viewModel = new JsonFileViewModel(application);

            // act
            var controller = CreateInstance();
            var model = (RedirectToRouteResult)controller.Edit(viewModel);

            // verify if all properties are passed to DAL
            jsonFileRepo.Verify(p => p.UpdateAndSubmit(It.Is<JsonFile>(
                x => x.Id == application.Id
                && x.FileName == application.FileName
                && x.JsonString == application.JsonString
                && x.InUse == application.InUse)), Times.Once());

            // verify
            Assert.AreEqual("Index", model.RouteValues["Action"]);
            Assert.IsNotNull(controller.ViewBag.SuccessMessage);
            Assert.IsNull(controller.ViewBag.ErrorMessage);
        }

        [TestMethod]
        public void Edit_sets_error_message_on_unhandled_DAL_exception_and_returns_same_view()
        {
            // setup
           
            var application = new JsonFileBuilder().WithName("A").Build();
            jsonFileRepo.Setup(p => p.GetById(application.Id)).Returns(application);
            jsonFileRepo.Setup(p => p.UpdateAndSubmit(It.IsAny<JsonFile>())).Throws(new InvalidOperationException());
            var viewModel = new JsonFileViewModel(application);

            // act
            var controller = CreateInstance();
            var model = (ViewResult)controller.Edit(viewModel);

            // verify 
            Assert.IsNull(controller.ViewBag.SuccessMessage);
            Assert.IsNotNull(controller.ViewBag.ErrorMessage);
        }

        [TestMethod]
        public void Delete_returns_view_with_application_details_to_confirm_delete()
        {
            // setup
            var application = new JsonFileBuilder().WithName("A").Build();
            jsonFileRepo.Setup(p => p.GetById(application.Id)).Returns(application);

            // act
            var controller = CreateInstance();
            ViewResult model = (ViewResult)controller.Delete(application.Id);

            // verify if correct application was returned
            Assert.AreEqual(application.FileName, ((JsonFileViewModel)model.Model).FileName);
            Assert.AreEqual(application.JsonString, ((JsonFileViewModel)model.Model).JsonString);
            Assert.AreEqual(application.Id, ((JsonFileViewModel)model.Model).Id);
            Assert.AreEqual(application.InUse, ((JsonFileViewModel)model.Model).InUse);
        }

        [TestMethod]
        public void Delete_soft_deletes_application_upon_confirm_and_redirects_to_index_with_success_message()
        {
            // setup
            var application = new JsonFileBuilder().WithName("A").Build();
            jsonFileRepo.Setup(p => p.GetById(application.Id)).Returns(application);

            // set up application change for name and team
            var viewModel = new JsonFileViewModel(application);

            // act
            var controller = CreateInstance();
            var model = (RedirectToRouteResult)controller.Delete(viewModel);

            // verify if correct application is passed to DAL
            jsonFileRepo.Verify(p => p.SoftDeleteAndSubmit(It.Is<JsonFile>(
                x => x.Id == application.Id)), Times.Once());

            // verify
            Assert.AreEqual("Index", model.RouteValues["Action"]);
            Assert.IsNotNull(controller.ViewBag.SuccessMessage);
            Assert.IsNull(controller.ViewBag.ErrorMessage);
        }

        [TestMethod]
        public void Delete_returns_to_index_with_error_message_if_application_does_not_exist()
        {
            // setup
            var application = new JsonFileBuilder().Build();
            jsonFileRepo.Setup(p => p.GetById(application.Id)).Verifiable(); // don't return anything

            // act
            var controller = CreateInstance();
            var model = (RedirectToRouteResult)controller.Delete(application.Id);

            // verify
            jsonFileRepo.VerifyAll();
            Assert.AreEqual("Index", model.RouteValues["Action"]);
            Assert.IsNull(controller.ViewBag.SuccessMessage);
            Assert.IsNotNull(controller.ViewBag.ErrorMessage);
        }

        [TestMethod]
        public void Delete_throws_exception_if_application_does_not_exist_after_submit()
        {
            // setup
            var application = new JsonFileBuilder().Build();
            jsonFileRepo.Setup(p => p.GetById(application.Id)).Verifiable(); // don't return anything

            // act
            var controller = CreateInstance();

            try
            {
                controller.Delete(new JsonFileViewModel(application));
                Assert.Fail("ArgumentException was expected but not thrown");
            }
            catch (ArgumentException)
            {
                // should be thrown
            }
            finally
            {
                jsonFileRepo.VerifyAll();
            }
        }

        [TestMethod]
        public void Delete_sets_error_message_on_unhandled_DAL_exception_and_returns_same_view()
        {
            // setup
            var application = new JsonFileBuilder().Build();
            jsonFileRepo.Setup(p => p.GetById(application.Id)).Returns(application);
            jsonFileRepo.Setup(p => p.SoftDeleteAndSubmit(It.IsAny<JsonFile>())).Throws(new InvalidOperationException());
            var viewModel = new JsonFileViewModel(application);

            // act
            var controller = CreateInstance();
            var model = (ViewResult)controller.Delete(viewModel);

            // verify 
            Assert.IsNull(controller.ViewBag.SuccessMessage);
            Assert.IsNotNull(controller.ViewBag.ErrorMessage);
        }

        private JsonFileController CreateInstance()
        {
            return new JsonFileController(jsonFileRepo.Object, jsonService.Object);
        }
    }
}
