﻿using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
public class Watcher
{
    public static string server = "", datenbank = "", batchPfad = "", watcherPfad = "";

    public static void Main()
    {
        Run();
    }

    private static void Run()
    {
        // Create a new FileSystemWatcher and set its properties.
        using (FileSystemWatcher watcher = new FileSystemWatcher())
        {
            watcher.Path = @"C:\temp\Daten\Vorgang";
            
            // Watch for changes in LastAccess and LastWrite times, and
            // the renaming of files or directories.
            watcher.NotifyFilter = NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName;

            // Only watch text files.
            watcher.Filter = "*.txt";

            // Add event handlers.
            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnRenamed;

            // Begin watching.
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            // Wait for the user to quit the program.
            Console.WriteLine("Press 'q' to quit the sample.");
            while (Console.Read() != 'q') ;
        }
    }

    // Define the event handlers.
    private static void OnChanged(object source, FileSystemEventArgs e)
    {

        // Specify what is done when a file is changed, created, or deleted.
        Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
        
        using (Process process = new Process())
        {
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");

            Console.WriteLine("Main thread: Start a second thread.");

            Thread t = new Thread(delegate () { ThreadProc(e.FullPath); });

            t.Start();

            /*

            /*
            process.StartInfo.FileName = "C:\\Users\\michel\\Source\\Repos\\FileWatcherService\\FileWatcherService\\bin\\Debug\\Test.bat";
            server = "df59r3";
            datenbank = "BNW";

            //process.StartInfo.Arguments = server + " " + datenbank + " " + e.FullPath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.Start();
            process.Close();
            /*

            server = "df59r3";
            datenbank = "BNW";

            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            Console.WriteLine(path);

            process.StartInfo.FileName = path + @"\CallDOCUframe.bat";
            process.StartInfo.Arguments = server + " " + datenbank + " " + e.FullPath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.Start();
            process.Close();

            */
        }
    }

    private static void OnCreated(object source, FileSystemEventArgs e)
    {
        Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");

        Console.WriteLine("Main thread: Start a second thread.");
        // The constructor for the Thread class requires a ThreadStart
        // delegate that represents the method to be executed on the
        // thread.  C# simplifies the creation of this delegate.
        Thread t = new Thread(delegate() { ThreadProc(e.FullPath); });
        
        // Start ThreadProc.  Note that on a uniprocessor, the new
        // thread does not get any processor time until the main thread
        // is preempted or yields.  Uncomment the Thread.Sleep that
        // follows t.Start() to see the difference.
        t.Start();
        //Thread.Sleep(0);

        for (int i = 0; i < 20; i++)
        {
            Console.WriteLine("Main thread: Do some work.");
            Thread.Sleep(100);
        }
    }

    public static void ThreadProc(String path)
    {
        Console.WriteLine("Thread path: " + path);
        bool isOpen = true;
        while (isOpen)
        {
            Thread.Sleep(500);
            isOpen = false;
            FileStream fs = null;
            try
            {
                fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
                Console.WriteLine("Thread: is not open");
                //Datei ist nicht geöffnet -> kann importiert werden
            }
            catch(IOException)
            {
                //Datei ist geöffnet -> weiter warten
                isOpen = true;
                Console.WriteLine("Thread: is extern open");
            }
            finally
            {
                Console.WriteLine("finally");
                if (fs != null)
                    fs.Close();
            }
        }
        // Aufruf Schnittstellenmakro DOCUframe
        Console.WriteLine("Thread: Open DF");
    }

    private static void OnRenamed(object source, RenamedEventArgs e) =>
        // Specify what is done when a file is renamed.
        Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
}