using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CodeJam2019_02.Controllers
{
    /// <summary>
    /// Describes the actions used for training or predicting
    /// </summary>
    public class MLController : Controller
    {

        /// Trains the preprocessed data
        public IActionResult TrainModel()
        {
            return View();
        }

        /// Leads to view with input submission
        public IActionResult PredictSpending()
        {
            return View();
        }
    }
}