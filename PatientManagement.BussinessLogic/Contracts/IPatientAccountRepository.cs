using PatientManagement.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientManagement.BussinessLogic.Repository
{
    public interface IPatientAccountRepository
    {
        Task<string> SignUpAsync(SignUpModel signUpModel);
        Task<string> SignInAsync(SignInModel signInModel);
    }
}
