using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AWS.Bucket.Images
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                FileProcessor process = new FileProcessor();
                process.ProcessFiles();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception thrown::::" + ex.Message);
            }
        }
    }
}
