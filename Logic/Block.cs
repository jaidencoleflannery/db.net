using System;
using System.Collections.Generic;
using System.IO;
using System.Buffers.Binary;

namespace BlockStorage;

public class Block : IBlock {
	private readonly uint id;
	private readonly Stream stream;
	private readonly long[] headers = new long[5];
	private readonly BlockStorage storage;
	private readonly byte[] firstSector;

	private bool isDisposed = false;
	private bool isFirstSectorDirty = false;

	public event EventHandler disposed;

	public uint Id => id;

	public Block(BlockStorage storage, uint id, Stream stream, byte[] firstSector) {	
		if(stream == null) {
			throw new ArgumentNullException(nameof(stream));
		}
		if(firstSector.Length != storage.DiskSectorSize) {
			throw new ArgumentException($"firstSector must be of length {storage.DiskSectorSize}"); 
		}

		this.storage = storage;
		this.id = id;
		this.stream = stream;
		this.firstSector = firstSector;
	}

	public long GetHeader(int id) {
		if(id <= 0) {
			throw new ArgumentException(nameof(id));
		}
		if(stream == null) {
			throw new ArgumentNullException(nameof(stream);
		}

	
		// each header is a long type, which is 8 bytes of data 
		int position = (id * 8);
		byte[] buffer = new byte[8];

		this.stream.ReadExactly(buffer, 0, position);
		return BinaryPrimitives.ReadInt64LittleEndian(buffer);
	}

	public void setHeader(uint id, long value) {	
		if(id > 5) return ArgumentException("id must be <= 5 (maximum number of headers per block).");
		
		// each header is a long type, which is 8 bytes of data
		int position = (id * 8);
		byte[] buffer = new byte[8];

		BinaryPrimitives.WriteInt64LittleEndian(buffer, value);
		this.stream.WriteExactly(buffer, position, headerSize);
	}

}
