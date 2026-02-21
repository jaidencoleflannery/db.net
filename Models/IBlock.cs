namespace db.net.Blocks;

public interface IBlock : IDisposable { 
    /// <summary>
    /// a block may contain >= 1 header
    /// get value of a specific header (identified by a number and 8 bytes of data)
    /// </summary>
    BlockHeader GetHeader (int id);

    /// <summary>
    /// change value of a specific header to {value};
    /// </summary>
    void SetHeader (uint id, BlockHeader header);

    /// <summary>
    /// write to {buffer} contents at offset position {offset} from {srcOffset} location
    /// </summary>
    void Read (byte[] buffer, int offset, int headerOffset, int count);

    /// <summary>
    /// write {buffer} contents into {offset} location
    /// </summary>
    void Write (byte[] buffer, int offset, int srcOffset, int count);
}
