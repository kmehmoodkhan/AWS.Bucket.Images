using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS.Bucket.Images
{
    public class FileProcessor
    {
        public void ProcessFiles()
        {
            string[] files = Directory.GetFiles(ApplicationSettings.DirectoryPath, "*.*", SearchOption.AllDirectories);
            ImageUpload image = new ImageUpload();
            
            foreach(string file in files)
            {
                try
                {
                    AddRecord(Path.GetFileName(file), file);
                    string url = image.UploadImage(file);
                    url = ModifyUrl(url);
                    AddRecord(Path.GetFileName(file), file, url, false);
                }
                catch(Exception ex)
                {
                    ;
                }
            }            
        }

        private string ModifyUrl(string url)
        {
            string path = url.Substring(0, url.IndexOf("?"));
            return path;
        }
        public void AddRecord(string title,string image,string url="",bool isAdd=true)
        {
            try
            {
                if (isAdd)
                {
                    bool isExists = false;

                    using (SqlConnection connection = new SqlConnection(ApplicationSettings.ConnectionString))
                    {
                        string query = "SELECT 1 FROM dbo.Product WHERE Title='" + title + "'";
                        connection.Open();
                        SqlCommand command = new SqlCommand(query, connection);
                       var result= command.ExecuteScalar();
                        if(result != null)
                        {
                            isExists = true;
                        }
                        connection.Close();
                    }

                    if (!isExists)
                    {
                        using (SqlConnection connection = new SqlConnection(ApplicationSettings.ConnectionString))
                        {
                            string query = "INSERT INTO dbo.Product (Title,Image) VALUES('" + title + "','" + image + "')";
                            connection.Open();
                            SqlCommand command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(ApplicationSettings.ConnectionString))
                    {
                        string query = "UPDATE dbo.Product Set AWSImageURL='"+ url + "' WHERE title='"+ title + "'";
                        connection.Open();
                        SqlCommand command = new SqlCommand(query, connection);
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                
            }
            catch(Exception ex)
            {
                throw (ex);
            }
        }
    }
}
