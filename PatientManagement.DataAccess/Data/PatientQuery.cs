using YourNamespace.Models;

namespace PatientManagement.Data
{
    public class PatientQuery
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ContactNumber { get; set; }
        public string SortBy { get; set; }
        public bool Descending { get; set; }
        public int PageNumber { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public List<Patient> GetEmployee { get; set; } = new List<Patient>();
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / Limit);
    }
}
