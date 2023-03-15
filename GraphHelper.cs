using Azure.Identity;
using Microsoft.Graph;



public class GraphHelper
{
    // Settings object
    private static Settings? _settings;
    // App-ony auth token credential
    private static ClientSecretCredential? _clientSecretCredential;
    // Client configured with app-only authentication
    private static GraphServiceClient? _appClient;
    private static string? _senderId;
    public static void InitializeGraphForAppOnlyAuth(Settings settings)
    {
        _settings = settings;

        // Ensure settings isn't null
        _ = settings ?? throw new System.NullReferenceException("Settings cannot be null");

        _settings = settings;

        _senderId = _settings.SenderId;
        try
        {
            if (_clientSecretCredential == null)
            {
                _clientSecretCredential = new ClientSecretCredential(
                    _settings.TenantId, _settings.ClientId, _settings.ClientSecret);
            }

            if (_appClient == null)
            {
                _appClient = new GraphServiceClient(_clientSecretCredential,
                new[] { "https://graph.microsoft.com/.default" });
                Console.WriteLine("Graph Client Created");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Something Went Wrong While Getting GraphClient:");
            Console.WriteLine(ex);
            throw;
        }
    }
    public static async Task<List<User>> GetUsersIds()
    {
        List<User> usersIdList = new List<User>();
        try
        {
            if (_appClient != null)
            {
                var userPage = await _appClient.Users.Request().Select(
                    u => new
                    {
                        u.Id
                    }
                ).GetAsync();

                usersIdList.AddRange(userPage.CurrentPage);
                while (userPage.NextPageRequest != null)
                {
                    userPage = await userPage.NextPageRequest.GetAsync();
                    usersIdList.AddRange(userPage.CurrentPage);
                }
                return usersIdList;
            }
            else
            {
                Console.WriteLine("Graph Client not initialized");
                throw new Exception("Graph Client not initialized");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Something Went Wrong While Getting Users");
            Console.WriteLine(ex);
            throw;
        }
    }
    public static async Task<List<User>> GetUserBirthday(List<User> users)
    {
        List<User> usersList = new List<User>();
        try
        {
            var today = DateTime.Now;
            var day = today.Day;
            var month = today.Month;
            foreach (var item in users)
            {
                var user = await _appClient!.Users[item.Id].Request().Select(u => new { u.DisplayName, u.Mail, u.Birthday }).GetAsync();
                user.Id = item.Id;
                var userBday = user.Birthday.GetValueOrDefault();
                var userDay = userBday.Day;
                var userMonth = userBday.Month;
                if (day == userDay && month == userMonth)
                {
                    usersList.Add(user);
                }
            }
            return usersList;
        }
        catch (Exception ex)
        {
            Console.WriteLine("something went wrong while fetching user detail");
            Console.WriteLine(ex.Message);
            throw;
        }
    }
    public static async Task SendUserEmail(User user)
    {

        var body = System.IO.File.ReadAllText("EmailTemplate\\index.html")
                    .Replace("{{DisplayName}}", user.DisplayName);
        var mesg = new Message
        {
            Subject = $"Happy Birthday! {user.DisplayName}",
            Body = new ItemBody
            {
                ContentType = BodyType.Html,
                Content = body
            },
            ToRecipients = new List<Recipient>
            {
                new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        //who will receive the email
                        Address = $"{user.Mail}"
                    }
                }
            },
            Attachments = new MessageAttachmentsCollectionPage()
        };
        await _appClient!.Users[_senderId].SendMail(mesg, false).Request().PostAsync();
    }
}