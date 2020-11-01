using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;

namespace AdvisorRegistrationTests.AutoFixture
{
    public class AutoNSubstituteAttribute : AutoDataAttribute
    {
        public AutoNSubstituteAttribute()
            : base(() =>
                 new Fixture()
                     .Customize(new MvcCustomization())
                     .Customize(new AutoNSubstituteCustomization()))
        {
        }

        private class MvcCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<ControllerContext>(c => c.OmitAutoProperties());
            }
        }
    }
}
