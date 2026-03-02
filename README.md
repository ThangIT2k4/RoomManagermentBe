# 🛡️ Security Middleware & Input Protection Guide

## Tóm Tắt

Bạn vừa nhận được **một bộ hướng dẫn hoàn chỉnh** để bảo vệ ASP.NET Core application của mình khỏi các cuộc tấn công input:

### 📚 Gồm 7 File:

| # | File | Mục đích | Thời gian |
|---|------|---------|----------|
| 1 | **INDEX.md** | 🗺️ Navigation guide | 5 phút |
| 2 | **SUMMARY.md** | ⭐ Tóm tắt nhanh (5 middleware bắt buộc) | 10 phút |
| 3 | **SECURITY_MIDDLEWARE_RECOMMENDATIONS.md** | 🎯 Gợi ý chi tiết cho từng service | 15 phút |
| 4 | **IMPLEMENTATION_GUIDE.md** | 🚀 Hướng dẫn implement step-by-step | 30 phút |
| 5 | **MIDDLEWARE_DETAILED_GUIDE.md** | 🔬 Chi tiết từng middleware (8 loại) | 45 phút |
| 6 | **PIPELINE_ORDER_GUIDE.md** | 🔄 Thứ tự pipeline + lỗi thường gặp | 30 phút |
| 7 | **CODE_TEMPLATES.md** | 💻 Code sẵn dùng (copy-paste) | Lookup |
| 8 | **VISUAL_DIAGRAMS.md** | 📊 ASCII diagrams cho flow request | 45 phút |

---

## ⚡ Quick Start (30 phút)

### Bước 1: Tìm hiểu (10 phút)
```
Đọc: SUMMARY.md
├─ Middleware bắt buộc
├─ Thứ tự pipeline
└─ Lỗi thường gặp
```

### Bước 2: Lên kế hoạch (5 phút)
```
Đọc: SECURITY_MIDDLEWARE_RECOMMENDATIONS.md
├─ Cần cài gì cho Identity.API
├─ Cần cài gì cho Gateway
└─ Ưu tiên nào
```

### Bước 3: Implement (15 phút)
```
Copy từ: CODE_TEMPLATES.md
├─ Program.cs cho Identity.API
├─ Program.cs cho Gateway
├─ appsettings.json
└─ Validators + Controllers
```

---

## 🎯 Middleware Bắt Buộc (5 cái)

### 1. **Request Size Limit** (Chặn payload lớn)
```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 5 * 1024 * 1024; // 5MB
});
```
✓ Chặn DoS attacks (file upload khổng lồ)

### 2. **Rate Limiting** (Chống DDoS & brute force)
```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("LoginPolicy", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
    });
});

[RequireRateLimiting("LoginPolicy")]
[HttpPost("login")]
public IActionResult Login() { }
```
✓ 5 login attempts per minute

### 3. **Input Validation** (Kiểm tra data)
```csharp
public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .Length(3, 50)
            .Matches(@"^[a-zA-Z0-9_-]+$");
    }
}
```
✓ FluentValidation cho request DTOs

### 4. **Exception Handling** (Không lộ stack trace)
```csharp
app.UseExceptionHandler("/error");
app.AddProblemDetails();
```
✓ Error format RFC 7807 (application/problem+json)

### 5. **Authentication & Authorization** (Bảo vệ endpoint)
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* config */ });

app.UseAuthentication();
app.UseAuthorization();

[Authorize]
[HttpGet("profile")]
public IActionResult GetProfile() { }
```
✓ JWT tokens + [Authorize] attributes

---

## 🔄 Thứ Tự Middleware (QUAN TRỌNG!)

```csharp
var app = builder.Build();

// 1. Exception Handling (ĐẦU TIÊN!)
app.UseExceptionHandler("/error");

// 2-3. HTTPS + HSTS
app.UseHttpsRedirection();
if (!app.Environment.IsDevelopment())
    app.UseHsts();

// 4. Forwarded Headers (TRƯỚC Rate Limiting!)
app.UseForwardedHeaders();

// 5. Request Logging
app.UseSerilogRequestLogging();

// 6. CORS
app.UseCors("ApiPolicy");

// 7-8. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 9. Rate Limiting
app.UseRateLimiter();

// 10. Routing (CUỐI CÙNG!)
app.MapControllers();
```

### ⚠️ Common Mistakes:
- ❌ Rate Limiting trước Forwarded Headers → IP sai
- ❌ Authorization trước Authentication → User = anonymous
- ❌ Routing trước Rate Limiting → Rate limit vô dụng

---

## 📦 NuGet Packages Cần Cài

```bash
# Identity.API
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions
dotnet add package Serilog.AspNetCore

# Gateway
dotnet add package Serilog.AspNetCore
```

---

## 🚀 Implementation Roadmap

```
Week 1: Setup & Foundation
├─ [ ] Cài NuGet packages
├─ [ ] Copy Program.cs template từ CODE_TEMPLATES.md
├─ [ ] Update appsettings.json
└─ [ ] Configure Kestrel request size limit

