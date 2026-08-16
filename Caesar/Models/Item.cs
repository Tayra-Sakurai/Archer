// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using System;
using System.Collections.Generic;
using System.Text;

namespace Caesar.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTimeOffset TimeTrade { get; set; } = DateTimeOffset.Now;
        public int SmallCategoryId { get; set; }
        public SmallCategory? SmallCategory { get; set; }
        public int PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public double Income { get; set; } = 0;
        public double Expense { get; set; } = 0;
        public float[] Vector { get; set; } = new float[Constants.DIMENSIONS];
    }
}
