namespace hw16
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string token = "7361498838:AAEN7kqKX2CfDTTL8iUdq44bb5EdvUze6wQ";
            var bot = new RecipeBot(token);

            using var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            await bot.StartBotAsync(cancellationToken);

            Console.ReadLine();
            cts.Cancel();
        }
    }
}
