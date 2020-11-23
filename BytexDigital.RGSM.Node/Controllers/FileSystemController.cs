using System;

using AutoMapper;

using BytexDigital.RGSM.Application.Exceptions;
using BytexDigital.RGSM.Node.Application.Shared.Services;
using BytexDigital.RGSM.Shared.TransferObjects.Models.Services.NodeFileSystemService;

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
        private readonly NodeFileSystemService _nodeFileSystemService;

        public FileSystemController(IMapper mapper, NodeFileSystemService nodeFileSystemService)
        {
            _mapper = mapper;
            _nodeFileSystemService = nodeFileSystemService;
        }

        [HttpGet("GetDirectoryInfo")]
        public DirectoryDto GetDirectory([FromQuery] string path)
        {
            try
            {
                return _mapper.Map<DirectoryDto>(_nodeFileSystemService.GetDirectory(path));

            }
            catch (UnauthorizedAccessException ex)
            {
                throw ServiceException.AddError().WithCode(nameof(UnauthorizedAccessException)).WithMessage(ex.Message).Build();
            }
        }
    }
}
