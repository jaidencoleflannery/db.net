namespace db.net.RecordStorage;

public interface IRecordStorage {
    /// <summary>
        /// get the record's first block's unique identifier
    /// </summary>
    public Id { get; }

    /// <summary>
        /// retrieve data from a specified block of data within the record instance
    /// </summary>
    byte[] Find(uint id);

    /// <summary>
        /// update a specific block of data within the record instance
    /// </summary>
    void Update(uint id, byte[] data);

    /// <summary>
        /// delete a specific block of data  within the record instance
    /// </summary>
    void Delete(uint id);

    /// <summary>
        /// create an empty record
    /// </summary>
    /// <returns>
        /// returns the generated id (based on the first unique identifier of leading block)
    /// </returns>
    uint Create(Func<uint, byte[]> generate);

    /// <summary>
        /// create a record with a built in data generator for extending the record instance
    /// </summary>
    /// <returns>
        /// returns the generated id (based on the first unique identifier of leading block)
    /// </returns>
    uint Create(Func<uint, byte[]> generate);

}
