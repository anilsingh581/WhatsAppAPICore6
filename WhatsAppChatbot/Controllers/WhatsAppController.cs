using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

[Route("api/[controller]")]
[ApiController]
public class WhatsAppController : ControllerBase
{
    private readonly ChatbotContext _context;

    public WhatsAppController(ChatbotContext context)
    {
        _context = context;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] WhatsAppMessage message)
    {
        var userMessage = new UserMessage
        {
            UserId = message.From,
            Message = message.Body,
            Timestamp = DateTime.UtcNow
        };

        var abc = _context.UserMessages.FirstOrDefault(x => x.UserId == userMessage.UserId);

        _context.UserMessages.Add(userMessage);
        await _context.SaveChangesAsync();

        var response = GenerateResponse(userMessage);

        var botResponse = new BotResponse
        {
            UserMessageId = userMessage.Id,
            Response = response,
            Timestamp = DateTime.UtcNow
        };

        _context.BotResponses.Add(botResponse);
        await _context.SaveChangesAsync();

        var result = await SendMessageToWhatsApp(message.From, response);

        return Ok(result);
    }

    private string GenerateResponse(UserMessage userMessage)
    {
        // Implement your bot's logic here
        return "This is a response to your message: " + userMessage.Message;
    }

    private async Task<ContentResult> SendMessageToWhatsApp(string to, string message)
    {
        dynamic finalResult =null;
        string SendMessagesAPIPath = "https://graph.facebook.com/v20.0/397043536824412/messages";

        // Create an instance of the Message boady. It is predefined body
        var messageBody = new
        {
            messaging_product = "whatsapp",
            to = "91" + to,
            type = "text",
            text = new
            {
                body = "API messages : " + message
            }
        };


        // Use HttpClient or any other method to send the message via WhatsApp API
        using (var httpClient = new HttpClient())
        {
            var authheader = new AuthenticationHeaderValue("Bearer", "EAAQI9IjUnZB4BOyQuNnCMbCmvIrAzV6rG28SXzVZCcxMIGSADgckBg1hlBlt5ZAXXy0wmZBikPyx9vuZC9Jfg4ISwkKUJJePfhyKdwviGCrFWBOl9P4bTngB2dZANCjK4TaAjVZA5Y06fE6NrHRW4iRVvSEVYZAmr30QzZBV22EWleE0zwVe5AsfBvZCnVr9YgbsffZAOi59L0dDLlomnppEogZD");
            httpClient.DefaultRequestHeaders.Authorization = authheader;
            var response = await httpClient.PostAsJsonAsync(SendMessagesAPIPath, messageBody);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                finalResult = new ContentResult
                {
                    Content = result,
                    ContentType = "application/json",
                    StatusCode = (int)response.StatusCode
                };

                return finalResult;
            }
        }

        return finalResult;
    }

    
}

public class WhatsAppMessage
{
    public string From { get; set; }
    public string Body { get; set; }
}


public class Text
{
    public string body { get; set; }
}

public class Message
{
    public string messaging_product { get; set; }
    public string to { get; set; }
    public string type { get; set; }
    public Text text { get; set; }
}