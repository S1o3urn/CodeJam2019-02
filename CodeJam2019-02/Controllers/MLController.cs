using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
            //TODO: Currently not implemented
            return View();
        }

        /// Leads to view with input submission
        public IActionResult PredictSpending()
        {
            return View();
        }

        /// <summary>
        /// This method takes in the uploaded test files and preprocess them for prediction
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ProcessData(List<IFormFile> files)
        {
            //TODO: Store files into SQL database
            foreach (var file in files)
            {
                // Create dataTable to hold csv file
                DataTable csvData = new DataTable();

                //Read csv into dataTable
                try
                {
                    
                }
                catch(Exception BadInputFile)
                {
                    return null;
                }
            }
            //TODO: Preprocess all three tables

            return Ok();
        }
    }
}