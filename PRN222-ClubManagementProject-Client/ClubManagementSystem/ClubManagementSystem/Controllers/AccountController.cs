using System.Data;
using System.Security.Claims;
using System.Text;
using BussinessObjects.Models;
using BussinessObjects.Models.Dtos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ClubManagementSystem.Controllers.Filter;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Implementation;
using Services.Interface;

namespace ClubManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;
        private readonly IImageHelperService _imageService;
        private readonly Cloudinary _cloudinary;
        public AccountController(IConfiguration configuration, IAccountService accountService, IImageHelperService imageHelperService, Cloudinary cloudinary)
        {
            _accountService = accountService;
            _configuration = configuration;
            _imageService = imageHelperService;
            _cloudinary = cloudinary;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckLogin(string Gmail, string password)
        {
            var user = await _accountService.CheckLogin(Gmail, password);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid email or password. Please try again!";
                return RedirectToAction("Login", "Account");
            }

            var adminEmail = _configuration["AdminAccount:Gmail"];
            var adminPassword = _configuration["AdminAccount:Password"];
            var profilePicture = user.ImagePicture!=null ? user.ImagePicture : (user.ProfilePicture != null
                ? _imageService.ConvertToBase64(user.ProfilePicture, "png")
                : string.Empty);
            //Check role
            string role;           
            if (user.Email == adminEmail && user.Password == adminPassword)
            {
                role = "SystemAdmin"; // Special System Admin
            }
            else
            {
                role = "User"; // Default role if no club role found
            }

            var clubMember = await _accountService.CheckRole(user.UserId);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.Username ?? "Admin"),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, role)
            };

            // Add claims for club-specific roles
            foreach (var clubRole in clubMember)
            {
                claims.Add(new Claim($"ClubRole_{clubRole.ClubId}", clubRole.Role.RoleName));
                claims.Add(new Claim($"RoleName", clubRole.Role.RoleName));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            HttpContext.Session.SetString("userPicture", profilePicture);

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public async System.Threading.Tasks.Task GoogleLogin()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }

        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            var googleClaims = result.Principal.Identities.FirstOrDefault()?.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);
            if (googleClaims == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var email = googleClaims.GetValueOrDefault(ClaimTypes.Email);
            var name = googleClaims.GetValueOrDefault(ClaimTypes.Name);
            var avatarUrl = googleClaims.GetValueOrDefault("urn:google:picture");

            byte[] avatarByte;
            using (var httpClient = new HttpClient())
            {
                avatarByte = await httpClient.GetByteArrayAsync(avatarUrl);
            }

            var user = await _accountService.CheckEmailExist(email);
            string profilePicture = "";

            if (user != null)
            {
                
                profilePicture = user.ImagePicture!=null ? user.ImagePicture : _imageService.ConvertToBase64(user.ProfilePicture, "png");
                string role= "User";
                var clubMember = await _accountService.CheckRole(user.UserId);
                var claims = new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new(ClaimTypes.Name, user.Username ?? "Admin"),
                        new(ClaimTypes.Email, user.Email),
                        new(ClaimTypes.Role, role)
                    };

                // Add claims for club-specific roles
                foreach (var clubRole in clubMember)
                {
                    claims.Add(new Claim($"ClubRole_{clubRole.ClubId}", clubRole.Role.RoleName));
                    claims.Add(new Claim($"RoleName", clubRole.Role.RoleName));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                HttpContext.Session.SetString("userPicture", profilePicture);
                return RedirectToAction("Index", "Home");
            }

            // Add new user into database
            var newUser = new User
            {
                Username = name,
                Email = email,
                ProfilePicture = avatarByte,
                Password = "@123@"
            };

            var newGmailUser = await _accountService.AddGmailUser(newUser);
            profilePicture = _imageService.ConvertToBase64(newGmailUser.ProfilePicture, "png");

            var newClaims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, newGmailUser.UserId.ToString()),
                new(ClaimTypes.Name, newGmailUser.Username),
                new(ClaimTypes.Email, newGmailUser.Email),
                new(ClaimTypes.Role, "User")
            };

            var newClaimsIdentity = new ClaimsIdentity(newClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(newClaimsIdentity));
            HttpContext.Session.SetString("userPicture", profilePicture);

            return RedirectToAction("Index", "Home");
        }

        //[SessionAuthorize("User,ClubMember,ClubAdmin")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public IActionResult SignUp()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpUserDto newUser)
        {
            if (!ModelState.IsValid)
            {
                return View(newUser);
            }
            bool isUserExist = CheckExistUser(newUser.Email, newUser.Username);

            if (isUserExist == true)
            {
                return RedirectToAction("SignUp", "Account");
            }
            byte[] defaultProfilePicture = await _accountService.GetDefaultProfilePictureAsync();
            User user = new User
            {
                Username = newUser.Username,
                Email = newUser.Email,
                Password = newUser.Password,
                CreatedAt = DateTime.Now,
                ProfilePicture = defaultProfilePicture
            };

            await _accountService.AddUser(user);

            TempData["SuccessMessage"] = "Sign up successfully! Please login.";
            return RedirectToAction("Login", "Account");
        }


        //[Authorize(Roles = "SystemAdmin")]
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var user = await _accountService.FindUserAsync(id.Value);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(user);
        //}

        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _accountService.FindUserAsync(int.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            var editUserDto = new EditUserDto
            {
                Username = user.Username,
                CurrentPassword = user.Password,
                Email = user.Email
            };

            return View(editUserDto);
        }


        // POST: Save changes to profile
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserDto editUser)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var (success, message) = await _accountService.UpdateUserProfileAsync(userId, editUser);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] = message;
            return View(editUser);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
        {
            if (profilePicture != null && profilePicture.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(profilePicture.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("profilePicture", "Only .jpg, .jpeg, and .png files are allowed.");
                    return View();
                }

                // Upload to Cloudinary
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(profilePicture.FileName, profilePicture.OpenReadStream()),
                    Folder = "profile_pictures", // optional: folder in Cloudinary
                    PublicId = $"user_{User.FindFirst(ClaimTypes.NameIdentifier)?.Value}_{DateTime.Now.Ticks}", // optional: custom naming
                    Transformation = new Transformation().Width(300).Height(300).Crop("fill") // optional resize
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    ModelState.AddModelError("profilePicture", "Upload failed. Please try again.");
                    return View();
                }

                var imageUrl = uploadResult.SecureUrl.ToString();

                // Save imageUrl to DB
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                {
                    var user = await _accountService.FindUserAsync(int.Parse(userId));
                    if (user != null)
                    {
                        user.ImagePicture = imageUrl; // Assuming your User model has ProfilePictureUrl
                        await _accountService.UpdateUserAsync(user);

                        HttpContext.Session.SetString("userPicture", imageUrl); // Store URL in session

                        return RedirectToAction("Edit", "Account");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("profilePicture", "No file selected.");
                return View();
            }

            return View();
        }

        public bool CheckExistUser(string email, string username)
        {
            bool emailExists = _accountService.CheckEmailExist(email).Result != null;
            bool usernameExists = _accountService.CheckUsernameExist(username).Result != null;

            if (emailExists)
            {
                TempData["ErrorMessage"] = "Email already exists!";
                //return RedirectToAction("SignUp", "Account");
                return true;
            }
            if (usernameExists)
            {
                TempData["ErrorMessage"] = "Username already exists!";
                //return RedirectToAction("SignUp", "Account");
                return true;
            }

            return false;
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}


