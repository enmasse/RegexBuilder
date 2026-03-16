# Work Log

## 1. API scaffold
> *Create an API for a regex builder. All calls should throw NotImplementedException.*

- Replaced the placeholder class with a fluent `RegexBuilder` API.
- Added regex-building members as unimplemented methods.
- Verified the project built successfully.

## 2. File rename
> *Rename the file to reflect the name of the class*

- Renamed `RegexBuilder\Class1.cs` to `RegexBuilder\RegexBuilder.cs`.
- Verified the project built successfully after the rename.

## 3. Initial test project creation
> *Create a new test project with tunit and integrated fscheck tests. Do not implement the builder yet.*

- Created `RegexBuilder.Tests` targeting `.NET 10`.
- Added `TUnit` and `TUnit.FsCheck`.
- Added the test project to the solution workspace.

## 4. TDD correction
> *No, are you dense? You should follow TDD praxis. Create test scenarios for the expected behavior and make sure that it builds. All tests should fail.*

- Replaced placeholder-exception tests with expected-behavior TDD scenarios.
- Kept production code unimplemented.
- Ensured the solution built successfully.
- Confirmed the tests failed as intended.

## 5. Recovery after interrupted edit
> *continue*

- Restored the emptied test file content.
- Re-established the TDD scenarios.
- Continued validation of build and failing-test behavior.

## 6. Neutral naming correction
> *Don't call it anything with "Placeholder" in it. I expect fully functional although failing tests.*

- Renamed the scenario test file to use neutral naming.
- Kept tests fully formed while still failing against unimplemented code.
- Enabled the `.NET 10` test runner flow with `global.json` and project configuration.

## 7. Coverage review
> *Do we have good test converage?*

- Reviewed the public API against the current tests.
- Identified broad gaps across tokens, grouping, quantifiers, and regex materialization.

## 8. Coverage expansion
> *Make sure that we have good test coverage.*

- Expanded the test suite to cover nearly all builder operations.
- Added example-based tests for tokens, groups, alternation, quantifiers, and regex creation.
- Verified the build still passed.
- Confirmed the expanded suite failed as intended.

## 9. FsCheck review
> *Can we make better use of FSCheck for property based testing?*

- Reviewed the existing property tests.
- Identified that they mostly checked output shape rather than behavioral invariants.
- Outlined stronger property-based directions.

## 10. Stronger property tests
> *Yes thank you.*

- Added explicit `FsCheck` support to the test project.
- Created `RegexBuilder.Tests\RegexBuilderArbitraries.cs` with custom arbitraries and sample types.
- Reworked FsCheck scenarios to validate regex behavior, round-tripping, captures, and quantifier semantics.
- Verified the build passed and the tests still failed as intended.

## 11. Replacement capability review
> *Do we support some kind of replace?*

- Reviewed the current API surface.
- Confirmed that replacement operations were not yet part of the builder.
- Proposed a replacement-oriented API shape.

## 12. Replacement API and tests
> *Yes, figure out how an api that supports replacement would look like, extend the API with new stubs and add tests for replacement scenarios.*

- Extended `RegexBuilder` with replacement method stubs.
- Added `RegexBuilder\RegexReplacementBuilder.cs` as a fluent replacement-template builder.
- Added `RegexBuilder.Tests\RegexReplacementScenarios.cs` with TDD tests for replacement behavior.
- Verified the build passed.
- Confirmed the full suite failed as intended against unimplemented production code.

## 13. Session logging
> *Add a sequential log of our actions in a LOG.md markup file, add my prompts as quoted italic strings.*

- Created `LOG.md` at the workspace root.
- Recorded the work in chronological order.
- Quoted each prompt as an italic block quote.

## 14. RegexBuilder implementation
> *implement the RegexBuilder*

- Implemented the fluent `RegexBuilder` pattern composition API.
- Implemented `RegexReplacementBuilder` and the replacement-oriented builder flows.
- Verified the main project built successfully.
- Verified the `RegexBuilder.Tests` test project passed.

## 15. Test coverage follow-up
> *Go ahead.*

- Expanded the test suite with invalid-input and quantifier-guard coverage.
- Added stronger property-based tests for `AtLeast`, `Optional`, `ZeroOrMore`, `OneOrMore`, and replacement behavior.
- Kept the work limited to tests and log updates.

## 16. Blazor app scaffold
> *Sounds good! Let's go!*

- Created `RegexBuilder.App` as a `.NET 10` Blazor WebAssembly frontend suitable for static hosting.
- Wired the app to the existing `RegexBuilder` library and added it to the solution.
- Implemented a browser-based regex workbench for composing builder steps, previewing generated code and patterns, and testing matches against input text.
- Updated the app shell and static host settings for a static-site deployment path.
- Verified the workspace built successfully.
- Verified the existing `RegexBuilder.Tests` project still passed.

## 17. Blazor app build repair
> *Execute the plan*

- Executed the pending repair plan for the Blazor app build failures.
- Fixed the app-side namespace collision by qualifying `RegexBuilder.App` imports with `global::`.
- Verified the workspace built successfully after the fix.

## 18. GitHub Pages publishing setup
> *Can you fix it?*

- Added a GitHub Pages deployment workflow for the Blazor WebAssembly app.
- Updated the app publish process to generate a `404.html` SPA fallback and `.nojekyll` in the published `wwwroot` output.
- Verified the app could be published with the new Pages artifacts in place.
