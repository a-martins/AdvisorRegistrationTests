using AdvisorRegistrationTests.AutoFixture;
using AdvisorRegistrationWebApi.Controllers;
using AdvisorRegistrationWebApi.Models;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AdvisorRegistrationTests
{
    public class AdvisorControllerTests
    {
        [Theory, AutoNSubstitute]
        public void Sut_ShouldGuardItsClause(GuardClauseAssertion assertion) => 
            assertion.Verify(typeof(AdvisorController).GetConstructors()); 

        [Theory, AutoNSubstitute]
        public async void Sut_GetAdvisors_ShouldReturnsAListOFAdvisorsWithOkResult(
            AdvisorController sut,
            Advisor advisor)
        {
            IEnumerable<Advisor> advisors = new List<Advisor>() { advisor };
            sut.AdvisorRepository.GetAdvisorsAsync().Returns(Task.FromResult(advisors));

            var result = await sut.GetAdvisors();
            var okResult = result.Result as ObjectResult;

            okResult.Should().NotBeNull();
            okResult.Should().BeOfType<OkObjectResult>();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().Be(advisors);
        }

        [Theory, AutoNSubstitute]
        public async void Sut_GetAdvisor_ShouldReturnsAnAdvisorWithOkResult_WhenTheAdvisorWasFound(
            AdvisorController sut,
            Advisor advisor)
        {
            sut.AdvisorRepository.GetAdvisorByIDAsync(advisor.Id).Returns(Task.FromResult(advisor));

            var result = await sut.GetAdvisor(advisor.Id);
            var okResult = result.Result as ObjectResult;

            okResult.Should().NotBeNull();
            okResult.Should().BeOfType<OkObjectResult>();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().Be(advisor);
        }

        [Theory, AutoNSubstitute]
        public async void Sut_GetAdvisor_ShouldReturnsNotFoundResult_WhenNoAdvisorWasFound(
            AdvisorController sut,
            int id)
        {
            sut.AdvisorRepository.GetAdvisorByIDAsync(id).Returns(Task.FromResult((Advisor)null));

            var result = await sut.GetAdvisor(id);
            var notFoundResult = result.Result as StatusCodeResult;

            notFoundResult.Should().NotBeNull();
            notFoundResult.Should().BeOfType<NotFoundResult>();
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Theory, AutoNSubstitute]
        public async void Sut_PutAdvisor_ShouldReturnsNotFoundResult_WhenNoAdvisorWasFound(
            AdvisorController sut,
            Advisor advisor)
        {
            sut.AdvisorRepository.Any(advisor.Id).Returns(false);

            var result = await sut.PutAdvisor(advisor.Id, advisor);
            var notFoundResult = result.Result as StatusCodeResult;

            notFoundResult.Should().NotBeNull();
            notFoundResult.Should().BeOfType<NotFoundResult>();
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Theory, AutoNSubstitute]
        public async void Sut_PutAdvisor_ShouldReturnsTheUpdatedAdvisor_WhenTheAdvisorWasFoundAndUpdated(
            AdvisorController sut,
            Advisor advisor)
        {
            sut.AdvisorRepository.Any(advisor.Id).Returns(true);

            var result = await sut.PutAdvisor(advisor.Id, advisor);
            var okResult = result.Result as ObjectResult;

            sut.AdvisorRepository.Received().UpdateAdvisor(advisor);
            await sut.AdvisorRepository.Received().SaveAsync();

            okResult.Should().NotBeNull();
            okResult.Should().BeOfType<OkObjectResult>();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().Be(advisor);
        }

        [Theory, AutoNSubstitute]
        public async void Sut_PostAdvisor_ShouldReturnsTheCreatedAdvisorWithCreatedResult(
            AdvisorController sut,
            Advisor advisor)
        {
            var result = await sut.PostAdvisor(advisor);
            var createdResult = result.Result as ObjectResult;

            sut.AdvisorRepository.Received().InsertAdvisor(advisor);
            await sut.AdvisorRepository.Received().SaveAsync();

            createdResult.Should().NotBeNull();
            createdResult.Should().BeOfType<CreatedAtActionResult>();
            createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
            createdResult.Value.Should().Be(advisor);
        }

        [Theory, AutoNSubstitute]
        public async void Sut_DeleteAdvisor_ShouldReturnsNotFoundResult_WhenNoAdvisorWasFound(
            AdvisorController sut,
            int id)
        {
            sut.AdvisorRepository.Any(id).Returns(false);

            var result = await sut.DeleteAdvisor(id);
            var notFoundResult = result.Result as StatusCodeResult;

            notFoundResult.Should().NotBeNull();
            notFoundResult.Should().BeOfType<NotFoundResult>();
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Theory, AutoNSubstitute]
        public async void Sut_DeleteAdvisor_ShouldReturnsOkResult_WhenTheAdvisorWasFoundAndDeleted(
            AdvisorController sut,
            int id)
        {
            sut.AdvisorRepository.Any(id).Returns(true);

            var result = await sut.DeleteAdvisor(id);
            var okResult = result.Result as StatusCodeResult;

            sut.AdvisorRepository.Received().DeleteAdvisor(id);
            await sut.AdvisorRepository.Received().SaveAsync();

            okResult.Should().NotBeNull();
            okResult.Should().BeOfType<OkResult>();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
    }
}
