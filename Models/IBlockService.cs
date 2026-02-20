namespace db.net.Blocks;

public interface IBlockService {

    /// <summary>
        /// retrieve total size of block
    /// </summary>
    int BlockSize { get; }

    /// <summary>
        /// retrieve size of block header
    /// </summary>
    int HeaderSize { get; }

    /// <summary>
        /// retrieve size of block's contents
    /// </summary>
    int ContentSize { get; } 

    /// <summary>
        /// find a specific block based on block's unique identifier {id}
    /// </summary>
    /// <returns>
        /// returns the specified Block object
    /// </returns>
    IBlock Find(uint id);

    /// <summary>
        /// create a new block of memory
    /// </summary>
    /// <returns>
        /// returns the generated Block object
    /// </returns>
    IBlock Create(byte[]? data);

}
