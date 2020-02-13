using System.Threading.Tasks;
using System.Transactions;
using CSharpWars.DataAccess.Repositories.Interfaces;
using CSharpWars.Logic.Interfaces;
using CSharpWars.Model;

namespace CSharpWars.Logic
{
    public class DangerLogic : IDangerLogic
    {
        private readonly IRepository<Player> _playerRepository;
        private readonly IRepository<Bot> _botRepository;
        private readonly IRepository<Message> _messageRepository;

        public DangerLogic(
            IRepository<Player> playerRepository,
            IRepository<Bot> botRepository,
            IRepository<Message> messageRepository)
        {
            _playerRepository = playerRepository;
            _botRepository = botRepository;
            _messageRepository = messageRepository;
        }

        public async Task CleanupMessages()
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var messages = await _messageRepository.GetAll();
                await _messageRepository.Delete(messages);

                transaction.Complete();
            }
        }

        public async Task CleanupBots()
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await CleanupMessages();
                var bots = await _botRepository.GetAll();
                await _botRepository.Delete(bots);

                transaction.Complete();
            }
        }

        public async Task CleanupPlayers()
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await CleanupBots();
                var players = await _playerRepository.GetAll();
                await _playerRepository.Delete(players);

                transaction.Complete();
            }
        }
    }
}