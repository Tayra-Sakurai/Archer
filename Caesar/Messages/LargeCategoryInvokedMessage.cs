// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using Caesar.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caesar.Messages
{
    public class LargeCategoryInvokedMessage : ValueChangedMessage<LargeCategory>
    {
        public LargeCategoryInvokedMessage(LargeCategory category)
            : base(category) { }
    }
}
