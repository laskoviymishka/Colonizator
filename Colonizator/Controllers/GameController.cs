﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Colonizator.Controllers
{
    public class GameController : Controller
    {
        //
        // GET: /Game/
        public ActionResult Index(string id)
        {
            return View();
        }

        public ActionResult All()
        {
            return View();
        }
    }
}