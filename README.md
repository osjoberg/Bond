# Bond
Bond generates TypeScript definition files (*.d.ts) from your .NET types.

Todo
- Inheritance / interface support

## Install via NuGet
To install Bond, run the following command in the Package Manager Console:

```cmd
PM> Install-Package Bond
```

You can also view the package page on [Nuget](https://www.nuget.org/packages/Bond/).

## Example usage
Setting Bond up with ASP.NET MVC Core, generating "wwwroot/js/Model.d.ts" from types in namespace "YourNamespace.Model".

```c#
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                .
                .
                .

                var typeCollection = new TypeCollection();
                typeCollection.AddNamespace("YourNamespace.Model");
                
                var typeScriptTyping = new TypesScriptTyping();
                typeScriptTyping.Generate(typeCollection, "wwwroot/js/Model.d.ts");
            }

            .
            .
            .
            
        }
```