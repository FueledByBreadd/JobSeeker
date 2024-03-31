namespace JobSeeker
{
    class Program
    {
        /* 
         * NOTE: the following 6 variables need to be assigned by the user
         */

        // Insert your URL here, and email & password if intending to semi-auto-apply to jobs
        public static string url = "";

        public static string email = "";
        public static string password = "";

        // Field words to help adjust the cover letters (leave last as 'other')
        public static string[] fieldWords = { "other" };

        // Keywords to filter out by the bot
        public static string[] keyWords = { };

        // Cover letters for each field
        // NOTE: array size must be the same as fieldWords
        // NOTE: role name placeholders must be defined as []
        public static string[] coverLetters = { };

        public static void Main(string[] args)
        {
            JobSeekerBot Bot = new JobSeekerBot();

            Bot.Login();
            Bot.Setup();
            Bot.Scan();
            Bot.Display();
        }
    }
}