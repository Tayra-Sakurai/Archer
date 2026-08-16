// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using System;
using System.Collections.Generic;
using System.Text;

namespace Caesar.Models
{
    public class MediumCategory : Category
    {
        public int LargeCategoryId { get; set; }
        public LargeCategory? LargeCategory { get; set; }
    }
}
