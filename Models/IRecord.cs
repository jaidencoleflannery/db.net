using db.net.Blocks;
using db.net.Records;

namespace db.net.Records;

public interface IRecord {
    /// <summary>
        /// set next node in linked list
    /// </summary>
    void Append(IBlock node);

}
