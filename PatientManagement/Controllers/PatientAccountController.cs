using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientManagement.BussinessLogic.Repository;
using PatientManagement.Models.Models;
using YourNamespace.Repositories;

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientAccountController : ControllerBase
    {
        private readonly IPatientAccountRepository _accountRepository;

        public PatientAccountController(IPatientAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel signUpModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _accountRepository.SignUpAsync(signUpModel);

            if (user != "User created successfully")
            {
                return BadRequest(new { Error = user });
            }

            return Ok(new { Message = user });
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInModel signInModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var token = await _accountRepository.SignInAsync(signInModel);

            if (token == "Invalid login attempt")
            {
                return Unauthorized(new { Error = token });
            }

            return Ok(new { Token = token });
        }
    }
}