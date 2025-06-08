using Moq;
using AutoMapper;
using System.Security.Claims;
using Organizer_przepisów_kulinarnych.BLL.Services;
using Organizer_przepisów_kulinarnych.DAL.Interfaces;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly IMapper _mapper;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserDto>().ReverseMap();
            cfg.CreateMap<UserRegistrationDto, User>();
        });

        _mapper = config.CreateMapper();
        _userService = new UserService(_userRepoMock.Object, _mapper);
    }

    [Fact]
    public async Task GetUserByIdAsync_Returns_UserDto_WhenUserExists()
    {
        // Arrange
        var user = new User { Id = 1, Username = "john" };
        _userRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("john", result.Data.Username);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsFail_WhenUserNotFound()
    {
        // Arrange
        _userRepoMock.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((User)null);

        // Act
        var result = await _userService.GetUserByIdAsync(99);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User not found.", result.Error);
    }

    [Fact]
    public async Task RegisterUserAsync_ReturnsFail_WhenUsernameExists()
    {
        // Arrange
        var dto = new UserRegistrationDto { Username = "test", Email = "test@example.com", Password = "123" };
        _userRepoMock.Setup(r => r.UsernameExistsAsync("test")).ReturnsAsync(true);

        // Act
        var result = await _userService.RegisterUserAsync(dto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Username is already taken.", result.Error);
    }

    [Fact]
    public async Task DeleteUserAsync_ReturnsFail_WhenUserIsAdmin()
    {
        // Arrange
        var admin = new User { Id = 1, Username = "admin", UserRole = UserRole.Admin };
        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(admin);

        // Act
        var result = await _userService.DeleteUserAsync(1);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Cannot delete an Admin account.", result.Error);
    }

    [Fact]
    public void ValidateCredentials_ReturnsFail_WhenPasswordIsIncorrect()
    {
        // Arrange
        var user = new User { Username = "user1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct") };
        _userRepoMock.Setup(r => r.GetByUsernameAsync("user1")).ReturnsAsync(user);

        // Act
        var result = _userService.ValidateCredentials("user1", "wrong");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid credentials.", result.Error);
    }

    [Fact]
    public void AuthenticateUser_ReturnsPrincipal_WhenCredentialsAreValid()
    {
        // Arrange
        var password = "secret";
        var user = new User
        {
            Id = 5,
            Username = "validUser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            UserRole = UserRole.User
        };

        _userRepoMock.Setup(r => r.GetByUsernameAsync("validUser")).ReturnsAsync(user);

        // Act
        var result = _userService.AuthenticateUser("validUser", password);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        var identity = result.Data.Identity as ClaimsIdentity;
        Assert.NotNull(identity);
        Assert.True(identity.IsAuthenticated);
        Assert.Equal("validUser", identity.Name);
    }
    [Fact]
    public async Task UpdateUserAsync_ReturnsFail_WhenUserDoesNotExist()
    {
        // Arrange
        var userDto = new UserDto { Id = 99, Email = "new@example.com" };
        _userRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User)null);

        // Act
        var result = await _userService.UpdateUserAsync(userDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User not found.", result.Error);
    }

    [Fact]
    public async Task UpdateUserAsync_UpdatesFieldsCorrectly_WhenUserExists()
    {
        // Arrange
        var user = new User { Id = 1, Username = "oldUser", Email = "old@example.com" };
        var updatedDto = new UserDto
        {
            Id = 1,
            Username = "newUser",
            Email = "new@example.com",
            FirstName = "John",
            Surname = "Doe"
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _userService.UpdateUserAsync(updatedDto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("newUser", user.Username);
        Assert.Equal("new@example.com", user.Email);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.Surname);
        _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetCurrentUserIdAsync_ReturnsFail_WhenUserNotAuthenticated()
    {
        // Arrange
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        // Act
        var result = await _userService.GetCurrentUserIdAsync(principal);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User is not authenticated.", result.Error);
    }

    [Fact]
    public async Task GetCurrentUserIdAsync_ReturnsUserId_WhenUserExists()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        var user = new User { Id = 7, Username = "testuser" };
        _userRepoMock.Setup(r => r.GetByUsernameAsync("testuser")).ReturnsAsync(user);

        // Act
        var result = await _userService.GetCurrentUserIdAsync(principal);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(7, result.Data);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsUserList()
    {
        // Arrange
        var users = new List<User>
    {
        new User { Id = 1, Username = "User1" },
        new User { Id = 2, Username = "User2" }
    };
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("User1", result.Data[0].Username);
        Assert.Equal("User2", result.Data[1].Username);
    }

    [Fact]
    public async Task CreateAsync_CallsAddAndSaveChanges()
    {
        // Arrange
        var user = new User { Id = 10, Username = "newuser" };

        // Act
        var result = await _userService.CreateAsync(user);

        // Assert
        Assert.True(result.Success);
        _userRepoMock.Verify(r => r.AddAsync(user), Times.Once);
        _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ThrowsArgumentNullException_WhenDtoIsNull()
    {
        // Arrange
        UserDto nullDto = null;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => _userService.UpdateUserAsync(nullDto));
    }

    [Fact]
    public async Task RegisterUserAsync_ReturnsFail_WhenDtoIsNull()
    {
        // Arrange
        UserRegistrationDto nullDto = null;

        // Act
        var ex = await Record.ExceptionAsync(() => _userService.RegisterUserAsync(nullDto));

        // Assert
        Assert.NotNull(ex);
        Assert.IsType<NullReferenceException>(ex);
    }

    [Fact]
    public void ValidateCredentials_ReturnsFail_WhenUsernameIsNull()
    {
        // Arrange
        string username = null;
        string password = "somePassword";

        // Act
        var result = _userService.ValidateCredentials(username, password);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid credentials.", result.Error);
    }

    [Fact]
    public void ValidateCredentials_ReturnsFail_WhenPasswordIsNull()
    {
        // Arrange
        string username = "user";
        string password = null;

        // Act
        var result = _userService.ValidateCredentials(username, password);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid credentials.", result.Error);
    }

    [Fact]
    public void AuthenticateUser_ReturnsFail_WhenUsernameOrPasswordIsNull()
    {
        // Arrange
        string username = null;
        string password = null;

        // Act
        var result = _userService.AuthenticateUser(username, password);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid credentials.", result.Error);
    }

}
