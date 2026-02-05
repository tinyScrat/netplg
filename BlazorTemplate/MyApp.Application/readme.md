# Application

MVVM + reactive state + effects

Use feature based structure

```
MyApp.Application
│
├── Features/
│   ├── Auth/
│   │   ├── AuthState.cs
│   │   ├── AuthStore.cs
│   │   ├── LoginEffect.cs
│   │   ├── RefreshTokenEffect.cs
│   │   ├── AuthViewModel.cs
│   │   ├── IAuthApi.cs
│   │   └── AuthFeature.cs
│   │
│   ├── Portfolio/
│   │   ├── PortfolioState.cs
│   │   ├── PortfolioStore.cs
│   │   ├── LoadPortfolioEffect.cs
│   │   ├── PortfolioViewModel.cs
│   │   └── PortfolioFeature.cs
│   │
│   └── App/
│       ├── AppState.cs
│       ├── AppStore.cs
│       └── AppFeature.cs
│
└── DependencyInjection.cs
```


```
┌───────────────────────────────┐
│           UI Layer             │
│                               │
│  - Subscribes to IObservable<OrderWithLines> │
│  - Renders data in dialog      │
│  - Can cancel subscription via CancellationToken │
└─────────────▲─────────────────┘
              │ IObservable<OrderWithLines>
              │
┌─────────────┴─────────────────┐
│       Application Layer       │
│                               │
│  - Composes multiple API calls│
│  - Maps DTOs → Application models │
│  - Applies Rx retry/backoff    │
│  - Respects cancellation       │
│  - Fail-fast on AccessTokenNotAvailableException │
│                               │
│ Example pipeline:             │
│ Observable.FromAsync(() => _api.GetOrderAsync(orderId)) │
│     .CombineLatest(           │
│         Observable.FromAsync(() => _api.GetOrderLinesAsync(orderId)), │
│         (orderDto, lineDtos) => orderDto != null ? OrderMappings.Compose(orderDto, lineDtos) : null) │
│     .RetryWithBackoff(maxRetries:3, initialDelay:500ms, factor:2.0, handleException: ex => !(ex is AccessTokenNotAvailableException)) │
└─────────────▲─────────────────┘
              │ Tasks / DTOs
              │
┌─────────────┴─────────────────┐
│       Infrastructure Layer     │
│                               │
│  - Calls backend APIs          │
│  - Deserializes JSON → DTOs    │
│  - Returns Task<OrderDto?> or Task<IEnumerable<OrderLineDto>> │
│  - No Rx / no retry / no UI concerns │
│                               │
│ Example:                       │
│ Task<OrderDto?> GetOrderAsync(Guid orderId) │
│ Task<IEnumerable<OrderLineDto>> GetOrderLinesAsync(Guid orderId) │
└───────────────────────────────┘
```
