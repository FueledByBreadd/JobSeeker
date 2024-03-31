# JobSeeker
TLDR: Job Seeker bot that works on Seek.com, and helps filter jobs

The JobSeeker bot was built for the purpose of filtering Seek.com jobs, given I was not happy with the existing.
It's capable of finding jobs through any URL input, and will filter each job through the use of keywords.
The moment a bad keyword is found, such as 'Senior', when the user is searching for Junior jobs, the job
will be skipped. JobSeeker is also able to collect jobs featuring a 'Quick Apply', which lets users apply to 
the job directly on the Seek.com platform. Combining this with its ability to filter jobs by fields, and the
user's input of a few cover letters, the bot can write the cover letters based on the job field. I've chosen 
to use a relatively universal resume for applications, which means this bot will not handle unique resumes
for each field. The bot will estimate the search time at the program beggining, then it will display its
status through a percentage. Upon completion, the output will feature the time it took, number of jobs found,
number of jobs filtered, number of jobs with 'Quick Apply' links, and sorted lists showing links for jobs with 
and without the 'Quick Apply' links. The user is then able to ask the bot to pull up each 'Quick Apply' job,
where it will insert the aforementioned details. The bot is not able to answer the questions for the user,
since these are quite varied. The user can repeat this process with the normal jobs too. 
