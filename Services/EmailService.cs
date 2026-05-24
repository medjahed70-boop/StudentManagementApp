using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace StudentManagementApp.Services
{
    /// <summary>
    /// خدمة البريد الإلكتروني - إرسال الإشعارات والإنذارات
    /// </summary>
    public class EmailService
    {
        private string _senderEmail = "your-email@gmail.com";
        private string _senderPassword = "your-app-password";
        private string _smtpServer = "smtp.gmail.com";
        private int _smtpPort = 587;

        /// <summary>
        /// إرسال إنذار أول (غياب 3 أيام)
        /// </summary>
        public async Task SendFirstWarningAsync(string guardianEmail, string studentName, string guardianName)
        {
            try
            {
                string subject = $"⚠️ إنذار أول - غياب {studentName}";
                string body = $@"
                السيد/السيدة: {guardianName}
                
                نتفيد حضرتكم بأن الطالب/الطالبة: {studentName}
                تغيب عن المؤسسة لمدة (3) أيام.
                
                يرجى التوجه للمدرسة أو الاتصال بنا في أقرب وقت.
                
                مع أطيب التحيات
                إدارة المدرسة";

                await SendEmailAsync(guardianEmail, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في إرسال الإنذار الأول: {ex.Message}");
            }
        }

        /// <summary>
        /// إرسال إنذار ثاني (غياب 10 أيام)
        /// </summary>
        public async Task SendSecondWarningAsync(string guardianEmail, string studentName, string guardianName)
        {
            try
            {
                string subject = $"⚠️ إنذار ثاني - غياب {studentName}";
                string body = $@"
                السيد/السيدة: {guardianName}
                
                نتفيد حضرتكم بأن الطالب/الطالبة: {studentName}
                تغيب عن المؤسسة لمدة (10) أيام.
                
                هذا إنذار ثاني - يرجى التحرك الفوري.
                
                مع أطيب التحيات
                إدارة المدرسة";

                await SendEmailAsync(guardianEmail, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في إرسال الإنذار الثاني: {ex.Message}");
            }
        }

        /// <summary>
        /// إرسال إعذار (غياب 17 يوم)
        /// </summary>
        public async Task SendExcuseLetterAsync(string guardianEmail, string studentName, string guardianName)
        {
            try
            {
                string subject = $"📋 إعذار - غياب {studentName}";
                string body = $@"
                السيد/السيدة: {guardianName}
                
                نتفيد حضرتكم بأن الطالب/الطالبة: {studentName}
                تغيب عن المؤسسة لمدة (17) يوم.
                
                يجب تقديم عذر رسمي أو سيتم اتخاذ إجراءات إدارية.
                
                مع أطيب التحيات
                إدارة المدرسة";

                await SendEmailAsync(guardianEmail, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في إرسال الإعذار: {ex.Message}");
            }
        }

        /// <summary>
        /// إرسال إشعار شطب (غياب 32 يوم)
        /// </summary>
        public async Task SendDelistingNotificationAsync(string guardianEmail, string studentName, string guardianName)
        {
            try
            {
                string subject = $"❌ إشعار شطب - {studentName}";
                string body = $@"
                السيد/السيدة: {guardianName}
                
                نتفيد حضرتكم بأن الطالب/الطالبة: {studentName}
                قد تم شطبه من قوائم المدرسة بسبب تجاوز حد الغياب المسموح (32) يوم.
                
                يمكن استئناف هذا القرار بتقديم طلب رسمي.
                
                مع أطيب التحيات
                إدارة المدرسة";

                await SendEmailAsync(guardianEmail, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في إرسال إشعار الشطب: {ex.Message}");
            }
        }

        /// <summary>
        /// إرسال بريد إلكتروني
        /// </summary>
        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using (SmtpClient client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_senderEmail, _senderPassword);

                    MailMessage message = new MailMessage
                    {
                        From = new MailAddress(_senderEmail),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false
                    };

                    message.To.Add(toEmail);

                    await client.SendMailAsync(message);
                    Console.WriteLine($"✅ تم إرسال الرسالة إلى: {toEmail}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في إرسال البريد: {ex.Message}");
            }
        }
    }
}