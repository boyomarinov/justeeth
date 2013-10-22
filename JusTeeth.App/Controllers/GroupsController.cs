﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JusTeeth.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using JusTeeth.App.ViewModels;
using System.Threading;
using Microsoft.AspNet.Identity;

namespace JusTeeth.App.Controllers
{
    public class GroupsController : BaseController
    {
        //
        // GET: /Groups/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(CreateGroupViewModel model)
        {
            var chosenPlace = this.Db.PlaceRepository.All().FirstOrDefault(x => x.Name == model.PlacePlaceholder);
            var userId = User.Identity.GetUserId();
            var user = this.Db.UsersRepository.All().FirstOrDefault(x => x.Id == userId);

            var newGroup = new HungryGroup
            {
                Creator = user,
                Place = chosenPlace,
                StartingTime = model.StartingTime
            };

            this.Db.HungryGroupRepository.Add(newGroup);
            this.Db.SaveChanges();

            return View();
        }

        public ActionResult GetHungryGroups([DataSourceRequest] DataSourceRequest request)
        {
            var result = this.Db.HungryGroupRepository.All().Select(x => new HungryGroupViewModel
                    {
                        CreatorId = x.Creator.Id,
                        Creator = x.Creator.DisplayName,
                        EatTime = x.EatTime.ToString(),
                        Id = x.Id,
                        Place = x.Place.Name,
                        PlaceId = x.Place.Id,
                        StartingTime = x.StartingTime.ToString("H:mm:ss"),
                        UserName = x.Creator.UserName
                    });
            return Json(result.ToDataSourceResult(request));
        }

        public ActionResult GetGrpupEatTime(int id)
        {

            return Json("");
        }

        public ActionResult GroupDetails(int? id)
        {
            if (id == null)
            {
                //TODO: some bad exception
            }

            var groupEntity = this.Db.HungryGroupRepository.All().FirstOrDefault(x => x.Id == id);
            if (groupEntity == null)
            {
                //TODO: error handling
            }

            int piePercent = 10;
            string color = "#37e800";
            switch (groupEntity.EatTime)
            {
                case (EatTimeType)0:
                    piePercent = 10;
                    color = "#37e800";
                    break;
                case (EatTimeType)1:
                    piePercent = 25;
                    color = "#dae800";
                    break;
                case (EatTimeType)2:
                    piePercent = 50;
                    color = "#e8a400";
                    break;
                case (EatTimeType)3:
                    piePercent = 70;
                    color = "#dae800";
                    break;
                case (EatTimeType)4:
                    piePercent = 90;
                    color = "#e8a400";
                    break;
                default:
                    piePercent = 10;
                    color = "#37e800";
                    break;
            }
            int remainingPercent = 100 - piePercent;

            int pricePiePercent = 10;
            string priceColor = "#37e800";
            switch (groupEntity.Place.PriceType)
            {
                case (PriceType)0:
                    pricePiePercent = 10;
                    priceColor = "#37e800";
                    break;
                case (PriceType)1:
                    pricePiePercent = 25;
                    priceColor = "#dae800";
                    break;
                case (PriceType)2:
                    pricePiePercent = 50;
                    priceColor = "#e8a400";
                    break;
                case (PriceType)3:
                    pricePiePercent = 70;
                    priceColor = "#dae800";
                    break;
                case (PriceType)4:
                    pricePiePercent = 90;
                    priceColor = "#e8a400";
                    break;
                default:
                    pricePiePercent = 10;
                    priceColor = "#37e800";
                    break;
            }
            int priceRemainingPercent = 100 - pricePiePercent;

            var model = new DetailedHungryGroupViewModel()
            {
                Creator = groupEntity.Creator.DisplayName,
                CreatorUsername = groupEntity.Creator.UserName,
                CreatorId = groupEntity.Creator.Id,
                EatTime = groupEntity.EatTime.ToString(),
                Id = groupEntity.Id,
                Place = groupEntity.Place.Name,
                Address = groupEntity.Place.Address,
                PlaceId = groupEntity.Place.Id,
                StartingTime = groupEntity.StartingTime.ToString("MM/dd/yy H:mm:ss"),
                Latitude = groupEntity.Place.Latitude,
                Longitude = groupEntity.Place.Longitude,
                EatColor = color,
                EatPiePercent = piePercent,
                EatRemainingPercent = remainingPercent,
                PriceColor = priceColor,
                PricePiePercent = pricePiePercent,
                PriceRemainingPercent = priceRemainingPercent,
                Users = (from user in groupEntity.HungryUsers
                         select new HungryUserViewModel()
                         {
                             Id = user.Id,
                             DisplayName = user.DisplayName,
                             Avatar = user.Avatar
                         }).ToList()
            };

            return View(model);
        }

