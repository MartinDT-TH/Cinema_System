// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Cinema.Models;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using static QRCoder.PayloadGenerator.WiFi;
using Cinema.Utility;

namespace Cinema_System.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;


        public LoginModel(SignInManager<IdentityUser> signInManager,
                          ILogger<LoginModel> logger,
                          UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        // public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        // {
        //     returnUrl ??= Url.Content("~/");

        //     ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        //     if (ModelState.IsValid)
        //     {
        //         var user = await _userManager.FindByEmailAsync(Input.Email);
        //         if (user != null)
        //         {
        //             // Attempt login
        //             var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: true);

        //             if (result.Succeeded)
        //             {
        //                 _logger.LogInformation("User logged in.");
        //                 await _userManager.ResetAccessFailedCountAsync(user); // Reset access failed count

        //                 // Retrieve updated user details
        //                 var appUser = user as ApplicationUser;
        //                 var userImage = appUser?.UserImage ?? "/images/default-avatar.png";
        //                 var userFullName = appUser?.FullName ?? "User";

        //                 // Remove old claims and add updated ones
        //                 var existingClaims = await _userManager.GetClaimsAsync(user);
        //                 await _userManager.RemoveClaimsAsync(user, existingClaims);

        //                 var claims = new List<Claim>
        //         {
        //             new Claim("FullName", userFullName),
        //             new Claim("UserImage", userImage)
        //         };
        //                 await _userManager.AddClaimsAsync(user, claims);

        //                 // Refresh authentication session ------------------------------
        //                 await _signInManager.SignInAsync(user, isPersistent: false);
        //                 var roles = await _userManager.GetRolesAsync(user);
        //                 if (roles.Contains(SD.Role_Admin))
        //                 {
        //                     return RedirectToAction("Index", "Home", new { area = "Admin" });
        //                 }
        //                 else
        //                 {
        //                     return RedirectToAction("Index", "Home", new { area = "Guest" });
        //                 }
                       
        //                 //------------------- Allow information loading even it is first login , if not first login in4 is null !
        //                 //                        When a user logs in, ASP.NET Identity creates an authentication cookie that stores user claims.
        //                 //However, if claims are updated after login, the authentication cookie is not refreshed automatically.
        //                 //SignInAsync(user, false) forces a re - authentication, updating the session cookie immediately with the new claims.
        //                 //As a result, the UI can access the correct FullName and UserImage right after login, instead of showing outdated or null values.
        //                 return LocalRedirect(returnUrl);
        //             }

        //             if (result.RequiresTwoFactor)
        //             {
        //                 return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
        //             }
        //             if (result.IsLockedOut)
        //             {
        //                 _logger.LogWarning("User account locked out.");
        //                 return RedirectToPage("./Lockout", new  { userId = user.Id});
        //             }
        //         }

        //         ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //         return Page();
        //     }

        //     return Page();
        // }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: true);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");
                        await _userManager.ResetAccessFailedCountAsync(user);

                        // Cập nhật claims cho user
                        var appUser = user as ApplicationUser;
                        var claims = new List<Claim>
                        {
                            new Claim("FullName", appUser?.FullName ?? "User"),
                            new Claim("UserImage", appUser?.UserImage ?? "/images/default-avatar.png")
                        };

                        // Xóa claims cũ và thêm claims mới
                        var existingClaims = await _userManager.GetClaimsAsync(user);
                        await _userManager.RemoveClaimsAsync(user, existingClaims);
                        await _userManager.AddClaimsAsync(user, claims);

                        // Đăng nhập lại để refresh claims
                        await _signInManager.SignInAsync(user, Input.RememberMe);

                        // Chuyển hướng dựa trên role
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Contains(SD.Role_Admin))
                        {
                            return RedirectToAction("Index", "Users", new { area = "Admin" });
                        }
                        return RedirectToAction("Index", "Home", new { area = "Guest" });
                    }

                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }

                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout", new { userId = user.Id });
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

    }
}
