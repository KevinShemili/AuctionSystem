using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;

namespace UnitTests.RepositoryTests {
	public class BaseRepositoryTests {

		// A concrete subclass to access the protected methods
		public class FakeUserRepository : BaseRepository<User> {
			public FakeUserRepository(DatabaseContext ctx) : base(ctx) { }
		}

		private readonly Mock<DatabaseContext> _mockContext;
		private readonly Mock<DbSet<User>> _mockSet;
		private readonly FakeUserRepository _repository;

		public BaseRepositoryTests() {

			var options = new DbContextOptions<DatabaseContext>();
			_mockContext = new Mock<DatabaseContext>(options);
			_mockSet = new Mock<DbSet<User>>();

			_mockContext
				.Setup(x => x.Set<User>())
				.Returns(_mockSet.Object);

			_repository = new FakeUserRepository(_mockContext.Object);
		}

		#region CreateAsync
		[Fact]
		public async Task CreateAsync_PassNull_ThrowsArgumentNullException() {
			// Act & Assert
			var exception = await Assert.ThrowsAsync<ArgumentNullException>(
					() => _repository.CreateAsync(null!));
		}

		[Fact]
		public async Task CreateAsync_CommitTrue_HappyPath() {
			// Arrange
			var user = new User();

			_mockSet
				.Setup(x => x.AddAsync(user, It.IsAny<CancellationToken>()))
				.ReturnsAsync((EntityEntry<User>)null!);

			_mockContext
				.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(1);

			// Act
			var result = await _repository.CreateAsync(user, commitChanges: true);

			// Assert
			_mockSet.Verify(x => x.AddAsync(user, It.IsAny<CancellationToken>()), Times.Once);
			_mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
			Assert.Same(user, result);
			Assert.NotNull(user.DateCreated);
		}

		[Fact]
		public async Task CreateAsync_CommitFalse_HappyPath() {
			// Arrange
			var user = new User();

			_mockSet.Setup(x => x.AddAsync(user, It.IsAny<CancellationToken>()))
				.ReturnsAsync((EntityEntry<User>)null!);

			// Act
			var result = await _repository.CreateAsync(user, commitChanges: false, It.IsAny<CancellationToken>());

			// Assert
			_mockSet.Verify(x => x.AddAsync(user, It.IsAny<CancellationToken>()), Times.Once);
			_mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
			Assert.Same(user, result);
			Assert.NotNull(user.DateCreated);
		}
		#endregion

		#region UpdateAsync
		[Fact]
		public async Task UpdateAsync_PassNull_ThrowsArgumentNullException() {
			// Act & Assert
			var exception = await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repository.UpdateAsync(null!));
		}

		[Fact]
		public async Task UpdateAsync_CommitTrue_HappyPath() {
			// Arrange
			var user = new User();

			_mockSet
				.Setup(x => x.Update(user))
				.Returns((EntityEntry<User>)null!);

			_mockContext
				.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(1);

			// Act
			var result = await _repository.UpdateAsync(user, commitChanges: true);

			// Assert
			_mockSet.Verify(x => x.Update(user), Times.Once);
			_mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
			Assert.Same(user, result);
			Assert.NotNull(user.DateUpdated);
		}

		[Fact]
		public async Task UpdateAsync_CommitFalse_HappyPath() {
			// Arrange
			var user = new User();

			_mockSet
				.Setup(x => x.Update(user))
				.Returns((EntityEntry<User>)null!);

			// Act
			var result = await _repository.UpdateAsync(user, commitChanges: false);

			// Assert
			_mockSet.Verify(x => x.Update(user), Times.Once);
			_mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
			Assert.Same(user, result);
			Assert.NotNull(user.DateUpdated);
		}
		#endregion

		#region DeleteAsync
		[Fact]
		public async Task DeleteAsync_PassNull_ThrowsArgumentNullException() {
			// Act & Assert
			var exception = await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repository.DeleteAsync(null!));
		}

		[Fact]
		public async Task DeleteAsync_CommitTrue_HappyPath() {
			// Arrange
			var user = new User();

			_mockSet
				.Setup(x => x.Update(user))
				.Returns((EntityEntry<User>)null!);

			_mockContext
				.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(1);

			// Act
			var result = await _repository.DeleteAsync(user, commitChanges: true);

			// Assert
			_mockSet.Verify(x => x.Update(user), Times.Once);
			_mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
			Assert.True(user.IsDeleted);
			Assert.NotNull(user.DateUpdated);
			Assert.True(result);
		}

		[Fact]
		public async Task DeleteAsync_CommitFalse_HappyPath() {
			// Arrange
			var user = new User();

			_mockSet
				.Setup(x => x.Update(user))
				.Returns((EntityEntry<User>)null!);

			// Act
			var result = await _repository.DeleteAsync(user, commitChanges: false);

			// Assert
			_mockSet.Verify(x => x.Update(user), Times.Once);
			_mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
			Assert.True(user.IsDeleted);
			Assert.NotNull(user.DateUpdated);
			Assert.True(result);
		}
		#endregion

		#region HardDeleteAsync
		[Fact]
		public async Task HardDeleteAsync_PassNull_ThrowsArgumentNullException() {
			// Act & Assert
			var ex = await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repository.HardDeleteAsync(null!));
		}

		[Fact]
		public async Task HardDeleteAsync_CommitTrue_HappyPath() {
			// Arrange
			var user = new User();

			_mockSet
				.Setup(x => x.Remove(user))
				.Returns((EntityEntry<User>)null!);

			_mockContext
				.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(1);

			// Act
			var result = await _repository.HardDeleteAsync(user, commitChanges: true);

			// Assert
			_mockSet.Verify(s => s.Remove(user), Times.Once);
			_mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
			Assert.True(result);
		}

		[Fact]
		public async Task HardDeleteAsync_CommitFalse_HappyPath() {
			// Arrange
			var user = new User();

			_mockSet
				.Setup(x => x.Remove(user))
				.Returns((EntityEntry<User>)null!);

			// Act
			var result = await _repository.HardDeleteAsync(user, commitChanges: false);

			// Assert
			_mockSet.Verify(s => s.Remove(user), Times.Once);
			_mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
			Assert.True(result);
		}
		#endregion
	}
}
