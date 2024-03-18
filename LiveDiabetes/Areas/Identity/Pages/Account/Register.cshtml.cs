// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using LiveDiabetes.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace LiveDiabetes.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
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
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

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
            /// 
            [Required(ErrorMessage = "Por favor, insira o seu primeiro nome.")]
            [DataType(DataType.Text)]
            [RegularExpression("^[a-zA-ZÀ-ÿ\\s]*$", ErrorMessage = "Este campo não deve conter números ou caracteres especiais.")]
            [Display(Name = "Primeiro Nome")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Por favor, insira o seu último nome.")]
            [DataType(DataType.Text)]
            [RegularExpression("^[a-zA-ZÀ-ÿ\\s]*$", ErrorMessage = "Este campo não deve conter números ou caracteres especiais.")]
            [Display(Name = "Último Nome")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Por favor, selecione o seu género.")]
            [DataType(DataType.Text)]
            [Display(Name = "Género")]
            public string Gender { get; set; }

            [Required(ErrorMessage = "Por favor, insira a sua data de nascimento.")]
            [DataType(DataType.Date)]
            [CheckYear(ErrorMessage = "O ano não pode ultrapassar 2018.")]
            [Display(Name = "Data de Nascimento:")]
            public DateTime DateOfBirth { get; set; }

            [Required(ErrorMessage = "Por favor, insira o seu e-mail.")]
            [EmailAddress(ErrorMessage = "O campo Email não é um endereço de e-mail válido.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Por favor, insira a sua password.")]
            [StringLength(100, ErrorMessage = "A password tem de conter no mínimo 6 caracteres.", MinimumLength = 6)]
            [RegularExpression(@"^(?=.*[a-z]).+$", ErrorMessage = "A password deve conter pelo menos uma letra.")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            ///
            [Required(ErrorMessage = "Por favor, insira a sua password.")]
            [DataType(DataType.Password)]
            [RegularExpression(@"^(?=.*[0-9]).*$", ErrorMessage = "As passwords têm de conter pelo menos um número.")]
            [Display(Name = "Confirmar password")]
            [Compare("Password", ErrorMessage = "As passwords não são iguais.")]
            public string ConfirmPassword { get; set; }
        }

        public class CheckYearAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                DateTime dt = (DateTime)value;
                if (dt.Year > 2018)
                {
                    return new ValidationResult(ErrorMessage ?? "O ano não pode ultrapassar 2018.");
                }

                return ValidationResult.Success;
            }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                if (Input.DateOfBirth.Year > 2018)
                {
                    ModelState.AddModelError("Input.DateOfBirth", "O ano não pode ultrapassar 2018.");
                    return Page();
                }

                var user = CreateUser();

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.DateOfBirth = Input.DateOfBirth;
                user.Gender = Input.Gender;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await SendEmailAsync(Input.Email, "LiveDiabetes: Nova Conta", $"<h1>Confirmação de Registo</h1>" +
                                                                       $"<p> </p>" +
                                                                       $"<p>Olá, {Input.FirstName} {Input.LastName} </p>" +
                                                                       $"<p>Obrigado por te registares na nossa plataforma.</p>" +
                                                                       $"<p>Por favor, clica no botão abaixo para confirmares o teu registo:</p>" +
                                                                       $" <a href='" + HtmlEncoder.Default.Encode(callbackUrl) + "' style='background-color: #191970; color: white; padding: 10px 20px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px;'>Confirmar</a>" +
                                                                       $"<p>Se não solicitaste este registo, por favor, ignora este e-mail.</p>");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task<bool> SendEmailAsync(string email, string subject, string confirmLink)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();
                message.From = new MailAddress("201901381@estudantes.ips.pt");
                message.To.Add(email);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = confirmLink;

                smtpClient.Port = 587;
                smtpClient.Host = "smtp-mail.outlook.com";


                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("201901381@estudantes.ips.pt", "Ips@2000");
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
