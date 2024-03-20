using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using LiveDiabetes.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace LiveDiabetes.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Por favor, insira o seu e-mail.")]
            [EmailAddress(ErrorMessage = "Este e-mail não é um endereço de e-mail válido.")]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Não revelar que o usuário não existe ou não está confirmado
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code = code },
                    protocol: Request.Scheme);

                var emailContent = $"<h1>Alteração de Password</h1>" + $"<p> </p>" +
                                                                        $"<p>Verificamos que fizeste um pedido para alterares a tua password. </p>" +
                                                                        $"<p>Por favor, clica no botão abaixo para alterares a tua password:</p>" +
                                                                        $" <a href='" + HtmlEncoder.Default.Encode(callbackUrl) + "' style='background-color: #191970; color: white; padding: 10px 20px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px;'>Alterar Password</a>" +
                                                                        $"<p>Se não solicitaste esta alteração, por favor, ignora este e-mail.</p>";



                await SendEmailAsync(Input.Email, "LiveDiabetes: Alteração de password", emailContent);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }

        private static async Task<bool> SendEmailAsync(string email, string subject, string messageBody)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();
                message.From = new MailAddress("201901381@estudantes.ips.pt");
                message.To.Add(email);
                message.Subject = subject;
                message.IsBodyHtml = true; 
                message.Body = messageBody;

                smtpClient.Port = 587;
                smtpClient.Host = "smtp-mail.outlook.com";


                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("201901381@estudantes.ips.pt", "Ips@2000");
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(message);

                await smtpClient.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
