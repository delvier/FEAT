using GovanifY.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEAT.Library.FE3D_Bin
{
    class FE3D_Bin
    {
        #region Code Helpers
        private static uint ReadUint32FromArray(byte[] array)
        {
            return (uint)(array[0] | (array[1] << 8) | (array[2] << 16) | (array[3] << 24));
        }
        private static uint ReadUint32FromArray(byte[] array, uint index)
        {
            return (uint)(array[index + 0] | (array[index + 1] << 8) | (array[index + 2] << 16) | (array[index + 3] << 24));
        }

        private static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }
        #endregion

        //Arc files are a form of FE3D bin but store files instead of data
        //because of this they can be packed and unpacked differently than normal
        public static void CreateArc(string inpath, int Alignment, bool Padding)
        {
            string outfile = $"{Path.GetDirectoryName(inpath)}//{Path.GetFileNameWithoutExtension(inpath)}.arc";
            Console.WriteLine("Creating archive {0}", Path.GetFileName(outfile));
            FileStream newfilestream = File.Create(outfile);
            string[] files = Directory.GetFiles(inpath, "*", SearchOption.AllDirectories);

            uint FileCount = (uint)files.Length;
            Console.WriteLine("{0} files detected!", FileCount);

            var ShiftJIS = Encoding.GetEncoding(932);

            BinaryStream newFile = new BinaryStream(newfilestream);

            MemoryStream infos = new MemoryStream();
            BinaryWriter FileInfos = new BinaryWriter(infos);


            Console.WriteLine("Creating dummy header...");
            newFile.Write(0);

            newFile.Write(0);
            newFile.Write(FileCount);
            newFile.Write(FileCount + 3);

            byte nil = 0;
            if (Padding)
            {
                for (int i = 0; i < 0x70; i++)
                {
                    newFile.Write(nil);
                }
            }
            else
            {
                for (int i = 0; i < 0x10; i++)
                {
                    newFile.Write(nil);
                }
            }
            int z = 0;
            foreach (string fileName in files)
            {
                Console.WriteLine("Adding file {0}...", fileName.Replace(inpath + Path.DirectorySeparatorChar, "").Replace("\\", "/"));
                byte[] filetoadd = File.ReadAllBytes(fileName);
                uint fileoff = (uint)newFile.Tell();
                newFile.Write(filetoadd); //File is written to arc
                if (Alignment != 0)
                {
                    while ((int)newFile.Tell() % Alignment != 0)
                    {
                        newFile.Write(nil); //writes between file padding
                    }
                }
                FileInfos.Write(0);
                FileInfos.Write(z);
                FileInfos.Write(filetoadd.Length);
                if (Padding)
                {
                    FileInfos.Write(fileoff - 0x80);
                }
                else
                {
                    FileInfos.Write(fileoff - 0x20);
                }
                z++;
            }

            long countinfo = newFile.Tell();
            newFile.Write(FileCount);
            long infopointer = newFile.Tell();
            Console.WriteLine("Adding dummy FileInfos...");

            infos.Seek(0, SeekOrigin.Begin);
            var infopos = newFile.Tell();
            newFile.Write(infos.ToArray());

            Console.WriteLine("Rewriting header...");
            long metapos = newFile.Tell();
            newFile.Seek(4, SeekOrigin.Begin);
            newFile.Write((uint)metapos - 0x20);

            newFile.Seek(metapos, SeekOrigin.Begin);

            Console.WriteLine("Adding FileInfos pointer...");
            for (int i = 0; i < FileCount; i++)
            {
                newFile.Write((uint)((infopointer + i * 16) - 0x20));
            }

            Console.WriteLine("Adding Dummy Pointer 2 Region...");

            if (Padding)
            {
                newFile.Write((uint)0x60);
            }
            else
            {
                newFile.Write((uint)0x0);
            }
            newFile.Write(0);
            newFile.Write((uint)(countinfo - 0x20));
            newFile.Write((uint)5);
            newFile.Write((uint)(countinfo + 4 - 0x20));
            newFile.Write((uint)0xB);
            long ptr2region = newfilestream.Position;
            for (int i = 0; i < FileCount; i++)
            {
                newFile.Write((uint)((countinfo + 4) + i * 16) - 0x20);
                if (i == 0)
                {
                    newFile.Write((uint)0x10);
                }
                else
                {
                    if (i == 1)
                    {
                        newFile.Write((uint)0x1C);
                    }
                    else
                    {
                        newFile.Write((uint)(0x1C + (10 * (i - 1))));
                    }
                }
            }

            Console.WriteLine("Adding Filenames and Rewriting Pointer 2 Region...");
            var datcount = new byte[] { 0x44, 0x61, 0x74, 0x61, 0x00, 0x43, 0x6F, 0x75, 0x6E, 0x74, 0x00, 0x49, 0x6E, 0x66, 0x6F, 0x00 };
            newFile.Write(datcount);
            int y = 0;
            int nameloc = 16;

            foreach (string fileName in files)
            {
                FileInfos.Seek(y * 16, SeekOrigin.Begin);
                long namepos = newFile.Tell();
                FileInfos.Write((uint)namepos - 0x20);
                newFile.Write(ShiftJIS.GetBytes(fileName.Replace(inpath + Path.DirectorySeparatorChar, "").Replace("\\", "/")));
                newFile.Write(nil);
                long NameEnd = newfilestream.Position;
                long pointerpos = (ptr2region + 4 + (y * 8));
                newFile.Seek(pointerpos, SeekOrigin.Begin);
                newFile.Write(nameloc);
                newFile.Seek(NameEnd, SeekOrigin.Begin);
                byte[] onlyfilename = ShiftJIS.GetBytes(fileName.Replace(inpath + Path.DirectorySeparatorChar, "").Replace("\\", "/"));
                nameloc = nameloc + onlyfilename.Length + 1;
                y++;
            }
            Console.WriteLine("Rewriting FileInfos...");
            newFile.Seek(infopos, SeekOrigin.Begin);

            infos.Seek(0, SeekOrigin.Begin);
            newFile.Write(infos.ToArray());

            Console.WriteLine("Finishing the job...");
            newFile.Seek(0, SeekOrigin.Begin);
            UInt32 newlength = (UInt32)newFile.BaseStream.Length;
            newFile.Write(newlength);

            Console.WriteLine("Done!");
            newFile.Close();
        }

        public static void ExtractArc(string outdir, byte[] archive)
        {
            if (Directory.Exists(outdir))
                Directory.Delete(outdir, true);
            Directory.CreateDirectory(outdir);

            var ShiftJIS = Encoding.GetEncoding(932);

            uint MetaOffset = BitConverter.ToUInt32(archive, 4) + 0x20;
            uint FileCount = BitConverter.ToUInt32(archive, 0x8);

            bool awakening = (BitConverter.ToUInt32(archive, 0x20) != 0);

            for (int i = 0; i < FileCount; i++)
            {
                int FileMetaOffset = 0x20 + BitConverter.ToInt32(archive, (int)MetaOffset + 4 * i);
                int FileNameOffset = BitConverter.ToInt32(archive, FileMetaOffset) + 0x20;
                // int FileIndex = BitConverter.ToInt32(archive, FileMetaOffset + 4);
                uint FileDataLength = BitConverter.ToUInt32(archive, FileMetaOffset + 8);
                int FileDataOffset = BitConverter.ToInt32(archive, FileMetaOffset + 0xC) + (awakening ? 0x20 : 0x80);
                byte[] file = new byte[FileDataLength];
                Array.Copy(archive, FileDataOffset, file, 0, FileDataLength);
                string outpath = outdir + ShiftJIS.GetString(archive.Skip(FileNameOffset).TakeWhile(b => b != 0).ToArray());
                if (!Directory.Exists(Path.GetDirectoryName(outpath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(outpath));
                File.WriteAllBytes(outpath, file);
            }
        }

        //location of the ptr1 should never the same but they are able to point the same place
        //on the other side the destination of the ptr2 is unique because of their lack  
        //of data in the main section they can occupy the same location as other pointers
        static Dictionary<uint, uint> PTR1; //location key, destination value
        static Dictionary<uint, uint> PTR1_des; //destination key, id value
        static Dictionary<uint, uint> PTR2; //destination key, location value
        static Dictionary<string, uint> PTR2_Data;
        static Dictionary<uint, string> PTR1_Data;
        static Dictionary<string, uint> StringSection;

        public static void ExtractBin(string inpath)
        {
            PTR1 = new Dictionary<uint, uint>();
            PTR1_des = new Dictionary<uint, uint>();
            PTR2 = new Dictionary<uint, uint>();

            //open file stream and then open a binary stream
            FileStream fileStream = new FileStream(inpath, FileMode.Open);
            BinaryStream bin = new BinaryStream(fileStream, true, false);
            var ShiftJIS = Encoding.GetEncoding(932);

            //read header info
            uint filesize = bin.ReadUInt32();
            uint datasize = bin.ReadUInt32();
            uint ptr1count = bin.ReadUInt32();
            uint ptr2count = bin.ReadUInt32();
            bin.Seek(0x10, SeekOrigin.Current);
            byte[] datasection = bin.ReadBytes((int)datasize);

            //read pointer region 1 info
            for (uint i = 0; i < ptr1count; i++)
            {
                uint loc = bin.ReadUInt32();
                uint des = ReadUint32FromArray(datasection, loc);

                try 
                {
                    PTR1.Add(loc, des);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Key Already Exist in PTR1");
                }
                try
                {
                    PTR1_des.Add(des, i);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Key Already Exist in PTR1_des");
                }
            }

            //read pointer region 2 locations and destinations then store them for later
            for (int i = 0; i < ptr2count; i++)
            {
                uint loc = bin.ReadUInt32();
                uint des = bin.ReadUInt32();

                try
                {
                    PTR2.Add(des, loc);
                }
                catch
                {
                    Console.WriteLine("Key Already Exist in PTR1");
                }
            }

            //log the location after the pointer regions and get the raw bytes for the string array
            uint stringtablepos = (uint)bin.Tell();
            byte[] rawstrings = bin.ReadBytes(Convert.ToInt32(filesize - stringtablepos));


            //return to the start of the data section to begin processing it
            bin.Seek(0x20, SeekOrigin.Begin);

            //Create a txt file to store the bin data
            string outfile = Path.GetDirectoryName(inpath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(inpath) + ".txt";
            if (File.Exists(outfile))
                File.Delete(outfile);

            //open a stream to write the output file
            using (StreamWriter outstream = new StreamWriter(File.Create(outfile)))
            {
                for (int i = 0; i < datasize / 4; i++)
                {
                    uint currentpos = Convert.ToUInt32(bin.Tell() - 0x20);

                    //Check for ptr1 destinations and write them before the ptr2
                    if (PTR1_des.ContainsKey(currentpos))
                    {
                        PTR1_des.TryGetValue(currentpos, out uint value);
                        outstream.WriteLine($"PTR1: {value}");
                    }

                    //Check if there is a ptr2 for the current position and write it before the data
                    foreach ( KeyValuePair<uint, uint> ptr2 in PTR2)
                    {
                        if (currentpos == ptr2.Value)
                        {
                            string ptr2data = ShiftJIS.GetString(rawstrings.Skip(Convert.ToInt32(ptr2.Key)).TakeWhile(b => b != 0).ToArray()).Replace("－", "−");
                            outstream.WriteLine($"PTR2: {ptr2data}");
                        }
                    }

                    //Check if there is a ptr1 for the current position otherwise right the bytes
                    if (PTR1.ContainsKey(currentpos))
                    {
                        PTR1.TryGetValue(currentpos, out uint value);
                        if (value > datasize)
                        {
                            string ptr1data = ShiftJIS.GetString(rawstrings.Skip(Convert.ToInt32(value - (datasize + (ptr1count * 4) + (ptr2count * 8)))).TakeWhile(b => b != 0).ToArray());
                            outstream.WriteLine(ptr1data);
                        }
                        else
                        {
                            PTR1_des.TryGetValue(value, out uint value2);
                            outstream.WriteLine($"POINTER1: {value2}");
                        }
                        bin.Seek(4, SeekOrigin.Current);
                    }
                    else
                    {
                        byte[] data = bin.ReadBytes(4);
                        outstream.WriteLine($"0x{BitConverter.ToString(data).Replace("-", "")}");
                    }
                    Console.WriteLine($"Processed {currentpos}");
                }
            }
            bin.Close();
            fileStream.Close();
        }

        //bin files have 5 major sections, header, main data, pointer region 1 and 2, and the end region
        //the header is formatted with file size, data size, num of ptr1, num of prt2, and 16 bytes of padding
        //main sections just holds all the data that the game using exclusing strings
        //pointer region 1 is a list of where each pointer in the main section is, each item is 4 bytes
        //pointer region 2 is a list of embbed lables in side the main section which doesn't show as data, each item is 8 bytes, location of pointer and it's string location
        //the end secions hold a list of all the strings in null terminated shift-jis, first are the ptr2, then the prt1
        public static void PackBin(string inpath)
        {
            string[] txtfile = File.ReadAllLines(inpath);
            FileStream newfilestream = File.Create(inpath.Replace(".txt", ".bin"));
            PTR1 = new Dictionary<uint, uint>();
            PTR1_des = new Dictionary<uint, uint>();
            PTR2_Data = new Dictionary<string, uint>();
            PTR1_Data = new Dictionary<uint, string>();
            StringSection = new Dictionary<string, uint>();
            var ShiftJIS = Encoding.GetEncoding(932);

            using (BinaryStream outbin = new BinaryStream(newfilestream))
            {
                //Write innital pass included dummy sections, main data, and strings
                //write dummy header
                for (int i = 0; i < 0x20; i++)
                {
                    outbin.Write((byte)0);
                }

                //Phrase Text file
                foreach (string line in txtfile)
                {
                    long CurrentPos = outbin.Tell();
                    if (line.StartsWith("0x")) //Convert the raw data string into bytes
                    {
                        uint data = Convert.ToUInt32(ReadUint32FromArray(ConvertHexStringToByteArray(line.Replace("0x",""))));
                        outbin.Write(data);
                    }
                    else if (line.StartsWith("POINTER1")) //Write dummy bytes and log pointer number
                    {
                        PTR1.Add((uint)CurrentPos, Convert.ToUInt32(line.Replace("POINTER1: ","")));
                        outbin.Write((uint)0);
                    }
                    else if (line.StartsWith("PTR1")) //Log location to for Pointer1 rewrite
                    {
                        PTR1_des.Add(Convert.ToUInt32(line.Replace("PTR1: ", "")), (uint)CurrentPos);
                    }
                    else if (line.StartsWith("PTR2")) //Log string and Position
                    {
                        PTR2_Data.Add(line.Replace("PTR2: ", ""), (uint)CurrentPos);
                    }
                    else //Log Location and string to write later
                    {
                        PTR1_Data.Add((uint)CurrentPos, line);
                        outbin.Write((uint)0);
                    }
                }
                int EndofDataSecton = (int)outbin.Tell();

                uint ptr2Count;
                uint ptr1Count = (uint)PTR1.Count + (uint)PTR1_Data.Count;
                if (PTR2_Data == null)
                    ptr2Count = 0;
                else
                    ptr2Count = (uint)PTR2_Data.Count;

                //write dummy ptr sections
                for (int i = 0; i < ptr1Count; i++)
                {
                    outbin.Write((uint)0);
                }
                for (int i = 0; i < ptr2Count; i++)
                {
                    outbin.Write((long)0);
                }
                int BeginningOfTheEnd = (int)outbin.Tell();

                //sort strings and remove dupicates
                List<string> strings = new List<string>();
                foreach (var data in PTR2_Data)
                {
                    if (!strings.Contains(data.Key))
                        strings.Add(data.Key);
                }
                foreach (var data in PTR1_Data)
                {
                    if (!strings.Contains(data.Value))
                        strings.Add(data.Value);
                }

                //Write String Section
                foreach (string line in strings)
                {
                    long CurrentPos = outbin.Tell();
                    StringSection.Add(line, (uint)CurrentPos);
                    byte[] linebytes = ShiftJIS.GetBytes(line.Replace("\\", "/"));
                    outbin.Write(linebytes);
                    outbin.Write((byte)0);
                }

                //Write Second pass with count data and pointers
                int EOF = (int)outbin.Tell();
                outbin.Seek(0, SeekOrigin.Begin);

                //Write Header info
                outbin.Write(EOF);
                outbin.Write(EndofDataSecton - 0x20);
                outbin.Write(ptr1Count);
                outbin.Write(ptr2Count);

                List<uint> PTR1Sec = new List<uint>();
                //Write PTR1 data pointers
                foreach (var data in PTR1)
                {
                    outbin.Seek(data.Key, SeekOrigin.Begin);
                    outbin.Write(PTR1_des[data.Value] - 0x20);
                    PTR1Sec.Add(data.Key);
                }
                //Write PTR1 string pointers
                foreach (var data in PTR1_Data)
                {
                    outbin.Seek(data.Key, SeekOrigin.Begin);
                    outbin.Write(StringSection[data.Value] - 0x20);
                    PTR1Sec.Add(data.Key);
                }
                //Write the pointer list to the PTR1 section
                outbin.Seek(EndofDataSecton, SeekOrigin.Begin);
                foreach (uint pointer in PTR1Sec)
                {
                    outbin.Write(pointer - 0x20);
                }
                //Write Data for PTR2 section
                foreach (var data in PTR2_Data)
                {
                    outbin.Write(data.Value - 0x20);
                    outbin.Write((uint)(StringSection[data.Key] - BeginningOfTheEnd));
                }

            }
            newfilestream.Close();
        }
    }
}
