using AutoFixture;
using BlogPlatform.Domain.Entities;

namespace BlogPlatform.Tests.Database;

/// <summary>
/// Перевірка, що сутності з домену коректно збираються через AutoFixture (поля з таблиці завдання)
/// окремо від бізнес-полів, які виставляються явно.
/// </summary>
public sealed class AutoFixtureEntitySanityTests
{
    [Fact]
    public void Tag_Name_can_come_from_fixture_before_persisting()
    {
        var fixture = new Fixture();
        fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        fixture.Customize<Tag>(c => c.Without(t => t.Id).Without(t => t.PostTags));

        var tag = fixture.Create<Tag>();
        Assert.False(string.IsNullOrWhiteSpace(tag.Name));
    }
}
