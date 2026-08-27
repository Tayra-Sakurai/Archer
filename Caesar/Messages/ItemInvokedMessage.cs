using Caesar.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caesar.Messages
{
    public class ItemInvokedMessage : ValueChangedMessage<Item>
    {
        public ItemInvokedMessage(Item item) : base(item) { }
    }
}
