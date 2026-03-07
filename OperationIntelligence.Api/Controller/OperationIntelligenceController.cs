using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using OperationIntelligence.Api.Models;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Security;
using OperationIntelligence.DB;
using OperationIntelligence.Api.Helpers;

namespace OperationIntelligence.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class OperationIntelligenceController : ControllerBase
    {
        private readonly IDonationService _donationService;
        private readonly BotDetectionService _botDetection;
        private readonly ILogger<OperationIntelligenceController> _logger;

        public OperationIntelligenceController(
            IDonationService donationService,
            BotDetectionService botDetection,
            ILogger<OperationIntelligenceController> logger)
        {
            _donationService = donationService;
            _botDetection = botDetection;
            _logger = logger;
        }

        // GET: api/donation
        [HttpGet]
        public IActionResult GetAllDonations([FromQuery] PaginationMeta query)
        {
            query.Page = query.Page <= 0 ? 1 : query.Page;
            query.Limit = query.Limit <= 0 ? 10 : query.Limit;

            var (items, totalCount) = _donationService.GetDonations(query.Page, query.Limit);

            var pagination = new PaginationMeta
            {
                Page = query.Page,
                Limit = query.Limit,
                Total = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / query.Limit)
            };

            return Ok(new ApiResponse<List<Donation>>
            {
                Data = items,
                Meta = new ApiMeta
                {
                    RequestId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow,
                    Pagination = pagination
                }
            });
        }

        // GET: api/donation/{id}
        [HttpGet("{id}", Name = "GetDonation")]
        public IActionResult GetDonationById(int id)
        {
            var donation = _donationService.GetDonationById(id);

            if (donation == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Data = null,
                    Errors = new List<ApiError>
                    {
                        new ApiError
                        {
                            Code = ErrorCode.NOT_FOUND,
                            Message = $"Donation with ID {id} was not found."
                        }
                    },
                    Meta = new ApiMeta
                    {
                        RequestId = Guid.NewGuid().ToString(),
                        Timestamp = DateTime.UtcNow
                    }
                });
            }

            return Ok(new ApiResponse<Donation>
            {
                Data = donation,
                Meta = new ApiMeta
                {
                    RequestId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow
                }
            });
        }

        // POST: api/donation
        [HttpPost]
        public IActionResult CreateAndUpdate([FromBody] Donation donation)
        {
            if (_botDetection.IsSuspiciousRequest(Request))
            {
                var response = new ApiResponse<object>
                {
                    Data = null,
                    Errors = new List<ApiError>
                    {
                        new ApiError
                        {
                            Code = ErrorCode.VALIDATION_ERROR,
                            Message = "Suspicious activity detected."
                        }
                    },
                    Meta = new ApiMeta
                    {
                        RequestId = Guid.NewGuid().ToString(),
                        Timestamp = DateTime.UtcNow
                    }
                };
                


                
                return new JsonResult(response)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            var newDonation = _donationService.CreateAndUpdate(donation);
            var res = new ApiResponse<Donation>
            {
                Data = newDonation,
                Meta = new ApiMeta
                {
                    RequestId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow
                }
            };


            return CreatedAtRoute("GetDonation", new { id = newDonation.Id }, res);
        }

        // DELETE: api/donation/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteDonation(int id)
        {
            var existing = _donationService.GetDonationById(id);
            if (existing == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Data = null,
                    Errors = new List<ApiError>
                    {
                        new ApiError
                        {
                            Code = ErrorCode.NOT_FOUND,
                            Message = $"Donation with ID {id} was not found."
                        }
                    },
                    Meta = new ApiMeta
                    {
                        RequestId = Guid.NewGuid().ToString(),
                        Timestamp = DateTime.UtcNow
                    }
                });
            }

            _donationService.DeleteDonation(id);

            return Ok(new ApiResponse<object>
            {
                Data = $"Donation with ID {id} deleted successfully.",
                Meta = new ApiMeta
                {
                    RequestId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow
                }
            });
        }
    }
}
