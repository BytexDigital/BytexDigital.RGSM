using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using AutoMapper;

using BytexDigital.Common.Errors.Exceptions;
using BytexDigital.RGSM.Node.Application.Commands.NodeFileSystemService;
using BytexDigital.RGSM.Shared.TransferObjects.Models.Services.NodeFileSystemService;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BytexDigital.RGSM.Node.Controllers
{
    [Route("API/[controller]")]
    [ApiController]
    [Authorize]
    public class FileSystemController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public FileSystemController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [HttpGet("GetDirectoryInfo")]
        public async Task<ActionResult<DirectoryDto>> GetDirectoryAsync([FromQuery, Required] string path)
        {
            try
            {
                var response = await _mediator.Send(new GetDirectoryQuery
                {
                    Path = path
                });

                return _mapper.Map<DirectoryDto>(response.Directory);

            }
            catch (UnauthorizedAccessException ex)
            {
                throw new ServiceException().WithCode(nameof(UnauthorizedAccessException)).WithMessage(ex.Message);
            }
        }
    }
}