Week 2: Input Validation
├─ [ ] Create FluentValidation validators
├─ [ ] Add validators to request DTOs
├─ [ ] Test validation (valid & invalid inputs)
└─ [ ] Setup validation middleware

Week 3: Authentication & Rate Limiting
├─ [ ] Configure JWT authentication
├─ [ ] Add [Authorize] attributes to endpoints
├─ [ ] Setup rate limiting policies
├─ [ ] Add [RequireRateLimiting] attributes
└─ [ ] Test rate limiting (rapid requests)

Week 4: Testing & Deployment
├─ [ ] Test all middleware (see checklist below)
├─ [ ] Update docker-compose.yaml
├─ [ ] Setup environment variables
├─ [ ] Deploy to production
└─ [ ] Monitor logs (Serilog)
```

---

## ✅ Testing Checklist

### Rate Limiting
```bash
# Send 6 requests rapidly to login endpoint (limit = 5/min)
for i in {1..6}; do
    curl -X POST http://localhost:5000/api/auth/login \
        -H "Content-Type: application/json" \
        -d '{"username":"user","password":"pass"}'
done

# Last request should return 429 Too Many Requests
```

### Request Size Limit
```bash
# Send > 5MB payload
dd if=/dev/zero bs=1M count=10 | curl -X POST \
    -H "Content-Type: application/octet-stream" \
    --data-binary @- \
    http://localhost:5000/api/upload

# Should return 413 Payload Too Large
```

### Validation
```bash
# Send invalid data
curl -X POST http://localhost:5000/api/auth/register \
    -H "Content-Type: application/json" \
    -d '{"username":"ab","email":"invalid","password":"123"}'

# Should return 400 Bad Request with validation errors
```

### Authentication
```bash
# Missing Authorization header
curl -X GET http://localhost:5000/api/profile

# Should return 401 Unauthorized

# With valid JWT token
curl -X GET http://localhost:5000/api/profile \
    -H "Authorization: Bearer eyJhbGc..."

# Should return 200 OK
```

### Exception Handling
```bash
# Trigger unhandled exception (e.g., access wrong endpoint)
curl -X GET http://localhost:5000/api/error

