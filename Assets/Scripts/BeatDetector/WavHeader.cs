// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
namespace BeatProcessor
{
	public struct WavHeader
	{
		public byte[] riffID; // "riff"
		public uint size;  
		public byte[] wavID;  // "WAVE"
		public byte[] fmtID;  // "fmt "
		public uint fmtSize;
		public ushort format;
		public ushort channels;
		public uint sampleRate;
		public uint bytePerSec;
		public ushort blockSize;
		public ushort bit;
		public byte[] dataID; // "data"
		public uint dataSize;
	}
}

