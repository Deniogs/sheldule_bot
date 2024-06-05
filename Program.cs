using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using System.Net.Mime;
using File = System.IO.File;
using System.Data;
using System.Data.OleDb;
using System.Runtime.Intrinsics.X86;
using schedule_bot;





class Program
{
    
    
    
    // Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
    private static ITelegramBotClient _botClient;
    
    // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
    private static ReceiverOptions _receiverOptions;

    DateTime date;
    static string facultyName;
    static string speciality;
    static int course;
    
    static async Task Main()
    {
        _receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
        {
            AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов, о них подробнее расказано тут https://core.telegram.org/bots/api#update
            {
                UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
                UpdateType.CallbackQuery // Inline кнопки
            },
            // Параметр, отвечающий за обработку сообщений, пришедших за то время, когда ваш бот был оффлайн
            // True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
            ThrowPendingUpdates = true, 
        };
        
        using var cts = new CancellationTokenSource();
        
        _botClient = new TelegramBotClient("7318126142:AAETLwM6ZqdgtUKegTbh51tEIb-e5zysXyI"); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather
        
        // UpdateHander - обработчик приходящих Update`ов
        // ErrorHandler - обработчик ошибок, связанных с Bot API
        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота
        
        var me = await _botClient.GetMeAsync(); // Создаем переменную, в которую помещаем информацию о нашем боте.
        Console.WriteLine($"{me.FirstName} запущен!");
        
        await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
    }
    
    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        //string connectionString =
        //   "provider=Microsoft.Jet.OLEDB.4.0;" +
        //   @"data source=D:\works\schedule_bot\schedule_bot\TGBot_pups_0_1ver.mdb";
        //OleDbConnection myOleDbConnection = new OleDbConnection(connectionString);
        //OleDbCommand myOleDbCommand = myOleDbConnection.CreateCommand();
        
        // Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
        try
        {
            // Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    // эта переменная будет содержать в себе все связанное с сообщениями
                    var message = update.Message;
                    
                    var oneTimeKeyboard = new ReplyKeyboardRemove();
                    
                    // From - это от кого пришло сообщение
                    var user = message.From;
                    
                    // Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
                    Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");
                    
                    // Chat - содержит всю информацию о чате
                    var chat = message.Chat;
                    
                    // Добавляем проверку на тип Message
                    switch (message.Type)
                    {
                        // Тут понятно, текстовый тип
                        case MessageType.Text:
                        {
                            // тут обрабатываем команду /start, остальные аналогично
                            if (message.Text == "/start" || message.Text == "Веруться на главную")
                            {
                                CallStartMenu(botClient, chat);
                            }

                            if (message.Text == "Я студент" || message.Text == "Назад к выбору курса")
                            {
                                SelectCourse(botClient, chat);
                            }
                            
                            if (message.Text == "Я преподаватель" || message.Text == "Назад к выбору курса")
                            {
                                SelectCourse(botClient, chat);
                            }
                            
                            if (message.Text == "Оставить отзыв")
                            {
                                SendReviews(botClient, chat);
                                
                                string filePath = "D://1.txt";

                                
                                
                                string reviews = message.Text;
                                
                                /*botClient.OnCallbackQuery += async (sender, args) =>
                                {
                                    // Проверяем, есть ли у сообщения кнопка
                                    if (args.CallbackQuery.Message.ReplyMarkup == null)
                                        return;

                                    // Получаем текст сообщения
                                    var messageText = args.CallbackQuery.Message.Text;

                                    // Отправляем ответное сообщение
                                    await botClient.SendTextMessageAsync(args.CallbackQuery.Message.Chat.Id, $"Вы нажали кнопку: {args.CallbackQuery.Data} в сообщении: {messageText}");
                                };
                                
                                using (StreamWriter fileStream = File.Exists(filePath) ? File.AppendText(filePath) : File.CreateText(filePath))
                                {
                                    fileStream.WriteLine(reviews);
                                }
                                
                                await botClient.SendTextMessageAsync(
                                    chat.Id,
                                    "Напишите ваш отзыв или предложение"); // передаем клавиатуру в параметр replyMarkup
                                
                                CallStartMenu(botClient, chat);
                                
                                return;*/
                            }
                            
                            if (message.Text == "Отправить")
                            {
                                CallStartMenu(botClient, chat);
                            }
                            
                            if (message.Text == "1 курс" || message.Text == "2 курс" || message.Text == "3 курс" || message.Text == "4 курс" || message.Text == "Назад к выбору факультета" || message.Text == "Вернуться к выбору факультета")
                            {
                                course = Convert.ToInt32(message.Text[0])-48;
                                Console.WriteLine(course);
                                
                                SelectFaculty(botClient, chat);
                            }
                            
                            //Специальности тех фака
                            if (message.Text == "Технический факультет")
                            {
                                facultyName = "Технический факультет";
                                SelectSpecialTF(botClient, chat);
                            }
                            
                            if (message.Text == "Факультет гуманитарно-педагогических наук")
                            {
                                facultyName = "Факультет гуманитарно-педагогических наук";
                                SelectSpecialPed(botClient, chat);
                            }
                            
                            if (message.Text == "Факультет управления, экономики и права")
                            {
                                facultyName = "Факультет управления, экономики и права";
                                SelectSpecialEco(botClient, chat);
                            }
                            
                            if (message.Text == "Агротехнологический факультет")
                            {
                                facultyName = "Агротехнологический факультет";
                                SelectSpecialAgr(botClient, chat);
                            }
                            
                            if (message.Text == "Факультет естественных наук")
                            {
                                facultyName = "Факультет естественных наук";
                                SelectSpecialSci(botClient, chat);
                            }
                            
                            if (facultyName == "Технический факультет" && (message.Text == "ФИИТ" || message.Text == "ПОВИ" || message.Text == "ИБ" || message.Text == "ПМ" || message.Text == "АИ" || message.Text == "ЭЭ"))
                            {
                                speciality = message.Text;
                                
                                SelectActions(botClient, chat);
                            }

                            if (facultyName == "Факультет гуманитарно-педагогических наук" && (message.Text == "СДО,ПОД,ПДЛ,ПДН" || message.Text == "ПСИ" || message.Text == "ПИО, ИСТ, УОО" || message.Text == "СОЦ" || message.Text == "ПОМ,ПОХ,ХИБ" ||  message.Text == "ПСП"|| message.Text == "ПОФ, ПТС" || message.Text == "ПОН,ПНИ"))
                            {
                                speciality = message.Text;
                                SelectActions(botClient, chat);
                            }
                            
                            if (facultyName == "Факультет управления, экономики и права" && (message.Text == "ЮР" || message.Text == "ГМУ, МН" || message.Text == "МК,УА" || message.Text == "ФК" ||  message.Text == "ЭП"|| message.Text == "МБ ГМУ,МН,ЮР,УА,ЭП,ФК" || message.Text == "ПОНБ, ЭБ"))
                            {
                                speciality = message.Text;
                                SelectActions(botClient, chat);
                            }
                            
                            if (facultyName == "Агротехнологический факультет" && (message.Text == "АГ" || message.Text == "ТР" || message.Text == "ТОП" || message.Text == "ППР" || message.Text == "ТБ" || message.Text == "ТМ"))
                            {
                                speciality = message.Text;
                                
                                SelectActions(botClient, chat);
                            }
                            
                            if (facultyName == "Факультет управления, экономики и права" && (message.Text == "ЮР" || message.Text == "ГМУ, МН" || message.Text == "МК,УА" || message.Text == "ФК" ||  message.Text == "ЭП"|| message.Text == "МБ ГМУ,МН,ЮР,УА,ЭП,ФК" || message.Text == "ПОНБ, ЭБ"))
                            {
                                speciality = message.Text;
                                SelectActions(botClient, chat);
                            }
                            
                            if (facultyName == "Факультет естественных наук" && (message.Text == "1ПБХ-ХБ-БМД" || message.Text == "ПБХ-ПБП-ПХБ-БФР" || message.Text == "ГЕОГРАФИЯ" || message.Text == "ЭКОЛОГИЯ" ||  message.Text == "ФИЛОЛОГИЯ"))
                            {
                                speciality = message.Text;
                                SelectActions(botClient, chat);
                            }

                            if (message.Text == "Рассписание на завтра")
                            {
                                //string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\\works\\schedule_bot\\schedule_bot\\TGBot_pups_0_1ver.mdb";
                                AccessDatabase database = new AccessDatabase();

                                database.ReadData();

                                /*DateTime dateTime = DateTime.Now;

                                Console.WriteLine(dateTime.DayOfWeek);

                                string dayOfWeek = ConvertDayOfWeekToRussian(dateTime.DayOfWeek);

                                myOleDbCommand.CommandText =
                                    "SELECT рассписание.[день недели], рассписание.время, предметы.[назв пред], препод.ФИО, рассписание.аудитория, рассписание.[неделя(цвет)]" +
                                    "FROM факультет INNER JOIN (препод INNER JOIN ((группа INNER JOIN предметы ON группа.id = предметы.группа) INNER JOIN рассписание ON предметы.id = рассписание.предмет) ON препод.id = предметы.препод) ON факультет.id = группа.Факультет " +
                                    "WHERE рассписание.день недели = 'dayOfWeek'" +
                                    "{ORDER BY рассписание.время";

                                myOleDbConnection.Open();

                                OleDbDataReader myOleDbDataReader = myOleDbCommand.ExecuteReader();

                                myOleDbDataReader.Read();
                                */
                            }
                            return;
                        }

                        // Добавил default, чтобы показать вам разницу типов Message
                        default:
                        {
                            await botClient.SendTextMessageAsync(
                                chat.Id,
                                "Используй только текст!");
                            return;
                        }
                    }
                }
                
