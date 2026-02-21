namespace db.net.StorageConstants;

public static class Storage {
    public const int BlockSize = 4096; // 4096b == 4kb
    public const int HeaderSize = 12; // 3 * 4b (uint) = 12b
    public const int MaxHeaders = 8; // 3 * 4b (num of uints) * 8 (num of headers) = 96b
    public const int UnitOfWork = (BlockSize >= 4096) ? 4096 : 128; // this will practically always be 4kb, but if it is edited to be smaller we want to use the minimum. 
    // most modern os will read / write at 4kb so we never need to go over that.
    public const int ContentSize = BlockSize - HeaderSize;
    
       
}
