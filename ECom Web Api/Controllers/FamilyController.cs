using ECom_Web_Api.Data;
using ECom_Web_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ECom_Web_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamilyController : ControllerBase
    {
        private readonly ECommerceContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public FamilyController(ECommerceContext context, IConfiguration configuration, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFamilyDetail([FromBody] FamilyDetailDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return BadRequest("Family name is required.");

                if (dto.Date == default)
                    return BadRequest("Date is required.");

                foreach (var child in dto.Children)
                {
                    if (string.IsNullOrWhiteSpace(child.Name))
                        return BadRequest("Child name is required.");
                    if (child.Dob == default)
                        return BadRequest("Child DOB is required.");

                    foreach (var identity in child.Identities)
                    {
                        if (string.IsNullOrWhiteSpace(identity.IdentityType))
                            return BadRequest("Identity type is required.");
                        if (string.IsNullOrWhiteSpace(identity.IdentityNumber))
                            return BadRequest("Identity number is required.");

                        if (identity.IdentityType.ToLower() == "aadhaar")
                        {
                            if (!AadhaarValidator.IsValidAadhaar(identity.IdentityNumber))
                                return BadRequest($"Invalid Aadhaar number: {identity.IdentityNumber}");
                        }
                    }
                }

                var family = new FamilyDetail
                {
                    Name = dto.Name,
                    Date = dto.Date,
                    Children = dto.Children.Select(c => new FamilyChild
                    {
                        Name = c.Name,
                        Gender = c.Gender,
                        Dob = c.Dob,
                        Photo = c.Photo,
                        Identities = c.Identities.Select(i => new FamilyIdentity
                        {
                            IdentityType = i.IdentityType,
                            IdentityNumber = i.IdentityNumber
                        }).ToList()
                    }).ToList()
                };

                _context.FamilyDetails.Add(family);
                await _context.SaveChangesAsync();

                return Ok(family);
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public static class AadhaarValidator
        {
            // Multiplication table d
            private static readonly int[,] d = new int[,]
            {
        {0,1,2,3,4,5,6,7,8,9},
        {1,2,3,4,0,6,7,8,9,5},
        {2,3,4,0,1,7,8,9,5,6},
        {3,4,0,1,2,8,9,5,6,7},
        {4,0,1,2,3,9,5,6,7,8},
        {5,9,8,7,6,0,4,3,2,1},
        {6,5,9,8,7,1,0,4,3,2},
        {7,6,5,9,8,2,1,0,4,3},
        {8,7,6,5,9,3,2,1,0,4},
        {9,8,7,6,5,4,3,2,1,0}
            };

            // Permutation table p
            private static readonly int[,] p = new int[,]
            {
        {0,1,2,3,4,5,6,7,8,9},
        {1,5,7,6,2,8,3,0,9,4},
        {5,8,0,3,7,9,6,1,4,2},
        {8,9,1,6,0,4,3,5,2,7},
        {9,4,5,3,1,2,6,8,7,0},
        {4,2,8,6,5,7,9,3,0,1},
        {2,7,9,3,8,0,6,4,1,5},
        {7,0,4,6,9,1,3,2,5,8}
            };

            // Inverse table inv
            private static readonly int[] inv = { 0, 4, 3, 2, 1, 5, 6, 7, 8, 9 };

            public static bool IsValidAadhaar(string aadhaarNumber)
            {
                if (string.IsNullOrWhiteSpace(aadhaarNumber) || aadhaarNumber.Length != 12 || !aadhaarNumber.All(char.IsDigit))
                    return false;

                int c = 0;
                int[] myArray = aadhaarNumber.ToCharArray().Select(x => x - '0').Reverse().ToArray();

                for (int i = 0; i < myArray.Length; i++)
                {
                    c = d[c, p[(i % 8), myArray[i]]];
                }

                return c == 0;
            }
        }


    }
}
