using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace hw16
{
    public class RecipeBot
    {
        private readonly TelegramBotClient _botClient;
        private readonly Dictionary<string, string> RecipeBook;
        public RecipeBot(string token)
        {
            _botClient = new TelegramBotClient(token);
            RecipeBook = new Dictionary<string, string>
            {
                { "pasta", "Ingredients: pasta, tomato sauce, cheese. \nSteps: Boil pasta, prepare sauce, mix and serve." },
                { "salad", "Ingredients: lettuce, tomato, cucumber. \nSteps: Chop vegetables, mix and serve." },
                { "soup", "Ingredients: Potatoes, Carrots, Onion.\nSteps: Boil vegetables, blend, and serve." },
                { "meat", "Ingredients: Chicken, Garlic, Olive oil.\nSteps: Roast chicken with garlic and olive oil." },
                { "dessert", "\"Ingredients: 200 g dark chocolate, 150 g butter, 150 g sugar, 3 eggs, 100 g flour.\nSteps: Melt chocolate and butter. Beat eggs with sugar. Mix in chocolate, then add flour. Bake at 180°C for 25-30 minutes.\"" }
            };
        }

        public async Task StartBotAsync(CancellationToken cancellationToken)
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );

            Console.WriteLine($"Bot started ...");
        }

        private async Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine(exception.Message);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message && update.Message?.Text is { } messageText)
            {
                var chatId = update.Message.Chat.Id;
                var firstName = update.Message?.From.FirstName ?? "User";
                switch (messageText.ToLower())
                {
                    case "/start":
                        await _botClient.SendTextMessageAsync(chatId, $"Hello {firstName}, I'm a recipe book bot, and I will help you find the recipe you want." +
                            $"Please press any button below to see all the dishes, or you can send me the name of a dish, and if I have that dish, I will send you the recipe.",
                            replyMarkup: GetInlineKeyboard()
                            );
                        break;
                    default:
                        if (RecipeBook.TryGetValue(messageText.ToLower(), out var recipe))
                        {
                            await _botClient.SendTextMessageAsync(chatId, $"Here is the recipe for {messageText}:\n{recipe}");
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(chatId, $"Sorry, I don't have a recipe for {messageText}.");
                        }
                        break;
                }
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                await SendRecipe(update);
            }
            
        }

        private async Task SendRecipe(Update update)
        {
            if (update.CallbackQuery == null)
                return;

            var callbackQuery = update.CallbackQuery;
            var callbackData = callbackQuery.Data;
            var chatId = callbackQuery.Message.Chat.Id;
            switch (callbackData)
            {
                case "category_salad":
                    await _botClient.SendTextMessageAsync(chatId, "Here is a salad recipe:\nIngredients: Lettuce, Tomato, Cucumber.\nSteps: Chop the vegetables, mix and serve.");
                    break;

                case "category_soup":
                    await _botClient.SendTextMessageAsync(chatId, "Here is a soup recipe:\nIngredients: Potatoes, Carrots, Onion.\nSteps: Boil vegetables, blend, and serve.");
                    break;

                case "category_meat":
                    await _botClient.SendTextMessageAsync(chatId, "Here is a meat dish recipe:\nIngredients: Chicken, Garlic, Olive oil.\nSteps: Roast chicken with garlic and olive oil.");
                    break;

                case "category_desserts":
                    await _botClient.SendTextMessageAsync(chatId, "Here is a chocolate cake recipe:\n" +
                        "Ingredients: 200 g dark chocolate, 150 g butter, 150 g sugar, 3 eggs, 100 g flour.\n" +
                        "Steps: Melt chocolate and butter. Beat eggs with sugar. Mix in chocolate, then add flour. Bake at 180°C for 25-30 minutes.");
                    break;

                case "show_all_recipes":
                    await _botClient.SendTextMessageAsync(chatId, "Here are all the available categories:\n1. Salad 🥗\n2. Soup 🍲\n3. Meat 🍖\n4. Dessert\nSend me any category, and I will reply with a recipe.");
                    break;

                default:
                    await _botClient.SendTextMessageAsync(chatId, "Sorry, I don't have a recipe for this category.");
                    break;
            }
        }

        private InlineKeyboardMarkup GetInlineKeyboard()
        {
            return new InlineKeyboardMarkup(
                new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Salads 🥗", callbackData: "category_salad"),
                        InlineKeyboardButton.WithCallbackData(text: "Soups 🍲", callbackData: "category_soup"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Meat Dishes 🍖", callbackData: "category_meat"),
                        InlineKeyboardButton.WithCallbackData(text: "Desserts 🍰", callbackData: "category_desserts"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "See All Recipes 🔍", callbackData: "show_all_recipes"),
                    }
                }
            );
        }
    }
}
