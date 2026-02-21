using db.net.StorageConstants;

namespace db.net.Data;

public static class DataService {

    // section data into a list of {contentSize} sections
    public static List<byte[]> Partition(byte[] data) {
        int contentSize = Storage.ContentSize;
        var sections = new List<byte[]>();
        int offset = 0;
        while(offset < data.Length) {
            int size = Math.Min(contentSize, data.Length - offset);
            var chunk = new byte[contentSize];
            Buffer.BlockCopy(data, offset, chunk, 0, size);
            sections.Add(chunk);
            offset += size;
        }
        return sections;
    }
}
