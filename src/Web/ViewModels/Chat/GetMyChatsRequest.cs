using System.ComponentModel.DataAnnotations;
using Application.Chats.Commands;

namespace Web.ViewModels.Chat;

public class GetMyChatsRequest
{
    /// <summary>
    /// Page number of the results to return (1-any). If not set, then return the first page.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int? PageNumber { get; set; }
    
    /// <summary>
    /// Amount of records to return (1-50). If not set, then return max possible value.
    /// </summary>
    [Range(1, MaxAmount)]
    public int? Amount { get; set; } = 50;
    
    /// <summary>
    /// Filter to search chats with. Values: all, private.
    /// </summary>
    public ChatTypeFilter Filter { get; set; }

    public const int MaxAmount = 50;
}