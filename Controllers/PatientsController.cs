using AutoMapper;
using HospitalApi.Data;
using HospitalApi.DTO;
using HospitalApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Controllers
{
    /// <summary>
    /// CRUD контроллер пациентов
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : Controller
    {
        private readonly HospitalContext _context;
        private readonly IMapper _mapper;

        public PatientsController(HospitalContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientListDto>>> GetPatients(
            string sortField = "LastName",  // Поле для сортировки (по умолчанию Фамилия)
            bool sortDescending = false,    // Порядок сортировки (по возрастанию или убыванию)
            int page = 1,    // Номер страницы
            int pageSize = 10        // Размер страницы (по умолчанию 10 записей)
        )
        {
            // Начинаем с основного запроса к базе данных
            var query = _context.Patients
                .Include(p => p.Region)
                .AsQueryable();

            // Сортировка по указанному полю (по умолчанию LastName)
            query = sortDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, sortField))
                : query.OrderBy(e => EF.Property<object>(e, sortField));

            // Постраничный возврат данных
            var totalRecords = await query.CountAsync();  // Общее количество записей
            var patients = await query
                .Skip((page - 1) * pageSize)      // Пропуск записей для предыдущих страниц
                .Take(pageSize)           // Количество записей на текущей странице
                .ToListAsync();

            // Маппинг из Patient в PatientListDto
            var patientListDto = _mapper.Map<List<PatientListDto>>(patients);

            // Возвращаем данные вместе с мета-данными о пагинации
            return Ok(new
            {
                TotalRecords = totalRecords,      // Общее количество записей
                CurrentPage = page,         // Текущая страница
                PageSize = pageSize,     // Размер страницы
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),  // Общее количество страниц
                Data = patientListDto      // Сами данные (список пациентов)
            });
        }


        // GET: api/patients/1
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatientById(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            // Маппинг из Patient в PatientDto
            var patientDto = _mapper.Map<PatientDto>(patient);
            return Ok(patientDto);
        }

        // POST: api/patients
        [HttpPost]
        public async Task<ActionResult<PatientDto>> PostPatient(PatientDto patientDto)
        {
            // Маппинг из PatientDto в Patient
            var patient = _mapper.Map<Patient>(patientDto);
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patientDto);
        }

        // PUT: api/patients/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, PatientDto patientDto)
        {
            if (id != patientDto.Id)
            {
                return BadRequest();
            }

            // Маппинг из PatientDto в Patient
            var patient = _mapper.Map<Patient>(patientDto);
            _context.Entry(patient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
    }
}
