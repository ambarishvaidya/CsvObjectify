# CsvObjectify Design Decisions

This document captures key architectural and design decisions made for the CsvObjectify library.

## Version 2.0.0.0 - .NET 10

### Decision: Synchronous API Design

**Date**: 2025
**Status**: Accepted

#### Context
During development of version 2.0.0.0, we evaluated whether to introduce asynchronous APIs (`IAsyncEnumerable<T>`, `async`/`await`) alongside or instead of the synchronous `IEnumerable<T>` API.

#### Decision
The library will remain **synchronous-only** with `IEnumerable<T>` as the return type.

#### Rationale

1. **Sequential Processing Nature**
   - CSV parsing is inherently sequential - each line must be processed in order
   - Cannot parallelize: line N+1 cannot be parsed before line N
   - No benefit from concurrent operations

2. **Balanced I/O and Compute**
   - `StreamReader.ReadLine()` and string parsing operations execute in lockstep
   - Both are lightweight, fast operations
   - No heavy I/O waits that would benefit from yielding control
   - Parsing (string operations) takes comparable time to I/O

3. **Lazy Evaluation Already Optimal**
   - `IEnumerable<T>` with `yield return` provides streaming, memory-efficient iteration
   - Consumers control memory footprint and timing
   - No eager loading - items are produced on-demand

4. **Library Simplicity**
   - Synchronous API is simpler to understand and use
   - No async state machine overhead
   - Easier testing and debugging

5. **Consumer Flexibility**
   - File size is uncontrollable - library cannot assume small or large files
   - Usage patterns vary (batch processing, UI apps, services)
   - Consumers can easily wrap in async if needed:
     ```csharp
     await Task.Run(() => parser.Parse().ToList());
     ```
   - Or consume lazily in async context:
     ```csharp
     foreach (var item in parser.Parse())
     {
         await ProcessAsync(item);
     }
     ```

6. **Performance Optimization via Span**
   - Version 2.0.0.0 achieves performance gains through `ReadOnlySpan<char>`
   - Span<T> is synchronous and stack-allocated by design
   - Zero-allocation string slicing is the actual performance win
   - Async would not enhance this benefit

#### Alternatives Considered

**Option: Add `IAsyncEnumerable<T> ParseAsync()`**
- Rejected: Would require `await ReadLineAsync()` with no measurable benefit
- Adds API complexity and maintenance burden
- Async state machine overhead may reduce throughput
- Span-based parsing remains synchronous regardless

**Option: Make API async-only**
- Rejected: Forces async on all consumers, including those in synchronous contexts
- Breaking change with no value proposition

#### Consequences

**Positive:**
- Simple, focused API surface
- Maximum performance for streaming scenarios
- Easy to understand and maintain
- Consumers maintain control over threading/async strategy

**Negative:**
- Consumers wanting async must wrap the API themselves (minimal burden)

#### Implementation Notes
- Primary parsing method: `ParseWithSpan()` using `ReadOnlySpan<char>`
- Legacy method `ParseWithoutSpan()` retained for compatibility (uses `TextFieldParser`)
- `Parse()` delegates to `ParseWithSpan()` for optimal performance

---

## Performance Optimizations

### Version 2.0.0.0: Span-Based Parsing

**Change**: Migrated from `TextFieldParser` to `ReadOnlySpan<char>`-based parsing.

**Benefits**:
- Zero-allocation string slicing
- Reduced GC pressure
- Improved throughput for large files
- Modern .NET 10 memory management

**Key Methods**:
- `ParseWithSpan()`: New default implementation
- `Unescape()`: Operates on `ReadOnlySpan<char>` for efficient quote handling
