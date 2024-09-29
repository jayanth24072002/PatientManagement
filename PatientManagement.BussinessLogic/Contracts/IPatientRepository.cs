using PatientManagement.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using YourNamespace.Models;

namespace YourNamespace.Repositories
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllPatientsAsync(PatientQuery patients);
        Task<Patient> GetPatientByIdAsync(int patientId);
        Task AddPatientAsync(Patient patient);
        Task UpdatePatientAsync(Patient patient);
        Task DeletePatientAsync(int id);
        Task<bool> PatientExistsAsync(int id);
    }
}
