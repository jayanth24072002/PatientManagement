using AutoMapper;
using PatientManagement.Data;
using YourNamespace.Models;

namespace PatientManagement.Helpers
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<GetPatient, Patient>().ReverseMap();
        }
    }
}

