using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReadLargeFileDemo
{
    class Program
    {
        // 來源檔案
        // private static readonly string logfile = @"/tmp/log20170630.csv";
        private static readonly string logfile = @"./sample.log";

        // 設定要輸入的檔名
        private static readonly string outputfile = @"./output.log";

        // 是否顯示符合的字串
        private static readonly bool DisplayLine = true;

        static void Main(string[] args)
        {
            DateTime end;
            DateTime start = DateTime.Now;

            Console.WriteLine("### Start at: " + start.ToLongTimeString());
            Console.WriteLine();

            // Test Case #1
            //TestReadingUsingStreamReaderAndRegex();
            // Test Case #2
            TestReadingUsingStreamReaderAndLinq();

            end = DateTime.Now;
            Console.WriteLine("### Finished at: " + start.ToLongTimeString());
            Console.WriteLine("### During Time: " + (end - start));
            Console.WriteLine();
            Console.WriteLine("Hit Enter to Exit!!");
        }

        static void TestReadingUsingStreamReaderAndRegex()
        {
            DateTime jobEnd;
            DateTime jobStart = DateTime.Now;
            Console.WriteLine(" [Test Case] TestReadingUsingStreamReaderAndRegex");
            Console.WriteLine("Reading file： " + logfile);
            Console.WriteLine("Job Start Time： " + jobStart.ToLongTimeString());

            try
            {
                using (StreamReader sr = File.OpenText(logfile))
                {
                    string line = "";
                    long total = 0;
                    long matchlines = 0;

                    string pattern = "^(.*gge.*19617.*xml).*$";   /* 使用 Regex 查詢三個關鍵字 */

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (RegexIsMatch(pattern, line))
                        {
                            if (DisplayLine) Console.WriteLine(string.Format("Line({0}) : {1}", total, line));
                            matchlines++;
                        }
                        total++;
                    }
                    Console.WriteLine(string.Format("Line({0}) {1}", total, line));
                    Console.WriteLine("Match Lines: " + matchlines);
                    Console.WriteLine("Total Lines: " + total);
                }

            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine("Not enough memory. Couldn't perform this test.");
                Console.WriteLine();
            }
            catch (Exception)
            {
                Console.WriteLine("EXCEPTION. Couldn't perform this test.");
                Console.WriteLine();
            }
            jobEnd = DateTime.Now;
            Console.WriteLine("Job End Time： " + jobEnd.ToLongTimeString());
            Console.WriteLine("Job During： " + (jobEnd - jobStart));
            Console.WriteLine("====================================================");

            GC.Collect();
        }

        static async void TestReadingUsingStreamReaderAndLinq()
        {
            DateTime jobEnd;
            DateTime jobStart = DateTime.Now;
            Console.WriteLine(" [Test Case] TestReadingUsingStreamReaderAndLinq");
            Console.WriteLine("Reading file： " + logfile);
            Console.WriteLine("Job Start Time： " + jobStart.ToLongTimeString());

            try
            {
                using (StreamReader sr = File.OpenText(logfile))
                {
                    string line = "";
                    long total = 0;
                    long matchlines = 0;
                    string[] toSearchFor = new string[] { "xslF345X02/primary_doc.xml", "725363", "3537" };

                    StringBuilder sb = new StringBuilder();
                    while ((line = sr.ReadLine()) != null)
                    {
                        // 使用Linq 查詢字串是否包含特色字串
                        if (ContainsAll(line, toSearchFor))
                        {
                            if (DisplayLine)
                            {
                                sb.AppendLine(string.Format("Line({0}) : {1}", total, line));
                                // Console.WriteLine(string.Format("Line({0}) : {1}", total, line));
                            }
                            matchlines++;
                        }
                        total++;
                    }

                    // 非同步寫入符合的記錄 
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(outputfile))
                    {
                        // Can write either a string or char array
                        await file.WriteAsync(sb.ToString());
                    }

                    Console.WriteLine("Match Lines: " + matchlines);
                    Console.WriteLine("Total Lines: " + total);
                }

            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine("Not enough memory. Couldn't perform this test.");
                Console.WriteLine();
            }
            catch (Exception)
            {
                Console.WriteLine("EXCEPTION. Couldn't perform this test.");
                Console.WriteLine();
            }
            jobEnd = DateTime.Now;
            Console.WriteLine("Job End Time： " + jobEnd.ToLongTimeString());
            Console.WriteLine("Job During： " + (jobEnd - jobStart));
            Console.WriteLine("====================================================");

            GC.Collect();
        }

        static bool RegexIsMatch(string pattern, string line)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(line, pattern, RegexOptions.IgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool ContainsAll(string input, string[] toSearchFor)
        {
            // 109.145.75.gge,2017-06-30,00:00:00,0.0,725363.0,0001236031-06-000019,xslF345X02/primary_doc.xml,200.0,3539.0,0.0,0.0,0.0,10.0,0.0,
            //string[] toSearchFor = new string[] { "gge", "19617", "xml", "mac" };
            var result = toSearchFor.All(x => input.Contains(x));
            return result;
        }

    }

}
