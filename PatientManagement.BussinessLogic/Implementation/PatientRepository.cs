using AutoMapper;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PatientManagement.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YourNamespace.Data;
using YourNamespace.Models;
using YourNamespace.Repositories;

namespace PatientManagement.BussinessLogic.Implementation
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cacheProvider;

        public PatientRepository(ApplicationDbContext context, IMapper mapper, IMemoryCache cacheProvider)
        {
            _context = context;
            _mapper = mapper;
            _cacheProvider = cacheProvider;
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync(PatientQuery patients)
        {
            // Generate a unique cache key based on query options
            var cacheKey = $"patients_{patients.FirstName ?? ""}_{patients.LastName ?? ""}_{patients.ContactNumber ?? ""}_{patients.SortBy ?? ""}" +
                $"_{patients.Descending}_{patients.PageNumber}_{patients.Limit}";

            var patient = _context.Patients.AsQueryable();

            if (!string.IsNullOrEmpty(patients.FirstName))
            {
                patient = patient.Where(GetPatient => GetPatient.FirstName.Contains(patients.FirstName));
            }

            if (!string.IsNullOrEmpty(patients.LastName))
            {
                patient = patient.Where(GetPatient => GetPatient.LastName.Contains(patients.LastName));
            }

            if (!string.IsNullOrEmpty(patients.ContactNumber))
            {
                patient = patient.Where(GetPatient => GetPatient.ContactNumber.Contains(patients.ContactNumber));
            }

            // Apply sorting with explicit type
            patient = patients.SortBy?.ToLower() switch
            {
                "name" => patients.Descending ? patient.OrderByDescending(GetPatients => GetPatients.FirstName)
                    : patient.OrderBy(GetPatients => GetPatients.FirstName),
                "salary" => patients.Descending ? patient.OrderByDescending(GetPatients => GetPatients.LastName)
                    : patient.OrderBy(GetPatients => GetPatients.LastName),
                "location" => patients.Descending ? patient.OrderByDescending(GetPatients => GetPatients.ContactNumber)
                    : patient.OrderBy(GetPatients => GetPatients.ContactNumber),
                "department" => patients.Descending ? patient.OrderByDescending(GetPatients => GetPatients.Gender)
                    : patient.OrderBy(GetPatients => GetPatients.Gender),
                _ => patients.Descending ? patient.OrderByDescending(GetPatient => GetPatient.Id)
                    : patient.OrderBy(GetPatients => GetPatients.Id) // Default sort by Id
            };

            patient = patient.Skip((patients.PageNumber - 1) * patients.Limit).Take(patients.Limit);

            var employees = await patient.ToListAsync();
            var patientData = _mapper.Map<List<Patient>>(employees);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30),
                SlidingExpiration = TimeSpan.FromSeconds(30)
            };

            _cacheProvider.Set(cacheKey, patientData, cacheEntryOptions);

            return patientData;
        }

        public async Task<Patient> GetPatientByIdAsync(int patientId)
        {
            var cacheKey = $"patient_{patientId}";

            if (_cacheProvider.TryGetValue(cacheKey, out Patient cachedEmployee))
            {
                return cachedEmployee;
            }

            var patientEntity = await _context.Patients
                .FirstOrDefaultAsync(x => x.Id == patientId);

            if (patientEntity == null)
            {
                return null;
            }

            var patient = _mapper.Map<Patient>(patientEntity);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5000),
                SlidingExpiration = TimeSpan.FromSeconds(5000)
            };
            _cacheProvider.Set(cacheKey, patient, cacheEntryOptions);

            return patient;
        }

        public async Task AddPatientAsync(Patient patients)
        {
            var patient = _mapper.Map<Patient>(patients);
            patient.CreatedDate = DateTime.UtcNow;
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePatientAsync(Patient patient)
        {
            var existingPatient = await _context.Patients.FindAsync(patient.Id);
            if (existingPatient != null)
            {
                _mapper.Map(patient, existingPatient);
                existingPatient.UpdatedDate = DateTime.UtcNow;
                _context.Patients.Update(existingPatient);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeletePatientAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> PatientExistsAsync(int id)
        {
            return await _context.Patients.AnyAsync(patient => patient.Id == id);
        }
    }
}
