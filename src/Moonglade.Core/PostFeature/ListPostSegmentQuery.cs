﻿using MoongladePure.Data.Spec;
using System.Linq.Expressions;

namespace MoongladePure.Core.PostFeature;

public class ListPostSegmentQuery(PostStatus postStatus, int offset, int pageSize, string keyword = null)
    : IRequest<(IReadOnlyList<PostSegment> Posts, int TotalRows)>
{
    public PostStatus PostStatus { get; set; } = postStatus;

    public int Offset { get; set; } = offset;

    public int PageSize { get; set; } = pageSize;

    public string Keyword { get; set; } = keyword;
}

public class ListPostSegmentQueryHandler(IRepository<PostEntity> repo)
    : IRequestHandler<ListPostSegmentQuery, (IReadOnlyList<PostSegment> Posts, int TotalRows)>
{
    public async Task<(IReadOnlyList<PostSegment> Posts, int TotalRows)> Handle(ListPostSegmentQuery request, CancellationToken ct)
    {
        if (request.PageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(request.PageSize),
                $"{nameof(request.PageSize)} can not be less than 1, current value: {request.PageSize}.");
        }
        if (request.Offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.Offset),
                $"{nameof(request.Offset)} can not be less than 0, current value: {request.Offset}.");
        }

        var spec = new PostPagingSpec(request.PostStatus, request.Keyword, request.PageSize, request.Offset);
        var posts = await repo.SelectAsync(spec, PostSegment.EntitySelector);

        Expression<Func<PostEntity, bool>> countExp = p => null == request.Keyword || p.Title.Contains(request.Keyword);

        countExp = request.PostStatus switch
        {
            PostStatus.Draft => countExp.AndAlso(p => !p.IsPublished && !p.IsDeleted),
            PostStatus.Published => countExp.AndAlso(p => p.IsPublished && !p.IsDeleted),
            PostStatus.Deleted => countExp.AndAlso(p => p.IsDeleted),
            PostStatus.Default => countExp.AndAlso(p => true),
            _ => throw new ArgumentOutOfRangeException(nameof(request.PostStatus), request.PostStatus, null),
        };

        var totalRows = await repo.CountAsync(countExp, ct);
        return (posts, totalRows);
    }
}