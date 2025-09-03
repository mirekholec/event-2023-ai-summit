using System.Text;
using GptConsole;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

const double temperature = 0.8;
const double topP = 0.7;
const int conversationlength = 4;

var configuration = Configuration.Resolve();

List<ChatMessage> messages = new()
{
};

Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.WriteLine($"ChatGPT 4.0; Temp: {temperature} / TopP: {topP}");
Console.Write("Use on your own risk. Tool created by ");
Console.ForegroundColor = ConsoleColor.DarkRed;
Console.WriteLine("https://www.holec.ai");
Console.ResetColor();
Console.WriteLine();
while (true)
{
    Console.ForegroundColor = ConsoleColor.Blue;
    string question = Console.ReadLine();
    Console.ResetColor();
    messages.Add(new ChatMessage(ChatMessageRole.User, question));

    var ai = new OpenAIAPI(new APIAuthentication(configuration.ApiKey, configuration.OrganisationId));

    ChatRequest request = new ChatRequest
    {
        user = "Holec",
        Model = Model.GPT4,
        Messages = messages.Skip(messages.Count - conversationlength).ToList(),
        Temperature = temperature,
        TopP = topP
    };

    try
    {
        IAsyncEnumerable<ChatResult> response = ai.Chat.StreamChatEnumerableAsync(request);

        StringBuilder sb = new();
        await foreach (ChatResult item in response)
        {
            string delta = item.Choices.FirstOrDefault().Delta.Content;
            Console.Write(delta);
            sb.Append(delta);
        }

        messages.Add(new ChatMessage(ChatMessageRole.Assistant, sb.ToString()));
    }
    catch (System.Security.Authentication.AuthenticationException)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Autentizační údaje k API nejsou správné.");
        Configuration.Clear();
        Environment.Exit(0);
    }
    catch (Exception e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Došlo k neočekávané chybě. {e.Message}");
        Environment.Exit(0);
    }

    Console.WriteLine();
    Console.WriteLine();
}