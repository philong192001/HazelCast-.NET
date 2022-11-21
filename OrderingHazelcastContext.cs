using Hazelcast;
using Hazelcast.DistributedObjects;
using Hazelcast.Transactions;

namespace HazelcastDemo
{
    public class OrderingHazelcastContext
    {
        private readonly IHazelcastClient _client;
        private ITransactionContext _transaction;

        public OrderingHazelcastContext(IHazelcastClient client)
        {
            _client = client;
        }

        public async Task<ITransactionContext> BeginTransactionAsync()
        {
            if (_transaction != null) return null;
            var client =  HazelcastClientFactory.GetNewStartingClient().Client;
            _transaction = await client.BeginTransactionAsync(new TransactionOptions() { Type = TransactionOptions.TransactionType.OnePhase });
            return _transaction;
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null) return;
            try
            {
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        private async Task RollbackTransactionAsync()
        {
            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<IHMap<string, string>> GetMapAsync(string IdentityName)
        {
            if (IdentityName == null) throw new ArgumentNullException(nameof(IdentityName));
            return await _client.GetMapAsync<string, string>(IdentityName);
        }

        public async Task<IHTxMap<string, string>> GetTransactionableMapAsync(string IdentityName)
        {
            if (IdentityName == null) throw new ArgumentNullException(nameof(IdentityName));
            if (_transaction == null) throw new ArgumentNullException(nameof(_transaction));

            return await _transaction.GetMapAsync<string, string>(IdentityName);
        }
    }
}
