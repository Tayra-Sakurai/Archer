using Caesar.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caesar.Messages
{
    public class SmallCategoryRemovedMessage : ValueChangedMessage<SmallCategory>
    {
        public SmallCategoryRemovedMessage(SmallCategory value)
            : base(value)
        {
        }
    }
}
