using FluentAssertions;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Collections.Immutable;
using Xunit;

namespace NSB.Extensions.Tests;

[TestSubject(typeof(EnclosedMessageTypesMapper))]
public class EnclosedMessageTypesMapperTest
{

    [Fact]
    public void NewAssemblyVersion_MapsToConcreteType()
    {
        var input = "Company.Product.Contracts.ThingHappened, Company.Product.Contracts, Version=1.3.79.0, Culture=neutral, PublicKeyToken=null;Company.Product.Contracts.IThingHappened, Company.Product.Contracts, Version=1.3.79.0, Culture=neutral, PublicKeyToken=null";
        var typeMappings = new Dictionary<string, string>
        {{
            "Company.Product.Contracts.ThingHappened",
            "Company.Product.Contracts.ThingHappened, Company.Product.Contracts, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null"
        }}.ToImmutableDictionary();

        var result = EnclosedMessageTypesMapper.Transform(input, typeMappings);

        result.Should().Be("Company.Product.Contracts.ThingHappened, Company.Product.Contracts, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null;Company.Product.Contracts.IThingHappened, Company.Product.Contracts, Version=1.3.79.0, Culture=neutral, PublicKeyToken=null");
    }

    [Fact]
    public void TotalTypeReplacement_MapsTargetType()
    {
        var input = "Company.Product.Contracts.ThingHappened, Company.Product.Contracts, Version=1.3.79.0, Culture=neutral, PublicKeyToken=null;Company.Product.Contracts.IThingHappened, Company.Product.Contracts, Version=1.3.79.0, Culture=neutral, PublicKeyToken=null";
        var typeMappings = new Dictionary<string, string>
        {{
            "Company.Product.Contracts.ThingHappened",
            "Company.Product.Contracts.OtherThingHappened, Company.Product.Contracts, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null"
        }}.ToImmutableDictionary();

        var result = EnclosedMessageTypesMapper.Transform(input, typeMappings);

        result.Should().Be("Company.Product.Contracts.OtherThingHappened, Company.Product.Contracts, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null;Company.Product.Contracts.IThingHappened, Company.Product.Contracts, Version=1.3.79.0, Culture=neutral, PublicKeyToken=null");
    }
}