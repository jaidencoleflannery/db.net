namespace db.net.Index;

public interface IIndex<K, V> {
    /// <summary>
        /// inserts a new index with its related value
    /// </summary>
    void Insert(K key, V value);

    /// <summary>
        /// gets the value related to a specified key
    /// </summary>
    Tuple<K, V> Get(K key);

    /// <summary>
        /// gives all entries that are larger than or equal to the specified key
    /// </summary>
    IEnumerable<Tuple<K, V>> LargerThanOrEqualTo(K key);

    /// <summary>
        /// gives all entries that are larger than or equal to the specified key
    /// </summary>
    IEnumerable<Tuple<K, V>> LargerThan(K key);

    /// <summary>
        /// gives all entries that are less than or equal to the specified key
    /// </summary>
    IEnumerable<Tuple<K, V>> LessThanOrEqualTo(K key);

    /// <summary>
        /// gives all entries that are less then a specified key
    /// </summary>
    IEnumerable<Tuple<K, V>> LessThan(K key);

    /// <summary>
        /// delete a specific index and optionally use a comparer to compare values 
    /// </summary>
    bool Delete(K key, V value, IComparer<V> comparer = null);

    /// <summary>
        /// delete a specific index
    /// </summary>
    bool Delete(K key);
}
