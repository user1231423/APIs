using API.Files.DTO;
using Business.Files.Interfaces;
using Business.Files.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Files.Controllers
{
    /// <summary>
    /// Files controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        /// <summary>
        /// File service
        /// </summary>
        private readonly IFileService _fileService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileService"></param>
        public FilesController(IFileService fileService)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        /// <summary>
        /// Get file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetFile(int id)
        {
            var file = await _fileService.Load(id);

            if (file == null)
                return NotFound(new { message = "File not found" });

            var fileDTO = new FileDTO()
            {
                ContentType = file.ContentType,
                Id = file.Id,
                Key = file.Key,
                OriginalName = file.OriginalName
            };

            return Ok(fileDTO);
        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Download")]
        public async Task<ActionResult> DownloadFile(int id)
        {
            var file = await _fileService.Load(id);

            if (file == null)
                return NotFound(new { message = "File not found" });

            return File(System.IO.File.ReadAllBytes(file.Path), file.ContentType, file.OriginalName);
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("Upload")]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            var uploadedId = await _fileService.Save(file);

            return Ok(uploadedId);
        }

        /// <summary>
        /// Update file
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateFile(int id, IFormFile file)
        {
            var updatedId = await _fileService.Update(id, file);

            return Ok(updatedId);
        }

        /// <summary>
        /// Update file
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateFile"></param>
        /// <returns></returns>
        [HttpPut("{id}/OriginalName")]
        public async Task<ActionResult> UpdateFile(int id, UpdateFileDTO updateFile)
        {
            var oldFile = await _fileService.Load(id);

            if (oldFile == null)
                return NotFound(new { message = "File not found" });

            if (!string.IsNullOrEmpty(updateFile.OriginalName))
                oldFile.OriginalName = updateFile.OriginalName;

            var updatedId = await _fileService.Update(id, oldFile);

            return Ok(updatedId);
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFile(int id)
        {
            var deletedId = await _fileService.Delete(id);

            return Ok(deletedId);
        }
    }
}
