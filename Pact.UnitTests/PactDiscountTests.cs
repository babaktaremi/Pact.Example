using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Pact.UnitTests.Base;
using Pact.UnitTests.TestModels;
using PactNet;
using Xunit.Abstractions;

namespace Pact.UnitTests;

public class PactDiscountTests(ITestOutputHelper outputHelper)
{
    private readonly IPactBuilderV3 _mockProviderService = PactConfiguration.BuildServer();

    [Theory]
    [InlineData(10)]
    [InlineData(40)]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(60)]
    [InlineData(20)]
    [InlineData(30)]
    public async Task Discount_Should_Be_Valid_Based_On_User_Joined_Date(int daysJoined)
    {
        var discountModel = new CustomerDiscountModel(daysJoined);

        _mockProviderService
            .UponReceiving(@$"Given a customer joined days,
a valid discount percent (maxed 30%) should be returned
                        {Guid.NewGuid()}")
            .Given($"DaysJoined-{daysJoined}-{Guid.NewGuid()}")
            .WithRequest(HttpMethod.Post, "/CalculateUserDiscount")
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(discountModel)
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithJsonBody(CalculateCustomerDiscount(daysJoined));


        await _mockProviderService.VerifyAsync(async context =>
        {
            using var client = new HttpClient();

            var response = await client.PostAsJsonAsync($"{context.MockServerUri}CalculateUserDiscount",
                new CustomerDiscountModel(daysJoined));

            var discountResponseString = await response.Content.ReadAsStringAsync();

            outputHelper.WriteLine(discountResponseString);

            var discountResponse = await response.Content.ReadFromJsonAsync<CustomerDiscountResponseModel>();


            if (discountResponse != null)
                Assert.Equal(CalculateCustomerDiscount(daysJoined).DiscountPercent,discountResponse.DiscountPercent);
        });



    }

    private CustomerDiscountResponseModel CalculateCustomerDiscount(int daysJoined)
    {
        var discount = (daysJoined / 365d) * 100d;

        if (discount > 20d)
            discount = 20d;

        discount= Math.Ceiling(discount * Math.Pow(10, 2)) / Math.Pow(10, 2);

        return new CustomerDiscountResponseModel(discount);
    }
}