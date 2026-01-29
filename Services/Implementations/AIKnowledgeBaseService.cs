using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.Models;

namespace Services.Implementations
{
    /// <summary>
    /// Implementation của AI Knowledge Base Service
    /// Quản lý knowledge base và tích hợp AI theo giáo trình FPT
    /// </summary>
    public class AIKnowledgeBaseService : IAIKnowledgeBaseService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AIKnowledgeBaseService> _logger;

        public AIKnowledgeBaseService(
            AppDbContext context, 
            IConfiguration configuration,
            ILogger<AIKnowledgeBaseService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        #region CRUD Operations

        public async Task<List<AIKnowledgeBaseModel>> GetAllAsync()
        {
            return await _context.AIKnowledgeBases
                .OrderByDescending(k => k.Priority)
                .ThenByDescending(k => k.UsageCount)
                .Select(k => MapToModel(k))
                .ToListAsync();
        }

        public async Task<AIKnowledgeBaseModel?> GetByIdAsync(int id)
        {
            var entity = await _context.AIKnowledgeBases.FindAsync(id);
            return entity != null ? MapToModel(entity) : null;
        }

        public async Task<List<AIKnowledgeBaseModel>> GetByCategoryAsync(string category)
        {
            return await _context.AIKnowledgeBases
                .Where(k => k.Category == category && k.IsActive)
                .OrderByDescending(k => k.Priority)
                .Select(k => MapToModel(k))
                .ToListAsync();
        }

        public async Task<List<AIKnowledgeBaseModel>> SearchAsync(string query, string? category = null, int maxResults = 200)
        {
            var queryable = _context.AIKnowledgeBases.Where(k => k.IsActive);

            if (!string.IsNullOrEmpty(category))
            {
                queryable = queryable.Where(k => k.Category == category);
            }

            if (!string.IsNullOrEmpty(query))
            {
                var lowerQuery = query.ToLower();
                queryable = queryable.Where(k =>
                    k.Title.ToLower().Contains(lowerQuery) ||
                    k.Content.ToLower().Contains(lowerQuery) ||
                    (k.Tags != null && k.Tags.ToLower().Contains(lowerQuery)));
            }

            return await queryable
                .OrderByDescending(k => k.Priority)
                .ThenByDescending(k => k.UsageCount)
                .Take(maxResults)
                .Select(k => MapToModel(k))
                .ToListAsync();
        }

        public async Task<AIKnowledgeBaseModel> CreateAsync(AIKnowledgeBaseModel model)
        {
            var entity = new AIKnowledgeBase
            {
                Title = model.Title,
                Content = model.Content,
                Category = model.Category,
                SubCategory = model.SubCategory,
                Tags = model.Tags,
                Priority = model.Priority,
                IsActive = model.IsActive,
                Language = model.Language,
                MetadataJson = model.MetadataJson,
                CreatedBy = model.CreatedBy,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.AIKnowledgeBases.Add(entity);
            await _context.SaveChangesAsync();

            return MapToModel(entity);
        }

        public async Task<AIKnowledgeBaseModel> UpdateAsync(AIKnowledgeBaseModel model)
        {
            var entity = await _context.AIKnowledgeBases.FindAsync(model.KnowledgeId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Knowledge base item with ID {model.KnowledgeId} not found");
            }

            entity.Title = model.Title;
            entity.Content = model.Content;
            entity.Category = model.Category;
            entity.SubCategory = model.SubCategory;
            entity.Tags = model.Tags;
            entity.Priority = model.Priority;
            entity.IsActive = model.IsActive;
            entity.Language = model.Language;
            entity.MetadataJson = model.MetadataJson;
            entity.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return MapToModel(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.AIKnowledgeBases.FindAsync(id);
            if (entity == null) return false;

            _context.AIKnowledgeBases.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleActiveAsync(int id)
        {
            var entity = await _context.AIKnowledgeBases.FindAsync(id);
            if (entity == null) return false;

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region AI Processing

        public async Task<AIProcessingResponse> ProcessWithKnowledgeBase(AIProcessingRequest request)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new AIProcessingResponse();

            try
            {
                // 1. Lấy relevant knowledge từ database (tối đa 200 items)
                var relevantKnowledge = await GetRelevantKnowledge(request);
                response.UsedKnowledgeIds = relevantKnowledge.Select(k => k.KnowledgeId).ToList();

                // 2. Build context từ knowledge base
                var context = BuildContextFromKnowledge(relevantKnowledge);

                // 3. Gọi AI Service (OpenAI hoặc fallback)
                var aiResult = await CallAIService(request, context);
                
                response.Success = aiResult.Success;
                response.Response = aiResult.Response;
                response.ModelUsed = aiResult.ModelUsed;
                response.TokensUsed = aiResult.TokensUsed;

                // 4. Increment usage count cho các knowledge đã sử dụng
                foreach (var kb in relevantKnowledge)
                {
                    await IncrementUsageCountAsync(kb.KnowledgeId);
                }

                // 5. Log conversation
                await LogConversation(request, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AI request");
                response.Success = false;
                response.ErrorMessage = ex.Message;
                response.ModelUsed = "Fallback";
                
                // Fallback response
                response.Response = GetFallbackResponse(request.RequestType);
            }

            stopwatch.Stop();
            response.ProcessingTimeMs = (int)stopwatch.ElapsedMilliseconds;
            return response;
        }

        public async Task<AcademicAnalysisResponse> AnalyzeStudentPerformance(AcademicAnalysisRequest request)
        {
            var response = new AcademicAnalysisResponse();

            try
            {
                // Lấy knowledge liên quan đến GPA và academic analysis
                var knowledge = await SearchAsync("gpa analysis academic", AIKnowledgeCategories.FLOW2_GRADING, 50);
                
                // Phân tích môn mạnh/yếu dựa trên điểm
                response.StrongSubjects = request.CompletedCourses
                    .Where(c => c.Grade == "A" || c.Grade == "B")
                    .Select(c => c.CourseName)
                    .ToList();

                response.WeakSubjects = request.CompletedCourses
                    .Where(c => c.Grade == "D" || c.Grade == "F")
                    .Select(c => c.CourseName)
                    .ToList();

                // Generate recommendations
                response.Recommendations = GenerateRecommendations(request, knowledge);

                // Try to get AI-enhanced recommendations
                var aiRequest = new AIProcessingRequest
                {
                    StudentId = request.StudentId,
                    RequestType = "ACADEMIC_ANALYSIS",
                    UserQuery = JsonSerializer.Serialize(request),
                    AdditionalData = new Dictionary<string, object>
                    {
                        { "overallGPA", request.OverallGPA },
                        { "strongSubjects", response.StrongSubjects },
                        { "weakSubjects", response.WeakSubjects }
                    }
                };

                var aiResponse = await ProcessWithKnowledgeBase(aiRequest);
                if (aiResponse.Success && !string.IsNullOrEmpty(aiResponse.Response))
                {
                    try
                    {
                        var aiAnalysis = JsonSerializer.Deserialize<AcademicAnalysisResponse>(aiResponse.Response);
                        if (aiAnalysis != null)
                        {
                            response.Recommendations = aiAnalysis.Recommendations;
                        }
                    }
                    catch
                    {
                        // Keep fallback recommendations
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing student performance");
                response.Recommendations = "Không thể phân tích tự động. Vui lòng liên hệ cố vấn học tập.";
            }

            return response;
        }

        public async Task<LearningPathResponse> GenerateLearningPath(LearningPathRequest request)
        {
            var response = new LearningPathResponse();

            try
            {
                // Lấy knowledge liên quan đến learning path
                var knowledge = await SearchAsync("learning path recommendation", AIKnowledgeCategories.LEARNING_PATH, 50);

                // Basic recommendation logic
                var recommended = new List<RecommendedCourse>();
                int priority = 1;

                // Ưu tiên các môn có thể cải thiện điểm yếu
                foreach (var weak in request.WeakSubjects.Take(2))
                {
                    var relatedCourse = request.AvailableCourses
                        .FirstOrDefault(c => c.Contains(weak, StringComparison.OrdinalIgnoreCase));
                    
                    if (relatedCourse != null)
                    {
                        recommended.Add(new RecommendedCourse
                        {
                            CourseName = relatedCourse,
                            Priority = priority++,
                            Reason = $"Giúp cải thiện kiến thức về {weak} - một trong những điểm yếu của bạn"
                        });
                    }
                }

                // Thêm các môn phát huy điểm mạnh
                foreach (var strong in request.StrongSubjects.Take(2))
                {
                    var advancedCourse = request.AvailableCourses
                        .FirstOrDefault(c => c.Contains(strong, StringComparison.OrdinalIgnoreCase) && 
                                           !recommended.Any(r => r.CourseName == c));
                    
                    if (advancedCourse != null)
                    {
                        recommended.Add(new RecommendedCourse
                        {
                            CourseName = advancedCourse,
                            Priority = priority++,
                            Reason = $"Phát triển thế mạnh về {strong}"
                        });
                    }
                }

                response.RecommendedCourses = recommended;
                response.OverallStrategy = GenerateLearningStrategy(request, knowledge);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating learning path");
                response.OverallStrategy = "Không thể tạo lộ trình tự động. Vui lòng liên hệ cố vấn học tập.";
            }

            return response;
        }

        #endregion

        #region Statistics

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.AIKnowledgeBases.CountAsync();
        }

        public async Task<Dictionary<string, int>> GetCountByCategoryAsync()
        {
            return await _context.AIKnowledgeBases
                .GroupBy(k => k.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category, x => x.Count);
        }

        public async Task IncrementUsageCountAsync(int knowledgeId)
        {
            var entity = await _context.AIKnowledgeBases.FindAsync(knowledgeId);
            if (entity != null)
            {
                entity.UsageCount++;
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Seed Data

        public async Task SeedInitialKnowledgeBase()
        {
            if (await _context.AIKnowledgeBases.AnyAsync())
            {
                return; // Already seeded
            }

            var seedData = GetInitialKnowledgeData();
            _context.AIKnowledgeBases.AddRange(seedData);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Seeded {seedData.Count} AI Knowledge Base items");
        }

        private List<AIKnowledgeBase> GetInitialKnowledgeData()
        {
            var data = new List<AIKnowledgeBase>();

            // FLOW 1: Đăng ký sinh viên
            data.AddRange(new[]
            {
                new AIKnowledgeBase
                {
                    Title = "Quy trình tạo tài khoản sinh viên",
                    Category = AIKnowledgeCategories.FLOW1_REGISTRATION,
                    Content = @"Quy trình tạo tài khoản sinh viên mới tại FPT:
1. Admin/Manager nhập thông tin: Họ tên, Email (@fpt.edu.vn), SĐT (10 số), Ngày sinh (≥16 tuổi), Mã lớp
2. Hệ thống tự động sinh: Mã SV (STU + Năm + Số thứ tự), Mật khẩu mặc định (NgàySinh@fpt)
3. Gửi email chào mừng với thông tin tài khoản và link đăng nhập
4. Sinh viên đăng nhập lần đầu PHẢI đổi mật khẩu
5. Mật khẩu mới: tối thiểu 8 ký tự, có chữ HOA, thường, số, ký tự đặc biệt",
                    Tags = "đăng ký,tạo tài khoản,sinh viên,mật khẩu",
                    Priority = 10,
                    Language = "vi"
                },
                new AIKnowledgeBase
                {
                    Title = "Validation đăng ký sinh viên",
                    Category = AIKnowledgeCategories.VALIDATION_RULE,
                    Content = @"Các quy tắc validation khi đăng ký sinh viên:
- Email: Không được trùng, định dạng hợp lệ
- SĐT: Định dạng Việt Nam (0xxxxxxxxx), 10 số
- Ngày sinh: Tuổi >= 16 và <= 60
- Họ tên: Bắt buộc, tối đa 100 ký tự
- Mã lớp: Phải tồn tại trong hệ thống",
                    Tags = "validation,kiểm tra,sinh viên",
                    Priority = 9,
                    Language = "vi"
                }
            });

            // FLOW 2: Nhập điểm
            data.AddRange(new[]
            {
                new AIKnowledgeBase
                {
                    Title = "Công thức tính GPA theo FPT",
                    Category = AIKnowledgeCategories.GPA_CALCULATION,
                    Content = @"Công thức tính GPA tại FPT University:
1. GPA môn học = (Midterm × 0.4) + (Final × 0.6)
2. Quy đổi Grade:
   - A: GPA >= 8.5
   - B: 7.0 <= GPA < 8.5
   - C: 5.5 <= GPA < 7.0
   - D: 4.0 <= GPA < 5.5
   - F: GPA < 4.0 (Rớt môn)
3. Overall GPA = Σ(GPA_môn × Credits_môn) / Σ(Credits)",
                    Tags = "gpa,điểm,tính toán,grade",
                    Priority = 10,
                    Language = "vi"
                },
                new AIKnowledgeBase
                {
                    Title = "Validation điểm số",
                    Category = AIKnowledgeCategories.VALIDATION_RULE,
                    Content = @"Quy tắc validation điểm số:
- Điểm Midterm: 0-10, cho phép 1 chữ số thập phân
- Điểm Final: 0-10, cho phép 1 chữ số thập phân
- Không được để trống cả hai khi lưu
- Grade được tính tự động theo công thức",
                    Tags = "validation,điểm,midterm,final",
                    Priority = 9,
                    Language = "vi"
                }
            });

            // FLOW 3: Thông báo
            data.AddRange(new[]
            {
                new AIKnowledgeBase
                {
                    Title = "Hệ thống thông báo đa kênh",
                    Category = AIKnowledgeCategories.FLOW3_NOTIFICATION,
                    Content = @"Hệ thống thông báo FPT gồm 3 kênh:
1. EMAIL: Template HTML, các loại: Chào mừng, Xác nhận đăng ký, Điểm mới, Cảnh báo
2. SMS: Giới hạn 160 ký tự, chỉ cho thông báo quan trọng
3. IN-APP (SignalR): Real-time, hiển thị badge số chưa đọc
Events trigger: Score Update, Performance Alert, Achievement, Learning Path",
                    Tags = "thông báo,email,sms,signalr",
                    Priority = 8,
                    Language = "vi"
                }
            });

            // Điều kiện tiên quyết
            data.AddRange(new[]
            {
                new AIKnowledgeBase
                {
                    Title = "Kiểm tra điều kiện tiên quyết",
                    Category = AIKnowledgeCategories.COURSE_PREREQUISITE,
                    Content = @"Logic kiểm tra điều kiện tiên quyết:
1. Truy vấn bảng Scores: Check sinh viên đã qua môn tiên quyết chưa (Grade != 'F')
2. Nếu CHƯA qua: Trả lỗi 'Bạn chưa đủ điều kiện. Cần qua môn [Tên môn tiên quyết]'
3. Ví dụ: Phải qua 'Lập trình C' mới đăng ký 'Đồ án 1'",
                    Tags = "tiên quyết,prerequisite,điều kiện",
                    Priority = 9,
                    Language = "vi"
                }
            });

            // Cảnh báo học vụ
            data.AddRange(new[]
            {
                new AIKnowledgeBase
                {
                    Title = "Quy định cảnh báo học vụ FPT",
                    Category = AIKnowledgeCategories.ACADEMIC_WARNING,
                    Content = @"Điều kiện kích hoạt cảnh báo học vụ:
1. Overall GPA < 2.0 (Cảnh cáo học vụ)
2. Có >= 2 môn điểm F trong cùng 1 kỳ
3. Không đăng ký môn nào trong kỳ hiện tại

Hành động:
- Gửi email cảnh báo cho sinh viên
- Thông báo cho Manager (Giáo vụ)
- Hiển thị cảnh báo đỏ trên Dashboard
- Đề xuất gặp cố vấn học tập",
                    Tags = "cảnh báo,học vụ,gpa,warning",
                    Priority = 10,
                    Language = "vi"
                }
            });

            // Learning Path
            data.AddRange(new[]
            {
                new AIKnowledgeBase
                {
                    Title = "Chiến lược gợi ý lộ trình học tập",
                    Category = AIKnowledgeCategories.LEARNING_PATH,
                    Content = @"Chiến lược gợi ý lộ trình học tập AI:
1. Thu thập: GPA, môn mạnh/yếu, môn đã học
2. Phân tích: So sánh với yêu cầu ngành
3. Gợi ý 3-4 môn phù hợp nhất cho kỳ tới
4. Ưu tiên:
   - Môn cải thiện điểm yếu
   - Môn phát huy thế mạnh
   - Môn bắt buộc còn thiếu
5. Cung cấp lý do cho mỗi gợi ý",
                    Tags = "lộ trình,learning path,gợi ý,recommendation",
                    Priority = 9,
                    Language = "vi"
                }
            });

            // FPT Curriculum
            data.AddRange(new[]
            {
                new AIKnowledgeBase
                {
                    Title = "Cấu trúc chương trình đào tạo FPT",
                    Category = AIKnowledgeCategories.FPT_CURRICULUM,
                    Content = @"Cấu trúc chương trình đào tạo FPT University:
- Tổng: 145 tín chỉ (4 năm)
- Đại cương: 30 tín chỉ
- Cơ sở ngành: 35 tín chỉ
- Chuyên ngành: 45 tín chỉ
- Thực tập + Đồ án: 20 tín chỉ
- Tự chọn: 15 tín chỉ

Các ngành chính: SE (Software Engineering), AI, IS, IA",
                    Tags = "fpt,chương trình,đào tạo,curriculum",
                    Priority = 8,
                    Language = "vi"
                },
                new AIKnowledgeBase
                {
                    Title = "Các môn cơ sở ngành SE",
                    Category = AIKnowledgeCategories.FPT_CURRICULUM,
                    SubCategory = "SE",
                    Content = @"Các môn cơ sở ngành Software Engineering:
1. PRF192 - Programming Fundamentals (C)
2. PRO192 - Object-Oriented Programming
3. CSD201 - Data Structures and Algorithms
4. DBI202 - Database Systems
5. PRN211 - C# Programming
6. SWE201 - Software Engineering
7. SWR302 - Software Requirements
8. SWD392 - Software Architecture and Design",
                    Tags = "se,software engineering,môn học,cơ sở ngành",
                    Priority = 7,
                    Language = "vi"
                }
            });

            return data;
        }

        #endregion

        #region Private Methods

        private static AIKnowledgeBaseModel MapToModel(AIKnowledgeBase entity)
        {
            return new AIKnowledgeBaseModel
            {
                KnowledgeId = entity.KnowledgeId,
                Title = entity.Title,
                Content = entity.Content,
                Category = entity.Category,
                SubCategory = entity.SubCategory,
                Tags = entity.Tags,
                Priority = entity.Priority,
                UsageCount = entity.UsageCount,
                IsActive = entity.IsActive,
                Language = entity.Language,
                MetadataJson = entity.MetadataJson,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                CreatedBy = entity.CreatedBy
            };
        }

        private async Task<List<AIKnowledgeBaseModel>> GetRelevantKnowledge(AIProcessingRequest request)
        {
            var category = request.RequestType switch
            {
                "ACADEMIC_ANALYSIS" => AIKnowledgeCategories.FLOW2_GRADING,
                "LEARNING_PATH" => AIKnowledgeCategories.LEARNING_PATH,
                "COURSE_RECOMMENDATION" => AIKnowledgeCategories.COURSE_PREREQUISITE,
                _ => null
            };

            return await SearchAsync(request.UserQuery, category, 200);
        }

        private string BuildContextFromKnowledge(List<AIKnowledgeBaseModel> knowledge)
        {
            var context = "Dựa trên các quy định và hướng dẫn của FPT University:\n\n";
            
            foreach (var kb in knowledge.Take(50)) // Limit context size
            {
                context += $"## {kb.Title}\n{kb.Content}\n\n";
            }

            return context;
        }

        private async Task<AIProcessingResponse> CallAIService(AIProcessingRequest request, string context)
        {
            // TODO: Implement actual OpenAI API call
            // For now, return a fallback response
            await Task.Delay(100); // Simulate API call

            return new AIProcessingResponse
            {
                Success = true,
                Response = GetFallbackResponse(request.RequestType),
                ModelUsed = "Fallback",
                TokensUsed = 0
            };
        }

        private string GetFallbackResponse(string requestType)
        {
            return requestType switch
            {
                "ACADEMIC_ANALYSIS" => JsonSerializer.Serialize(new AcademicAnalysisResponse
                {
                    StrongSubjects = new List<string>(),
                    WeakSubjects = new List<string>(),
                    Recommendations = "Hãy tập trung ôn tập các môn có điểm thấp và duy trì phong độ với các môn có điểm cao."
                }),
                "LEARNING_PATH" => JsonSerializer.Serialize(new LearningPathResponse
                {
                    RecommendedCourses = new List<RecommendedCourse>(),
                    OverallStrategy = "Hãy cân nhắc đăng ký các môn cơ sở ngành trước, sau đó đến các môn chuyên ngành."
                }),
                _ => "Không thể xử lý yêu cầu tự động. Vui lòng liên hệ cố vấn học tập."
            };
        }

        private string GenerateRecommendations(AcademicAnalysisRequest request, List<AIKnowledgeBaseModel> knowledge)
        {
            var recommendations = new List<string>();

            if (request.OverallGPA < 2.0)
            {
                recommendations.Add("⚠️ GPA dưới 2.0 - Cần cải thiện ngay lập tức. Hãy tập trung vào các môn cơ bản.");
            }
            else if (request.OverallGPA >= 8.5)
            {
                recommendations.Add("🎉 Xuất sắc! Hãy duy trì phong độ và thử thách bản thân với các môn nâng cao.");
            }

            var weakCourses = request.CompletedCourses.Where(c => c.Grade == "D" || c.Grade == "F").ToList();
            if (weakCourses.Any())
            {
                recommendations.Add($"📚 Cần cải thiện: {string.Join(", ", weakCourses.Select(c => c.CourseName))}");
            }

            var strongCourses = request.CompletedCourses.Where(c => c.Grade == "A").ToList();
            if (strongCourses.Any())
            {
                recommendations.Add($"💪 Điểm mạnh: {string.Join(", ", strongCourses.Select(c => c.CourseName))}");
            }

            return string.Join("\n", recommendations);
        }

        private string GenerateLearningStrategy(LearningPathRequest request, List<AIKnowledgeBaseModel> knowledge)
        {
            var strategy = new List<string>();

            if (request.CurrentGPA < 2.0)
            {
                strategy.Add("Ưu tiên hàng đầu: Cải thiện GPA để tránh cảnh báo học vụ.");
            }

            if (request.WeakSubjects.Any())
            {
                strategy.Add($"Tập trung cải thiện: {string.Join(", ", request.WeakSubjects.Take(3))}");
            }

            if (request.StrongSubjects.Any())
            {
                strategy.Add($"Phát huy thế mạnh: {string.Join(", ", request.StrongSubjects.Take(3))}");
            }

            strategy.Add("Đăng ký 4-5 môn/kỳ để cân bằng học tập và thời gian cá nhân.");

            return string.Join("\n", strategy);
        }

        private async Task LogConversation(AIProcessingRequest request, AIProcessingResponse response)
        {
            try
            {
                var log = new AIConversationLog
                {
                    StudentId = request.StudentId,
                    RequestType = request.RequestType,
                    Prompt = request.UserQuery,
                    Response = response.Response,
                    UsedKnowledgeIds = response.UsedKnowledgeIds != null 
                        ? string.Join(",", response.UsedKnowledgeIds) 
                        : null,
                    ModelUsed = response.ModelUsed,
                    TokensUsed = response.TokensUsed,
                    ProcessingTimeMs = response.ProcessingTimeMs,
                    Status = response.Success ? "Success" : "Failed",
                    ErrorMessage = response.ErrorMessage,
                    CreatedAt = DateTime.Now
                };

                _context.AIConversationLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log AI conversation");
            }
        }

        #endregion
    }
}