        public ActionResult JoinGroup(int id)
        {
            var currentUserName = User.Identity.Name;
            var curretnUser = this.Db.UsersRepository.GetUserByUsername(currentUserName);

            var group = this.Db.HungryGroupRepository.GetById(id);
            group.HungryUsers.Add(curretnUser);
            this.Db.SaveChanges();

            return RedirectToAction("GroupDetails", new { id = id });
        }

        public ActionResult FilterBy(string filterParam)
        {
            switch (filterParam)
            {
                case "company":
                    return PartialView("_GroupsList", this.GetGroupsByCompany());
                case "department":
                    return PartialView("_GroupsList", this.GetGroupsByDepartment());
                case "startTime":
                    return PartialView("_GroupsList", this.GetGroupsByStartTime());
                case "timeToEat":
                    return PartialView("_GroupsList", this.GetGroupsByEatingTime());
                case "money":
                    return PartialView("_GroupsList", this.GetGroupsByMoney());
                default:
                    return PartialView("_GroupsList", this.GetGroupsByStartTime());
            }
        }

        public ActionResult GetCreateGroupFormPartial()
        {
            return PartialView("_CreateGroupForm", new CreateGroupViewModel());
        }

        //public ActionResult CreateGroup(CreateGroupViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return PartialView("_CreateGroupForm", model);
        //    }
        //    return View();
        //}

        public ActionResult ChatWindow(int id)
        {
            return PartialView("_ChatWindow", id);
        }

        //public JsonResult GetAutocompleteData(string text)
        //{ 
            
        //}

        private IEnumerable<HungryGroupViewModel> GetGroupsByMoney()
        {
            var currentUserName = User.Identity.Name;
            var curretnUser = this.Db.UsersRepository.All().FirstOrDefault(x => x.UserName == currentUserName);
            var groups = this.Db.HungryGroupRepository
               .All()
               .Include(x => x.Place)
               .Include(x => x.Creator)
               .Where(x => x.StartingTime > DateTime.Now)
               .OrderBy(x => x.Place.PriceType)
               .ToList()
               .Select(
                    x => new HungryGroupViewModel
                    {
                        Creator = x.Creator.DisplayName,
                        CreatorId = x.Creator.Id,
                        EatTime = x.EatTime.ToString(),
                        Id = x.Id,
                        Place = x.Place.Name,
                        PlaceId = x.Place.Id,
                        StartingTime = x.StartingTime.ToString("H:mm:ss MM/dd/yy"),
                        UserName = x.Creator.UserName,
                        Price = x.Place.PriceType.ToString(),
                        IsCurrentUserJoint = x.HungryUsers.Contains(curretnUser)
                    }
               );

            return groups;
        }

        private IEnumerable<HungryGroupViewModel> GetGroupsByEatingTime()
        {
            var currentUserName = User.Identity.Name;
            var curretnUser = this.Db.UsersRepository.All().FirstOrDefault(x => x.UserName == currentUserName);
            var groups = this.Db.HungryGroupRepository
               .All()
               .Where(x => x.StartingTime > DateTime.Now)
               .OrderBy(x => (int)x.EatTime)
               .ToList()
               .Select(
                    x => new HungryGroupViewModel
                    {
                        Creator = x.Creator.DisplayName,
                        CreatorId = x.Creator.Id,
                        EatTime = x.EatTime.ToString(),
                        Id = x.Id,
                        Place = x.Place.Name,
                        PlaceId = x.Place.Id,
                        StartingTime = x.StartingTime.ToString("H:mm:ss MM/dd/yy"),
                        UserName = x.Creator.UserName,
                        Price = x.Place.PriceType.ToString(),
                        IsCurrentUserJoint = x.HungryUsers.Contains(curretnUser)
                    }
               );

            return groups;
        }

