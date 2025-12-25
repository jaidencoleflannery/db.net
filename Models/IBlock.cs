using System;

namespace BlockStorage;

public interface IBlock : IDisposable {
    /// <summary>
    /// unique block identifier
    /// </summary>
    uint Id { get; }

    /// <summary>
    /// a block may contain >= 1 header
    /// get value of a specific header (identified by a number and 8 bytes of data)
    /// </summary>
    long GetHeader (int id);

    /// <summary>
    /// change value of a specific header to {value};
    /// </summary>
    void SetHeader (int id, long value);

    /// <summary>
    /// write to {buffer} contents at offset position {offset} from {srcOffset} location
    /// </summary>
    void Read (byte[] buffer, int offset, int srcOffset, int count);

    /// <summary>
    /// write {buffer} contents into {offset} location
    /// </summary>
    void Write (byte[] buffer, int offset, int srcOffset, int count);
}
