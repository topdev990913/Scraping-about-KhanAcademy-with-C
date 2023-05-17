using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumConsoleApp
{
    class Program
    {
        public const int DEBUGLIMIT = 10;

        static void Main(string[] args)
        {
            FileStream fstream = File.OpenWrite("outtab.csv");
            string outstring = $"Begin: {DateTime.Now.ToString()}\n";
            fstream.Write(Encoding.UTF8.GetBytes(outstring), 0, Encoding.UTF8.GetBytes(outstring).Length);
            outstring = "repository type\t"
                + "course title\t"
                + "course summary\t"
                + "unit title\t"
                + "lesson title\t"
                + "video title\t"
                + "video url\t"
                + "Type\n";
            fstream.Write(Encoding.UTF8.GetBytes(outstring), 0, Encoding.UTF8.GetBytes(outstring).Length);


            string base_url = "https://www.khanacademy.org/";
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            IWebDriver driver = new ChromeDriver(service, options, TimeSpan.FromMinutes(3));
            driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));
            driver.Url = base_url;

            // Find the main course groupings:
            //  e.g. "MATH: Pre-K - 8th Grade", "MATH: Get Ready Courses"...
            var elements = driver.FindElements(By.ClassName("_1b2zmqj"));
            foreach (IWebElement elem in elements)
            {
                string course_title = elem.FindElement(By.ClassName("_17zmj242")).GetAttribute("innerHTML");
                var summaries = elem.FindElements(By.ClassName("_xy39ea8"));
                foreach (IWebElement summ in summaries)
                {
                    string course_summary_title = summ.GetAttribute("innerHTML");
                    Console.WriteLine($"  Processing course summary: {course_title} > {course_summary_title}");
                    IWebElement pare = summ.FindElement(By.XPath(".."));
                    IWebDriver driver1 = new ChromeDriver(service, options, TimeSpan.FromMinutes(3));
                    driver1.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));
                    driver1.Url = pare.GetAttribute("href");
                    var units = driver1.FindElements(By.ClassName("_k50sd54"));  
                    foreach (IWebElement unit in units)
                    {
                         string unit_title = unit.GetAttribute("innerHTML");
                         Console.WriteLine($"  Processing Unit:          > {unit_title}");
                         IWebElement paren = unit.FindElement(By.XPath(".."));
                         IWebDriver driver2 = new ChromeDriver(service, options, TimeSpan.FromMinutes(3));
                         driver2.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));
                         driver2.Url = paren.GetAttribute("href");
                         var lessons = driver2.FindElements(By.ClassName("_2qe0as")); 
                         foreach (IWebElement lesson in lessons)
                        {
                            string lesson_title = lesson.FindElement(By.CssSelector("a[data-test-id='lesson-card-link']")).GetAttribute("innerHTML");
                            lesson_title = lesson_title.Replace("<!-- -->", "");
                            Console.WriteLine($"  Processing Lesson:              > {lesson_title}");
                            IWebDriver driver3 = new ChromeDriver(service, options, TimeSpan.FromMinutes(3));
                            driver3.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));
                            driver3.Url = lesson.FindElement(By.CssSelector("a[data-test-id='lesson-card-link']")).GetAttribute("href");                            
                              
                            var videowebs = driver3.FindElements(By.ClassName("_1jz2ixj")); 
                            foreach (IWebElement videoweb in videowebs)
                            {
                                string video_title = videoweb.FindElement(By.ClassName("_1naqmn96")).GetAttribute("innerHTML");
                                video_title = video_title.Replace("<!-- -->", "");
                                IWebElement parenn = videoweb.FindElement(By.XPath(".."));                                
                                string video_link = parenn.GetAttribute("href");
                                Console.WriteLine($"  Processing Video:                     > {video_title} > {video_link}");

                                IWebDriver driver4 = new ChromeDriver(service, options, TimeSpan.FromMinutes(3));
                                driver4.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));
                                driver4.Url = video_link;
                                var actualvideos = driver4.FindElements(By.ClassName("ka-player-iframe")); 
                                foreach (IWebElement actualvideo in actualvideos)
                                {
                                    string actualvideo_link = actualvideo.GetAttribute("src");
                                    Console.WriteLine($"  Processing Video:                     > {video_title} > {video_link} > {actualvideo_link}");

                                
                                }
                                driver4.Close();
                            
                                string out_line_string = "KA\t";
                                out_line_string += course_title + "\t";
                                out_line_string += course_summary_title + "\t";
                                out_line_string += unit_title + "\t";
                                out_line_string += lesson_title + "\t";
                                out_line_string += video_title + "\t";
                                out_line_string += video_link + "\t";
                                out_line_string += "\t";                                
                                fstream.Write(Encoding.UTF8.GetBytes(out_line_string), 0, Encoding.UTF8.GetBytes(out_line_string).Length);
                                fstream.Flush();
                            
                            }
                            driver3.Close();  
                        }
                         driver2.Close();
                    }
                    
                    driver1.Close();
                }
            }

            driver.Close();

            outstring = "End: " + DateTime.Now.ToString();
            fstream.Write(Encoding.UTF8.GetBytes(outstring), 0, Encoding.UTF8.GetBytes(outstring).Length);
            fstream.Close();
        }
    }
}
