using System.Text.Json.Serialization;

namespace Health.Application.Common;

public sealed class KeysetPagedResult<T>
{
    #region Properties

    public IReadOnlyList<T> Items { get; init; } = [];
    public int PageSize { get; init; }
    public Guid? NextKey { get; init; }

    #endregion

    #region Getters

    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
    public int Count => Items.Count;

    #endregion

    #region Constructors

    [JsonConstructor]
    private KeysetPagedResult() { }

    #endregion

    #region Factory Methods

    public static KeysetPagedResult<T> Create(
        List<T> items,
        int pageSize,
        Func<T, Guid> keySelector,
        bool hasPreviousPage)
    {
        var hasNextPage = items.Count > pageSize;
        if (hasNextPage) items.RemoveAt(items.Count - 1);

        Guid? nextKey = hasNextPage ? keySelector(items[^1]) : null;
        return new KeysetPagedResult<T>
        {
            Items = items.AsReadOnly(),
            PageSize = pageSize,
            NextKey = nextKey,
            HasNextPage = hasNextPage,
            HasPreviousPage = hasPreviousPage
        };
    }

    #endregion
}