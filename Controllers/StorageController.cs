using FraudexPy.Api.Common.Models;
using FraudexPy.Api.Features.Storage.Commands;
using FraudexPy.Api.Features.Storage.DTOs;
using FraudexPy.Api.Features.Storage.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FraudexPy.Api.Controllers;

[Authorize]
public class StorageController : BaseApiController
{
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<FileUploadResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<FileUploadResultDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadFile(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<FileUploadResultDto>.Fail("A valid file is required."));
        }

        using var stream = file.OpenReadStream();
        var command = new UploadFileCommand(stream, file.FileName, file.ContentType);
        var result = await Mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("download-url/{objectName}")]
    [ProducesResponseType(typeof(ApiResponse<FileDownloadUrlDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<FileDownloadUrlDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDownloadUrl([FromRoute] string objectName, [FromQuery] int expirationSeconds = 3600)
    {
        var result = await Mediator.Send(new GetPresignedDownloadUrlQuery(objectName, expirationSeconds));
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
