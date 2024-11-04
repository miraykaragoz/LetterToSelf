using System.Net;
using System.Net.Mail;
using Dapper;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using LetterToSelf.Models;
using Microsoft.Data.SqlClient;
using RestSharp;

namespace LetterToSelf.Controllers;

public class HomeController : Controller
{
    private readonly string connectionString = "";

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("/SendLetter")]
    public IActionResult SendLetter()
    {
        return View();
    }

    [HttpPost("/SendLetter")]
    public IActionResult SendLetter(FutureMessage model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.MessageCssClass = "alert alert-secondary";
            ViewBag.Message = "Form eksik veya hatalı!";
            return View("Message");
        }
        
        var captchaToken = Request.Form["g-recaptcha-response"];
        
        if (!VerifyCaptcha(captchaToken))
        {
            ViewBag.CaptchaError = true;
            return View();
        }

        using (var connection = new SqlConnection(connectionString))
        {
            var sql = @"INSERT INTO FutureMessage (Email, Message, CurrentDate, SendDate, IsSent) VALUES (@Email, @Message, @CurrentDate, @SendDate, @IsSent)";

            var rowsAffected = connection.Execute(sql, new
            {
                model.Email,
                model.Message,
                CurrentDate = DateTime.Now,
                model.SendDate,
                IsSent = false
            });

            if (rowsAffected > 0)
            {
                ScheduleMessageSending(model.Email, model.Message, model.SendDate);
                return RedirectToAction("Confirmation");
            }
        }

        return View("Index");
    }

    [HttpGet("/Confirmation")]
    public IActionResult Confirmation()
    {
        return View();
    }

    private void ScheduleMessageSending(string email, string message, DateTime sendDate)
    {
        BackgroundJob.Schedule(() => SendEmail(email, message), sendDate);
    }

    public void SendEmail(string email, string message)
    {
        var client = new SmtpClient("smtp.eu.mailgun.org", 587)
        {
            Credentials = new NetworkCredential("",
                ""),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(""),
            Subject = "Your Future Message",
            Body = $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        margin: 0;
                        padding: 20px;
                        background-color: #f4f4f4;
                    }}
                    .container {{
                        background-color: white;
                        padding: 20px;
                        border-radius: 10px;
                        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                        max-width: 600px;
                        margin: 0 auto;
                    }}
                    .header {{
                        font-size: 24px;
                        color: #4A90E2;
                        text-align: center;
                        margin-bottom: 20px;
                    }}
                    .message {{
                        font-size: 16px;
                        color: #333;
                        line-height: 1.5;
                        white-space: pre-wrap;
                    }}
                    .footer {{
                        margin-top: 30px;
                        text-align: center;
                        font-size: 14px;
                        color: #999;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>Your Future Message</div>
                    <div class='message'>{message}</div>
                    <div class='footer'>Delivered by LetterToSelf</div>
                </div>
            </body>
            </html>",
            IsBodyHtml = true,
        };

        mailMessage.To.Add(email);

        client.Send(mailMessage);
    }
    
    public bool VerifyCaptcha(string captchaToken)
    {
        var client = new RestClient("https://www.google.com/recaptcha");
        var request = new RestRequest("api/siteverify", Method.Post);
        request.AddParameter("secret", "");
        request.AddParameter("response", captchaToken);

        var response = client.Execute<CaptchaResponse>(request);

        if(response.Data.Success && response.Data.Score > 0.6)
        {
            return true;
        }
            
        return false;
    }
}