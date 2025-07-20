## Task 1: Proper Layering & Clean Architecture ✅

### Controllers Layer
-  ClaimsController and CoversController - Thin controllers that delegate to services
-  Follow Single Responsibility Principle (SRP) - only handle HTTP concerns

### Services Layer
-  ClaimsService and CoverService - Business logic implementation
-  PremiumCalculatorService - Specialized calculation logic
-  Dependency Inversion Principle (DIP) - Services depend on abstractions (interfaces)

### Repository Layer
-  ClaimsRepository and CoverRepository - Data access abstraction
-  UnitOfWork pattern for transaction management

### Infrastructure
-  Separate contexts for main data (ClaimsContext) and auditing (AuditContext)
-  Global exception handler using `IExceptionHandler` with `ProblemDetails` responses
-  Custom domain exceptions (ClaimNotFoundException, CoverNotFoundException, etc.)
-  Strategic logging focused on business events and error scenarios

## Task 2: Validation Implementation ✅

Implemented comprehensive validation using FluentValidation:

#### Claim Validation (CreateClaimDtoValidator)
-  ✅ Damage cost cannot exceed 100,000
-  ✅ Cover ID validation
-  ✅ Business rule validation in service layer (coverage period check)

#### Cover Validation (CreateCoverDtoValidator)
-  ✅ Start date cannot be in the past
-  ✅ Total insurance period cannot exceed 1 year
-  ✅ End date must be after start date

## Task 3: Asynchronous Auditing ✅

Background processing pattern using Channels and HostedService:

#### Components:
-  AuditBackgroundService - Background service that processes audit queue
-  InMemoryAuditQueue - Thread-safe queue using .NET Channels
-  Auditer - Enqueues audit events without blocking HTTP requests

## Task 4: Comprehensive Testing 🤖⚠️
*Full disclosure: I may have "borrowed" some AI assistance for this section. Good, clean code is inherently testable, which made it surprisingly easy for the AI to generate comprehensive test coverage. I can totally justify this approach in our upcoming interview. I also made sure to limit the AI to 1–2 tests per service to avoid overdoing it.*

## Task 5: Premium Calculation Fix ✅

### Premium Tiers:
-  FirstTierPremiumCalculator - First 30 days at full rate
-  SecondTierPremiumCalculator - Next 150 days with discount
-  ThirdTierPremiumCalculator - Remaining days with additional discount

#### Business Rules Implemented:
-  ✅ Base rate: 1250 per day
-  ✅ Yacht: +10%, Passenger Ship: +20%, Tanker: +50%, Others: +30%
-  ✅ Yacht discount: 5% (days 31-180), additional 3% (180+)
-  ✅ Other types: 2% (days 31-180), additional 1% (180+)

## Key Packages & Technologies Used

Validation & Mapping
-  FluentValidation - Declarative validation rules
-  Riok.Mapperly - Source generator for object mapping

Testing
-  Moq - Mocking framework (Should be replaced with something like Nsubstitue because of some shady practices collecting emails)