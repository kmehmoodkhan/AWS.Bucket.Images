using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS.Bucket.Images
{
    public class FileProcessor
    {
        const int PageSize = 500;
        public void ProcessFiles()
        {
            int totalRecords = GetTotalRecords();

            int pages = 0;

            pages = (int)Math.Ceiling((double)totalRecords / PageSize);

            for (int i= 0; i < pages; i++) {
                DataSet dataSet = GetNextRecords();
                ImageUpload image = new ImageUpload();

                if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        foreach (DataRow dr in dataSet.Tables[0].Rows)
                        {
                            string tempFileName = dr["Image"].ToString();
                            int recordId = Convert.ToInt32(dr["Product_Id"]);
                            string fileName = string.Empty;

                            fileName = ApplicationSettings.DirectoryPath;

                            if (tempFileName.StartsWith(@"\"))
                            {
                                fileName += tempFileName;
                            }
                            else
                            {
                                fileName += "\\"+tempFileName;
                            }



                            string url = image.UploadImage(fileName);
                            url = ModifyUrl(url);
                            AddRecord(Path.GetFileName(fileName), fileName, recordId, url);

                        }
                    }
                    catch (Exception ex)
                    {
                        ;
                    }
                }
            }   
        }

        private string ModifyUrl(string url)
        {
            string path = url.Substring(0, url.IndexOf("?"));
            return path;
        }

        public DataSet GetNextRecords()
        {
            DataSet dsRecords = null;
            try
            {                
                using (SqlConnection connection = new SqlConnection(ApplicationSettings.ConnectionString))
                {

                    string query = $"SELECT TOP {PageSize} Product_Id,Title,[Image],AWSImageUrl FROM dbo.Product WHERE AWSImageURL IS NULL";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, ApplicationSettings.ConnectionString);

                    dsRecords = new DataSet();
                    adapter.Fill(dsRecords);
                }
            }
            catch(Exception ex)
            {
                ;
            }
            return dsRecords;
        }

        public int GetTotalRecords()
        {
            int totalRecords = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ApplicationSettings.ConnectionString))
                {

                    string query = "SELECT COUNT(1) FROM dbo.Product WHERE AWSImageURL IS NULL";
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    var result = command.ExecuteScalar();

                    if (result != null)
                    {
                        totalRecords = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                ;
            }
            return totalRecords;
        }


        public void AddRecord(string title,string image, int productId,string url)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ApplicationSettings.ConnectionString))
                {
                    string query = "UPDATE dbo.Product Set AWSImageURL='" + url + "' WHERE product_Id='" + productId + "'";
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }

            }
            catch(Exception ex)
            {
                throw (ex);
            }
        }
    }
}
