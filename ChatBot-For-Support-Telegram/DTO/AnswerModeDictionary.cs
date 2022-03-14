using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBotForSupport.DTO
{
    public class AnswerModeDictionary
    {
        //id пользователя в Телеграм (отправивший обращение)
        public int InlineMessageId { get; set; }
        //id сообщения обращения от пользователя
        public int ResponseNotificationId { get; set; }
    }
}
