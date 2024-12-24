using Microsoft.AspNetCore.SignalR;

public class NewsHub : Hub
{
    public async Task SendNewsNotification(string title, int newsId)
    {
        // Admin ve Gazetecilere ayrı ayrı bildirim gönder
        await Clients.Groups(new List<string> { "Admins", "Journalists" })
            .SendAsync("ReceiveNewsNotification", title, newsId);
    }

    public async Task SendApprovalNotification(string title, string status, string userId)
    {
        // Spesifik kullanıcıya bildirim gönder
        await Clients.User(userId).SendAsync("ReceiveApprovalNotification", title, status);
    }

    public override async Task OnConnectedAsync()
    {
        // Kullanıcının rolüne göre gruba ekle
        if (Context.User.IsInRole("Admin"))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }
        else if (Context.User.IsInRole("Gazetici"))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Journalists");
        }
        await base.OnConnectedAsync();
    }
} 