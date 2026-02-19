using db.net.Blocks;

namespace db.net.Records;

interface IRecord {
    /// <summary>
        /// retrieve block
    /// </summary>
    IBlock block { get; }

    /// <summary>
        /// retrieve next record
    /// </summary>
    IBlock next { get; }

    /// <summary>
        /// set next node in linked list
    /// </summary>
    void Append(IBlock node);

    /// <summary>
        /// unlink next block to break the linked list
    /// </summary>
    void Break();

}
