using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Login;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NetScape.Modules.DAL.Test
{
    public class EntityFrameworkPlayerSerializerTests
    {
        private readonly EntityFrameworkPlayerSerializer _entityFrameworkPlayerSerializer;
        private readonly IDbContextFactory<DatabaseContext> _fakeDbContextFactory;

        public EntityFrameworkPlayerSerializerTests()
        {
            var seedDbId = Guid.NewGuid().ToString();
            _fakeDbContextFactory = Substitute.For<IDbContextFactory<DatabaseContext>>();
            _fakeDbContextFactory.CreateDbContext().ReturnsForAnyArgs(x =>
                new DatabaseContext(
                    new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(seedDbId).Options
                )
            );
            _entityFrameworkPlayerSerializer = new EntityFrameworkPlayerSerializer(_fakeDbContextFactory);
        }

        [Fact]
        public async Task CanAddPlayer()
        {
            await AddSeedPlayer("Test", "Test");
            using (var dbContext = _fakeDbContextFactory.CreateDbContext())
            {
                var player = await dbContext.Players.FirstOrDefaultAsync(t => t.Username.Equals("Test"));
                player.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task CanGet()
        {
            await AddSeedPlayer("Test", "Test");
            var player = await _entityFrameworkPlayerSerializer.GetAsync("Test");
            player.Should().NotBeNull();
        }

        [Fact]
        public async Task CanUpdatePlayer()
        {
            var player = await AddSeedPlayer("Test", "Test");
            using (var dbContext = _fakeDbContextFactory.CreateDbContext())
            {
                player.Username = "Jill";
                await _entityFrameworkPlayerSerializer.AddOrUpdateAsync(player);

                var dbPlayer = await dbContext.Players.FirstOrDefaultAsync(t => t.Username.Equals("Jill"));
                dbPlayer.Should().NotBeNull();
            }
        }

        [Theory]
        [InlineData("Test", "Test")]
        [InlineData("Noob", "Letaaa")]
        [InlineData("Nooba", "Letaaa")]
        public async Task CanGetOrCreate(string username, string password)
        {
            var player = await _entityFrameworkPlayerSerializer.GetOrCreateAsync(new PlayerCredentials
            {
                Username = username,
                Password = password
            });

            player.Should().NotBeNull();
            player.Username.Should().Be(username);
        }


        private async Task<Player> AddSeedPlayer(string user, string pass)
        {
            var player = new Player
            {
                Username = user,
                Password = pass,
                Appearance = new Appearance { Style = new int[1], Colors = new int[1], Gender = Gender.Female }
            };
            await _entityFrameworkPlayerSerializer.AddOrUpdateAsync(player);
            return player;
        }
    }
}
