using FaceLook.Enums;
﻿namespace FaceLook.Data.Entities
{
    public class Message : BaseEntity
    {
        public required string Content { get; set; }
        public required MessageStatus MessageStatus { get; set; }
    }
}
