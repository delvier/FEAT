using GovanifY.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEAT.Library.FE3D_Bin
{
    class FE3D_Bin
    {
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

        public class PTR1
        {
            public uint Location { get; set; }
            public uint Destination { get; set; }
            public uint ID { get; set; }
        }

        public class PTR2
        {
            public uint Location { get; set; }
            public uint Destination { get; set; }
        }

        public static void ExtractBin(string inpath)
        {
            //open file stream and then open a binary stream
            FileStream fileStream = new FileStream(inpath, FileMode.Open);
            BinaryStream bin = new BinaryStream(fileStream, true, false);
            var ShiftJIS = Encoding.GetEncoding(932);

            //read header info
            uint filesize = bin.ReadUInt32();
            uint datasize = bin.ReadUInt32();
            uint ptr1count = bin.ReadUInt32();
            uint ptr2count = bin.ReadUInt32();

            //read pointer region 1 locations and destinations then store them for later
            bin.Seek(0x20 + datasize, SeekOrigin.Begin);
            List<PTR1> ptr1List = new List<PTR1>();
            uint id = 0;
            for (int i = 0; i < ptr1count; i++)
            {
                uint ptr1 = bin.ReadUInt32();
                uint pos = (uint)bin.Tell();

                bin.Seek(ptr1 + 0x20, SeekOrigin.Begin);
                uint pointer = bin.ReadUInt32();

                PTR1 pointer1 = ptr1List.Find(x => (x.Destination == pointer));
                if (pointer1 == null)
                {
                    ptr1List.Add(new PTR1 { Location = ptr1, Destination = pointer, ID = id });
                    id++;
                }
                else
                {
                    ptr1List.Add(new PTR1 { Location = ptr1, Destination = pointer, ID = pointer1.ID });
                }
                
                bin.Seek(pos, SeekOrigin.Begin);
            }

            //read pointer region 2 locations and destinations then store them for later
            List<PTR2> ptr2List = new List<PTR2>();
            for (int i = 0; i < ptr2count; i++)
            {
                uint ptr2 = bin.ReadUInt32();
                uint pointer = bin.ReadUInt32();

                ptr2List.Add(new PTR2 { Location = ptr2, Destination = pointer });
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
                    uint currentpos = Convert.ToUInt32(bin.Tell());

                    //Check for ptr1 destinations and write them before the ptr2
                    if (ptr1List.Any(x => (x.Destination == currentpos - 0x20)))
                    {
                        var ptr1 = ptr1List.Find(x => (x.Destination == currentpos - 0x20));
                        outstream.WriteLine($"PTR1: {ptr1.ID}");
                    }

                    //Check if there is a ptr2 for the current position and write it before the data
                    foreach (var ptr2 in ptr2List.FindAll(x => (x.Location == currentpos -0x20)))
                    {
                        if (currentpos - 0x20 == ptr2.Location)
                        {
                            string ptr2data = ShiftJIS.GetString(rawstrings.Skip(Convert.ToInt32(ptr2.Destination)).TakeWhile(b => b != 0).ToArray()).Replace("－", "−");
                            outstream.WriteLine($"PTR2: {ptr2data}");
                        }
                    }

                    //Check if there is a ptr1 for the current position otherwise right the bytes
                    if (ptr1List.Any(x => (x.Location == currentpos - 0x20)))
                    {
                        var ptr1 = ptr1List.Find(x => (x.Location == currentpos - 0x20));
                        if (ptr1.Destination > datasize)
                        {
                            string ptr1data = ShiftJIS.GetString(rawstrings.Skip(Convert.ToInt32(ptr1.Destination - (datasize + (ptr1count * 4) + (ptr2count * 8)))).TakeWhile(b => b != 0).ToArray());
                            outstream.WriteLine(ptr1data);
                        }
                        else
                        {
                            outstream.WriteLine($"POINTER1: {ptr1.ID}");
                        }
                        bin.Seek(4, SeekOrigin.Current);
                    }
                    else
                    {
                        byte[] data = bin.ReadBytes(4);
                        outstream.WriteLine($"0x{BitConverter.ToString(data).Replace("-", "")}");
                    }
                    
                }
            }
            bin.Close();
            fileStream.Close();
        }
    }
}
