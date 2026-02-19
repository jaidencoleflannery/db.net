using System.Buffers.Binary;

namespace db.net.Blocks;

public readonly struct BlockHeader {
	public const uint Size = 12; // 3 * uint
	public uint Id { get; }
	public uint NextBlockId { get; } // 0 = no next block
	public uint UsedLength { get; }

	public BlockHeader(uint id, uint nextBlockId, uint usedLength) {
		this.Id = id;
		this.NextBlockId = nextBlockId;
		this.UsedLength = usedLength;
	}

	public static void Write(byte[] buffer, int offset, BlockHeader header) {
		
		try {
			BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(offset), header.Id);
			BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(offset + 4), header.NextBlockId);
			BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(offset + 8), header.UsedLength);
		} catch(Exception e) {
			Console.WriteLine(e);
		}
	}

	public static byte[] Read(byte[] buffer, int offset) {
		validateBuffer(buffer, offset);
		try {
			uint id = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(offset));
			uint nextBlockId = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(offset + 4));
			uint usedLength = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(offset + 8));
		} catch(Exception e) {
			Console.WriteLine(e);
		}
		return buffer;
	}

	public static void validateBuffer(byte[] buffer, int offset) {
		if(buffer == null || buffer.Length < offset + Size)
			throw new ArgumentException("Buffer size incorrect.");
	}
}