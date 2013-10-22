﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JusTeeth.Models;
using JusTeeth.Data;
using JusTeeth.App.Areas.Administration.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace JusTeeth.App.Areas.Administration.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HungryGroupsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Administration/HungryGroups/
        public ActionResult Index()
        {
            return View(db.HungryGroups.ToList());
        }

        public ActionResult ReadHungryGroups([DataSourceRequest]DataSourceRequest request)
        {
            DataSourceResult gridSource = db.HungryGroups.ToDataSourceResult(request);

            return Json(gridSource);
        }

        // GET: /Administration/HungryGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HungryGroup hungrygroup = db.HungryGroups.Find(id);
            if (hungrygroup == null)
            {
                return HttpNotFound();
            }
            return View(hungrygroup);
        }

        // GET: /Administration/HungryGroups/Create

        // POST: /Administration/HungryGroups/Create
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // 
        // Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpGet]
        public ActionResult Create()
        {
            HungryGroupViewModel hungrygroup = new HungryGroupViewModel();

            hungrygroup.EatItemsDropdown = GetEatTypeItems();
            hungrygroup.PlacesDropdown = GetPlaces();

            return View(hungrygroup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HungryGroupViewModel hungrygroup)
        {
            if (ModelState.IsValid)
            {
                hungrygroup.Creator = this.Db.UsersRepository.All().FirstOrDefault(x => x.UserName == User.Identity.Name);

                hungrygroup.Place = this.Db.PlaceRepository.All().FirstOrDefault(x => x.Id == hungrygroup.PlaceId);

                HungryGroup newGroup = new HungryGroup()
                {
                    Creator = hungrygroup.Creator,
                    EatTime = hungrygroup.EatTime,
                    HungryUsers = hungrygroup.HungryUsers,
                    Id = hungrygroup.Id,
                    Place = hungrygroup.Place,
                    StartingTime = hungrygroup.StartingTime
                };
                this.Db.HungryGroupRepository.Add(newGroup);
                this.Db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hungrygroup);
        }

        // GET: /Administration/HungryGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            HungryGroupViewModel hungryGroupModel = new HungryGroupViewModel();

            hungryGroupModel.EatItemsDropdown = GetEatTypeItems();
            hungryGroupModel.PlacesDropdown = GetPlaces();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HungryGroup hungrygroup = this.Db.HungryGroupRepository.All().FirstOrDefault(x => x.Id == id);
            if (hungrygroup == null)
            {
                return HttpNotFound();
            }
            else
            {
                hungryGroupModel.EatTime = hungrygroup.EatTime;
                hungryGroupModel.EatItemsDropdown.FirstOrDefault(x => x.Text == hungryGroupModel.EatTime.ToString()).Selected = true;
                hungryGroupModel.StartingTime = hungrygroup.StartingTime;
            }

            return View(hungryGroupModel);
        }

        // POST: /Administration/HungryGroups/Edit/5
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // 
        // Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HungryGroupViewModel hungrygroup)
        {
            var place = db.Places.FirstOrDefault(x => x.Id == hungrygroup.Id);

            HungryGroup editedHungryGroup = new HungryGroup()
            {
                Creator = hungrygroup.Creator,
                EatTime = hungrygroup.EatTime,
                HungryUsers = hungrygroup.HungryUsers,
                Id = hungrygroup.Id,
                Place = place,
                StartingTime = hungrygroup.StartingTime
            };

            if (ModelState.IsValid)
            {
                db.Entry(editedHungryGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hungrygroup);
        }

        // GET: /Administration/HungryGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HungryGroup hungrygroup = db.HungryGroups.Find(id);
            if (hungrygroup == null)
            {
                return HttpNotFound();
            }
            return View(hungrygroup);
        }

        // POST: /Administration/HungryGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HungryGroup hungrygroup = db.HungryGroups.Find(id);
            db.HungryGroups.Remove(hungrygroup);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private List<SelectListItem> GetEatTypeItems()
        {
            string[] rawTypes = Enum.GetNames(typeof(EatTimeType));
            List<SelectListItem> eatList = new List<SelectListItem>();
            for (int i = 0; i < rawTypes.Length; i++)
            {
                eatList.Add(new SelectListItem()
                {
                    Text = rawTypes[i],
                    Value = (i + 1).ToString()
                });
            }

            return eatList;
        }

        private List<SelectListItem> GetPlaces()
        {
            var places = this.Db.PlaceRepository.All();
            List<SelectListItem> placesList = new List<SelectListItem>();
            foreach (var item in places)
            {
                placesList.Add(new SelectListItem() 
                { 
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            return placesList;
        }
    }
}
