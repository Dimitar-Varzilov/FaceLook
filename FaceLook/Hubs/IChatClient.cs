namespace FaceLook.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(string sender, string message);
    }
}
