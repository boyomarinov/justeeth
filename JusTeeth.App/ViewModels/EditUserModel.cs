﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JusTeeth.App.ViewModels
{
    public class EditUserModel
    {
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public HttpPostedFileBase Avatar { get; set; }

        public string FacebookProfile { get; set; }

        public string UserName { get; set; }

        public string Department { get; set; }

        public string Workplace { get; set; }
    }
}