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
      if (type == "Error")
      {
        message = "[!] " + message;
        Console.ForegroundColor = ConsoleColor.Red;
      }
      else if (type == "Success")
        Console.ForegroundColor = ConsoleColor.Green;

      Console.WriteLine(message);
      Console.ResetColor();
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
    private static bool CreateNewFile(string OrigFile, string NewFile)
    {
      const int ExpectedCols = 20;
      const int IndxUnitNo = 7;
      const int IndxSwl = 9;
      const int IndxRswl = 10;
      bool isHeader = true;
      int lineNum = 0;
      string line;

      Print("\n\nCreating new file...");
      using (StreamWriter writer = new StreamWriter(NewFile))
      {
        using (StreamReader reader = new StreamReader(OrigFile))
        {
          while ((line = reader.ReadLine()) != null)
          {
            lineNum++;
            List<string> cols = line.Split(',').ToList<string>();

            // Assuming that comment will not contain newline.
            // Skipping any line with less than 20 columns.
            if (cols.Count < ExpectedCols)
            {
              Print(string.Format("Skipping line:{0}, cols: {1} ", lineNum.ToString(), cols.Count), "Error");
              continue;
            }

            // Number of columns can be more if comment column contain commas.
            // As comment column is at last, wrapping up all extra columns as a single column.
            while(cols.Count > ExpectedCols)
            {
              cols[cols.Count - 2] += "," + cols[cols.Count - 1];
              cols.RemoveAt(cols.Count - 1);
            }

            // Check for and put the closing quote character if necessary, needed to handle newline in comment (e.g. line 8079)
            int lastCol = cols.Count - 1;
            string strLastCol = cols[lastCol];
            if (strLastCol != "")
            {
              char lastColFirstChar = strLastCol[0];
              char lastColLastChar = strLastCol[strLastCol.Length - 1];
              if (lastColFirstChar == '"' && lastColLastChar != '"')
              {
                cols[lastCol] += '"';
              }
            }

            // Removing Unit No column.
            cols.RemoveAt(IndxUnitNo);

            if (isHeader)
            {
              isHeader = false;
              cols.Add("calc");
            }
            else
            {
              // Calculating the value for new calc column
              if (!double.TryParse(cols[IndxSwl], out double dbl_swl))
                dbl_swl = 0.0;

              if (!double.TryParse(cols[IndxRswl], out double dbl_rswl))
                dbl_rswl = 0.0;

              cols.Add((dbl_swl + dbl_rswl).ToString());
            }

            writer.WriteLine(string.Join(",", cols));
          }
        }
      }
      Print("\nNew file created.");
      return true;
    }
    static void Main(string[] args)
    {
      const string Url = @"https://apps.waterconnect.sa.gov.au/file.csv";
      const string OrigFile = "file.csv";
      const string NewFile = "newFile.csv";

      // Downloading file from specified url.
      if (DownloadFile(Url, OrigFile))
        Print("File Downloaded Successfully");
      else
      {
        Print("\nProgram Finished. Press Enter to exit.");
        Console.ReadLine();
        return;
      }

      // Creating new file
      CreateNewFile(OrigFile, NewFile);

      Print("\nNew file saved at: " + (new System.IO.FileInfo(NewFile)).FullName, "Success");

      Print("\nProgram Finished. Press Enter to exit.");
      Console.ReadLine();
    }
  }
}