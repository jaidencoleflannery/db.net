namespace db.net.StorageConstants;

public static class Storage {
    public const int BlockSize = 4096; // 4096b == 4kb
    public const int HeaderSize = 16; // 4 * 4b (uint) = 16b
    // most modern os will read / write at 4kb so we never need to go over that.
    public const int ContentSize = BlockSize - HeaderSize;
    
       
}
