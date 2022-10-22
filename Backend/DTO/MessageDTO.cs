using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.DTO;

public class MessageDTO
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public string SenderUsername { get; set; }
    public string SenderPhotoUrl { get; set; }
    public int RecipientId { get; set; }
    public string RecipientUsername { get; set; }
    public string RecipientPhotoUrl { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }

}

public class MessageToCreateDTO
{
    public string RecipientUsername { get; set; }
    public string Content { get; set; }

}