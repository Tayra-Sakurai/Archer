using Caesar.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caesar.Messages
{
    public class PaymentMethodInvokedMessage : ValueChangedMessage<PaymentMethod>
    {
        public PaymentMethodInvokedMessage(PaymentMethod value) : base(value)
        {
        }
    }
}
