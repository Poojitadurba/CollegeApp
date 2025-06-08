using AutoMapper;
using CollegeApp.Data;
using CollegeApp.Models;

namespace CollegeApp.Configurations
{
    public class AutoMapperConfig:Profile
    {
        public AutoMapperConfig()
        {
            //CreateMap<Student,StudentDTO>();
            //CreateMap<StudentDTO, Student>();

            CreateMap<StudentDTO, Student>().ReverseMap();

            //Config if you have different property names
            //CreateMap<Student, StudentDTO>().ReverseMap().ForMember(n => n.Name, opt => opt.MapFrom(x => x.StudentName));

            //Config if you want to ignore mapping any property
            //CreateMap<StudentDTO, Student>().ReverseMap().ForMember(n => n.StudentName, opt => opt.Ignore());

            //Config if you want to print some message instead of null in any field.
           CreateMap<StudentDTO, Student>().ForMember(n=>n.Address,opt=>opt.
           MapFrom(n=>string.IsNullOrEmpty(n.Address)?"No address found":n.Address)).ReverseMap().AddTransform<string>(n => string.IsNullOrEmpty(n) ? "No address found" : n);
        }
    }
}