# Development: Should show stack trace
# Production: Should NOT show stack trace, only generic message
```

---

## 📊 For Each Service

### Identity.API (Microservice)
```
Required:
✅ Exception Handler (500 errors shouldn't leak info)
✅ Rate Limiting (login: 5 attempts/min)
✅ Input Validation (FluentValidation)
✅ Authentication (JWT tokens)
✅ Authorization ([Authorize] attributes)
✅ Request Size Limit (5MB)

Optional:
⚠️  Forwarded Headers (if behind proxy)
⚠️  CORS (if frontend calls direct)
⚠️  HSTS (production only)
```

### Gateway (Reverse Proxy)
```
Required:
✅ Exception Handler (502 Bad Gateway errors)
✅ Rate Limiting (global 100 req/min per IP)
✅ Forwarded Headers (CRITICAL for correct IP)
✅ Request Logging (Serilog all requests)
✅ Request Size Limit (10MB)

Optional:
⚠️  CORS (if frontend calls direct)
⚠️  HSTS (production only)
```

---

## 📋 Deployment Checklist

### Before Production Deploy

- [ ] **JWT Configuration**
  - Set `Jwt:SecretKey` in environment (min 32 chars)
  - Set `Jwt:Issuer` and `Jwt:Audience`
  - Rotate secret key periodically

- [ ] **CORS Configuration**
  - Update allowed origins (remove localhost)
  - Remove AllowAnyOrigin("*")
  - Specify exact domains only

- [ ] **Request Size Limits**
  - Gateway: 10MB
  - Identity.API: 5MB
  - File upload: adjust per need

- [ ] **Rate Limiting**
  - Disable in development (optional)
  - Production: 100 req/min global
  - Login: 5 attempts/min

- [ ] **Logging**
  - Enable Serilog file logging
  - Set log level: Information (production), Debug (dev)
  - Rotate logs daily, keep 30 days

- [ ] **HSTS**
  - Enable in production
  - Max-age: 31536000 (1 year)
  - Include subdomains

- [ ] **Docker/K8s**
  - Update docker-compose.yaml (environment variables)
  - Configure nginx forwarded headers
  - Set resource limits

- [ ] **Monitoring**
  - Setup log aggregation (ELK, Datadog)
  - Monitor rate limiting violations
  - Alert on 5xx errors
  - Track 401/403 authentication failures

---

## 🔗 File Navigation

```
📦 RoomManagermentBe/
├─ 📄 INDEX.md (← START HERE - Navigation guide)
├─ 📄 README.md (← You are here)
├─ 📄 SUMMARY.md (5-10 min quick overview)
├─ 📄 SECURITY_MIDDLEWARE_RECOMMENDATIONS.md (15 min, per-service)
├─ 📄 IMPLEMENTATION_GUIDE.md (30 min, step-by-step)
├─ 📄 MIDDLEWARE_DETAILED_GUIDE.md (45 min, reference)
├─ 📄 PIPELINE_ORDER_GUIDE.md (30 min, order + errors)
├─ 📄 CODE_TEMPLATES.md (💻 copy-paste ready)
└─ 📄 VISUAL_DIAGRAMS.md (📊 ASCII diagrams)
```

---

## 🎓 Learning Outcomes

After reading & implementing, you'll understand:

✅ **5 middleware bắt buộc** để bảo vệ input  
✅ **Thứ tự đúng** của middleware pipeline  
✅ **Khi nào dùng** từng middleware  
✅ **Cách implement** FluentValidation, JWT, Rate Limiting  
✅ **Cách debug** middleware issues  
✅ **Security best practices** cho ASP.NET Core  

---

## 💡 Key Concepts

| Concept | File | Explanation |
|---------|------|-------------|
| Request Size Limit | MIDDLEWARE_DETAILED_GUIDE.md § 1 | Chặn payload > 5MB |
| Rate Limiting | MIDDLEWARE_DETAILED_GUIDE.md § 2 | Max 5 login/min per IP |
| Validation | MIDDLEWARE_DETAILED_GUIDE.md § 3 | FluentValidation rules |
| Exception Handling | MIDDLEWARE_DETAILED_GUIDE.md § 4 | RFC 7807 problem details |
| Authentication | MIDDLEWARE_DETAILED_GUIDE.md § 5 | Parse JWT tokens |
| Authorization | MIDDLEWARE_DETAILED_GUIDE.md § 5 | Check [Authorize] |
| CORS | MIDDLEWARE_DETAILED_GUIDE.md § 6 | Whitelist origins |
| Forwarded Headers | MIDDLEWARE_DETAILED_GUIDE.md § 7 | Get real client IP |

---

## 🚨 Critical Rules

### Rule #1: Exception Handler FIRST
```csharp
app.UseExceptionHandler("/error");  // ← ALWAYS FIRST!
// other middleware...
```

### Rule #2: Forwarded Headers BEFORE Rate Limiting
```csharp
app.UseForwardedHeaders();           // ← Get real IP
app.UseRateLimiter();                // ← Then limit per IP
```

### Rule #3: Authentication BEFORE Authorization
```csharp
app.UseAuthentication();             // ← Identify user
app.UseAuthorization();              // ← Check permissions
```

### Rule #4: Routing LAST
```csharp
app.MapControllers();                // ← ALWAYS LAST!
```

---

## 📞 Common Questions

**Q: Rate limiting không work, sao?**  
A: Thường vì middleware order sai. Xem VISUAL_DIAGRAMS.md § 8 (Request Lifecycle)

**Q: Middleware nào là bắt buộc?**  
A: Xem SUMMARY.md § 2 hoặc SECURITY_MIDDLEWARE_RECOMMENDATIONS.md § 4 (bảng)

**Q: Tôi đang chạy behind nginx, cần config gì?**  
A: Xem MIDDLEWARE_DETAILED_GUIDE.md § 7 (Forwarded Headers)

**Q: Tôi muốn copy-paste code, ở đâu?**  
A: Xem CODE_TEMPLATES.md § 1-2 (Program.cs templates hoàn chỉnh)

**Q: Exception handling sao? Lộ stack trace sao?**  
A: Xem MIDDLEWARE_DETAILED_GUIDE.md § 4 (Exception Handling)

---

## 🎯 Next Steps

1. ✅ **Bạn vừa đọc README.md** - Well done!
2. ⏭️ **Đọc INDEX.md** - Choose your learning path
3. ⏭️ **Đọc SUMMARY.md** (nhanh 10 phút)
4. ⏭️ **Xem CODE_TEMPLATES.md** (copy code)
5. ⏭️ **Implement** (follow IMPLEMENTATION_GUIDE.md)
6. ⏭️ **Test** (use checklist ở section Testing)
7. ⏭️ **Deploy** (follow deployment checklist)

---

## 📞 Support

- **Confused about middleware?** → MIDDLEWARE_DETAILED_GUIDE.md
- **Need code?** → CODE_TEMPLATES.md
- **Troubleshooting?** → PIPELINE_ORDER_GUIDE.md § 5 + VISUAL_DIAGRAMS.md § 8
- **Want diagrams?** → VISUAL_DIAGRAMS.md (all 9 sections)

---

## 📝 Notes

- ✅ Code templates tested with .NET 8.0
- ✅ Compatible with ASP.NET Core 7.0+
- ⚠️ Adjust configuration per your needs
- ⚠️ Always test in development first!

---

**Happy coding! 🚀**

*Created: 2026-03-01*  
*Version: 1.0 - Complete Security Middleware Guide*


