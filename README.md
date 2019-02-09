# Shipwreck.Collections

Provides helper methods for .NET Framework collections.

## BinarySearch

Provides `BinarySearch(T value, TComparer comparer = null)` and `BinarySearch(T value, int start, int length, TComparer comparer = null)` to `T[]`, `List<T>`, `Collection<T>` (and `IList<T>` and `IReadOnlyList<T>`).
And can support other list-like type by implementing `IBinarySearchable<T>`.

### Use Cases

- For list types that is not shipped with built in `BinarySearch`.
- Search `Array` or `List<T>` with custom `IComparer<T>`. (30%-50% faster with struct `IComparer<T>`)
- *NOTICE: If the `IComparer<T>.Compare(T, T)` is not so fast as mere arithmetics operation, the `Compare` invocation is likely to limit the whole binary search performance.*

## Usage

### NuGet

TBD

### Source code

You copy use each functionalities by embedding `.cs` files below:

- `BinarySearch.cs`

Following symbols are available.

- `SHIPWRECK_COLLECTIONS_PUBLIC`: Make helpers `public`.
- `SHIPWRECK_COLLECTIONS_NO_TARGETED_PATCHING_OPT_OUT`: Disable `[TargetedPatchingOptOutAttribute]` (it's not supported in netstandard 1.x).
- `SHIPWRECK_COLLECTIONS_NO_ARRAY`: Omit specialized extension methods for typed array.
- `SHIPWRECK_COLLECTIONS_NO_LIST`: Omit specialized extension methods for `List<T>`.
- `SHIPWRECK_COLLECTIONS_NO_COLLECTION`: Omit specialized extension methods for `Collection<T>`.
- `SHIPWRECK_COLLECTIONS_NO_ILIST`: Omit specialized extension methods for `IList<T>`.
- `SHIPWRECK_COLLECTIONS_NO_IREADONLYLIST`: Omit specialized extension methods for `IReadOnlyList<T>`.

## License

MIT