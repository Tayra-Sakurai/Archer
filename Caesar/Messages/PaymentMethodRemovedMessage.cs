using Caesar.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caesar.Messages
{
    public class PaymentMethodRemovedMessage : ValueChangedMessage<PaymentMethod>
    {
        public PaymentMethodRemovedMessage(PaymentMethod value) : base(value)
        {
        }
    }
}
