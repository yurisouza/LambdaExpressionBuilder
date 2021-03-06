# Change Log

## Version 1.2.2: [CURRENT RELEASE]
### Summary
* Added a shorthand function for operations that require no values (i.e. `Operation.IsNull`)
* Added method to fetch the correct `MatchType`, when default is provided
* More bug fixes. _When one is solved another is found_

### Changes
* Added shorthand `FilterStatement` "creation" method for operations that require no values
* Added method to `OperationHelper` called `FetchMatchType`
  * This method returns the correct `MatchType` when the provided one is `MatchType.Default`, else it returns the one provided
  * Implemented this method into `FilterStatements` `ToString()` method
* Fixed: `NumberOfValuesAcceptable` function should only return `MatchType` modified number when the number of acceptable values is 1. _`Operation.Between` was not being picked up correctly on last update_
* Fixed: Using `MatchType.Default` now works as expected
* Fixed: The `MatchType` for the `Filter.By` method is now defaulted to `MatchType.Default`

### Minor Changes (Not Affecting NuGet Package)
* 'WinForm' changes
  * Changed the control `UcFilter`'s `MatchType` to default to `MatchType.Default`

---

## Version 1.2.1:
### Summary
**PLEASE NOTE THIS IS A BREAKING CHANGE**
* **Simplified enumerables** - To make it easier to type, and easier to read, I have simplified 2 enums. _This is the "breaking change" I was refering to._
* **Operations & default match-types** - Operations now have a default MatchType. This is the match-type that will be called by default.
* Bug fixes and other minor improvements

### Changes
* Simplified Enumerables
  * `FilterStatementConnector` is now just `Connector`
  * `FilterStatementMatchType` is now just `MatchType`
* Removed `NumberOfValuesAttribute` in place of `OperationSettingsAttribute`
  * I renamed the attribute so it makes more sense against the fields within it
* Bug fixes
  * Creating a `FilterStatement`, using it's IEnumerable method and a value of `NULL` throws an exception. Catching the `Null` so it works in the existing null-coalescing operator.
* Minor improvements
  * `Filter.By` and `FilterStatement`: In a poor attempt to force IEnumerables to fire the correct method, I placed this function first. This fires it only when it has a value of `NULL` which is not it's intended purpose. I have moved it down, but still kept it.
  * The `FilterStatement` `ToString` method has been tweaked slightly so a list of 1 value reads as just a value (no `MatchType` mentioned).

### Minor Changes (Not Affecting NuGet Package)
* `Tests` and `WinForm` have had their enumerable type names changed to match the new, simpler, ones

---

## Version 1.2.0:
### Summary
* **Support for very complex expressions** Allowing groups within groups as well as a close group functionality ([Improvement on the previous grouping](https://github.com/dbelmont/ExpressionBuilder/issues/10))
* **Added multi-match types** Match a list of values (i.e. A name that contains any of: "John", "Jess") [See Documentation](README.md#multi-match-types)
* **Removed `Operation.In`** See below for reasoning
* Namespace changed from `ExpressionBuilder` to `LambdaExpressionBuilder`. Only important to people transitioning from original work (it does help you though)

### Changes
* Removed `Operation.In`
  * This operation is now obsolete in place of `Operation.EqualTo` with `FilterStatementMatchType.Any`
  1) I made this decision (in this early stage) to simplify and remove duplicated code.
  1) As the default 'FilterStatementMatchType' is `All`, you would need to go change all uses of `In` to work as expected (setting `FilterStatementMatchType.Any`).
  1) Sorry for any inconvenience caused. Hopefully not too much as the download for the original work is quite low
* Namespace changed from `ExpressionBuilder` to `LambdaExpressionBuilder`
  * I decided to make this change, while in beta, because:
  1) After trying to transition from the original work to my LambdaExpressionBuilder, it took the site down breifly
  2) As per NuGet's spec, the namespace has been brought in line (more or less) with the NuGet Id
* Created the `FilterStatementMatchType` enumeration
  * Added "Allowed MatchTypes" to `Operations` and defined usage in `NumberOfValuesAttribute`
* Created `IFilterGroup`, `FilterGroup` and `IFilterStatementOrGroup` (abstract classes)
  * `IFilterGroup` contains a Group (`List<IFilterStatementOrGroup>`), this can contain either Statements or Groups (Also applies to `FilterGroup`) [Built in XML serialisation]
  * `IFilterStatementOrGroup` is now used instead of FilterStatement, this allows for groups in groups. The `Build` method is used to build the expression
* Changes to `Filter`
  * Changed `By` method (removing second value). You now create arrays/lists of values (when needed).
    * Added a method to maintain backward compatibility
    * Added another method for Enumerables (items need to be declared `AsEnumerable()`) [Not ideal, but the changed `By` statement still does the correct working]
  * Filter's statements are now a list of `IFilterStatementOrGroup`
  * Added `private` `_nest` which keeps track of current position in the `Statements`
  * `ToString()` now writes `List`s correctly (See changes to `FilterStatement`)
  * Adjusted `XML` methods
  * `Group` replaced with `OpenGroup`
    * Modified `StartGroup()` and added `EndGroup()` to support this
* Changes to `IFilterStatementConnector`
  * Added `CloseGroup` here, as this made more sense for fluent builds
* Changes to `IFilterStatement` 
  * It is now an abstract class (needed to do this to maintain `FilterBuilder`'s hidden status)
* Changes to `FilterStatement` 
  * Added `GetPropertyType` method to correctly fetch the type (used in various places)
  * Added a method to return `ToString()` of each value (used in the `ToString()` methods and in various other places)
  * Modified and added new `ToString` method to show various types of `Value`'s submitted
  * _NOTE: May have fixed `TODO:` on line 142_
* Changes to `FilterBuilder`
  * `Expressions` (dictionary) now only needs 2 expressions passing to it. As we've changed the way values work (allows lists)
  * Added `GetSafeExpression` method that correctly handles whether the `Value` is a `List` or not
  * `Between` method is now passed an `(ConstantExpression)Expression((Array)List<Value's>)` which is indexed to be used in the expression

### Minor Changes (Not Affecting NuGet Package)
* Made changes to the Form application to all the use of [`Groups`](\#groups-in-winform) and `Match Any|All`
* Revised tests to make them work with the new methods. Also removed some test as they no longer apply
* Added changelog
* Revised README to mention new updates and extra functionality (also added new image to assist the change)

---

## Version 1.1.2:
### Summary
* Intial release
  * The released version used to improve on. See link at the bottom.

# License
&copy; Copyright 2018 Glenn Marks

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at [LICENSE](LICENSE)

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

---

[Original Work](https://github.com/dbelmont/ExpressionBuilder/) &copy; Copyright 2018 David Belmont