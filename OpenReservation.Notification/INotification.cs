namespace OpenReservation.Notification;

public interface INotification
{
    Task SendAsync(NotificationRequest request);
}