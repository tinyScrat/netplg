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
│   │   ├── LoginViewModel.cs
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
