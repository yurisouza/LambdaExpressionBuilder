# Lambda Expression Builder
In short words, this library basically provides you with a simple way to create lambda expressions to filter lists and database queries by delivering an easy-to-use fluent interface that enables the creation, storage and transmission of those filters. That can be used to help to turn WebApi requests parameters into expressions, create advanced search screens with the capability to save and re-run those filters, among other things.  If you would like more details on how it works, please, check out the article [Build Lambda Expression Dynamically](https://www.codeproject.com/Articles/1079028/Build-Lambda-Expressions-Dynamically).

Would this help you in any way? Well, if your answer is 'yes', you just made my day a bit better. :smile:

## Features:
* Ability to reference properties by their names
* Ability to reference properties from a property
* Ability to reference properties from list items
* Built-in null-checks
* Built-in XML serialization
* Globalization support

## What's New: Version 1.2.0
* **Support for very complex expressions** Allowing groups within groups as well as a close group functionality ([Improvement on the previous grouping](https://github.com/dbelmont/ExpressionBuilder/issues/10))
* **Added multi-match types** Match a list of values (i.e. A name that contains any of: "John", "Jess") [See Documentation](#multi-match-types)
* **Removed `Operation.In`** See change log for reasoning

For a full list of changes and previous revisions, see the [Change Log](ExpressionBuilder/ChangeLog.md)

## Suggestions & Issues
Feel free to leave comments, and to place issues, if you find errors or realise there is any missing feature.

# How to use it
Let us imagine we have classes like this...
```CSharp
public enum PersonGender
{
    Male,
    Female
}

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public PersonGender Gender { get; set; }
    public BirthData Birth { get; set; }
    public List<Contact> Contacts { get; private set; }
    public Company Employer { get; set; }

    public class BirthData
    {
        public DateTime Date { get; set; }
        public string Country { get; set; }
    }

    public class Company {
        public string Name { get; set; }
        public string Industry { get; set; }
    }
}

public enum ContactType
{
    Telephone,
    Email
}

public class Contact
{
    public ContactType Type { get; set; }
    public string Value { get; set; }
    public string Comments { get; set; }
}
```
...and we have to build the code behind a form like this one to filter a list of Person objects:

![FormUI](docs/BuildLinqExpressionsDynacallyFormUI.png)

Now, what about being able to do it in a way like this:
```CSharp
var filter = new Filter<Person>();
filter.By("Id", Operation.Between, 2, 4,  FilterStatementConnector.And);
filter.By("Contacts[Value]", Operation.EndsWith, "@email.com", default(string), FilterStatementConnector.And);
filter.By("Birth.Country", Operation.IsNotNull, default(string), default(string),  FilterStatementConnector.Or);
filter.By("Name", Operation.Contains, " John");
var people = People.Where(filter);

//or like this...

var filter = new Filter<Person>();
filter.By("Id", Operation.Between, 2, 4)
      .And.By("Birth.Country", Operation.IsNotNull)
      .And.By("Contacts[Value]", Operation.EndsWith, "@email.com")
      .Or.By("Name", Operation.Contains, " John ");
var people = People.Where(filter);
```
So that would generate an expression like this:
```CSharp
People.Where(p => (p.Id >= 2 && p.Id <= 4)
             && (p.Birth != null && p.Birth.Country != null)
             && (p.Contacts != null && p.Contacts.Any(c => c.Value.Trim().ToLower().EndsWith("@email.com")))
             || (p.Name != null  && p.Name.Trim().ToLower().Contains("john")));
```

## Conventions
The convention around the properties names is, probably, the heart of this project. It defines the way in which the properties are addressed, how to reference a property, or the property of a property, or even the property of an item in a list property of the type being filtered.

How to address a property:
1. *Any value property of the type being filtered:* just mention its name (e.g. `Id`, `Name`, `Gender`, etc.)
2. *Any value property of a reference property of the type being filtered:* use the "dot notation" (e.g. `Birth.Date`, `Company.Name`, etc.)
3. *Any value property of an item in a list property:* mention the name of the list property followed by the name of its property between brackets (e.g. `Contacts[Type]`, `Contacts[Value]`)

## Supported types/operations
The operations are grouped together into logical type groups to simplify the association of a type with an operation:
* Default
  * EqualTo
  * NotEqualTo
* Text
  * Contains
  * DoesNotContain
  * EndsWith
  * EqualTo
  * IsEmpty
  * IsNotEmpty
  * IsNotNull
  * IsNotNullNorWhiteSpace
  * IsNull
  * IsNullOrWhiteSpace
  * NotEqualTo
  * StartsWith
* Number
  * Between
  * EqualTo
  * GreaterThan
  * GreaterThanOrEqualTo
  * LessThan
  * LessThanOrEqualTo
  * NotEqualTo
* Boolean
  * EqualTo
  * NotEqualTo
* Date
  * Between
  * EqualTo
  * GreaterThan
  * GreaterThanOrEqualTo
  * LessThan
  * LessThanOrEqualTo
  * NotEqualTo

This way, when a type is associated with a type group, that type will "inherit" the list of supported operations from its group.

While compiling the filter into a lambda expression, the expression builder will validate if the selected operation is supported by the property's type and throw an exception if it's not. To overcome situations in which you would like to add support to a specific type, you may configure it by adding the following to your config file:
```Xml
<configuration>
  ...
  <configSections>
    <section name="LambdaExpressionBuilder" type="LambdaExpressionBuilder.Configuration.ExpressionBuilderConfig, LambdaExpressionBuilder" />
  </configSections>

  ...

  <LambdaExpressionBuilder>
    <SupportedTypes>
      <add typeGroup="Date" type="System.DateTimeOffset" />
    </SupportedTypes>
  </LambdaExpressionBuilder>
  ...
</configuration>
```

## Globalization support
You just need to perform some easy steps to add globalization support to the UI:
1. Add a resource file to the project, naming it after the type you'll create your filter to (e.g. `Person.resx`);
2. Add one entry for each property you'd like to globalize following the conventions (previously mentioned), but replacing the dots (`.`) and the brackets (`[`, `]`) by underscores (`_`):  
`Person.resx`  
![Person.resx](docs/Person.resx.PNG)  
`Person.pt-BR.resx`  
![Person.pt-BR.resx](docs/Person.pt-BR.resx.PNG)
3. You can globalize the operations on a similar way as well by adding a resources file named `Operations.resx`:  
`Operations.resx`  
![Operations.resx](docs/Operations.resx.PNG)  
`Operations.pt-BR.resx`  
![Operations.pt-BR.resx](docs/Operations.pt-BR.resx.PNG)
4. For the properties, you'll instantiate a `PropertyCollection` : `new PropertyCollection(typeof(Person), Resources.Person.ResourceManager)`. That will give you a collection of objects with three members:
  * `Id`: The conventionalised property identifier (previously mentioned)
  * `Name`: The resources file matching value for the property id
  * `Info`: The `PropertyInfo` object for the property
5. And for the operations, you have an extension method: `Operation.GreaterThanOrEqualTo.GetDescription(Resources.Operations.ResourceManager)`.

#### Note on globalization
Any property or operation not mentioned in the resources files will be replaced by its conventionalised property identifier.

## Complex expressions
Complex expressions are handled basically by grouping up filter statements, like in the example below:
```CSharp
var filter = new Filter<Products>();
filter.StartGroup();
filter.StartGroup();
filter.By("Name", Operation.DoesNotContain, "doe", connector: FilterStatementConnector.Or);
filter.StartGroup();
filter.By("Name", Operation.EndsWith, "Doe", connector: FilterStatementConnector.Or);
filter.By("Name", Operation.StartsWith, "Jo", connector: FilterStatementConnector.And);
filter.EndGroup();
filter.EndGroup();
filter.By("Employer", Operation.IsNull, FilterStatementConnector.And);
filter.EndGroup();
filter.By("Birth.Country", Operation.EqualTo, "USA");
var people = db.Products.Where(filter);

//or using the fluent interface...

var filter = new Filter<Products>();
filter
    .OpenGroup
        .OpenGroup
            .By("Name", Operation.DoesNotContain, "doe")
            .Or
            .OpenGroup
                .By("Name", Operation.EndsWith, "Doe")
                .Or
                .By("Name", Operation.StartsWith, "Jo")
            .CloseGroup
        .CloseGroup
        .And
        .By("Employer", Operation.IsNull)
    .CloseGroup
    .And
    .By("Birth.Country", Operation.EqualTo, "USA");
var people = db.Products.Where(filter);
```

That would produce an expression like this: (Excluding all the `NotNull` checks and `.Trim().ToLower()` functions)
```CSharp
db.Products
  .Where(p => ( ( !p.Name.Contains("doe") || ( p.Name.EndsWith("Doe") || p.Name.StartsWith("Jo") ) ) && p.Employer == null ) && p.Birth.Country == "USA" );
```

Every time you start a group that means all further statements will be at the same "parenthesis" until EndGroup is called.
You can even add groups to groups! (For those super complex expressions)

### Groups In WinForm
The WinForms has been updated to allow tests of groups. Simply right-click on a `+` button and choose `Add Group`. Doing this can produce a form like the below image:\
![FormUI - Group Example](docs/FormGroupBuild.png)

## Multi-Match Types
See the below examples of common cases:
* You have a few words entered as a search term, looking for a product.
* You want to find all Tasks currently assigned to a group of users at work.

This can now be done, thanks to multi-match types!

Let's take the first item as an example. We want to find a "Bright Blue Bicycle"! Simple, split the term and filter by it.

```CSharp
var filter = new Filter<Products>();
var termArr = "Bright Blue Bicycle".Split(' ');

// Connector and MatchType are defaulted to what we want
// So, giving an array (or list) works straight away. No extra code required!
filter.By("Name", Operation.Contains, termArr);

// or 
// Declare `matchType:`
filter.By("Name", Operation.Contains, termArr, matchType: FilterStatementMatchType.All);

// or 
// Declare `connector` &&  `matchType:`
filter.By("Name", Operation.Contains, termArr, FilterStatementConnector.And, FilterStatementMatchType.All);
```

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