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
					() => _repository.CreateAsync(null!)
				);
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

		[Fact]
		public async Task CreateAsync_SaveChangesProblem_ThrowsException() {
			// Arrange
			var user = new User();
			var expectedException = new DbUpdateException();

			_mockSet
				.Setup(x => x.AddAsync(user, It.IsAny<CancellationToken>()))
				.ReturnsAsync((EntityEntry<User>)null!);

			_mockContext
				.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
				.ThrowsAsync(expectedException);

			// Act & Assert
			var actualException = await Assert.ThrowsAsync<DbUpdateException>(
					() => _repository.CreateAsync(user, commitChanges: true)
				);

			Assert.Same(expectedException, actualException);
		}

		[Fact]
		public async Task CreateAsync_UserWithID_IDRemainsTheSame() {
			// Arrange
			var expectedId = Guid.NewGuid();
			var user = new User { Id = expectedId };

			_mockSet
				.Setup(x => x.AddAsync(user, It.IsAny<CancellationToken>()))
				.ReturnsAsync((EntityEntry<User>)null!);

			// Act
			var result = await _repository.CreateAsync(user, commitChanges: false);

			// Assert
			Assert.Equal(expectedId, result.Id);
		}
		#endregion

		#region UpdateAsync
		[Fact]
		public async Task UpdateAsync_PassNull_ThrowsArgumentNullException() {
			// Act & Assert
			var exception = await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repository.UpdateAsync(null!)
			);
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

		[Fact]
		public async Task UpdateAsync_SaveChangesProblem_ThrowsException() {
			// Arrange
			var user = new User();
			var expectedException = new DbUpdateException();

			_mockSet
				.Setup(x => x.Update(user))
				.Returns((EntityEntry<User>)null!);

			_mockContext
				.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
				.ThrowsAsync(expectedException);

			// Act & Assert
			var actualException = await Assert.ThrowsAsync<DbUpdateException>(
				() => _repository.UpdateAsync(user, commitChanges: true)
			);

			Assert.Same(expectedException, actualException);
		}

		[Fact]
		public async Task UpdateAsync_EntityWithID_IDRemainsTheSame() {
			// Arrange
			var expectedId = Guid.NewGuid();
			var user = new User { Id = expectedId };

			_mockSet
				.Setup(x => x.Update(user))
				.Returns((EntityEntry<User>)null!);

			// Act
			var result = await _repository.UpdateAsync(user, commitChanges: false);

			// Assert
			Assert.Equal(expectedId, result.Id);
		}
		#endregion

		#region DeleteAsync

		[Fact]
		public async Task DeleteAsync_PassNull_ThrowsArgumentNullException() {
			// Act & Assert
			var exception = await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repository.DeleteAsync(null!)
			);
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
			Assert.True(result);
			Assert.True(user.IsDeleted);
			Assert.NotNull(user.DateUpdated);
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
			Assert.True(result);
			Assert.True(user.IsDeleted);
			Assert.NotNull(user.DateUpdated);
		}

		[Fact]
		public async Task DeleteAsync_SaveChangesProblem_ThrowsException() {
			// Arrange
			var user = new User();
			var expectedException = new DbUpdateException();

			_mockSet
				.Setup(x => x.Update(user))
				.Returns((EntityEntry<User>)null!);

			_mockContext
				.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
				.ThrowsAsync(expectedException);

			// Act & Assert
			var actual = await Assert.ThrowsAsync<DbUpdateException>(
				() => _repository.DeleteAsync(user, commitChanges: true));

			Assert.Same(expectedException, actual);
		}
		#endregion

		#region HardDeleteAsync

		[Fact]
		public async Task HardDeleteAsync_PassNull_ThrowsArgumentNullException() {
			// Act & Assert
			var ex = await Assert.ThrowsAsync<ArgumentNullException>(
				() => _repository.HardDeleteAsync(null!)
			);
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

		[Fact]
		public async Task HardDeleteAsync_SaveChangesProblem_ThrowsException() {
			// Arrange
			var user = new User();
			var expectedException = new DbUpdateException();

			_mockSet
				.Setup(x => x.Remove(user))
				.Returns((EntityEntry<User>)null!);
			_mockContext
				.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
				.ThrowsAsync(expectedException);

			// Act & Assert
			var actual = await Assert.ThrowsAsync<DbUpdateException>(
				() => _repository.HardDeleteAsync(user, commitChanges: true));

			Assert.Same(expectedException, actual);
		}
		#endregion

		#region SetTracking
		[Fact]
		public void SetTracking_HappyPath() {
			// Act
			var result = _repository.SetTracking();

			// Assert
			_mockContext.Verify(c => c.Set<User>(), Times.Once);
			Assert.Same(_mockSet.Object, result);
		}
		#endregion

		#region SaveChangesAsync
		[Fact]
		public async Task SaveChangesAsync_HappyPath() {
			// Arrange
			_mockContext
				.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(3);

			// Act
			var result = await _repository.SaveChangesAsync();

			// Assert
			_mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
			Assert.True(result);
		}
		#endregion
	}
}
