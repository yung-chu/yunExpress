using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using System.IO;
using System.Collections;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;

namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// 功能：压缩文件
    /// </summary>
    public static class Zip
    {
        /// <summary>
        /// 压缩单个文件
        /// </summary>
        /// <param name="fileToZip">被压缩的文件名称(包含文件路径)</param>
        /// <param name="zipedFile">压缩后的文件名称(包含文件路径)</param>
        /// <param name="compressionLevel">压缩率0（无压缩）-9（压缩率最高）</param>
        /// <param name="BlockSize">缓存大小</param>
        public static void ZipFile(string fileToZip, string zipedFile, int compressionLevel)
        {
            //如果文件没有找到，则报错 
            if (!System.IO.File.Exists(fileToZip))
            {
                throw new System.IO.FileNotFoundException("文件：" + fileToZip + "没有找到！");
            }

            if (zipedFile == string.Empty)
            {
                zipedFile = Path.GetFileNameWithoutExtension(fileToZip) + ".zip";
            }

            if (Path.GetExtension(zipedFile) != ".zip")
            {
                zipedFile = zipedFile + ".zip";
            }

            //如果指定位置目录不存在，创建该目录
            string zipedDir = zipedFile.Substring(0, zipedFile.LastIndexOf("\\", System.StringComparison.Ordinal));
            if (!Directory.Exists(zipedDir))
                Directory.CreateDirectory(zipedDir);

            //被压缩文件名称
            string filename = fileToZip.Substring(fileToZip.LastIndexOf('\\') + 1);

            System.IO.FileStream StreamToZip = new System.IO.FileStream(fileToZip, System.IO.FileMode.Open,
                                                                        System.IO.FileAccess.Read);
            System.IO.FileStream ZipFile = System.IO.File.Create(zipedFile);
            ZipOutputStream ZipStream = new ZipOutputStream(ZipFile);
            ZipEntry ZipEntry = new ZipEntry(filename);
            ZipStream.PutNextEntry(ZipEntry);
            ZipStream.SetLevel(compressionLevel);
            byte[] buffer = new byte[2048];
            System.Int32 size = StreamToZip.Read(buffer, 0, buffer.Length);
            ZipStream.Write(buffer, 0, size);
            try
            {
                while (size < StreamToZip.Length)
                {
                    int sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                    ZipStream.Write(buffer, 0, sizeRead);
                    size += sizeRead;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                ZipStream.Finish();
                ZipStream.Close();
                StreamToZip.Close();
            }
        }

        /// <summary>
        /// 压缩多个文件
        /// </summary>
        /// <param name="fileToZip">被压缩的文件名称(包含文件路径)</param>
        /// <param name="zipedFile">压缩后的文件名称(包含文件路径)</param>
        /// <param name="compressionLevel">压缩率0（无压缩）-9（压缩率最高）</param>
        public static void ZipMultiFile(string[] fileToZip, string zipedFile, int compressionLevel)
        {
            try
            {
                //使用正则表达式-判断压缩文件路径
                System.Text.RegularExpressions.Regex newRegex = new System.Text.
                    RegularExpressions.Regex(@"^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w   ]*.*))");
                if (!newRegex.Match(zipedFile).Success)
                {
                    File.Delete(zipedFile);
                }


                if (zipedFile == string.Empty)
                {
                    zipedFile = Path.GetFileNameWithoutExtension(fileToZip[0]) + ".zip";
                }

                if (Path.GetExtension(zipedFile) != ".zip")
                {
                    zipedFile = zipedFile + ".zip";
                }

                //如果指定位置目录不存在，创建该目录
                string zipedDir = zipedFile.Substring(0, zipedFile.LastIndexOf("\\", System.StringComparison.Ordinal));
                if (!Directory.Exists(zipedDir))
                    Directory.CreateDirectory(zipedDir);
                //创建ZipFileOutPutStream
                ZipOutputStream newzipstream = new ZipOutputStream(File.Open(zipedFile,
                                                                             FileMode.OpenOrCreate));

                //设置CompressionLevel
                newzipstream.SetLevel(compressionLevel); //-查看0 - means store only to 9 - means best compression 

                //执行压缩
                foreach (string file in fileToZip)
                {
                    if(!File.Exists(file)) continue;//文件不存在
                    string filename = file.Substring(file.LastIndexOf('\\') + 1);
                    FileStream newstream = File.OpenRead(file); //打开预压缩文件
                    //判断路径
                    if (!newRegex.Match(zipedFile).Success)
                    {
                        File.Delete(zipedFile);
                    }
                    byte[] setbuffer = new byte[newstream.Length];
                    newstream.Read(setbuffer, 0, setbuffer.Length); //读入文件
                    //新建ZipEntrity
                    ZipEntry newEntry = new ZipEntry(filename);
                    //设置时间-长度
                    newEntry.DateTime = DateTime.Now;
                    newEntry.Size = newstream.Length;
                    newstream.Close();
                    newzipstream.PutNextEntry(newEntry); //压入
                    newzipstream.Write(setbuffer, 0, setbuffer.Length);

                }
                //重复压入操作
                newzipstream.Finish();
                newzipstream.Close();

            }
            catch (Exception e)
            {
                //出现异常
                File.Delete(zipedFile);
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 压缩文件夹的方法
        /// </summary>
        /// <param name="dicToZip">要压缩的目录</param>
        /// <param name="zipedFile">压缩后的文件名称(包含文件路径)</param>
        /// <param name="compressionLevel">压缩率0（无压缩）-9（压缩率最高）</param>
        public static void ZipDir(string dirToZip, string zipedFile, int compressionLevel)
        {
            //压缩文件为空时默认与压缩文件夹同一级目录
            if (zipedFile == string.Empty)
            {
                zipedFile = dirToZip.Substring(dirToZip.LastIndexOf("\\") + 1);
                zipedFile = dirToZip.Substring(0, dirToZip.LastIndexOf("\\")) + "\\" + zipedFile + ".zip";
            }

            if (Path.GetExtension(zipedFile) != ".zip")
            {
                zipedFile = zipedFile + ".zip";
            }

            using (ZipOutputStream zipoutputstream = new ZipOutputStream(File.Create(zipedFile)))
            {
                zipoutputstream.SetLevel(compressionLevel);
                Crc32 crc = new Crc32();
                Hashtable fileList = GetAllFies(dirToZip);
                foreach (DictionaryEntry item in fileList)
                {
                    FileStream fs = File.OpenRead(item.Key.ToString());
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    ZipEntry entry = new ZipEntry(item.Key.ToString().Substring(dirToZip.Length + 1));
                    entry.DateTime = (DateTime) item.Value;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    zipoutputstream.PutNextEntry(entry);
                    zipoutputstream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        /// <summary>
        /// 获取所有文件
        /// </summary>
        /// <returns></returns>
        private static Hashtable GetAllFies(string dir)
        {
            Hashtable FilesList = new Hashtable();
            DirectoryInfo fileDire = new DirectoryInfo(dir);
            if (!fileDire.Exists)
            {
                throw new System.IO.FileNotFoundException("目录:" + fileDire.FullName + "没有找到!");
            }

            GetAllDirFiles(fileDire, FilesList);
            GetAllDirsFiles(fileDire.GetDirectories(), FilesList);
            return FilesList;
        }

        /// <summary>
        /// 获取一个文件夹下的所有文件夹里的文件
        /// </summary>
        /// <param name="dirs"></param>
        /// <param name="filesList"></param>
        private static void GetAllDirsFiles(DirectoryInfo[] dirs, Hashtable filesList)
        {
            foreach (DirectoryInfo dir in dirs)
            {
                foreach (FileInfo file in dir.GetFiles("*.*"))
                {
                    filesList.Add(file.FullName, file.LastWriteTime);
                }
                GetAllDirsFiles(dir.GetDirectories(), filesList);
            }
        }

        /// <summary>
        /// 获取一个文件夹下的文件
        /// </summary>
        /// <param name="strDirName">目录名称</param>
        /// <param name="filesList">文件列表HastTable</param>
        private static void GetAllDirFiles(DirectoryInfo dir, Hashtable filesList)
        {
            foreach (FileInfo file in dir.GetFiles("*.*"))
            {
                filesList.Add(file.FullName, file.LastWriteTime);
            }
        }
    }
}
