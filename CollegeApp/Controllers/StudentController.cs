﻿using CollegeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using CollegeApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using AutoMapper;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController:ControllerBase
    {
        private readonly ILogger<StudentController> _logger;
        private readonly CollegeDbContext _dbContext;
        private readonly IMapper _mapper;
        public StudentController(ILogger<StudentController> logger,CollegeDbContext dbContext,IMapper mapper)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetAllStudents()
        {
            _logger.LogInformation("GetStudents started");
            var students = await _dbContext.students.ToListAsync();
            var studentDTOData = _mapper.Map<List<StudentDTO>>(students);
            //var students = await _dbContext.students.Select(s => new StudentDTO()
            //{
            //    Id = s.Id,
            //    StudentName = s.StudentName,
            //    Address = s.Address,
            //    Email = s.Email,
            //}).ToListAsync();

            return Ok(studentDTOData);
        }

        [HttpGet("{id:int}",Name ="GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDTO>> GetStudentById(int id)
        {
            _logger.LogInformation("GetStudentById started");
            if (id <= 0)
            {
                _logger.LogWarning("Bad Request");
                return BadRequest();
            }
            var student= await _dbContext.students.Where(n=>n.Id==id).FirstOrDefaultAsync();
            if (student == null)
            {
                _logger.LogError("Student not found");
                return NotFound("The request with given id is not found");
            }
            var studentDTO = _mapper.Map<StudentDTO>(student);
            //var studentDTO = new StudentDTO
            //{
            //    Id = student.Id,
            //    Email = student.Email,
            //    Address = student.Address,
            //    StudentName = student.StudentName
            //};
          
            return Ok(studentDTO);
        }

        [HttpGet("{name:alpha}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Student>> GetStudentByName(string name)
        {
            if(string.IsNullOrEmpty(name))
                return BadRequest();
            var student = await _dbContext.students.Where(n => n.StudentName == name).FirstOrDefaultAsync();
            var studentDTO = _mapper.Map<StudentDTO>(student);
            if (student == null)
                return NotFound();
            return Ok(studentDTO);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteStudent(int id)
        {
            if (id < 0)
                return BadRequest();
            var student = await _dbContext.students.Where(n => n.Id == id).FirstOrDefaultAsync();
            if (student == null)
                return NotFound();
            _dbContext.students.Remove(student);
            _dbContext.SaveChangesAsync();
            return Ok(true);
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StudentDTO>> CreateStudent([FromBody] StudentDTO model)
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

            //Student student = new Student
            //{

            //    StudentName=model.StudentName,
            //    Email=model.Email,
            //    Address=model.Address,
            //};
            var student = _mapper.Map<Student>(model);
            await _dbContext.students.AddAsync(student);
            _dbContext.SaveChangesAsync();
            model.Id = student.Id;
            return CreatedAtRoute("GetStudentById", new {id=model.Id }, model);
         
        }

        //[HttpPut]
        [HttpPatch("{id:int}/UpdatePartial")]
        //[Route("{id:int}/Updatepartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StudentDTO>> UpdateStudent(int id,[FromBody] JsonPatchDocument<StudentDTO> model)
        {
            if (model == null||id<=0)
                return BadRequest();
            var estudent = await _dbContext.students.AsNoTracking().Where(n => n.Id == id).FirstOrDefaultAsync();
            if (estudent == null)
                return NotFound();
            //var studentDTO = new StudentDTO
            //{
            //    Id = estudent.Id,
            //    StudentName = estudent.StudentName,
            //    Email = estudent.Email,
            //    Address = estudent.Address
            //};

            var studentDTO = _mapper.Map<StudentDTO>(estudent);
            model.ApplyTo(studentDTO, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //estudent.StudentName = studentDTO.StudentName;
            //estudent.Email = studentDTO.Email;
            //estudent.Address = studentDTO.Address;
            estudent = _mapper.Map<Student>(studentDTO);
            _dbContext.students.Update(estudent);
            _dbContext.SaveChangesAsync();
            return NoContent();
        }


        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateStudent([FromBody] StudentDTO model)
        {
            var student = await _dbContext.students.AsNoTracking().Where(n => n.Id == model.Id).FirstOrDefaultAsync();

            //var newRecord = new Student(){
            //   Id=student.Id,
            //   Email=model.Email,
            //   Address=model.Address,
            //   StudentName=model.StudentName
            //};

            var newRecord = _mapper.Map<Student>(model);

            _dbContext.students.Update(newRecord);

            _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
