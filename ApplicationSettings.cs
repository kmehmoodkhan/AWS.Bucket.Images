using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS.Bucket.Images
{
    public static class ApplicationSettings
    {
        public static string DirectoryPath
        {
            get
            {
                string directoryPath = string.Empty;

                if(!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Path"]))
                {
                    directoryPath = ConfigurationManager.AppSettings["Path"].ToString();
                }
                return directoryPath;
            }
        }

        public static string AccessKey
        {
            get
            {
                string accessKey = string.Empty;

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AccessKey"]))
                {
                    accessKey = ConfigurationManager.AppSettings["AccessKey"].ToString();
                }
                return accessKey;
            }
        }

        public static string SecreteKey
        {
            get
            {
                string secreteKey = string.Empty;

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SecreteKey"]))
                {
                    secreteKey = ConfigurationManager.AppSettings["SecreteKey"].ToString();
                }
                return secreteKey;
            }
        }
        public static string ConnectionString
        {
            get
            {
                string connectionString = string.Empty;

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    connectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
                }
                return connectionString; 
            }
        }
    }
}
