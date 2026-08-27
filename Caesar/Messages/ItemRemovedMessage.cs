using Caesar.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caesar.Messages
{
    public class ItemRemovedMessage : ValueChangedMessage<Item>
    {
        public ItemRemovedMessage(Item item)
            : base(item) { }
    }
}
