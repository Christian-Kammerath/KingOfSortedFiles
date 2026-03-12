global using Xunit;

// CustomLogSystem is a static class with global state (log file path, ListBox binding).
// Running test classes in parallel causes race conditions where one test class
// overwrites the static binding mid-test in another class.
// Disabling parallelization across test collections fixes this until CustomLogSystem
// is refactored to be non-static or injectable.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
