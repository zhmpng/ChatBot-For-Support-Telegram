using System.ComponentModel;

namespace ChatBotForSupport
{
    internal static class Program
    {
        public static BackgroundWorker bw;
        private static readonly string publicKey = Configuration.Default.publicKey;
        private static int offset = 0;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());

            var inner = Task.Factory.StartNew(() =>  // вложенная задача
            {
                Bot();
            });
            Console.WriteLine("Бот был успешно запущен!");
        }

        private static async void Bot()
        {
            bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;

            var boolText = string.IsNullOrEmpty(publicKey);

            if (!boolText && bw.IsBusy != true)
            {
                bw.RunWorkerAsync(publicKey);
            }
        }

        static async void bw_DoWork(object sender, DoWorkEventArgs e)
        {
        }
    }
}