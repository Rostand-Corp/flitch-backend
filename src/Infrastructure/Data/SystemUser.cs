using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data;

public class SystemUser : IdentityUser<Guid>
{
    public string FullName { get; set; }
    public Guid? MessengerUserId { get; set; }
    public User? MessengerUser { get; set; } // A link to the domain entity of the messenger subdomain.
                                            // If there were more subdomains (subsystems), it would have been more proper
                                            // to store a list of entries in those subsystems:
                                            // [ {Id: "123", Subsystem: "Messenger"}, {Id: "ab111", Subsystem: "Market"}, ... ]
                                            // which is definitely a case for microservice arch / decentralized auth server
}