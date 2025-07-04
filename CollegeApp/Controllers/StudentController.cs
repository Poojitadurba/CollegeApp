﻿using CollegeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using CollegeApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using AutoMapper;
using CollegeApp.Data.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes="LoginForLocalUsers",Roles ="SuperAdmin,Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    //   [EnableCors(PolicyName ="AllowAll")]
    public class StudentController:ControllerBase
    {
        private readonly ILogger<StudentController> _logger;
        //private readonly CollegeDbContext _dbContext;
        private readonly IMapper _mapper;
        //private readonly IStudentRepository _studentRepository;
       // private readonly ICollegeRepository<Student> _studentRepository;
        private readonly IStudentRepository _studentRepository;
        private APIResponse _apiResponse;

        public StudentController(ILogger<StudentController> logger,CollegeDbContext dbContext,IMapper mapper,IStudentRepository studentRepository)
        {
            _logger = logger;
           // _dbContext = dbContext;
            _mapper = mapper;
            _studentRepository = studentRepository;
            _apiResponse = new();
        }

        [HttpGet]
        [Route("all")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        // [AllowAnonymous]
       // public async Task<ActionResult<IEnumerable<StudentDTO>>> GetAllStudents()
         public async Task<ActionResult<APIResponse>> GetAllStudents()
        {
            try
            {
                _logger.LogInformation("GetStudents started");
                //var students = await _dbContext.students.ToListAsync();
                var students = await _studentRepository.GetAllAsync();
                var studentDTOData = _mapper.Map<List<StudentDTO>>(students);
                _apiResponse.Data = _mapper.Map<List<StudentDTO>>(students);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                //var students = await _dbContext.students.Select(s => new StudentDTO()
                //{
                //    Id = s.Id,
                //    StudentName = s.StudentName,
                //    Address = s.Address,
                //    Email = s.Email,
                //}).ToListAsync();

                // return Ok(studentDTOData);
                return Ok(_apiResponse);
            }
            catch(Exception ex)
            {
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Status = false;
                return _apiResponse;
            }
        }

        [HttpGet("{id:int}",Name ="GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        //  [DisableCors]
        //public async Task<ActionResult<StudentDTO>> GetStudentById(int id)
        public async Task<ActionResult<APIResponse>> GetStudentById(int id)
        {
            try
            {
                _logger.LogInformation("GetStudentById started");
                if (id <= 0)
                {
                    _logger.LogWarning("Bad Request");
                    return BadRequest();
                }
                //var student= await _dbContext.students.Where(n=>n.Id==id).FirstOrDefaultAsync();
                var student = await _studentRepository.GetAsync(student => student.Id == id, false);
                if (student == null)
                {
                    _logger.LogError("Student not found");
                    return NotFound("The request with given id is not found");
                }
                var studentDTO = _mapper.Map<StudentDTO>(student);
                _apiResponse.Data = _mapper.Map<StudentDTO>(student);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                //var studentDTO = new StudentDTO
                //{
                //    Id = student.Id,
                //    Email = student.Email,
                //    Address = student.Address,
                //    StudentName = student.StudentName
                //};

                // return Ok(studentDTO);
                return Ok(_apiResponse);
            }
            catch(Exception ex)
            {
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Status = false;
                return _apiResponse;
            }
        }

        [HttpGet("{name:alpha}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        //public async Task<ActionResult<Student>> GetStudentByName(string name)
        public async Task<ActionResult<APIResponse>> GetStudentByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return BadRequest();
                //var student = await _dbContext.students.Where(n => n.StudentName == name).FirstOrDefaultAsync();
                // var student = await _studentRepository.GetByNameAsync(name);
                var student = await _studentRepository.GetAsync(s => s.StudentName.Contains(name), false);
                var studentDTO = _mapper.Map<StudentDTO>(student);
                if (student == null)
                    return NotFound();
                return Ok(studentDTO);
            }
            catch (Exception ex)
            {
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Status = false;
                return _apiResponse;
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        //public async Task<ActionResult<bool>> DeleteStudent(int id)
        public async Task<ActionResult<APIResponse>> DeleteStudent(int id)
        {
            try
            {
                if (id < 0)
                    return BadRequest();
                //var student = await _dbContext.students.Where(n => n.Id == id).FirstOrDefaultAsync();
                var student = await _studentRepository.GetAsync(student => student.Id == id, false);
                //var student =_studentRepository.DeleteAsync(id);
                if (student == null)
                    return NotFound();
                await _studentRepository.DeleteAsync(student);
                //_dbContext.students.Remove(student);
                //_dbContext.SaveChangesAsync();
                return Ok(true);
            }
            catch (Exception ex)
            {
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Status = false;
                return _apiResponse;
            }
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        //public async Task<ActionResult<StudentDTO>> CreateStudent([FromBody] StudentDTO model)
         public async Task<ActionResult<APIResponse>> CreateStudent([FromBody] StudentDTO model)
        {
            try
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
                var studentAfterCreation = await _studentRepository.CreateAsync(student);
                //await _dbContext.students.AddAsync(student);
                //_dbContext.SaveChangesAsync();
                model.Id = student.Id;
                return CreatedAtRoute("GetStudentById", new { id = model.Id }, model);
            }
            catch (Exception ex)
            {
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Status = false;
                return _apiResponse;
            }

        }

        //[HttpPut]
        [HttpPatch("{id:int}/UpdatePartial")]
        //[Route("{id:int}/Updatepartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
       // public async Task<ActionResult<StudentDTO>> UpdateStudent(int id,[FromBody] JsonPatchDocument<StudentDTO> model)
        public async Task<ActionResult<APIResponse>> UpdateStudent(int id, [FromBody] JsonPatchDocument<StudentDTO> model)
        {
            try
            {
                if (model == null || id <= 0)
                    return BadRequest();
                var estudent = await _studentRepository.GetAsync(student => student.Id == id, true);

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
                await _studentRepository.UpdateAsync(estudent);
                return NoContent();
            }
            catch (Exception ex)
            {
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Status = false;
                return _apiResponse;
            }
        }


        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        //public async Task<ActionResult> UpdateStudent([FromBody] StudentDTO model)
        public async Task<ActionResult<APIResponse>> UpdateStudent([FromBody] StudentDTO model)
        {
            try
            {
                //var student = await _dbContext.students.AsNoTracking().Where(n => n.Id == model.Id).FirstOrDefaultAsync();

                var student = await _studentRepository.GetAsync(student => student.Id == model.Id, true);

                //var newRecord = new Student(){
                //   Id=student.Id,
                //   Email=model.Email,
                //   Address=model.Address,
                //   StudentName=model.StudentName
                //};

                var newRecord = _mapper.Map<Student>(model);
                _apiResponse.Data = _mapper.Map<Student>(model);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                await _studentRepository.UpdateAsync(newRecord);

                return NoContent();
            }
            catch (Exception ex)
            {
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Status = false;
                return _apiResponse;
            }
        }
    }
}
