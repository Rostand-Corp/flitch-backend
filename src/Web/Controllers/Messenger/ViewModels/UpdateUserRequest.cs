using System.ComponentModel.DataAnnotations;

namespace Web.Controllers.Messenger.ViewModels;

public record UpdateUserRequest([MaxLength(16)] string DisplayName, [MaxLength(128)] string FullName, string Status); // todo: y record