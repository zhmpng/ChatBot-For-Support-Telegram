using ChatBotForSupport.UpdateHandlers;
using System.ComponentModel;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChatBotForSupport
{
    internal static class Program
    {
        public static BackgroundWorker _bw;
        private static readonly string _publicKey = Configuration.Default.publicKey;
        private static int _offset = 0;
        private static bool _stopProgram = false;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //ApplicationConfiguration.Initialize();
            //Application.Run(new Form1());
            
            var inner = Task.Factory.StartNew(() =>
            {
                Bot();
            });
            Console.WriteLine("Бот был успешно запущен!");
            inner.Wait();
            do { } while (!_stopProgram);
        }

        private static async void Bot()
        {
            _bw = new BackgroundWorker();
            _bw.DoWork += _bw_DoWork;

            var boolText = string.IsNullOrEmpty(_publicKey);

            if (!boolText && _bw.IsBusy != true)
            {
                _bw.RunWorkerAsync(_publicKey);
            }
        }

        static async void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            var key = e.Argument as String;
            var Bot = new TelegramBotClient(key);
            await Bot.SetWebhookAsync("");
            var preUpdates = await Bot.GetUpdatesAsync(-1);
            _offset = preUpdates.Select(s => s.Id).LastOrDefault();
            _offset += 1;

            try {
                bool repeat = true;
                while (repeat)
                {
                    var updates = new Update[0];
                    try
                    {
                        updates = await Bot.GetUpdatesAsync(_offset);
                    }
                    catch (TaskCanceledException)
                    {
                        // Don't care
                    }
                    catch (Exception ex)
                    {
                        await Bot.SendTextMessageAsync("441224506", "ERROR WHILE GETTIGN UPDATES - " + ex);
                    }
                    foreach (var update in updates)
                    {
                        _offset = update.Id + 1;
                        switch (update.Type)
                        {
                            case UpdateType.Message:
                                if (update.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text && update.Message.Text.Replace("@WeekendFacePolice_bot", "").ToLower() == ("/restart"))
                                {
                                    repeat = false;
                                    _offset = 0;
                                    throw new Exception($"Бот был перезапущен при помощи команды /restart , данную команду запустил @{update.Message.From.Username} - {update.Message.From.FirstName}");
                                }
                                if (update.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text && update.Message.Text.Replace("@WeekendFacePolice_bot", "").ToLower() == ("/stop"))
                                {
                                    repeat = false;
                                    _offset = 0;
                                    _stopProgram = true;
                                    throw new Exception($"Бот был перезапущен при помощи команды /restart , данную команду запустил @{update.Message.From.Username} - {update.Message.From.FirstName}");
                                }
                                MessageBase.MessageHandler(update, Bot);
                                break;
                            case UpdateType.CallbackQuery:
                                break;
                            case UpdateType.InlineQuery:
                                break;
                            case UpdateType.EditedMessage:
                                break;
                            case UpdateType.EditedChannelPost:
                                break;
                            case UpdateType.ChosenInlineResult:
                                break;
                            case UpdateType.ChannelPost:
                                break;
                            case UpdateType.Poll:
                                break;
                            case UpdateType.PollAnswer:
                                break;
                            case UpdateType.PreCheckoutQuery:
                                break;
                            case UpdateType.ShippingQuery:
                                break;
                            case UpdateType.Unknown:
                                break;
                            case UpdateType.MyChatMember:
                                break;
                            case UpdateType.ChatJoinRequest:
                                break;
                            case UpdateType.ChatMember:
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Bot.SendTextMessageAsync("441224506", ex.Message + "-" + ex.StackTrace);
                _bw.RunWorkerAsync(_publicKey);
            }
        }
    }
}