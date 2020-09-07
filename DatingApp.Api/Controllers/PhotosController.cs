using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.Api.Data;
using DatingApp.Api.DTOs;
using DatingApp.Api.Helpers;
using DatingApp.Api.MOdels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.Api.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    //[ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repository;
        private readonly IOptions<CloudinarySettings> _CloudinaryConfig;
        private readonly IMapper _mapper;
        private Cloudinary _Cloudinary;
        public PhotosController(IDatingRepository repository, IMapper mapper,
                                 IOptions<CloudinarySettings> CloudinaryConfig)
        {
           _CloudinaryConfig = CloudinaryConfig;
           _mapper = mapper;
            _repository = repository;

            Account acc = new Account(
                _CloudinaryConfig.Value.CloudName,
                _CloudinaryConfig.Value.ApiKey,
                _CloudinaryConfig.Value.ApiSecret
            );

            _Cloudinary = new Cloudinary(acc);
        }
 
        [HttpGet("{id}", Name = "GetPhoto")]
        public async  Task<IActionResult> GetPhoto(int Id)
        {
            var photoFromRepo = await _repository.GetPhoto(Id);
            var photo =  _mapper.Map<PhotoForReturnDto> (photoFromRepo);

            return Ok(photo);
        }


        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId,
                         [FromForm]PhotoForCreationDto userPhoto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var user = await _repository.GetUser(userId);

            var file = userPhoto.File;

             var uploadResult = new ImageUploadResult();

             if(file.Length > 0)
             {
                 using (var stream = file.OpenReadStream())
                 {
                     var uploadParams = new ImageUploadParams()
                     {
                         File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")

                     } ;
                     uploadResult = _Cloudinary.Upload(uploadParams);
                 }
             }

             userPhoto.Url = uploadResult.Url.ToString();
             userPhoto.PublicId = uploadResult.PublicId;

             var photo = _mapper.Map<Photo> (userPhoto);

             if(!user.Photos.Any(x => x.IsMainPhoto))
             {
                 photo.IsMainPhoto = true;
             }

               user.Photos.Add(photo);
             
             if(await _repository.SaveAll())
             {
                 var phototoReturn = _mapper.Map<PhotoForReturnDto> (photo);
                 return CreatedAtRoute("GetPhoto", new {userId = userId, id = photo.Id}, phototoReturn);
             }

             return BadRequest("Could not save photo");
        }

        [HttpPost("{id}/setMain")] 
        public async Task<IActionResult> SetMainPhoto (int userId, int id){
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
             var user = await _repository.GetUser(userId);

             if (!user.Photos.Any(x => x.Id == id))
             return Unauthorized();

             var photoFromRepo = await _repository.GetPhoto(id);

             if (photoFromRepo.IsMainPhoto)
             return BadRequest("This is already the main Photo");

             var currentMainPhoto = await _repository.GetMainPhotoForUser(userId);
                currentMainPhoto.IsMainPhoto = false;
            photoFromRepo.IsMainPhoto =true;

            if(await _repository.SaveAll())
                return NoContent();

              return BadRequest("Could not set photo to main-photo ")  ;
        }
}
}