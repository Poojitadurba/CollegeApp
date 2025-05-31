namespace CollegeApp.Models
{
    public static class CollegeRepository
    {
        public static List<Student> Students { get; set; } = new List<Student>()
        {
            new Student{Id=1,StudentName="Poojita",Email="poojitha@gmail.com",Address="Hyderabad,India" },
            new Student{Id=2,StudentName="Bunny",Email="bunny@gmail.com",Address="Hyderabad,India" },
        };


    }
}
