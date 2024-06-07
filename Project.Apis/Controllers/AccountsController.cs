using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Apis.DTOS;
using Project.Core.Entities;
using Project.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using Project.Apis.Extensions;
using System;
using System.Security.Cryptography;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.EntityFrameworkCore;
using Project.Repository.Identity;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;
using Newtonsoft.Json;

namespace Project.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _usermanager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly AppIdentityDbContext _dbContext;


        public AccountsController(UserManager<AppUser> usermanager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService, AppIdentityDbContext dbContext)
        {
            _usermanager = usermanager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _dbContext = dbContext;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (CheckEmailExists(model.Email).Result.Value)
                return BadRequest("This Email Already used");
            var User = new AppUser()
            {
                FullName = model.FullName,
                UserName = model.Email.Split('@')[0],
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };
            var Result = await _usermanager.CreateAsync(User, model.password);
            if (!Result.Succeeded) return BadRequest();

            var ReturnedUser = new UserDto()
            {
                FullName = User.FullName,
                Email = User.Email,
                PhoneNumber = User.PhoneNumber,
                Token = await _tokenService.CreateTokenAsync(User, _usermanager)
                
            };

            return Ok(ReturnedUser);
        }
        [HttpPost("SendToConfirmEmail")]
        public async Task<ActionResult> SendToConfirmEmail(string email)
        {
            var user = await _usermanager.FindByEmailAsync(email);
            if (user != null)
            {
                user.Token = await _usermanager.GeneratePasswordResetTokenAsync(user);
                user.ConfirmCode = random.Next(100000, 999999);
                await _usermanager.UpdateAsync(user);
                var Email = new Email()
                {
                    Subject = "confirm Email",
                    To = email,
                    Body = user.ConfirmCode
                };

                EmailSettings.SendEmail(Email);

                return Ok("code sent to your email");
            }
            else
            {
                return BadRequest("Email is Not Exist");
            }
        }
        [HttpPost("ConfirmGmail")]
        public async Task<ActionResult> ConfirmGmail(ConfirmDto confirm)
        {
            var user = await _usermanager.FindByEmailAsync(confirm.Email);
            if (user != null)
            {
                if (user.ConfirmCode == confirm.Code || string.IsNullOrWhiteSpace(confirm.Email))

                {
                    user.EmailConfirmed = true;
                    _dbContext.SaveChanges();
                    return Ok("Email Confirmed Successfully");
                }
                else
                {
                    return BadRequest("Invalid optocode or email");
                }
            }
            else
            {
                return BadRequest("Invalid user");
            }
        }
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var User = await _usermanager.FindByEmailAsync(model.Email);
            if (User is null) return Unauthorized();
            if (User.EmailConfirmed == true)
            {
                var Result = await _signInManager.CheckPasswordSignInAsync(User, model.password, false);
                if (!Result.Succeeded) return Unauthorized();

                var ReturnedUser = new UserDto()
                {
                    FullName = User.FullName,
                    Email = User.Email,
                    Token = await _tokenService.CreateTokenAsync(User, _usermanager),
                    PhoneNumber = User.PhoneNumber
                };
                return Ok(ReturnedUser);
            }
            else return BadRequest("Please Confirm your Email");

        }

        [Authorize]
        [HttpGet("CurrentUser")]
        public async Task<ActionResult<CurrentUserDto>> GetCurrentUser()
        {
            var Email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _usermanager.FindByEmailAsync(Email);
            var ReturnedUser = new CurrentUserDto()
            {
                FullName = user.FullName,
                Email = user.Email,
                PictureUrl = user.PictureUrl,
                Token = await _tokenService.CreateTokenAsync(user, _usermanager)

            };
            return Ok(ReturnedUser);
        }

        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            return await _usermanager.FindByEmailAsync(email) is not null;
        }

         Random random = new Random();

        [HttpPost("sendemail")]
        public async Task<ActionResult> SendEmail(string email)
        {

            var user = await _usermanager.FindByEmailAsync(email);
            if (user != null)
            {
                user.Token = await _usermanager.GeneratePasswordResetTokenAsync(user);
                user.Code = random.Next(100000, 999999);
                await _usermanager.UpdateAsync(user);
                var Email = new Email()
                {
                    Subject = "Reset Password",
                    To = email,
                    Body = user.Code
                };
                
                EmailSettings.SendEmail(Email);

                return Ok("Password reset link sent to your email");
            }
            else
            {
                return BadRequest("Email is Not Exist");
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto Reset)
        {
            var user = await _usermanager.FindByEmailAsync(Reset.Email);
            if (user != null)
            {
                if (user.Code == Reset.Code || string.IsNullOrWhiteSpace(Reset.Email)
                    ||
                   string.IsNullOrWhiteSpace(Reset.NewPassword) ||
                   string.IsNullOrWhiteSpace(Reset.ConfirmNewPassword))
                {

                    var Result = await _usermanager.ResetPasswordAsync(user, user.Token, Reset.NewPassword);

                    return Ok("Password Reset Successfully");
                }
                else
                {
                    return BadRequest("Invalid optocode or email");
                }
            }
            else
            {
                return BadRequest("Invalid user");
            }
        }


        [Authorize]
        [HttpPut("EditProfile")]
        public async Task<ActionResult> EditProfile(EditProfileDto model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _usermanager.FindByEmailAsync(email);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (!string.IsNullOrWhiteSpace(model.FullName))
            {
                user.FullName = model.FullName;
            }

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                user.PhoneNumber = model.PhoneNumber;
            }

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                user.Email = model.Email;
            }

            // Save changes to the database
            var result = await _usermanager.UpdateAsync(user);
            if (result.Succeeded)
            {
           
                return Ok("Profile changed successfully");
            }
            else
            {
                return BadRequest("Failed to update user profile");
            }
        }

        [Authorize]
        [HttpPut("ChangePassword")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _usermanager.FindByEmailAsync(email);

            if (user == null)
            {
                return BadRequest("User not found");
            }
               
            // Change password
            var result = await _usermanager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password changed successfully");
            }
            else
            {
                return BadRequest("Failed to change password");
            }
        }
        [Authorize]
        [HttpPost("SendImageAndPredict")]
        public async Task<ActionResult> SendImageAndPredict(IFormFile image)
        {
            // Call the Flask API to get the prediction
            string prediction = await FlaskApiService.GetPredictionFromFlaskAPI(image);
            // Store the prediction in the database
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _usermanager.FindByEmailAsync(email);

            if (user != null && !string.IsNullOrWhiteSpace(prediction))
            {
                // Store the prediction in the Projections table
                string imagePath = ImagePath.SaveImageFile(image);
                var predictionObj = JsonConvert.DeserializeObject<Prediction>(prediction);
                var projection = new Projection
                {
                    Image = imagePath, 
                    Text = predictionObj.caption,
                    AppUser = user,
                    predicted_label= predictionObj.predicted_label
                };

                if (predictionObj == null || string.IsNullOrWhiteSpace(predictionObj.predicted_label))
                {
                    return BadRequest("Invalid prediction format");
                }
                if (predictionObj.predicted_label == "Chest")
                {
                    _dbContext.Projections.Add(projection);
                    await _dbContext.SaveChangesAsync();

                    var json = JsonConvert.DeserializeObject<Prediction>(prediction);

                    return Ok(json);
                }
                else
                {
                    var json = JsonConvert.DeserializeObject<Prediction>(prediction);
                    return Ok(json);
                }
            }
            else
            {
                return BadRequest("Failed to store prediction");
            }
        }

        [Authorize]
        [HttpGet("PredictionHistory")]
        public async Task<ActionResult<IEnumerable<PredictDto>>> GetPredictionHistory()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _usermanager.FindByEmailAsync(email);

            if (user != null)
            {
                var history = _dbContext.Projections
                             .Where(p => p.AppUserId == user.Id)
                             .Select(p => new
                           {
                               caption = p.Text,
                               image = p.Image
                           })
                           .ToList();

                return Ok(history);
            }
            else
            {
                return BadRequest("User not found");
            }
        }

        [Authorize]
        [HttpDelete("DeletePrediction")]
        public async Task<ActionResult> DeletePrediction()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _usermanager.FindByEmailAsync(email);

            if (user != null)
            {
                var prediction = _dbContext.Projections
                    .Where(p => p.AppUserId == user.Id);

                if (prediction != null)
                {
                    foreach (var item in prediction)
                    {
                        _dbContext.Projections.Remove(item);
                    }

                    await _dbContext.SaveChangesAsync();

                    return Ok("Prediction deleted successfully");
                }
                else
                {
                    return NotFound("Prediction not found");
                }
            }
            else
            {
                return BadRequest("User not found");
            }
        }
        [Authorize]
        [HttpPost("UploadImage")]
        public async Task<ActionResult<UploadImageDto>> UploadImage(IFormFile imageFile)
        {

            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _usermanager.FindByEmailAsync(email);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            string imagePath = ImagePath.SaveImageFile(imageFile);

            // Update the user's profile image path
            user.PictureUrl = imagePath;

            var result = await _usermanager.UpdateAsync(user);
            if (result.Succeeded)
            {
                UploadImageDto uploadImage = new UploadImageDto()
                {
                    UploadedImage = imagePath,
                };
                return Ok(uploadImage);
            }
            else
            {
                return BadRequest("Failed to upload profile image");
            }
        }

        [Authorize]
        [HttpDelete("DeleteImage")]
        public async Task<ActionResult> DeleteImage()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _usermanager.FindByEmailAsync(email);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            user.PictureUrl = null;

            // Update the user to remove the profile image path
            var result = await _usermanager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok("Profile image deleted successfully");
            }
            else
            {
                return BadRequest("Failed to delete profile image");
            }
        }
       

    }

}
