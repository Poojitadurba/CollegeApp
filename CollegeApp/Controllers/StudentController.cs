using CollegeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController:ControllerBase
    {
        private readonly ILogger<StudentController> _logger;

        public StudentController(ILogger<StudentController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("all")]
        public ActionResult<IEnumerable<StudentDTO>> GetAllStudents()
        {
            _logger.LogInformation("GetStudents started");
            var students = new List<StudentDTO>();
            foreach(var item in CollegeRepository.Students)
            {
                StudentDTO student = new StudentDTO();
                student.Id = item.Id;
                student.StudentName = item.StudentName;
                student.Address = item.Address;
                student.Email = item.Email;
                students.Add(student);
            }
            return Ok(students);
        }

        [HttpGet("{id:int}",Name ="GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> GetStudentById(int id)
        {
            _logger.LogInformation("GetStudentById started");
            if (id <= 0)
            {
                _logger.LogWarning("Bad Request");
                return BadRequest();
            }
            var student=CollegeRepository.Students.Where(n=>n.Id==id).FirstOrDefault();
            if (student == null)
            {
                _logger.LogError("Student not found");
                return NotFound("The request with given id is not found");
            }
            var studentDTO = new StudentDTO
            {
                Id = student.Id,
                Email = student.Email,
                Address = student.Address,
                StudentName = student.StudentName
            };
          
            return Ok(student);
        }

        [HttpGet("{name:alpha}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Student> GetStudentByName(string name)
        {
            if(string.IsNullOrEmpty(name))
                return BadRequest();
            var student = CollegeRepository.Students.Where(n => n.StudentName == name).FirstOrDefault();
            if (student == null)
                return NotFound();
            return Ok(student);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<bool> DeleteStudent(int id)
        {
            if (id < 0)
                return BadRequest();
            var student = CollegeRepository.Students.Where(n => n.Id == id).FirstOrDefault();
            if (student == null)
                return NotFound();
            CollegeRepository.Students.Remove(student);
            return Ok(true);
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<StudentDTO> CreateStudent([FromBody] StudentDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model == null)
                return BadRequest();

            //if(model.AdmissionDate<DateTime.Now)
            //{
            //    ModelState.AddModelError("AdmissionDate Error","Admission date is wrong");
            //    return BadRequest(ModelState);
            //}

            int newId = CollegeRepository.Students.LastOrDefault().Id + 1;
            Student student = new Student
            {
                Id = newId,
                StudentName=model.StudentName,
                Email=model.Email,
                Address=model.Address,
            };
            CollegeRepository.Students.Add(student);
            model.Id = student.Id;
            return CreatedAtRoute("GetStudentById", new {id=model.Id }, model);
         
        }

        //[HttpPut]
        [HttpPatch("{id:int}/UpdatePartial")]
        //[Route("{id:int}/Updatepartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<StudentDTO> UpdateStudent(int id,[FromBody] JsonPatchDocument<StudentDTO> model)
        {
            if (model == null||id<=0)
                return BadRequest();
            var estudent = CollegeRepository.Students.Where(n => n.Id == id).FirstOrDefault();
            if (estudent == null)
                return NotFound();
            var studentDTO = new StudentDTO
            {
                Id = estudent.Id,
                StudentName = estudent.StudentName,
                Email = estudent.Email,
                Address = estudent.Address
            };
            model.ApplyTo(studentDTO, ModelState);
            estudent.StudentName = studentDTO.StudentName;
            estudent.Email = studentDTO.Email;
            estudent.Address = studentDTO.Address;

            return NoContent();
        }

    }
}
