using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using TextFieldParserCore;
using IronPython;

namespace CodeJam2019_02.Controllers
{
    /// <summary>
    /// Describes the actions used for training or predicting
    /// </summary>
    public class MLController : Controller
    {

        // Holds the filepath of a input file
        private string filePath;
        private string fileName;

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
        /// This method uploads a input file to the InputFile folder of the wwwroot
        /// </summary>
        /// <param name="ufile"></param>
        /// <returns></returns>
        private bool UploadFile(IFormFile ufile)
        {
            if (ufile != null && ufile.Length > 0)
            {
                fileName = Path.GetFileName(ufile.FileName);
                filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\InputFiles", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ufile.CopyTo(fileStream);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method takes in the uploaded test files and preprocess them for prediction
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ProcessData(List<IFormFile> file1, List<IFormFile> file2, List<IFormFile> file3)
        {
            // Store files into SQL database
            List<List<IFormFile>> files = new List<List<IFormFile>>()
            {
                file1,
                file2,
                file3
            };

            foreach (var aFiles in files)
            {
                foreach (var file in aFiles)
                {
                    // Upload file to website folder
                    var uploadFile = UploadFile(file);
                    if (uploadFile != true)
                    {
                        throw new Exception(file.FileName + " failed to be uploaded");
                    }

                    // Create dataTable to hold csv file
                    DataTable csvData = new DataTable();

                    //Read csv into dataTable
                    try
                    {
                        using (TextFieldParser csvReader = new TextFieldParser(filePath))
                        {
                            csvReader.SetDelimiters(new string[] { "," });
                            csvReader.HasFieldsEnclosedInQuotes = true;
                            string[] colFields = csvReader.ReadFields();
                            foreach (string column in colFields)
                            {
                                DataColumn datecolumn = new DataColumn(column);
                                datecolumn.AllowDBNull = true;
                                csvData.Columns.Add(datecolumn);
                            }
                            while (!csvReader.EndOfData)
                            {
                                string[] fieldData = csvReader.ReadFields();
                                //Making empty value as null
                                for (int i = 0; i < fieldData.Length; i++)
                                {
                                    if (fieldData[i] == "")
                                    {
                                        fieldData[i] = null;
                                    }
                                }
                                csvData.Rows.Add(fieldData);
                            }
                        }
                    }
                    catch (Exception BadInputFile)
                    {
                        // Error converting csv to dataTable
                        throw new Exception("Error converting csv to dataTable", BadInputFile);
                    }

                    // Upload file to sql
                    using (SqlConnection dbConnection = new SqlConnection("Data Source=codejam2019db.database.windows.net;Initial Catalog=Project-02DB;User id=CodeJamAdmin;Password=CodeJam2019;"))
                    {


                        dbConnection.Open();

                        // Check if test table already has data
                        string sql = "SELECT COUNT(*) FROM [dbo].[" + fileName.Substring(0, fileName.Length - 4) + "Test]";
                        SqlCommand comm = new SqlCommand(sql, dbConnection);
                        Int32 count = (Int32)comm.ExecuteScalar();

                        if (count > 0)
                        {
                            //Delete content
                            sql = "DELETE * FROM [dbo].[" + fileName.Substring(0, fileName.Length - 4) + "Test]";
                            comm = new SqlCommand(sql, dbConnection);
                            comm.ExecuteNonQuery();
                        }

                        // Copy data into test sql tables
                        using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                        {
                            sql = "[dbo].[" + fileName.Substring(0, fileName.Length - 4) + "Test]";
                            s.DestinationTableName = sql;
                            foreach (var column in csvData.Columns)
                                s.ColumnMappings.Add(column.ToString(), column.ToString());
                            s.WriteToServer(csvData);
                        }
                    }
                }
            }

            // Preprocess all three tables and output to dataTable
            DataTable preProcessedData = new DataTable();

            //execute sql script
            using (SqlConnection dbConnection = new SqlConnection("Data Source=codejam2019db.database.windows.net;Initial Catalog=Project-02DB;User id=CodeJamAdmin;Password=CodeJam2019;"))
            {

                // Check if test table already has data
                string sql = "Select [user_tableTest].UserID" +
                    ", substring([user_tableTest].BinAge,2,2) as MinAge" +
                    ",substring([user_tableTest].BinAge, 8,2) as MaxAge" +
                    ",substring([user_tableTest].CountryID, 9,1) as CountryID" +
                    ",datepart(year, [user_tableTest].GameInstallDate) as GameInstallYear" +
                    ",datepart(month, [user_tableTest].GameInstallDate) as GameInstallMonth" +
                    ",datepart(day, [user_tableTest].GameInstallDate) as GameInstallDay" +
                    ",datepart(hour, [user_tableTest].GameInstallDate) as GameInstallHour" +
                    ",[UserTable].Gender,substring([user_tableTest].OSVersion,1,1) as OSVersion" +
                    ",case when substring([user_tableTest].OSVersion,3,1) like '' then '0' else substring([user_tableTest].OSVersion,3,1) end as OSUpdate" +
                    ",case when substring([user_tableTest].OSVersion,5,1) like '' then '0' else substring([user_tableTest].OSVersion,5,1) end as OSPatch" +
                    ",[user_tableTest].Ref" +
                    ",ISNULL(substring([user_tableTest].SourceID, 8,2)" +
                    ",cast(-1 as nvarchar)) as SourceID" +
                    ",ISNULL([user_apps_statisticsTest].nShoppingApps" +
                    ",cast(0 as nvarchar)) as nShoppingApps" +
                    ",ISNULL([user_apps_statisticsTest].nTotalApps" +
                    ",cast(0 as nvarchar)) as nTotalApps" +
                    ",case when[user_purchase_eventsTest].AmountSpend IS NULL then '0' else '1' end as AmountSpend" +
                    " From [dbo].[user_tableTest] left join [dbo].[user_apps_statisticsTest] on [dbo].[user_tableTest].UserID = [dbo].[user_apps_statisticsTest].UserID" +
                    " left join [dbo].[user_purchase_eventsTest] on [dbo].[user_tableTest].UserID = [dbo].[user_purchase_eventsTest].UserID" +
                    " Where [dbo].[user_tableTest].UserID <> '' and substring([user_tableTest].BinAge, 8,2) <> '' and [user_tableTest].Gender IS NOT NULL";

                SqlCommand comm = new SqlCommand(sql, dbConnection);
                dbConnection.Open();
                SqlDataReader reader = comm.ExecuteReader();
                try
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        DataRow aRow  = preProcessedData.NewRow();
                        aRow[count] = reader.GetString(0);
                        preProcessedData.Rows.Add(aRow);
                        count++;
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }

            //Store dataTable
            TempData["PreProcessedData"] = preProcessedData;

            

            return View();
        }
    }
}

