using MangaShopping.Models.Data;
using MangaShopping.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MangaShopping.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //Declare List of PageVM
            List<PageVM> pagesList;

            using (Db db = new Db())
            {
                //Init the List
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            //Return view of list
            return View(pagesList);
        }

        //Get: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }
        //Post: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {

            //Check Model State
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {

                //Declare Slug
                string slug;
                //Init the PageDTO
                PageDTO dto = new PageDTO();


                //DTO Title
                dto.Title = model.Title;

                //Check for and set slug if need be
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //Make sure title and slug are unique
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That Title or Slug is already exists.");
                    return View(model);
                }

                //DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                //Save DTO
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            //Set Tempdata message
            TempData["SM"] = "Succesfully added a page.";
            //Redirect
            return RedirectToAction("AddPage");

        }
        //Get: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int Id)
        {
            //Declare PageVM
            PageVM model;


            using (Db db = new Db())
            {
                //Get The page
                PageDTO dto = db.Pages.Find(Id);
                //Confirm the page exists
                if (dto == null)
                {
                    return Content("The page does not exist");
                }

                //init the pageVM
                model = new PageVM(dto);

            }
            //return View with Model

            return View(model);
        }
        //Post: Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //check model state

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {

                //get page id
                int id = model.Id;

                //init slug
                string slug = "home";

                //get the page
                PageDTO dto = db.Pages.Find(id);

                //DTO The title
                dto.Title = model.Title;

                //check for slug and set if it needs be
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();

                    }
                }

                //make sure title and slug are unique
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) ||
                    db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That Title or Slug is already exists.");
                    return View(model);
                }

                //dto the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;


                //Save the dto
                db.SaveChanges();
            }

            //Set tempdata message
            TempData["SM"] = "You have edited the page";

            //redirect


            return RedirectToAction("EditPage");

        }

        //Get: Admin/Pages/PageDetails/id

        public ActionResult PageDetails(int id)
        {
            //declare PageVM
            PageVM model;
            using (Db db = new Db())
            {

                //Get the page
                PageDTO dto = db.Pages.Find(id);
                //Confirm the page exist
                if(dto==null)
                {
                    return Content("The Page does not exist");
                }
                model = new PageVM(dto);
                //init pageVM
            }
            //Return View with model
            return View(model);
        }

        //Get: Admin/Pages/DeletePage/id

         public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {

                //get the page
                PageDTO dto = db.Pages.Find(id);

                //remove the page 
                db.Pages.Remove(dto);

                //save the page
                db.SaveChanges();
            }
                //Redirect
            return RedirectToAction("Index");
        }

        //Post: Admin/Pages/ReorderPages
        [HttpPost]

        public void ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                //set initial count
                int count = 1;

                // Declare PageDTO
                PageDTO dto;

                //Set sorting for eah page
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;

                }
       
            }
        }
        //Get: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            //Declare the model
            SidebarVM model;
            using (Db db = new Db())
            {

                //get the DTO
                SidebarDTO dto = db.Sidebar.Find(1);
                //init the model
                model = new SidebarVM(dto);
            }
            //Return view with model

            return View(model);
        }
        //Post: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db =new Db())
            {
                //Get the DTO
                SidebarDTO dto = db.Sidebar.Find(1);

                //DTO the body
                dto.Body = model.Body;

                //Save
                db.SaveChanges();
        
            }

            //Set the tempdata message
            TempData["SM"] = "You have edited the sidebar";
            //Redirect
            return RedirectToAction("EditSidebar");
        }




    }
} 