namespace MyApp.Application.Abstractions;

/*
    One command should have one command handler
*/

public interface ICommand { }

public interface ICommand<TResult> : ICommand { }
