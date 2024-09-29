using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourNamespace.Data;
using YourNamespace.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YourNamespace.Repositories;
using PatientManagement.Data;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPatientRepository _patientRepository;

        public PatientsController(ApplicationDbContext context , IPatientRepository patientRepository)
        {
            _context = context;
            _patientRepository = patientRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Patient>>> GetPatients(
           [FromQuery] PatientQuery query)
        {
            var patients = await _patientRepository.GetAllPatientsAsync(query);

            if (patients == null )
            {
                return NotFound("No patients found in the database.");
            }

            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatientById(int id)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return patient;
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(Patient patient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Patients.Add(patient);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException )
            {
                return StatusCode(500, "Internal server error");
            }

            var patientsData = Url.Action(nameof(GetPatientById), new { id = patient.Id });
            return Created(patientsData, patient);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, Patient patient)
        {
            if (id != patient.Id)
            {
                return BadRequest();
            }

            _context.Entry(patient).State = EntityState.Modified;
            patient.UpdatedDate = DateTime.UtcNow;

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(patient => patient.Id == id);
        }
    }
}