        private IEnumerable<HungryGroupViewModel> GetGroupsByStartTime()
        {
            var currentUserName = User.Identity.Name;
            var curretnUser = this.Db.UsersRepository.All().FirstOrDefault(x => x.UserName == currentUserName);
            var groups = this.Db.HungryGroupRepository
               .All()
               .Where(x => x.StartingTime > DateTime.Now)
               .OrderBy(x => x.StartingTime)
               .ToList()
               .Select(
                    x => new HungryGroupViewModel
                    {
                        Creator = x.Creator.DisplayName,
                        CreatorId = x.Creator.Id,
                        EatTime = x.EatTime.ToString(),
                        Id = x.Id,
                        Place = x.Place.Name,
                        PlaceId = x.Place.Id,
                        StartingTime = x.StartingTime.ToString("H:mm:ss MM/dd/yy"),
                        UserName = x.Creator.UserName,
                        Price = x.Place.PriceType.ToString(),
                        IsCurrentUserJoint = x.HungryUsers.Contains(curretnUser)
                    }
               );

            return groups;
        }

        private IEnumerable<HungryGroupViewModel> GetGroupsByDepartment()
        {
            var currentUserName = User.Identity.Name;
            var curretnUser = this.Db.UsersRepository.All().FirstOrDefault(x => x.UserName == currentUserName);
            var currentUserWorkplace = curretnUser.Department.Workplace;
            var currentUserDepartment = curretnUser.Department.Name;

            var groups = this.Db.HungryGroupRepository
               .All()
               .Where(x => x.StartingTime > DateTime.Now
                   && x.Creator.Department.Workplace.Id == currentUserWorkplace.Id
                   && x.Creator.Department.Name == currentUserDepartment)
               .ToList()
               .Select(
                    x => new HungryGroupViewModel
                    {
                        Creator = x.Creator.DisplayName,
                        CreatorId = x.Creator.Id,
                        EatTime = x.EatTime.ToString(),
                        Id = x.Id,
                        Place = x.Place.Name,
                        PlaceId = x.Place.Id,
                        StartingTime = x.StartingTime.ToString("H:mm:ss MM/dd/yy"),
                        UserName = x.Creator.UserName,
                        Price = x.Place.PriceType.ToString(),
                        IsCurrentUserJoint = x.HungryUsers.Contains(curretnUser)
                    }
               );

            return groups;
        }

        private IEnumerable<HungryGroupViewModel> GetGroupsByCompany()
        {
            var currentUserName = User.Identity.Name;
            var curretnUser = this.Db.UsersRepository.All().FirstOrDefault(x => x.UserName == currentUserName);
            var currentUserWorkplace = curretnUser.Department.Workplace;

            var groups = this.Db.HungryGroupRepository
               .All()
               .Where(x => x.StartingTime > DateTime.Now && x.Creator.Department.Workplace.Id == currentUserWorkplace.Id)
               .ToList()
               .Select(
                    x => new HungryGroupViewModel
                        {
                            Creator = x.Creator.DisplayName,
                            CreatorId = x.Creator.Id,
                            EatTime = x.EatTime.ToString(),
                            Id = x.Id,
                            Place = x.Place.Name,
                            PlaceId = x.Place.Id,
                            StartingTime = x.StartingTime.ToString("H:mm:ss MM/dd/yy"),
                            UserName = x.Creator.UserName,
                            Price = x.Place.PriceType.ToString(),
                            IsCurrentUserJoint = x.HungryUsers.Contains(curretnUser)
                        }
               );

            return groups;
        }

    }
}