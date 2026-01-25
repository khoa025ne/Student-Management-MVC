using Xunit;
using Moq;
using Services.Implementations;
using Repositories.Interfaces;
using DataAccess.Entities;
using System.Threading.Tasks;

namespace StudentManagement.Tests.Services
{
    public class StudentServiceTests
    {
        private readonly Mock<IStudentRepository> _mockRepo;
        private readonly Mock<IEnrollmentRepository> _mockEnrollmentRepo;
        private readonly StudentService _service;

        public StudentServiceTests()
        {
            _mockRepo = new Mock<IStudentRepository>();
            _mockEnrollmentRepo = new Mock<IEnrollmentRepository>();
            _service = new StudentService(_mockRepo.Object, _mockEnrollmentRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAsync_AndReturnStudent()
        {
            // Arrange
            var student = new Student { StudentCode = "SV001", FullName = "Test" };
            
            // Mock GetByCodeAsync to return null (not existing)
            _mockRepo.Setup(repo => repo.GetByCodeAsync(student.StudentCode)).ReturnsAsync((Student?)null);
            
            // Mock AddAsync to return the student
            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Student>())).ReturnsAsync(student);

            // Act
            var result = await _service.CreateAsync(student);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SV001", result.StudentCode);
            _mockRepo.Verify(repo => repo.AddAsync(student), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenStudentExists()
        {
             // Arrange
            var student = new Student { StudentCode = "SV001", FullName = "Test" };
            
            // Mock GetByCodeAsync to return existing student
            _mockRepo.Setup(repo => repo.GetByCodeAsync(student.StudentCode)).ReturnsAsync(student);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(student));
        }

        [Fact]
        public async Task GetByCodeAsync_ShouldReturnStudent_WhenFound()
        {
            // Arrange
            var code = "SV001";
            var expectedStudent = new Student { StudentCode = code, FullName = "Found" };
            _mockRepo.Setup(repo => repo.GetByCodeAsync(code)).ReturnsAsync(expectedStudent);

            // Act
            var result = await _service.GetByCodeAsync(code);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Found", result.FullName);
        }
    }
}
