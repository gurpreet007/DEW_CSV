using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace ConsoleApp1
{
  class Program
  {
    private static void Print(string message, string type="Info")
    {
      ConsoleColor consoleColor = Console.ForegroundColor;
      if (type == "Error")
      {
        message = "[!] " + message;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
      }
      else
      {
        Console.WriteLine(message);
      }
      Console.ForegroundColor = consoleColor;
    }
    private static bool DownloadFile(string strUrl, string fileName)
    {
      Print("Downloading file from the server...");
      try
      {
        (new WebClient()).DownloadFile(strUrl, fileName);
      }
      catch (Exception ex)
      {
        Print("Error downloading file. Exception: " + ex.Message, "Error");
        return false;
      }
      return true;
    }
    static void Main(string[] args)
    {
      const string URL = @"https://apps.waterconnect.sa.gov.au/file.csv";
      const string origFile = "file.csv";
      const string newFile = "newFile.csv";
      bool isDownloaded = DownloadFile(URL, origFile);

      if (isDownloaded)
        Print("File Downloaded Successfully");
      else
        Print("Error Downloading File", "Error");

      using (StreamWriter writer = new StreamWriter(newFile))
      {
        using (StreamReader reader = new StreamReader(origFile))
        {
          string line;
          const int INDX_UNITNO = 7;
          const int INDX_SWL = 9;
          const int INDX_RSWL = 10;
          string swl, rswl;
          bool header = true;
          int count = 1;
          while ((line = reader.ReadLine()) != null)
          {
            List<string> cols = line.Split(',').ToList<string>();

            //Assuming that comment will not contain newline
            //Skipping any line with less than 20 columns
            if (cols.Count < 20)
            {
              Print(string.Format("Skipping line:{0}, cols: {1} ", count.ToString(), cols.Count), "Error");
              continue;
            }

            //Number of columns can be more if comments column contain commas
            //As comment column is at last, wrapping up all extra columns as a single column
            while(cols.Count>20)
            {
              cols[cols.Count - 2] += "," + cols[cols.Count - 1];
              cols.RemoveAt(cols.Count - 1);
            }

            //Removing Unit No column
            cols.RemoveAt(INDX_UNITNO);

            if (header)
            {
              header = false;
              cols.Add("calc");
            }
            else
            {
              swl = cols[INDX_SWL];
              rswl = cols[INDX_RSWL];
              if (!double.TryParse(swl, out double dbl_swl)) dbl_swl = 0.0;
              if (!double.TryParse(rswl, out double dbl_rswl)) dbl_rswl = 0.0;
              cols.Add((dbl_swl + dbl_rswl).ToString());
            }
            string newLine = string.Join(",", cols);
            writer.WriteLine(newLine);
            count++;
          }
        }
      }
      Print("New file saved at: " + (new System.IO.FileInfo(newFile)).FullName);
    }
  }
}