using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Chat;

public class GetChatMessagesRequest
{
    /// <summary>
    /// Page number of the results to return (1-any). If not set, then return the first page. Cannot be applied with 'Before'.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int? PageNumber { get; set; }

    /// <summary>
    /// Amount of records to return (1-50). If not set, then return max possible value.
    /// </summary>
    [Range(1, MaxAmount)]
    public int? Amount { get; set; } = MaxAmount;
    
    /// <summary>
    /// Key word to search for in messages' content. Max length - 50. If not set, searches all the records. Cannot be applied with 'Before'.
    /// </summary>
    [MaxLength(50)]
    public string? SearchKeyWord { get; set; }
    
    /// <summary>
    /// Message id to retrieve messages prior to. Also affected by Amount parameter.
    /// </summary>
    public Guid? Before { get; set; }

    public const int MaxAmount = 50;
}