using System.Buffers.Binary;
using db.net.StorageConstants;

namespace db.net.Blocks;

/*
	BlockHeader is a simple struct to represent each Block's header.
	It can turn itself into bytes so it can be stored on the Block, or take a buffer of bytes and reform the BlockHeader for usage.
*/

public struct BlockHeader {
	public uint Id { get; set; }
    public uint Offset { get; set; } // how deep the header is in the block
    public uint NextBlockId { get; set; } // 0 = no next block
	public uint UsedLength { get; set; } // usedlength represents how much of the block is used by the data

	public BlockHeader(uint id, uint offset, uint nextBlockId, uint usedLength) {
		this.Id = id;
        this.Offset = offset;
		this.NextBlockId = nextBlockId;
		this.UsedLength = usedLength;
	}

	// write given header to a buffer
	public void ToBuffer(byte[] buffer) {
		if(!ValidateBuffer(buffer))
			throw new ArgumentException("Buffer too small, each header is 12 bytes in size.");
		try {
			// uint == 4 bytes
			BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(), this.Id);
			BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(4), this.Offset);
            BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(8), this.NextBlockId);
			BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(12), this.UsedLength);
		} catch(Exception e) {
			Console.WriteLine(e);
			throw;
		}
	}

	// read a header from buffer and return it's BlockHeader version
	public static BlockHeader ToHeader(byte[] buffer) {
		if(!ValidateBuffer(buffer))
			throw new ArgumentException("Buffer too small, each header is 12 bytes in size.");
		try {
			// uint == 4 bytes
			var header = new BlockHeader() {
				Id = (uint)BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan()),
                Offset = (uint)BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(4)),
				NextBlockId = (uint)BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(8)),
				UsedLength = (uint)BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(12))
			};
			return header;
		} catch(Exception e) {
			Console.WriteLine(e);
			throw;
		}
	}

	public static bool ValidateBuffer(byte[] buffer) {
		if(buffer == null || buffer.Length < Size)
			return false;
		return true;
	}
}
