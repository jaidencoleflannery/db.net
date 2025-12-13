namespace Models.DbBlockStorage;

public interface IDbBlockStorage {

    /// <summary>
        /// retrieve total size of block
    /// </summary>
    public BlockSize {
        get;
    }

    /// <summary>
        /// retrieve size of block header
    /// </summary>
    public HeaderSize {
        get;
    }

    /// <summary>
        /// retrieve size of block's contents
    /// </summary>
    public BlockContentSize {
        get;
    } 

    /// <summary>
        /// find a specific block based on block's unique identifier {id}
    /// </summary>
    /// <returns>
        /// returns the specified Block object
    /// </returns>
    public IBlock Find(uint id);

    /// <summary>
        /// create a new block of memory
    /// </summary>
    /// <returns>
        /// returns the generated Block object
    /// </returns>
    public IBlock CreateNew();

}