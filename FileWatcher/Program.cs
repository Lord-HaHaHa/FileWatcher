
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
            watcher.Path = @"\\df59r3\C\TEMP\VorgangExport\Vorgang";
            
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
            //watcher.Created += OnCreated;
            //watcher.Deleted += OnChanged;
            //watcher.Renamed += OnRenamed;

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
        Thread.Sleep(100);
        while (isOpen)
        {
            Console.WriteLine("Waiting");
            Thread.Sleep(1000);
            isOpen = false;
            try
            {
                using(Stream stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite))
                {
                    Console.WriteLine("Thread: open");
                    stream.Close();
                    using (Process process = new Process())
                    {
                        //Import in DF
                        process.StartInfo.FileName = @"C:\Users\michel\Source\Repos\FileWatcherService\FileWatcherService\bin\Debug\CallDocuFrame.bat";
                        server = "df59r3";
                        datenbank = "BNW";

                        process.StartInfo.Arguments = server + " " + datenbank + " " + path;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardInput = true;
                        process.Start();
                        process.Close();
                    }
                }
            }
            catch(IOException)
            {
                //Datei ist geöffnet -> weiter warten
                isOpen = true;
                Console.WriteLine("Thread: Cant open");
            }
        }
    }

    private static void OnRenamed(object source, RenamedEventArgs e) =>
        // Specify what is done when a file is renamed.
        Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
}