using JobSeeker;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Text.RegularExpressions;

public class JobSeekerBot
{
    static string url = Program.url;
    static string email = Program.email;
    static string password = Program.password;
    static string[] fieldWords = Program.fieldWords;
    static string[] keyWords = Program.keyWords;
    static string[] coverLetters = Program.coverLetters;

    // Counter that logs the progress of the program
    public static int statusNum;

    // Initialise the Driver
    public static IWebDriver driver = new FirefoxDriver();

    public static List<List<string>> approvedJobLinksList = new List<List<string>>();
    public static List<List<string>> applyJobsList = new List<List<string>>();
    public static List<string> jobLinksList = new List<string>();

    static string[] specialURL = url.Split('?');
    static double jobsNumDec;
    static int totalPages;
    static DateTime start = DateTime.Now;
    static DateTime end;

    /// <summary>
    /// Log into account to quick apply to jobs
    /// </summary>
    public void Login()
    {
        driver.Navigate().GoToUrl(url);

        if (email != null && password != null)
        {
            // Click the sign in button
            driver.FindElement(By.XPath("/html/body/div[1]/div/div[4]/div[2]/div/div/div[1]/div[3]/div[1]/div/div/div/div/div/a")).Click();

            // Wait for the page to load, then enter the user's login
            while (true)
            {
                try
                {
                    driver.FindElement(By.XPath("//input[starts-with(@id, 'emailAddress')]")).SendKeys(email);
                    driver.FindElement(By.XPath("//input[starts-with(@id, 'password')]")).SendKeys(password);

                    driver.FindElement(By.XPath("/html/body/div[1]/div/div/div/div/div[2]/div/div/div[3]/div/div/div/div[2]/form/div/div[4]/div/div[1]/button")).Click();

                    break;
                }

                catch (NoSuchElementException) { }
            }

            // Wait for the page to load, then retrieve the total number of pages to scan through
            while (true)
            {
                try
                {
                    bool waitForMenu = driver.FindElement(By.XPath("/html/body/div[1]/div/div[5]/div/section/div[2]/div/div/div/div[1]/div/div/div[1]/div/div[1]/div/div/h1/span")).Displayed;
                    if (waitForMenu)
                    {
                        break;
                    }
                }

                catch (NoSuchElementException) { }
            }
        }
    }

    /// <summary>
    /// Get the number of pages and time estimate
    /// </summary>
    public void Setup()
    {
        // The while loops used throughout the program are for the purposes of waiting for the page to load before running the next line of code
        while (true)
        {
            try
            {
                // Retrieve the total number of pages to scan through
                var jobsNum = driver.FindElement(By.XPath("/html/body/div[1]/div/div[5]/div/section/div[2]/div/div/div/div[1]/div/div/div[1]/div/div[1]/div/div/h1/span")).Text;
                jobsNumDec = double.Parse(jobsNum);
                totalPages = Int32.Parse(Math.Ceiling(jobsNumDec / 22).ToString());

                // Estimate the execution time
                TimeSpan est = TimeSpan.FromSeconds(jobsNumDec * 2.7);
                Console.WriteLine("\nEstimated search time: " + est.ToString(@"mm\:ss"));

                break;
            }

            catch (NoSuchElementException) { }
            catch (StaleElementReferenceException) { }
        }
    }

