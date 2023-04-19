using System.ComponentModel.DataAnnotations;

namespace Web.Controllers.Messenger.Chat.ViewModels;

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
    /// Key word to search for. Max length - 50. If not set, searches all the records.
    /// </summary>
    [MaxLength(50)]
    public string? SearchKeyWord { get; set; }

    public const int MaxAmount = 50;
}