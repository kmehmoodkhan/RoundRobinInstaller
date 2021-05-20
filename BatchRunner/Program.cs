using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using InstallAndConfigure;

namespace BatchRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            try

            {
                //test
                string targetDir = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                string fileName = "";

                if (args.Length > 0)
                {
                    fileName = args[0];                    

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        if (fileName.ToLower() == "install")
                        {
                            string certificatePath = targetDir + "\\" + "roundrobin_com.crt";
                            AddCertificate(certificatePath);

                            string exePath = targetDir + "\\RoundRobin.exe";
                            CreateShortcut(exePath);

                            if (!string.IsNullOrEmpty(fileName))
                            {
                                Process p = new Process();

                                p.StartInfo.WorkingDirectory = targetDir;
                                p.StartInfo.FileName = "install_tdi_driver_x64.bat";
                                p.StartInfo.CreateNoWindow = false;
                                p.Start();
                            }
                        }
                        else if (fileName.ToLower() == "uninstall")
                        {
                            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                            string shortCutPath = Path.Combine(desktopPath, "RoundRobin.lnk");
                            File.Delete(shortCutPath);
                            
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                Process p = new Process();

                                p.StartInfo.WorkingDirectory = targetDir;
                                p.StartInfo.FileName = "uninstall_driver.bat";
                                p.StartInfo.CreateNoWindow = false;
                                p.Start();
                            }
                        }
                    }
                }
            } 

            catch (Exception ex)

            {
                ;
            }
        }


        private static void AddCertificate(string certificatePath)
        {
            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            store.Add(new X509Certificate2(X509Certificate2.CreateFromCertFile(certificatePath)));
            store.Close();
        }

        public static void CreateShortcut(string sourcePath)
        {
            bool runAsAdmin = true;
            IShellLink link = (IShellLink)new ShellLink();
            
            link.SetDescription("RoundRobin");
            link.SetPath(sourcePath);

            // save it
            IPersistFile file = (IPersistFile)link;
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            file.Save(Path.Combine(desktopPath, "RoundRobin.lnk"), false);

            if (runAsAdmin)
                using (var fs = new System.IO.FileStream(Path.Combine(desktopPath, "RoundRobin.lnk"), System.IO.FileMode.Open, FileAccess.ReadWrite))
                {
                    fs.Seek(21, SeekOrigin.Begin);
                    fs.WriteByte(0x22);
                }
        }

    }
}
