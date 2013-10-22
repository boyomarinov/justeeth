﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JusTeeth.App.ViewModels
{
    public class DetailedPlaceViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AlternativeName { get; set; }

        public string Address { get; set; }

        public double Rating { get; set; }

        public double MonthRating { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string EatColor { get; set; }

        public string Description { get; set; }

        public int EatPiePercent { get; set; }

        public int EatRemainingPercent { get; set; }

        public string PriceColor { get; set; }

        public int PricePiePercent { get; set; }

        public int PriceRemainingPercent { get; set; }

        public ICollection<LightUserViewModel> Users { get; set; }

        public ICollection<FeedbackViewModel> Feedbacks { get; set; }
    }
}