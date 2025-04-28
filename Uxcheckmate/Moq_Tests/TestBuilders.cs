using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net.Http;
using Uxcheckmate_Main.Controllers;
using Uxcheckmate_Main.Models;
using Uxcheckmate_Main.Services;

namespace Moq_Tests;
public static class TestBuilder
{
    public static HomeController BuildHomeController(HttpContext httpContext, UxCheckmateDbContext context)
    {
        return new HomeController(
            new LoggerFactory().CreateLogger<HomeController>(),
            new HttpClient(),
            context,
            new Mock<IOpenAiService>().Object,
            new Mock<IAxeCoreService>().Object,
            new ReportService(
                new HttpClient(),
                new LoggerFactory().CreateLogger<ReportService>(),
                context,
                new Mock<IOpenAiService>().Object,
                new Mock<IBrokenLinksService>().Object,
                new Mock<IHeadingHierarchyService>().Object,
                new Mock<IColorSchemeService>().Object,
                new Mock<IMobileResponsivenessService>().Object,
                new Mock<IScreenshotService>().Object,
                new Mock<IPlaywrightScraperService>().Object,
                new Mock<IPopUpsService>().Object,
                new Mock<IAnimationService>().Object,
                new Mock<IAudioService>().Object,
                new Mock<IScrollService>().Object,
                new Mock<IFPatternService>().Object,
                new Mock<IZPatternService>().Object,
                new Mock<ISymmetryService>().Object
            ),
            new Mock<PdfExportService>().Object,
            new Mock<IScreenshotService>().Object,
            new Mock<IViewRenderService>().Object
        )
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }
}


// TestHtmlElement is a mock class for testing purposes
public class TestHtmlElement : HtmlElement
{
    public TestHtmlElement(double x, double y, double width, double height, double desiredDensity)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        if (Area > 0){
            int textLength = (int)(desiredDensity * Area);
            Text = new string('A', Math.Max(textLength, 1)); // Fake character using different amount of As
        }else{
            Text = "";
        }
    }
}
