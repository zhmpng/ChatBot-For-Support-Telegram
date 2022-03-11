using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBotForSupport.DTO
{
    public class MessageDictionary
    {
        //id пользователя в Телеграм (отправивший обращение)
        public long UserId { get; set; }
        //id сообщения обращения от пользователя
        public int UserMessageId { get; set; }
    }
}
