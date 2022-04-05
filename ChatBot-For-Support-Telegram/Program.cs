using BeOpen.Common.Cache;
using ChatBotForSupport.DTO;
using ChatBotForSupport.UpdateHandlers;
using System.ComponentModel;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChatBotForSupport
{
    internal static class Program
    {
        //AdminsDictionary Key - id админа в Телеграмм | Value - null
        public static readonly SerializableCache<long, string> AdminsDictionary = new SerializableCache<long, string>($"{Directory.GetCurrentDirectory()}\\adminsDictionary.json");
        //MessageDictionary Key - id сообщения у админа об обращение | Value - от кого поступило обращение
        public static readonly SerializableCache<long, MessageDictionary> MessageDictionary = new SerializableCache<long, MessageDictionary>($"{Directory.GetCurrentDirectory()}\\messageDictionary.json");
        //AnswerModeDictionary Key - Для кого включен режим ответа | Value - на какое сообщение включен режим
        public static readonly SerializableCache<long, AnswerModeDictionary> AnswerModeDictionary = new SerializableCache<long, AnswerModeDictionary>($"{Directory.GetCurrentDirectory()}\\answerModeDictionary.json");
        //BotPublicKeyDictionary Key - Название ключа | Value - Ключ Апи бота
        public static readonly SerializableCache<string, string> BotPublicKeyDictionary = new SerializableCache<string, string>($"{Directory.GetCurrentDirectory()}\\botPublicKeyDictionary.json");

        public static BackgroundWorker _bw;
        private static readonly string _publicKey = BotPublicKeyDictionary.KeyValuePair.FirstOrDefault().Value;//Configuration.Default.publicKey;
        private static int _offset = 0;
        public static bool StopProgram = false;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!AdminsDictionary.Contains(703054356))
                AdminsDictionary.AddOrUpdate(703054356, "");//441224506
            var inner = Task.Factory.StartNew(() =>
            {
                Bot();
            });
            Console.WriteLine("Бот был успешно запущен!");
            inner.Wait();
            do { } while (true);
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
                                if (AnswerModeDictionary.Contains(update.Message.From.Id))
                                    await MessageBase.AdminResponseMessageHandlerAsync(update, Bot);
                                else
                                {
                                    await MessageBase.MessageComandHandlerAsync(update, Bot);
                                    await MessageBase.MessageHandlerAsync(update, Bot);
                                }
                                break;
                            case UpdateType.CallbackQuery:
                                await CallbackBase.CallbackHandlerAsync(update, Bot);
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
                _offset = 0;
                await Bot.SendTextMessageAsync("441224506", ex.Message + "-" + ex.StackTrace);
                _bw.RunWorkerAsync(_publicKey);
            }
        }
    }
}