using AutoMapper;
using HospitalApi.Data;
using HospitalApi.DTO;
using HospitalApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalApi.Controllers
{
    /// <summary>
    /// CRUD контроллер докторов
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : Controller
    {
        private readonly HospitalContext _context;
        private readonly IMapper _mapper;

        public DoctorsController(HospitalContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/doctors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorListDto>>> GetDoctors(
            string sortField = "FullName",  // Поле для сортировки
            bool sortDescending = false,    // Порядок сортировки
            int page = 1,     // Номер страницы
            int pageSize = 10     // Размер страницы
        )
        {
            // Начинаем с основного запроса к базе данных
            var query = _context.Doctors
                .Include(d => d.Cabinet)
                .Include(d => d.Specialization)
                .Include(d => d.Region)
                .AsQueryable();

            // Сортировка по указанному полю (по умолчанию FullName)
            query = sortDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, sortField))
                : query.OrderBy(e => EF.Property<object>(e, sortField));

            // Постраничный возврат данных
            var totalRecords = await query.CountAsync();  // Общее количество записей
            var doctors = await query
                .Skip((page - 1) * pageSize)    // Пропуск записей для предыдущих страниц
                .Take(pageSize)        // Количество записей на текущей странице
                .ToListAsync();

            // Маппинг из Doctor в DoctorListDto
            var doctorListDto = _mapper.Map<List<DoctorListDto>>(doctors);

            // Возвращаем данные вместе с мета-данными о пагинации
            return Ok(new
            {
                TotalRecords = totalRecords,     // Общее количество записей
                CurrentPage = page,        // Текущая страница
                PageSize = pageSize,         // Размер страницы
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),  // Общее количество страниц
                Data = doctorListDto      // Сами данные
            });
        }


        // GET: api/doctors/1
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDto>> GetDoctorById(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            // Маппинг из Doctor в DoctorDto
            var doctorDto = _mapper.Map<DoctorDto>(doctor);
            return Ok(doctorDto);
        }

        // POST: api/doctors
        [HttpPost]
        public async Task<ActionResult<DoctorDto>> PostDoctor(DoctorDto doctorDto)
        {
            // Маппинг из DoctorDto в Doctor
            var doctor = _mapper.Map<Doctor>(doctorDto);
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDoctorById), new { id = doctor.Id }, doctorDto);
        }

        // PUT: api/doctors/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctor(int id, DoctorDto doctorDto)
        {
            if (id != doctorDto.Id)
            {
                return BadRequest();
            }

            // Маппинг из DoctorDto в Doctor
            var doctor = _mapper.Map<Doctor>(doctorDto);
            _context.Entry(doctor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(id))
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

        // DELETE: api/doctors/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }
    }
}
