// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Caesar.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Item> Items { get; } = new HashSet<Item>();
        public float[] Vector { get; set; } = new float[Constants.DIMENSIONS];
        public double Remainder => Items.Sum(item => item.Income - item.Expense);
    }
}
