// SPDX-License-Identifier: AGPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Tayra Sakurai <tayra_sakurai@icloud.com>
using System;
using System.Collections.Generic;
using System.Text;

namespace Caesar.Models
{
    public class LargeCategory : Category
    {
        public ICollection<MediumCategory> MediumCategories { get; } = new HashSet<MediumCategory>();
    }
}
