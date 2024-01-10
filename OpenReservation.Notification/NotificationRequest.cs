using System.ComponentModel.DataAnnotations;

namespace OpenReservation.Notification;

public sealed class NotificationRequest
{
    public string? MsgId { get; set; }
    public string? Signature { get; set; }
    [Required]
    public required string Text { get; set; }

    public string? TimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
    public string? To { get; set; }

    public string GetMessage()
    {
        if (string.IsNullOrEmpty(Signature))
        {
            return Text;
        }
        
        return TimeFormat is null 
            ? $"{Text}\n[{Signature}]" 
            : $"{Text}\n[{Signature}] {DateTime.UtcNow.AddHours(8).ToString(TimeFormat)}"
            ;
    }
}