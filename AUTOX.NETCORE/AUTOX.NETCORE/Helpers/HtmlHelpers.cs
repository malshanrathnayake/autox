using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using System.Text;

namespace AUTOX.NETCORE.Helpers
{
    public static class HtmlHelpers
    {
        public static IHtmlContent RenderAction(this IHtmlHelper helper, string action, object parameters = null)
        {
            var controller = (string)helper.ViewContext.RouteData.Values["controller"];
            return RenderAction(helper, action, controller, null, parameters);
        }

        public static IHtmlContent RenderAction(this IHtmlHelper helper, string action, string controller, object parameters = null)
        {
            return RenderAction(helper, action, controller, null, parameters);
        }

        public static IHtmlContent RenderAction(
            this IHtmlHelper helper,
            string action,
            string controller,
            string area,
            object parameters = null)
        {
            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentNullException(nameof(action));

            if (string.IsNullOrWhiteSpace(controller))
                throw new ArgumentNullException(nameof(controller));

            return RenderActionAsync(helper, action, controller, area, parameters)
                .GetAwaiter()
                .GetResult();
        }

        private static async Task<IHtmlContent> RenderActionAsync(
            this IHtmlHelper helper,
            string action,
            string controller,
            string area,
            object parameters = null)
        {
            var currentHttpContext = helper.ViewContext.HttpContext;

            var httpContextFactory = GetServiceOrFail<IHttpContextFactory>(currentHttpContext);
            var actionInvokerFactory = GetServiceOrFail<IActionInvokerFactory>(currentHttpContext);
            var actionSelector = GetServiceOrFail<IActionDescriptorCollectionProvider>(currentHttpContext);

            // Create isolated HttpContext
            var newHttpContext = httpContextFactory.Create(currentHttpContext.Features);

            // IMPORTANT: do NOT dispose this stream
            var outputStream = new MemoryStream();
            newHttpContext.Response.Body = new NonDisposableStream(outputStream);

            var routeValues = new RouteValueDictionary(parameters ?? new { })
            {
                ["controller"] = controller,
                ["action"] = action
            };

            if (!string.IsNullOrEmpty(area))
                routeValues["area"] = area;

            var routeData = new RouteData();

            foreach (var router in helper.ViewContext.RouteData.Routers)
            {
                routeData.PushState(router, null, null);
            }

            routeData.PushState(null, routeValues, null);

            var actionDescriptor = actionSelector.ActionDescriptors.Items
                .FirstOrDefault(x =>
                    string.Equals(x.RouteValues["controller"], controller, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(x.RouteValues["action"], action, StringComparison.OrdinalIgnoreCase) &&
                    (string.IsNullOrEmpty(area) ||
                     string.Equals(x.RouteValues["area"], area, StringComparison.OrdinalIgnoreCase)));

            if (actionDescriptor == null)
            {
                throw new InvalidOperationException(
                    $"Action '{action}' in controller '{controller}'{(area != null ? $" (area '{area}')" : "")} not found.");
            }

            var actionContext = new ActionContext(newHttpContext, routeData, actionDescriptor);
            var invoker = actionInvokerFactory.CreateInvoker(actionContext);

            if (invoker == null)
                throw new InvalidOperationException("Could not create action invoker.");

            await invoker.InvokeAsync();

            outputStream.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(outputStream, Encoding.UTF8);
            var content = await reader.ReadToEndAsync();

            return new HtmlString(content);
        }

        private static TService GetServiceOrFail<TService>(HttpContext context)
        {
            return context.RequestServices.GetService(typeof(TService)) is TService service
                ? service
                : throw new InvalidOperationException(
                    $"Service '{typeof(TService).Name}' not available.");
        }
    }

    internal sealed class NonDisposableStream : Stream
    {
        private readonly Stream _inner;

        public NonDisposableStream(Stream inner)
        {
            _inner = inner;
        }

        protected override void Dispose(bool disposing)
        {
            // Prevent MVC from closing the stream
        }

        public override bool CanRead => _inner.CanRead;
        public override bool CanSeek => _inner.CanSeek;
        public override bool CanWrite => _inner.CanWrite;
        public override long Length => _inner.Length;

        public override long Position
        {
            get => _inner.Position;
            set => _inner.Position = value;
        }

        public override void Flush() => _inner.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);
        public override void SetLength(long value) => _inner.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => _inner.Write(buffer, offset, count);
    }
}