                case UpdateType.CallbackQuery:
                {
                    var callbackQuery = update.CallbackQuery;
                    //ChangeFaculty(callbackQuery, botClient);
                    
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
    
    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };
        
        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

    static async Task CallStartMenu(ITelegramBotClient botClient, Chat chat) //Вызов стартового меню с выбором студент вы, или препод
    {
        // Тут все аналогично Inline клавиатуре, только меняются классы
        // НО! Тут потребуется дополнительно указать один параметр, чтобы
        // клавиатура выглядела нормально, а не как абы что
                                
        var replyKeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Я студент"),
                    new KeyboardButton("Я преподаватель"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Оставить отзыв")
                }
            })
        {
            // автоматическое изменение размера клавиатуры, если не стоит true,
            // тогда клавиатура растягивается чуть ли не до луны,
            // проверить можете сами
            ResizeKeyboard = true,
        };
        
        await botClient.SendTextMessageAsync(
            chat.Id,
            "Вы студент или преподаватель",
            replyMarkup: replyKeyboard); // передаем клавиатуру в параметр replyMarkup
        return;
    }

    static async Task SelectFaculty(ITelegramBotClient botClient, Chat chat) //Вызов стартового меню с выбором студент вы, или препод
    {
        // Тут все аналогично Inline клавиатуре, только меняются классы
        // НО! Тут потребуется дополнительно указать один параметр, чтобы
        // клавиатура выглядела нормально, а не как абы что
        
        var replyKeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Факультет гуманитарно-педагогических наук"),
                    new KeyboardButton("Факультет естественных наук"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Технический факультет"),
                    new KeyboardButton("Агротехнологический факультет"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Факультет управления, экономики и права"),
                    new KeyboardButton("Веруться на главную"),
                }
            })
        {
            // автоматическое изменение размера клавиатуры, если не стоит true,
            // тогда клавиатура растягивается чуть ли не до луны,
            // проверить можете сами
            ResizeKeyboard = true,
        };
                                
                                
        await botClient.SendTextMessageAsync(
            chat.Id,
            "Выберите факультет",
            replyMarkup: replyKeyboard); // передаем клавиатуру в параметр replyMarkup
        return;
    }

    static async Task SelectSpecialTF(ITelegramBotClient botClient, Chat chat) //Вызов стартового меню с выбором студент вы, или препод
    {
        // Тут все аналогично Inline клавиатуре, только меняются классы
        // НО! Тут потребуется дополнительно указать один параметр, чтобы
        // клавиатура выглядела нормально, а не как абы что

        List<KeyboardButton[]> keyboard = null;
        var replyKeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("ЭЭ"),
                    new KeyboardButton("АИ"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("ПМ"),
                    new KeyboardButton("ИБ"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("ПОВИ"),
                    new KeyboardButton("ФИИТ"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Назад к выбору факультета"),
                }
            });
        
        //var replyKeyboard = new ReplyKeyboardMarkup(keyboard);
        
        replyKeyboard.ResizeKeyboard = true;
        
        await botClient.SendTextMessageAsync(
            chat.Id,
            "Выберите специальность",
            replyMarkup: replyKeyboard); // передаем клавиатуру в параметр replyMarkup
    }
    
    static async Task SelectActions(ITelegramBotClient botClient, Chat chat) //Вызов стартового меню с выбором студент вы, или препод
    {
        // Тут все аналогично Inline клавиатуре, только меняются классы
        // НО! Тут потребуется дополнительно указать один параметр, чтобы
        // клавиатура выглядела нормально, а не как абы что

        List<KeyboardButton[]> keyboard = null;
        var replyKeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Рассписание на неделю"),
                    new KeyboardButton("Рассписание на завтра"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Подписаться на ежедневную рассылку расписания"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Вернуться к выбору факультета"),
                }
            });
        
        //var replyKeyboard = new ReplyKeyboardMarkup(keyboard);
        
        replyKeyboard.ResizeKeyboard = true;
        
        await botClient.SendTextMessageAsync(
            chat.Id,
            "Выберите действие",
            replyMarkup: replyKeyboard); // передаем клавиатуру в параметр replyMarkup
    }
    
    static async Task SelectCourse(ITelegramBotClient botClient, Chat chat) //Вызов стартового меню с выбором студент вы, или препод
    {
        // Тут все аналогично Inline клавиатуре, только меняются классы
        // НО! Тут потребуется дополнительно указать один параметр, чтобы
        // клавиатура выглядела нормально, а не как абы что

        List<KeyboardButton[]> keyboard = null;
        var replyKeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("1 курс"),
                    new KeyboardButton("2 курс"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("3 курс"),
                    new KeyboardButton("4 курс"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Веруться на главную"),
                }
            });
        
        //var replyKeyboard = new ReplyKeyboardMarkup(keyboard);
        
        replyKeyboard.ResizeKeyboard = true;
        
        await botClient.SendTextMessageAsync(
            chat.Id,
            "Выберите курс",
            replyMarkup: replyKeyboard); // передаем клавиатуру в параметр replyMarkup
    }
    
    static async Task SelectSpecialPed(ITelegramBotClient botClient, Chat chat) //Вызов стартового меню с выбором студент вы, или препод
    {
        // Тут все аналогично Inline клавиатуре, только меняются классы
        // НО! Тут потребуется дополнительно указать один параметр, чтобы
        // клавиатура выглядела нормально, а не как абы что

        List<KeyboardButton[]> keyboard = null;
        var replyKeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("СДО,ПОД,ПДЛ,ПДН"),
                    new KeyboardButton("ПСИ"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("ПИО, ИСТ, УОО"),
                    new KeyboardButton("СОЦ"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("ПОМ,ПОХ,ХИБ"),
                    new KeyboardButton("ПСП"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("ПОФ, ПТС"),
                    new KeyboardButton("ПОН,ПНИ"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Назад к выбору факультета"),
                }
            });
        
        //var replyKeyboard = new ReplyKeyboardMarkup(keyboard);
        
        replyKeyboard.ResizeKeyboard = true;
        
        await botClient.SendTextMessageAsync(
            chat.Id,
            "Выберите специальность",
            replyMarkup: replyKeyboard); // передаем клавиатуру в параметр replyMarkup
    }
    
    static async Task SelectSpecialEco(ITelegramBotClient botClient, Chat chat) //Вызов стартового меню с выбором студент вы, или препод
    {
        // Тут все аналогично Inline клавиатуре, только меняются классы
        // НО! Тут потребуется дополнительно указать один параметр, чтобы
        // клавиатура выглядела нормально, а не как абы что

        List<KeyboardButton[]> keyboard = null;
        var replyKeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("ЮР"),
                    new KeyboardButton("ГМУ, МН"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("МК,УА"),
                    new KeyboardButton("ФК"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("ЭП"),
                    new KeyboardButton("МБ ГМУ,МН,ЮР,УА,ЭП,ФК"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("ПОНБ, ЭБ"),
                    new KeyboardButton("Назад к выбору факультета"),
                }
            });
        
        //var replyKeyboard = new ReplyKeyboardMarkup(keyboard);
        
        replyKeyboard.ResizeKeyboard = true;
        
        await botClient.SendTextMessageAsync(
            chat.Id,
            "Выберите специальность",
            replyMarkup: replyKeyboard); // передаем клавиатуру в параметр replyMarkup
    }
    
    static async Task SelectSpecialAgr(ITelegramBotClient botClient, Chat chat) //Вызов стартового меню с выбором студент вы, или препод
    {
        // Тут все аналогично Inline клавиатуре, только меняются классы
        // НО! Тут потребуется дополнительно указать один параметр, чтобы
        // клавиатура выглядела нормально, а не как абы что

        List<KeyboardButton[]> keyboard = null;
        var replyKeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("АГ"),
                    new KeyboardButton("ТР"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("ТОП"),
                    new KeyboardButton("ППР"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("ТБ"),
                    new KeyboardButton("ТМ"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Назад к выбору факультета"),
                }
            });
        
        //var replyKeyboard = new ReplyKeyboardMarkup(keyboard);
        
        replyKeyboard.ResizeKeyboard = true;
        
        await botClient.SendTextMessageAsync(
            chat.Id,
            "Выберите специальность",
            replyMarkup: replyKeyboard); // передаем клавиатуру в параметр replyMarkup
    }

    static async Task SendReviews(ITelegramBotClient botClient, Chat chat) //Вызов стартового меню с выбором студент вы, или препод
    {
        List<KeyboardButton[]> keyboard = null;
        var replyKeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Отправить")
                }
            });
        
        replyKeyboard.ResizeKeyboard = true;
        
        await botClient.SendTextMessageAsync(
            chat.Id,
            "Напишите отзыв",
            replyMarkup: replyKeyboard); // передаем клавиатуру в параметр replyMarkup
    }
    
    static async Task SelectSpecialSci(ITelegramBotClient botClient, Chat chat) //Вызов стартового меню с выбором студент вы, или препод
    {
        // Тут все аналогично Inline клавиатуре, только меняются классы
        // НО! Тут потребуется дополнительно указать один параметр, чтобы
        // клавиатура выглядела нормально, а не как абы что

        List<KeyboardButton[]> keyboard = null;
        var replyKeyboard = new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("1ПБХ-ХБ-БМД"),
                    new KeyboardButton("ПБХ-ПБП-ПХБ-БФР"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("ГЕОГРАФИЯ"),
                    new KeyboardButton("ЭКОЛОГИЯ"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("ФИЛОЛОГИЯ"),
                    new KeyboardButton("Назад к выбору факультета"),
                }
            });
        
        //var replyKeyboard = new ReplyKeyboardMarkup(keyboard);
        
        replyKeyboard.ResizeKeyboard = true;
        
        await botClient.SendTextMessageAsync(
            chat.Id,
            "Выберите специальность",
            replyMarkup: replyKeyboard); // передаем клавиатуру в параметр replyMarkup
    }
    public static string ConvertDayOfWeekToRussian(DayOfWeek dayOfWeek)
    {
        switch (dayOfWeek)
        {
            case DayOfWeek.Sunday:
                return "Воскресенье";
            case DayOfWeek.Monday:
                return "Понедельник";
            case DayOfWeek.Tuesday:
                return "Вторник";
            case DayOfWeek.Wednesday:
                return "Среда";
            case DayOfWeek.Thursday:
                return "Четверг";
            case DayOfWeek.Friday:
                return "Пятница";
            case DayOfWeek.Saturday:
                return "Суббота";
            default:
                throw new ArgumentException("Неизвестный день недели", nameof(dayOfWeek));
        }
    }

}