    /// <summary>
    /// Filter out the jobs from the job listings page
    /// </summary>
    public void TitleFilter()
    {
        Thread.Sleep(3000);

        jobLinksList.Clear();

        var jobLink = driver.FindElements(By.XPath("//a[starts-with(@id, 'job-title')]"));

        // Check for keywords in their job titles, and filter them out
        foreach (var job in jobLink)
        {
            bool pass = true;

            for (int k = 0; k < keyWords.Length; k++)
            {
                if (job.Text.IndexOf(keyWords[k], StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    pass = false;
                    statusNum++;
                    break;
                }
            }

            if (pass)
            {
                jobLinksList.Add(job.GetAttribute("href"));
            }
        }
    }

    /// <summary>
    /// Filter out the jobs through their descriptions
    /// </summary>
    public void FilterByKeyword(int index, List<string> subList)
    {
        subList.Add("other");
        subList.Add(jobLinksList[index]);
        approvedJobLinksList.Add(subList);

        for (int i = 0; i < keyWords.Length; i++)
        {
            string badWord = ($"//*[contains(translate(text(),'{keyWords[i].ToUpperInvariant()}', '{keyWords[i]}'), '{keyWords[i]}')]");

            try
            {
                driver.FindElement(By.XPath(badWord));
                approvedJobLinksList.Remove(subList);
                break;
            }

            catch (NoSuchElementException) { }
        }
    }

    /// <summary>
    /// Find the field keywords to determine which cover letter to use
    /// </summary>
    public List<string> FilterByField(int index, List<string> subList)
    {
        for (int i = 0; i < fieldWords.Length; i++)
        {
            string fieldWord = ($"//*[contains(translate(text(),'{fieldWords[i].ToUpperInvariant()}', '{fieldWords[i]}'), '{fieldWords[i]}')]");

            try
            {
                driver.FindElement(By.XPath(fieldWord));
                subList[0] = (fieldWords[i]);
                break;
            }

            catch (NoSuchElementException) { }
        }

        return subList;
    }

    /// <summary>
    /// Check whether the job is applicable for quick application
    /// </summary>
    public void QuickApplyFinder(List<string> subList)
    {
        try
        {
            var applyButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div[5]/div/div/div[2]/div[2]/div[2]/div/div/div[1]/div/div[6]/div/div/div/div/div/div/div[1]/div/div/a/span[4]/span"));

            if (applyButton.Text == "Quick apply")
            {
                subList.Add(driver.FindElement(By.XPath("/html/body/div[1]/div/div[5]/div/div/div[2]/div[2]/div/div/div/div[1]/div/div[2]/div/div/div/div[1]/h1")).Text);
                approvedJobLinksList.Remove(subList);
                applyJobsList.Add(subList);
            }
        }

        catch (NoSuchElementException)
        {
            try
            {
                var applyButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div[5]/div/div/div[2]/div[2]/div/div/div/div[1]/div/div[5]/div/div/div/div/div/div/div[1]/div/div/a/span[4]/span"));

                if (applyButton.Text == "Quick apply")
                {
                    subList.Add(driver.FindElement(By.XPath("/html/body/div[1]/div/div[5]/div/div/div[2]/div[2]/div/div/div/div[1]/div/div[2]/div/div/div/div[1]/h1")).Text);
                    approvedJobLinksList.Remove(subList);
                    applyJobsList.Add(subList);
                }
            }

            catch (NoSuchElementException) { }
        }
    }

    /// <summary>
    /// Run all methods in order
    /// </summary>
    public void Scan()
    {
        // Find all job listings from the main menu
        for (int j = 1; j <= totalPages; j++)
        {
            // quick search
            TitleFilter();

            // Filter out all the jobs that contain the keywords throughout the job posting
            for (int link = 0; link < jobLinksList.Count; link++)
            {
                driver.Navigate().GoToUrl(jobLinksList[link]);
                List<string> subList = new List<string>();

                FilterByKeyword(link, subList);
                approvedJobLinksList.Add(FilterByField(link, subList));
                QuickApplyFinder(subList);

                statusNum++;
                string status = ((float)Math.Round(Convert.ToDouble(statusNum) / jobsNumDec, 2) * 100).ToString();
                Console.Write("\rSearch status: {0}%", status);
            }

            // Navigate to the next page if valid (url is split and reconstructed since page num is included)
            if (j + 1 <= totalPages)
            {
                url = specialURL[0] + "?page=" + (j + 1) + "&" + specialURL[1];
                driver.Navigate().GoToUrl(url);
            }
        }
    }

    /// <summary>
    /// Display job links and execution time, then ask the user if they want to start auto-applying
    /// </summary>
    public void Display()
    {
        Console.WriteLine("\nNormal job links:\n");

        foreach (List<string> job in approvedJobLinksList)
        {
            Console.WriteLine(job[1]);
        }

        Console.WriteLine("\nQuick Apply job links:\n");

        foreach (List<string> job in applyJobsList)
        {
            Console.WriteLine(job[1]);
        }

        end = DateTime.Now;
        TimeSpan ts = (end - start);

        // Print the number of jobs found and the time taken for program to run
        Console.WriteLine("\nSeek jobs found: " + jobsNumDec +
                            "\nNormal jobs found: " + approvedJobLinksList.Count +
                            "\nQuick Apply jobs found: " + applyJobsList.Count +
                            "\nExecution time: " + ts.ToString(@"mm\:ss") +
                            "\n\nWould you like to auto-apply to jobs now? Yes[y] No[n]");

        if (email != null && password != null)
        {
            if (Console.ReadLine() == "y" || Console.ReadLine() == "Y")
                AutoApply();
        }
    }

    /// <summary>
    /// Apply to jobs automatically
    /// </summary>
    public void AutoApply()
    {
        for (int i = 0; i < applyJobsList.Count; i++)
        {
            driver.Navigate().GoToUrl(applyJobsList[i][1]);

            while (true)
            {
                try
                {
                    var applyButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div[5]/div/div/div[2]/div[2]/div[2]/div/div/div[1]/div/div[6]/div/div/div/div/div/div/div[1]/div/div/a"));
                    applyButton.Click();
                    break;
                }

                catch (NoSuchElementException)
                {
                    try
                    {
                        var applyButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div[5]/div/div/div[2]/div[2]/div/div/div/div[1]/div/div[5]/div/div/div/div/div/div/div[1]/div/div/a/span[4]/span"));
                        applyButton.Click();
                        break;
                    }

                    catch (NoSuchElementException) { }
                }
            }

            // Match the cover letter to the field
            string coverLetter = coverLetters[Array.IndexOf(fieldWords, applyJobsList[i][0])];

            // The bot will try its best to extract the name of the role, given many jobs have brackets () and hyphens -
            coverLetter = coverLetter.Replace("[]", applyJobsList[i][2]);

            coverLetter = Regex.Replace(coverLetter, @"\((.*?)\)", "");
            coverLetter = Regex.Replace(coverLetter, @"-.*", "");
            coverLetter = Regex.Replace(coverLetter, @"  ", " ");

            while (true)
            {
                try
                {
                    bool waitForMenu = driver.FindElement(By.XPath("//*[@id='writtenCoverLetter']")).Displayed;
                    {
                        driver.FindElement(By.XPath("//*[@id='writtenCoverLetter']")).Clear();
                        driver.FindElement(By.XPath("//*[@id='writtenCoverLetter']")).SendKeys(coverLetter);

                        driver.FindElement(By.XPath("/html/body/div[1]/div/div[1]/div[2]/div/div/div[2]/div/div/div[5]/div/div/button")).Click();
                        break;
                    }
                }

                catch (NoSuchElementException) { }
            }


            Console.WriteLine($"({i + 1}/{applyJobsList.Count}) Ready for next job? Yes[y] No[n]");

            if (Console.ReadLine() == "y" || Console.ReadLine() == "Y")
                continue;
        }

    }
}
