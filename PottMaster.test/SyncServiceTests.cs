using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PottMaster.Services;
using System.Linq;
using PottMasterLib.Services;
using PottMasterLib.Models;

namespace PottMaster.Tests.Services
{
    /// <summary>
    /// Unit tests for SyncService.
    /// Note: Tests involving Supabase client interactions are limited to the sealed nature
    /// of Supabase.Client. For comprehensive testing of sync operations, consider:
    /// 1. Integration tests with a test Supabase instance
    /// 2. Refactoring to use an ISupabaseClient wrapper interface
    /// </summary>
    [TestFixture]
    public class SyncServiceTests
    {
        private Mock<IDbService> _dbServiceMock;
        private ISyncService _syncService;

        [SetUp]
        public void SetUp()
        {
            _dbServiceMock = new Mock<IDbService>();
            _syncService = CreateSyncServiceWithMockDb();
        }

        [Test]
        public void GetPendingSyncCount_ReturnsZeroWhenNoWorks()
        {
            // Arrange
            _dbServiceMock.Setup(x => x.GetAllAsync<LocalWork>())
                .ReturnsAsync(new List<LocalWork>());

            // Act
            var count = _syncService.GetPendingSyncCount();

            // Assert
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void GetPendingSyncCount_CountsPendingWorks()
        {
            // Arrange
            var works = new List<LocalWork>
            {
                new LocalWork { Id = "1", SyncStatus = "PENDING" },
                new LocalWork { Id = "2", SyncStatus = "SYNCED" },
                new LocalWork { Id = "3", SyncStatus = "PENDING" }
            };
            
            _dbServiceMock.Setup(x => x.GetAllAsync<LocalWork>())
                .ReturnsAsync(works);

            // Act
            var count = _syncService.GetPendingSyncCount();

            // Assert
            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void GetPendingSyncCount_CountsErrorWorks()
        {
            // Arrange
            var works = new List<LocalWork>
            {
                new LocalWork { Id = "1", SyncStatus = "ERROR" },
                new LocalWork { Id = "2", SyncStatus = "SYNCED" }
            };
            
            _dbServiceMock.Setup(x => x.GetAllAsync<LocalWork>())
                .ReturnsAsync(works);

            // Act
            var count = _syncService.GetPendingSyncCount();

            // Assert
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void GetPendingSyncCount_CountsBothPendingAndErrorWorks()
        {
            // Arrange
            var works = new List<LocalWork>
            {
                new LocalWork { Id = "1", SyncStatus = "PENDING" },
                new LocalWork { Id = "2", SyncStatus = "ERROR" },
                new LocalWork { Id = "3", SyncStatus = "SYNCED" },
                new LocalWork { Id = "4", SyncStatus = "SYNCING" }
            };
            
            _dbServiceMock.Setup(x => x.GetAllAsync<LocalWork>())
                .ReturnsAsync(works);

            // Act
            var count = _syncService.GetPendingSyncCount();

            // Assert
            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void GetPendingSyncCount_HandlesExceptionGracefully()
        {
            // Arrange
            _dbServiceMock.Setup(x => x.GetAllAsync<LocalWork>())
                .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var count = _syncService.GetPendingSyncCount();

            // Assert
            Assert.That(count, Is.EqualTo(0), "Should return 0 when an exception occurs");
        }

        [Test]
        public async Task IsOnlineAsync_ReturnsBoolean()
        {
            // Act
            var isOnline = await _syncService.IsOnlineAsync();

            // Assert
            Assert.That(isOnline, Is.InstanceOf<bool>());
        }

        private ISyncService CreateSyncServiceWithMockDb()
        {
            // Create a minimal Supabase.Client instance
            // This won't be used for actual network calls in these unit tests
            var supabaseUrl = "http://localhost:54321";
            var supabaseKey = "test-anon-key";
            var supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey);
            
            return new SyncService(_dbServiceMock.Object, supabaseClient);
        }
    }
}
