using System;

namespace UK101Library
{
    public class MON01 : MemoryBusDevice
    {
		#region Fields

		#endregion
		#region Contructor

		public MON01(ushort Address)
		{
			this.Address = Address;
			ReadOnly = true;

			pData = new byte[] {
				0xa0, 0x00, 0x8c, 0x01, 0xc0, 0x8c, 0x00, 0xc0, 0xa2, 0x04, 0x8e, 0x01, 0xc0, 0x8c, 0x03, 0xc0,
				0x88, 0x8c, 0x02, 0xc0, 0x8e, 0x03, 0xc0, 0x8c, 0x02, 0xc0, 0xa9, 0xfb, 0xd0, 0x09, 0xa9, 0x02,
				0x2c, 0x00, 0xc0, 0xf0, 0x1c, 0xa9, 0xff, 0x8d, 0x02, 0xc0, 0x20, 0x99, 0xff, 0x29, 0xf7, 0x8d,
				0x02, 0xc0, 0x20, 0x99, 0xff, 0x09, 0x08, 0x8d, 0x02, 0xc0, 0xa2, 0x18, 0x20, 0x85, 0xff, 0xf0,
				0xdd, 0xa2, 0x7f, 0x8e, 0x02, 0xc0, 0x20, 0x85, 0xff, 0xad, 0x00, 0xc0, 0x30, 0xfb, 0xad, 0x00,
				0xc0, 0x10, 0xfb, 0xa9, 0x03, 0x8d, 0x10, 0xc0, 0xa9, 0x58, 0x8d, 0x10, 0xc0, 0x20, 0x90, 0xff,
				0x85, 0xfe, 0xaa, 0x20, 0x90, 0xff, 0x85, 0xfd, 0x20, 0x90, 0xff, 0x85, 0xff, 0xa0, 0x00, 0x20,
				0x90, 0xff, 0x91, 0xfd, 0xc8, 0xd0, 0xf8, 0xe6, 0xfe, 0xc6, 0xff, 0xd0, 0xf2, 0x86, 0xfe, 0xa9,
				0xff, 0x8d, 0x02, 0xc0, 0x60, 0xa0, 0xf8, 0x88, 0xd0, 0xfd, 0x55, 0xff, 0xca, 0xd0, 0xf6, 0x60,
				0xad, 0x10, 0xc0, 0x4a, 0x90, 0xfa, 0xad, 0x11, 0xc0, 0x60, 0x48, 0x2f, 0x44, 0x2f, 0x4d, 0x3f,
				0xd8, 0xa2, 0xd8, 0xa9, 0xd0, 0x85, 0xfe, 0xa0, 0x00, 0x84, 0xfd, 0xa9, 0x20, 0x91, 0xfd, 0xc8,
				0xd0, 0xfb, 0xe6, 0xfe, 0xe4, 0xfe, 0xd0, 0xf5, 0xa9, 0x03, 0x8d, 0x00, 0xfc, 0xa9, 0xb1, 0x8d,
				0x00, 0xfc, 0xb9, 0x9a, 0xff, 0x30, 0x0e, 0x99, 0xc6, 0xd0, 0xae, 0x01, 0xfe, 0xd0, 0x03, 0x20,
				0x0b, 0xfe, 0xc8, 0xd0, 0xed, 0xad, 0x01, 0xfe, 0xd0, 0x05, 0x20, 0x00, 0xfe, 0xb0, 0x03, 0x20,
				0xed, 0xfe, 0xc9, 0x48, 0xf0, 0x0a, 0xc9, 0x44, 0xd0, 0x0c, 0x20, 0x00, 0xff, 0x4c, 0x00, 0x22,
				0x4c, 0x00, 0xfd, 0x20, 0x00, 0xff, 0x6c, 0xfc, 0xfe, 0xea, 0x30, 0x01, 0xa0, 0xff, 0xc0, 0x01,
				0x8a, 0x48, 0x98, 0x48, 0xa9, 0x01, 0x8d, 0x00, 0xdf, 0xae, 0x00, 0xdf, 0xd0, 0x05, 0x0a, 0xd0,
				0xf5, 0xf0, 0x53, 0x4a, 0x90, 0x09, 0x2a, 0xe0, 0x21, 0xd0, 0xf3, 0xa9, 0x1b, 0xd0, 0x21, 0x20,
				0xc8, 0xfd, 0x98, 0x8d, 0x13, 0x02, 0x0a, 0x0a, 0x0a, 0x38, 0xed, 0x13, 0x02, 0x8d, 0x13, 0x02,
				0x8a, 0x4a, 0x20, 0xc8, 0xfd, 0xd0, 0x2f, 0x18, 0x98, 0x6d, 0x13, 0x02, 0xa8, 0xb9, 0xcf, 0xfd,
				0xcd, 0x15, 0x02, 0xd0, 0x26, 0xce, 0x14, 0x02, 0xf0, 0x2b, 0xa0, 0x05, 0xa2, 0xc8, 0xca, 0xd0,
				0xfd, 0x88, 0xd0, 0xf8, 0xf0, 0xae, 0xc9, 0x01, 0xf0, 0x35, 0xa0, 0x00, 0xc9, 0x02, 0xf0, 0x47,
				0xa0, 0xc0, 0xc9, 0x20, 0xf0, 0x41, 0xa9, 0x00, 0x8d, 0x16, 0x02, 0x8d, 0x15, 0x02, 0xa9, 0x02,
				0x8d, 0x14, 0x02, 0xd0, 0x8f, 0xa2, 0x96, 0xcd, 0x16, 0x02, 0xd0, 0x02, 0xa2, 0x14, 0x8e, 0x14,
				0x02, 0x8d, 0x16, 0x02, 0xa9, 0x01, 0x8d, 0x00, 0xdf, 0xad, 0x00, 0xdf, 0x4a, 0x90, 0x33, 0xaa,
				0x29, 0x03, 0xf0, 0x0b, 0xa0, 0x10, 0xad, 0x15, 0x02, 0x10, 0x0c, 0xa0, 0xf0, 0xd0, 0x08, 0xa0,
				0x00, 0xe0, 0x20, 0xd0, 0x02, 0xa0, 0xc0, 0xad, 0x15, 0x02, 0x29, 0x7f, 0xc9, 0x20, 0xf0, 0x07,
				0x8c, 0x13, 0x02, 0x18, 0x6d, 0x13, 0x02, 0x8d, 0x13, 0x02, 0x68, 0xa8, 0x68, 0xaa, 0xad, 0x13,
				0x02, 0x60, 0xd0, 0x92, 0xa0, 0x20, 0xd0, 0xdf, 0xa0, 0x08, 0x88, 0x0a, 0x90, 0xfc, 0x60, 0xd0,
				0xbb, 0x2f, 0x20, 0x5a, 0x41, 0x51, 0x2c, 0x4d, 0x4e, 0x42, 0x56, 0x43, 0x58, 0x4b, 0x4a, 0x48,
				0x47, 0x46, 0x44, 0x53, 0x49, 0x55, 0x59, 0x54, 0x52, 0x45, 0x57, 0x00, 0x00, 0x0d, 0x0a, 0x4f,
				0x4c, 0x2e, 0x00, 0xff, 0x2d, 0xba, 0x30, 0xb9, 0xb8, 0xb7, 0xb6, 0xb5, 0xb4, 0xb3, 0xb2, 0xb1,
				0xa2, 0x28, 0x9a, 0xd8, 0xad, 0x06, 0xfb, 0xa9, 0xff, 0x8d, 0x05, 0xfb, 0xa2, 0xd8, 0xa9, 0xd0,
				0x85, 0xff, 0xa9, 0x00, 0x85, 0xfe, 0x85, 0xfb, 0xa8, 0xa9, 0x20, 0x91, 0xfe, 0xc8, 0xd0, 0xfb,
				0xe6, 0xff, 0xe4, 0xff, 0xd0, 0xf5, 0x84, 0xff, 0xf0, 0x19, 0x20, 0xe9, 0xfe, 0xc9, 0x2f, 0xf0,
				0x1e, 0xc9, 0x47, 0xf0, 0x17, 0xc9, 0x4c, 0xf0, 0x43, 0x20, 0x93, 0xfe, 0x30, 0xec, 0xa2, 0x02,
				0x20, 0xda, 0xfe, 0xb1, 0xfe, 0x85, 0xfc, 0x20, 0xac, 0xfe, 0xd0, 0xde, 0x6c, 0xfe, 0x00, 0x20,
				0xe9, 0xfe, 0xc9, 0x2e, 0xf0, 0xd4, 0xc9, 0x0d, 0xd0, 0x0f, 0xe6, 0xfe, 0xd0, 0x02, 0xe6, 0xff,
				0xa0, 0x00, 0xb1, 0xfe, 0x85, 0xfc, 0x4c, 0x77, 0xfe, 0x20, 0x93, 0xfe, 0x30, 0xe1, 0xa2, 0x00,
				0x20, 0xda, 0xfe, 0xa5, 0xfc, 0x91, 0xfe, 0x20, 0xac, 0xfe, 0xd0, 0xd3, 0x85, 0xfb, 0xf0, 0xcf,
				0xad, 0x00, 0xfc, 0x4a, 0x90, 0xfa, 0xad, 0x01, 0xfc, 0xea, 0xea, 0xea, 0x29, 0x7f, 0x60, 0x00,
				0x00, 0x00, 0x00, 0xc9, 0x30, 0x30, 0x12, 0xc9, 0x3a, 0x30, 0x0b, 0xc9, 0x41, 0x30, 0x0a, 0xc9,
				0x47, 0x10, 0x06, 0x38, 0xe9, 0x07, 0x29, 0x0f, 0x60, 0xa9, 0x80, 0x60, 0xa2, 0x03, 0xa0, 0x00,
				0xb5, 0xfc, 0x4a, 0x4a, 0x4a, 0x4a, 0x20, 0xca, 0xfe, 0xb5, 0xfc, 0x20, 0xca, 0xfe, 0xca, 0x10,
				0xef, 0xa9, 0x20, 0x8d, 0xca, 0xd0, 0x8d, 0xcb, 0xd0, 0x60, 0x29, 0x0f, 0x09, 0x30, 0xc9, 0x3a,
				0x30, 0x03, 0x18, 0x69, 0x07, 0x99, 0xc6, 0xd0, 0xc8, 0x60, 0xa0, 0x04, 0x0a, 0x0a, 0x0a, 0x0a,
				0x2a, 0x36, 0xfc, 0x36, 0xfd, 0x88, 0xd0, 0xf8, 0x60, 0xa5, 0xfb, 0xd0, 0x91, 0x4c, 0x00, 0xfd,
				0xa9, 0xff, 0x8d, 0x00, 0xdf, 0xad, 0x00, 0xdf, 0x60, 0xea, 0x30, 0x01, 0x00, 0xfe, 0xc0, 0x01,
				0xd8, 0xa2, 0x28, 0x9a, 0x20, 0x22, 0xbf, 0xa0, 0x00, 0x8c, 0x12, 0x02, 0x8c, 0x03, 0x02, 0x8c,
				0x05, 0x02, 0x8c, 0x06, 0x02, 0xad, 0xe0, 0xff, 0x8d, 0x00, 0x02, 0xa9, 0x20, 0x99, 0x00, 0xd7,
				0x99, 0x00, 0xd6, 0x99, 0x00, 0xd5, 0x99, 0x00, 0xd4, 0x99, 0x00, 0xd3, 0x99, 0x00, 0xd2, 0x99,
				0x00, 0xd1, 0x99, 0x00, 0xd0, 0xc8, 0xd0, 0xe5, 0xb9, 0x5f, 0xff, 0xf0, 0x06, 0x20, 0x2d, 0xbf,
				0xc8, 0xd0, 0xf5, 0x20, 0xb8, 0xff, 0xc9, 0x4d, 0xd0, 0x03, 0x4c, 0x00, 0xfe, 0xc9, 0x57, 0xd0,
				0x03, 0x4c, 0x00, 0x00, 0xc9, 0x43, 0xd0, 0xa8, 0xa9, 0x00, 0xaa, 0xa8, 0x4c, 0x11, 0xbd, 0x43,
				0x2f, 0x57, 0x2f, 0x4d, 0x20, 0x3f, 0x00, 0x20, 0x2d, 0xbf, 0x48, 0xad, 0x05, 0x02, 0xf0, 0x22,
				0x68, 0x20, 0x15, 0xbf, 0xc9, 0x0d, 0xd0, 0x1b, 0x48, 0x8a, 0x48, 0xa2, 0x0a, 0xa9, 0x00, 0x20,
				0x15, 0xbf, 0xca, 0xd0, 0xfa, 0x68, 0xaa, 0x68, 0x60, 0x48, 0xce, 0x03, 0x02, 0xa9, 0x00, 0x8d,
				0x05, 0x02, 0x68, 0x60, 0x48, 0xa9, 0x01, 0xd0, 0xf6, 0xad, 0x12, 0x02, 0xd0, 0x19, 0xa9, 0x01,
				0x8d, 0x00, 0xdf, 0x2c, 0x00, 0xdf, 0x50, 0x0f, 0xa9, 0x04, 0x8d, 0x00, 0xdf, 0x2c, 0x00, 0xdf,
				0x50, 0x05, 0xa9, 0x03, 0x4c, 0x36, 0xa6, 0x60, 0x2c, 0x03, 0x02, 0x10, 0x19, 0xa9, 0x02, 0x8d,
				0x00, 0xdf, 0xa9, 0x10, 0x2c, 0x00, 0xdf, 0xd0, 0x0a, 0xad, 0x00, 0xfc, 0x4a, 0x90, 0xee, 0xad,
				0x01, 0xfc, 0x60, 0xee, 0x03, 0x02, 0x4c, 0xed, 0xfe, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x40, 0x3f, 0x01, 0x00, 0x03, 0xff, 0x3f, 0x00, 0x03, 0xff, 0x3f, 0x4c, 0xb8, 0xff, 0x4c, 0x67,
				0xff, 0x4c, 0x99, 0xff, 0x4c, 0x89, 0xff, 0x4c, 0x94, 0xff, 0x30, 0x01, 0x00, 0xff, 0xc0, 0x01,
				0x20, 0x0c, 0xfc, 0x6c, 0xfd, 0x00, 0x20, 0x0c, 0xfc, 0x4c, 0x00, 0xfe, 0xa0, 0x00, 0x8c, 0x01,
				0xc0, 0x8c, 0x00, 0xc0, 0xa2, 0x04, 0x8e, 0x01, 0xc0, 0x8c, 0x03, 0xc0, 0x88, 0x8c, 0x02, 0xc0,
				0x8e, 0x03, 0xc0, 0x8c, 0x02, 0xc0, 0xa9, 0xfb, 0xd0, 0x09, 0xa9, 0x02, 0x2c, 0x00, 0xc0, 0xf0,
				0x1c, 0xa9, 0xff, 0x8d, 0x02, 0xc0, 0x20, 0xa5, 0xfc, 0x29, 0xf7, 0x8d, 0x02, 0xc0, 0x20, 0xa5,
				0xfc, 0x09, 0x08, 0x8d, 0x02, 0xc0, 0xa2, 0x18, 0x20, 0x91, 0xfc, 0xf0, 0xdd, 0xa2, 0x7f, 0x8e,
				0x02, 0xc0, 0x20, 0x91, 0xfc, 0xad, 0x00, 0xc0, 0x30, 0xfb, 0xad, 0x00, 0xc0, 0x10, 0xfb, 0xa9,
				0x03, 0x8d, 0x10, 0xc0, 0xa9, 0x58, 0x8d, 0x10, 0xc0, 0x20, 0x9c, 0xfc, 0x85, 0xfe, 0xaa, 0x20,
				0x9c, 0xfc, 0x85, 0xfd, 0x20, 0x9c, 0xfc, 0x85, 0xff, 0xa0, 0x00, 0x20, 0x9c, 0xfc, 0x91, 0xfd,
				0xc8, 0xd0, 0xf8, 0xe6, 0xfe, 0xc6, 0xff, 0xd0, 0xf2, 0x86, 0xfe, 0xa9, 0xff, 0x8d, 0x02, 0xc0,
				0x60, 0xa0, 0xf8, 0x88, 0xd0, 0xfd, 0x55, 0xff, 0xca, 0xd0, 0xf6, 0x60, 0xad, 0x10, 0xc0, 0x4a,
				0x90, 0xfa, 0xad, 0x11, 0xc0, 0x60, 0xa9, 0x03, 0x8d, 0x00, 0xf0, 0xa9, 0x11, 0x8d, 0x00, 0xf0,
				0x60, 0x48, 0xad, 0x00, 0xf0, 0x4a, 0x4a, 0x90, 0xf9, 0x68, 0x8d, 0x01, 0xf0, 0x60, 0x49, 0xff,
				0x8d, 0x00, 0xdf, 0x49, 0xff, 0x60, 0x48, 0x20, 0xcf, 0xfc, 0xaa, 0x68, 0xca, 0xe8, 0x60, 0xad,
				0x00, 0xdf, 0x49, 0xff, 0x60, 0xc9, 0x1c, 0xf0, 0x03, 0x4c, 0x74, 0xa3, 0xca, 0x10, 0x04, 0xe8,
				0x4c, 0x59, 0xa3, 0x8a, 0x48, 0xae, 0x00, 0x02, 0xa9, 0x20, 0x9d, 0x00, 0xd3, 0xce, 0x00, 0x02,
				0xca, 0xa9, 0x9a, 0x9d, 0x00, 0xd3, 0x68, 0xaa, 0x4c, 0x59, 0xa3, 0xff, 0xff, 0xff, 0xff, 0xff,
				0x8a, 0x48, 0x98, 0x48, 0xa9, 0x01, 0x20, 0xbe, 0xfc, 0x20, 0xc6, 0xfc, 0xd0, 0x05, 0x0a, 0xd0,
				0xf5, 0xf0, 0x53, 0x4a, 0x90, 0x09, 0x2a, 0xe0, 0x21, 0xd0, 0xf3, 0xa9, 0x1b, 0xd0, 0x21, 0x20,
				0xc8, 0xfd, 0x98, 0x8d, 0x13, 0x02, 0x0a, 0x0a, 0x0a, 0x38, 0xed, 0x13, 0x02, 0x8d, 0x13, 0x02,
				0x8a, 0x4a, 0x20, 0xc8, 0xfd, 0xd0, 0x2f, 0x18, 0x98, 0x6d, 0x13, 0x02, 0xa8, 0xb9, 0xcf, 0xfd,
				0xcd, 0x15, 0x02, 0xd0, 0x26, 0xce, 0x14, 0x02, 0xf0, 0x2b, 0xa0, 0x05, 0xa2, 0xc8, 0xca, 0xd0,
				0xfd, 0x88, 0xd0, 0xf8, 0xf0, 0xae, 0xc9, 0x01, 0xf0, 0x35, 0xa0, 0x00, 0xc9, 0x02, 0xf0, 0x47,
				0xa0, 0xc0, 0xc9, 0x20, 0xf0, 0x41, 0xa9, 0x00, 0x8d, 0x16, 0x02, 0x8d, 0x15, 0x02, 0xa9, 0x02,
				0x8d, 0x14, 0x02, 0xd0, 0x8f, 0xa2, 0x96, 0xcd, 0x16, 0x02, 0xd0, 0x02, 0xa2, 0x14, 0x8e, 0x14,
				0x02, 0x8d, 0x16, 0x02, 0xa9, 0x01, 0x20, 0xbe, 0xfc, 0x20, 0xcf, 0xfc, 0x4a, 0x90, 0x33, 0xaa,
				0x29, 0x03, 0xf0, 0x0b, 0xa0, 0x10, 0xad, 0x15, 0x02, 0x10, 0x0c, 0xa0, 0xf0, 0xd0, 0x08, 0xa0,
				0x00, 0xe0, 0x20, 0xd0, 0x02, 0xa0, 0xc0, 0xad, 0x15, 0x02, 0x29, 0x7f, 0xc9, 0x20, 0xf0, 0x07,
				0x8c, 0x13, 0x02, 0x18, 0x6d, 0x13, 0x02, 0x8d, 0x13, 0x02, 0x68, 0xa8, 0x68, 0xaa, 0xad, 0x13,
				0x02, 0x60, 0xd0, 0x92, 0xa0, 0x20, 0xd0, 0xdf, 0xa0, 0x08, 0x88, 0x0a, 0x90, 0xfc, 0x60, 0xd0,
				0xbb, 0x2f, 0x20, 0x5a, 0x41, 0x51, 0x2c, 0x4d, 0x4e, 0x42, 0x56, 0x43, 0x58, 0x4b, 0x4a, 0x48,
				0x47, 0x46, 0x44, 0x53, 0x49, 0x55, 0x59, 0x54, 0x52, 0x45, 0x57, 0x00, 0x00, 0x0d, 0x5e, 0x4f,
				0x4c, 0x2e, 0x00, 0x1c, 0x2d, 0xba, 0x30, 0xb9, 0xb8, 0xb7, 0xb6, 0xb5, 0xb4, 0xb3, 0xb2, 0xb1,
				0xa2, 0x28, 0x9a, 0xd8, 0xea, 0xea, 0xea, 0xea, 0xea, 0xea, 0xea, 0xea, 0xa2, 0xd4, 0xa9, 0xd0,
				0x85, 0xff, 0xa9, 0x00, 0x85, 0xfe, 0x85, 0xfb, 0xa8, 0xa9, 0x20, 0x91, 0xfe, 0xc8, 0xd0, 0xfb,
				0xe6, 0xff, 0xe4, 0xff, 0xd0, 0xf5, 0x84, 0xff, 0xf0, 0x19, 0x20, 0xe9, 0xfe, 0xc9, 0x2f, 0xf0,
				0x1e, 0xc9, 0x47, 0xf0, 0x17, 0xc9, 0x4c, 0xf0, 0x43, 0x20, 0x93, 0xfe, 0x30, 0xec, 0xa2, 0x02,
				0x20, 0xda, 0xfe, 0xb1, 0xfe, 0x85, 0xfc, 0x20, 0xac, 0xfe, 0xd0, 0xde, 0x6c, 0xfe, 0x00, 0x20,
				0xe9, 0xfe, 0xc9, 0x2e, 0xf0, 0xd4, 0xc9, 0x0d, 0xd0, 0x0f, 0xe6, 0xfe, 0xd0, 0x02, 0xe6, 0xff,
				0xa0, 0x00, 0xb1, 0xfe, 0x85, 0xfc, 0x4c, 0x77, 0xfe, 0x20, 0x93, 0xfe, 0x30, 0xe1, 0xa2, 0x00,
				0x20, 0xda, 0xfe, 0xa5, 0xfc, 0x91, 0xfe, 0x20, 0xac, 0xfe, 0xd0, 0xd3, 0x85, 0xfb, 0xf0, 0xcf,
				0xad, 0x00, 0xf0, 0x4a, 0x90, 0xfa, 0xad, 0x01, 0xf0, 0xea, 0xea, 0xea, 0x29, 0x7f, 0x60, 0x00,
				0x00, 0x00, 0x00, 0xc9, 0x30, 0x30, 0x12, 0xc9, 0x3a, 0x30, 0x0b, 0xc9, 0x41, 0x30, 0x0a, 0xc9,
				0x47, 0x10, 0x06, 0x38, 0xe9, 0x07, 0x29, 0x0f, 0x60, 0xa9, 0x80, 0x60, 0xa2, 0x03, 0xa0, 0x00,
				0xb5, 0xfc, 0x4a, 0x4a, 0x4a, 0x4a, 0x20, 0xca, 0xfe, 0xb5, 0xfc, 0x20, 0xca, 0xfe, 0xca, 0x10,
				0xef, 0xa9, 0x20, 0x8d, 0x62, 0xd1, 0x8d, 0x63, 0xd1, 0x60, 0x29, 0x0f, 0x09, 0x30, 0xc9, 0x3a,
				0x30, 0x03, 0x18, 0x69, 0x07, 0x99, 0x5e, 0xd1, 0xc8, 0x60, 0xa0, 0x04, 0x0a, 0x0a, 0x0a, 0x0a,
				0x2a, 0x36, 0xfc, 0x36, 0xfd, 0x88, 0xd0, 0xf8, 0x60, 0xa5, 0xfb, 0xd0, 0x93, 0x4c, 0x00, 0xfd,
				0xba, 0xff, 0x69, 0xff, 0x9b, 0xff, 0x8b, 0xff, 0x96, 0xff, 0x30, 0x01, 0x00, 0xfe, 0xc0, 0x01,
				0xd8, 0xa2, 0x28, 0x9a, 0xa0, 0x0a, 0xb9, 0xef, 0xfe, 0x99, 0x17, 0x02, 0x88, 0xd0, 0xf7, 0x20,
				0xa6, 0xfc, 0x8c, 0x12, 0x02, 0x8c, 0x03, 0x02, 0x8c, 0x05, 0x02, 0x8c, 0x06, 0x02, 0xad, 0xe0,
				0xff, 0x8d, 0x00, 0x02, 0xa9, 0x20, 0x99, 0x00, 0xd3, 0x99, 0x00, 0xd2, 0x99, 0x00, 0xd1, 0x99,
				0x00, 0xd0, 0xc8, 0xd0, 0xf1, 0xb9, 0x5f, 0xff, 0xf0, 0x06, 0x20, 0x2d, 0xbf, 0xc8, 0xd0, 0xf5,
				0x20, 0xba, 0xff, 0xc9, 0x4d, 0xd0, 0x03, 0x4c, 0x00, 0xfe, 0xc9, 0x57, 0xd0, 0x03, 0x4c, 0x00,
				0x00, 0xc9, 0x43, 0xd0, 0x03, 0x4c, 0x11, 0xbd, 0xc9, 0x44, 0xd0, 0xa4, 0x4c, 0x00, 0xfc, 0x44,
				0x2f, 0x43, 0x2f, 0x57, 0x2f, 0x4d, 0x20, 0x3f, 0x00, 0x20, 0x2d, 0xbf, 0x48, 0xad, 0x05, 0x02,
				0xf0, 0x22, 0x68, 0x20, 0xb1, 0xfc, 0xc9, 0x0d, 0xd0, 0x1b, 0x48, 0x8a, 0x48, 0xa2, 0x0a, 0xa9,
				0x00, 0x20, 0xb1, 0xfc, 0xca, 0xd0, 0xfa, 0x68, 0xaa, 0x68, 0x60, 0x48, 0xce, 0x03, 0x02, 0xa9,
				0x00, 0x8d, 0x05, 0x02, 0x68, 0x60, 0x48, 0xa9, 0x01, 0xd0, 0xf6, 0xad, 0x12, 0x02, 0xd0, 0x19,
				0xa9, 0xfe, 0x8d, 0x00, 0xdf, 0x2c, 0x00, 0xdf, 0x70, 0x0f, 0xa9, 0xfb, 0x8d, 0x00, 0xdf, 0x2c,
				0x00, 0xdf, 0x70, 0x05, 0xa9, 0x03, 0x4c, 0x36, 0xa6, 0x60, 0x2c, 0x03, 0x02, 0x10, 0x19, 0xa9,
				0xfd, 0x8d, 0x00, 0xdf, 0xa9, 0x10, 0x2c, 0x00, 0xdf, 0xf0, 0x0a, 0xad, 0x00, 0xf0, 0x4a, 0x90,
				0xee, 0xad, 0x01, 0xf0, 0x60, 0xee, 0x03, 0x02, 0x4c, 0x00, 0xfd, 0xff, 0xff, 0xff, 0xff, 0xff,
				0xcd, 0x2f, 0x00, 0x00, 0x03, 0xff, 0x9f, 0x00, 0x03, 0xff, 0x9f, 0x6c, 0x18, 0x02, 0x6c, 0x1a,
				0x02, 0x6c, 0x1c, 0x02, 0x6c, 0x1e, 0x02, 0x6c, 0x20, 0x02, 0x30, 0x01, 0x00, 0xff, 0xc0, 0x01,
			};
		}

		#endregion
		#region Properties

		public UInt16 ROMSize { get; set; }

        #endregion
        #region Methods

        public override byte Read()
        {
            return pData[Address - StartsAt];
        }

		#endregion
    }
}