using Microsoft.AspNetCore.SignalR;

namespace Assignment1_EventSignup.Hubs
{
    public class EventHub : Hub
    {
        // Called by the JS client when a user opens an event's detail page.
        // Adds their connection to a group named "event-{eventId}" so they
        // receive live updates for that specific event.
        public async Task JoinEventGroup(int eventId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"event-{eventId}");
        }

        public async Task LeaveEventGroup(int eventId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"event-{eventId}");
        }
    }
}