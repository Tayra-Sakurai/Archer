// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using System;
using System.Collections.Generic;
using System.Text;

namespace Caesar.Models
{
    public class SmallCategory : Category
    {
        public int MediumCategoryId { get; set; }
        public MediumCategory? MediumCategory { get; set; }
        public ICollection<Item> Items { get; } = new HashSet<Item>();
    }
}
