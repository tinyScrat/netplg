# Blazor

just the UI part


```
MyApp.Client
│
├── Features/
│   ├── Auth/
│   │   ├── LoginPage.razor
│   │   └── LoginView.razor
│   │
│   ├── Portfolio/
│   │   ├── PortfolioPage.razor
│   │   └── PortfolioTable.razor
│
└── Shared/

```

## Things to Avoid

- Don't use `System.Reactive.Linq.Observable.ToEventPattern`, it is not trimming safe.
