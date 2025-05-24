﻿using MoongladePure.Data.Entities;
using MoongladePure.Data.Infrastructure;

namespace MoongladePure.Data.Spec;

public sealed class TagSpec : BaseSpecification<TagEntity>
{
    public TagSpec(int top) : base(t => true)
    {
        ApplyPaging(0, top);
        ApplyOrderByDescending(p => p.Posts.Count);
    }

    public TagSpec(string normalizedName) : base(t => t.NormalizedName == normalizedName)
    {

    }
}